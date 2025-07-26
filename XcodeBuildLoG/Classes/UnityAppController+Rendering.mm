#include "UnityAppController+Rendering.h"
#include "UnityAppController+ViewHandling.h"

#include "Unity/InternalProfiler.h"
#include "Unity/DisplayManager.h"

#include "UI/UnityView.h"

#include <dlfcn.h>

#import <Metal/Metal.h>

extern bool _skipPresent;
extern bool _didResignActive;

static int _renderingAPI = 0;
static void SelectRenderingAPIImpl();


@implementation UnityAppController (Rendering)

#if !PLATFORM_VISIONOS
- (BOOL)usingCompositorLayer
{
    return NO;
}
#endif

- (void)createCADisplayLink
{
    _displayLink = [CADisplayLink displayLinkWithTarget: self selector: @selector(repaintDisplayLink)];
    [self callbackFramerateChange: -1];
    [_displayLink addToRunLoop: [NSRunLoop currentRunLoop] forMode: NSRunLoopCommonModes];

    printf_console("CADisplayLink created\n");
}

#if UNITY_USES_METAL_DISPLAY_LINK
- (void)createMetalDisplayLink
{
    if (@available(iOS 17.0, tvOS 17.0, *))
    {
        _usesMetalDisplayLink = YES;

        _metalDisplayLink = [[CAMetalDisplayLink alloc] initWithMetalLayer:(CAMetalLayer*)_unityView.layer];
        _metalDisplayLink.preferredFrameLatency = 2;
        _metalDisplayLink.paused = NO;
        _metalDisplayLink.delegate = self;

        [self callbackFramerateChange: -1];
        [_metalDisplayLink addToRunLoop:[NSRunLoop currentRunLoop] forMode:NSRunLoopCommonModes];

        printf_console("CAMetalDisplayLink created\n");
    }
}
#endif

- (void)createDisplayLink
{
    _usesMetalDisplayLink = NO; // we will set it to YES inside createMetalDisplayLink

#if UNITY_USES_METAL_DISPLAY_LINK
    if(self.renderingAPI == apiMetal && [self shouldUseMetalDisplayLink])
        [self createMetalDisplayLink];
#endif

    if (!_usesMetalDisplayLink)
        [self createCADisplayLink];
}

- (void)destroyCADisplayLink
{
    [_displayLink invalidate];
    _displayLink = nil;
}

- (void)destroyDisplayLink
{
    [self destroyCADisplayLink];

#if UNITY_USES_METAL_DISPLAY_LINK
    if (@available(iOS 17.0, tvOS 17.0, *))
    {
        [_metalDisplayLink invalidate];
        _metalDisplayLink = nil;
    }
#endif
}

- (void)repaintDisplayLink
{
    if (self.usingCompositorLayer == NO)
    {
        UnityDisplayLinkCallback(_displayLink.timestamp);
        [self repaint];
    }
    else
    {
        [self repaintCompositorLayer];
    }
}

#if UNITY_USES_METAL_DISPLAY_LINK
- (void)metalDisplayLink:(CAMetalDisplayLink*)link needsUpdate:(CAMetalDisplayLinkUpdate*)update
{
    UnityDisplayLinkCallback(0);
    ((UnityDisplaySurfaceMTL*)_mainDisplay.surface)->nextDrawable = update.drawable;
    [self repaint];
}
#endif

- (void)repaint
{
    if (_unityView.skipRendering)
        return;

#if UNITY_SUPPORT_ROTATION
    [self checkOrientationRequest];
#endif
    [_unityView recreateRenderingSurfaceIfNeeded];
    [_unityView processKeyboard];
    UnityDeliverUIEvents();

    // we want to support both CADisplayLink and CAMetalDisplayLink
    // the major complication is that they work quite differently under the hood
    // CADisplayLink: you can consider this a simple timer-based callback
    //   so if we get this while in background - we might be not allowed to render at all
    //   and before we were having an explicit check to repain only if we are not paused
    // CAMetalDisplayLink: unlike CADisplayLink (where we query drawable from view),
    //   the callback comes when we are asked explicitly to render view contents (we are given drawable)
    //   and we cannot bypass rendering when asked at all

    if(UnityIsPaused())
    {
        if(self.unityUsesMetalDisplayLink)
            UnityRenderWithoutPlayerLoop();
    }
    else
    {
        UnityRepaint();
    }
}

#if !PLATFORM_VISIONOS
- (void)repaintCompositorLayer
{
}
#endif

- (void)callbackGfxInited
{
    assert(self.engineLoadState < kUnityEngineLoadStateRenderingInitialized && "Graphics should not have been initialized at this point");
    InitRendering();
    [self advanceEngineLoadState: kUnityEngineLoadStateRenderingInitialized];

    [self shouldAttachRenderDelegate];
    [_unityView updateUnityBackbufferSize];
    [_unityView recreateRenderingSurface];
    [_renderDelegate mainDisplayInited: _mainDisplay.surface];

    _mainDisplay.surface->allowScreenshot = 1;
}

- (void)callbackPresent:(const UnityFrameStats*)frameStats
{
    if (_skipPresent)
        return;

    // metal needs special processing, because in case of airplay we need extra command buffers to present non-main screen drawables
    if (UnitySelectedRenderingAPI() == apiMetal)
    {
        [[DisplayManager Instance].mainDisplay present];
#if !PLATFORM_VISIONOS
        [[DisplayManager Instance] enumerateNonMainDisplaysWithBlock:^(DisplayConnection* conn) {
            PreparePresentNonMainScreenMTL((UnityDisplaySurfaceMTL*)conn.surface);
        }];
#endif
    }
    else
    {
        [[DisplayManager Instance] present];
    }

    Profiler_FramePresent(frameStats);
}

- (void)callbackFramerateChange:(int)targetFPS
{
    if (targetFPS <= 0)
        targetFPS = UnityGetTargetFPS();

    // on tvos it is possible to start application without a screen attached
    // alas, mainScreen is set in this case, but the values provided are bogus
    //   and in the case of maxFPS = 0 we will end up in endless recursion
#if !PLATFORM_VISIONOS
    const int maxFPS = (int)[UIScreen mainScreen].maximumFramesPerSecond;
#else
    // no UIScreen on VisionOS
    const int maxFPS = 90;
#endif
    if (maxFPS > 0 && targetFPS > maxFPS)
    {
        targetFPS = maxFPS;
        // note that this changes FPS, resulting in UnityFramerateChangeCallback call, calling this method recursively recursively
        UnitySetTargetFPS(targetFPS);
        return;
    }

    if(_usesMetalDisplayLink)
    {
    #if UNITY_USES_METAL_DISPLAY_LINK
        if (@available(iOS 17.0, tvOS 17.0, *))
            _metalDisplayLink.preferredFrameRateRange = CAFrameRateRangeMake(targetFPS, targetFPS, targetFPS);
    #endif
    }
    else
    {
        if (@available(iOS 15.0, tvOS 15.0, *))
            _displayLink.preferredFrameRateRange = CAFrameRateRangeMake(targetFPS, targetFPS, targetFPS);
        else
            _displayLink.preferredFramesPerSecond = targetFPS;
    }
}

- (void)selectRenderingAPI
{
    NSAssert(_renderingAPI == 0, @"[UnityAppController selectRenderingApi] called twice");
    SelectRenderingAPIImpl();
}

- (UnityRenderingAPI)renderingAPI
{
    NSAssert(_renderingAPI != 0, @"[UnityAppController renderingAPI] called before [UnityAppController selectRenderingApi]");
    return (UnityRenderingAPI)_renderingAPI;
}

@end


extern "C" void UnityGfxInitedCallback()
{
    [GetAppController() callbackGfxInited];
}

extern "C" void UnityPresentContextCallback(struct UnityFrameStats const* unityFrameStats)
{
    [GetAppController() callbackPresent: unityFrameStats];
}

extern "C" void UnityFramerateChangeCallback(int targetFPS)
{
    [GetAppController() callbackFramerateChange: targetFPS];
}

static NSBundle*            _MetalBundle        = nil;
static id<MTLDevice>        _MetalDevice        = nil;
static id<MTLCommandQueue>  _MetalCommandQueue  = nil;

static void SelectRenderingAPIImpl()
{
    assert(_renderingAPI == 0 && "Rendering API selection was done twice");

    _renderingAPI = UnityGetRenderingAPI();
    if (_renderingAPI == apiMetal)
    {
        _MetalBundle        = [NSBundle bundleWithPath: @"/System/Library/Frameworks/Metal.framework"];
        _MetalDevice        = MTLCreateSystemDefaultDevice();
        _MetalCommandQueue  = [_MetalDevice newCommandQueueWithMaxCommandBufferCount: UnityCommandQueueMaxCommandBufferCountMTL()];

        assert(_MetalDevice != nil && _MetalCommandQueue != nil && "Could not initialize Metal.");
    }
}

extern "C" NSBundle*            UnityGetMetalBundle()       { return _MetalBundle; }
extern "C" MTLDeviceRef         UnityGetMetalDevice()       { return _MetalDevice; }
extern "C" MTLCommandQueueRef   UnityGetMetalCommandQueue() { return _MetalCommandQueue; }
extern "C" int                  UnitySelectedRenderingAPI() { return _renderingAPI; }
extern "C" void                 UnitySelectRenderingAPI()   { SelectRenderingAPIImpl(); }

// deprecated and no longer used by unity itself (will soon be removed)
extern "C" MTLCommandQueueRef   UnityGetMetalDrawableCommandQueue() { return UnityGetMetalCommandQueue(); }


extern "C" UnityRenderBufferHandle  UnityBackbufferColor()      { return GetMainDisplaySurface()->unityColorBuffer; }
extern "C" UnityRenderBufferHandle  UnityBackbufferDepth()      { return GetMainDisplaySurface()->unityDepthBuffer; }

extern "C" void                 DisplayManagerEndFrameRendering() { [[DisplayManager Instance] endFrameRendering]; }

extern "C" void                 UnityPrepareScreenshot()    { UnitySetRenderTarget(GetMainDisplaySurface()->unityColorBuffer, GetMainDisplaySurface()->unityDepthBuffer); }

extern "C" void UnityRepaint()
{
    @autoreleasepool
    {
        Profiler_FrameStart();
        if (UnityIsBatchmode())
            UnityBatchPlayerLoop();
        else
            UnityPlayerLoop();
        Profiler_FrameEnd();
    }
}
