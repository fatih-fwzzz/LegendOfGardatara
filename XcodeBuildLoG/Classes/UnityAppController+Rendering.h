#pragma once

#include "UnityForwardDecls.h"
#include "UnityAppController.h"
#include "UnityRendering.h"
#if PLATFORM_VISIONOS
#include "UnityAppController+Rendering+visionOS.h"
#endif

#if UNITY_USES_METAL_DISPLAY_LINK
@interface UnityAppController (Rendering) <CAMetalDisplayLinkDelegate>
#else
@interface UnityAppController (Rendering)
#endif

#if !PLATFORM_VISIONOS
@property (readonly) BOOL usingCompositorLayer;
#endif

- (void)createDisplayLink;
- (void)repaintDisplayLink;
- (void)destroyDisplayLink;
- (void)destroyCADisplayLink;

- (void)repaint;

#if UNITY_USES_METAL_DISPLAY_LINK
- (void)metalDisplayLink:(CAMetalDisplayLink *)link needsUpdate:(CAMetalDisplayLinkUpdate *)update API_AVAILABLE(ios(17.0), tvos(17.0));
#endif

#if !PLATFORM_VISIONOS
- (void)repaintCompositorLayer;
#endif

- (void)selectRenderingAPI;
@property (readonly, nonatomic) UnityRenderingAPI   renderingAPI;

@end

// helper to run unity loop along with proper handling of the rendering
#ifdef __cplusplus
extern "C" {
#endif

void UnityRepaint();

#ifdef __cplusplus
} // extern "C"
#endif
