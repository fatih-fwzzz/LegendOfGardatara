using UnityEngine;

public class HeroStateMachine : MonoBehaviour
{
    public HeroState heroState = HeroState.Idle;
    public float detectionRange = 6f;
    public LayerMask enemyLayer;
    public float moveSpeed = 2f;

    private HeroAnimationController heroAnimationController;
    private Rigidbody2D rb;

    private bool attackTriggered = false;
    private bool hitTriggered = false;
    private bool isKnockedBack = false;

    private void Start()
    {
        heroAnimationController = GetComponent<HeroAnimationController>();
        rb = GetComponent<Rigidbody2D>();
        SetState(HeroState.Walk); // langsung jalan saat spawn
    }

    private void FixedUpdate()
    {
        if (heroState == HeroState.Walk && !isKnockedBack)
        {
            rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
        }
        else if (!isKnockedBack)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    private void Update()
    {
        switch (heroState)
        {
            case HeroState.Walk:
                heroAnimationController.SetWalk(true);
                RaycastHit2D hit = Physics2D.Raycast(
                    transform.position,
                    Vector2.right,
                    detectionRange,
                    enemyLayer
                );
                if (hit.collider != null)
                {
                    SetState(HeroState.Attack);
                }
                break;

            case HeroState.Attack:
                heroAnimationController.SetWalk(false);
                if (!attackTriggered)
                {
                    Debug.Log("Attack not triggered!");
                    // heroAnimationController.SetAttack(true);
                    attackTriggered = true;
                    Invoke(nameof(ResumeAfterAttack), 0.5f);
                }
                break;

            case HeroState.Hit:
                heroAnimationController.SetWalk(false);
                if (!hitTriggered)
                {
                    heroAnimationController.TriggerHit();
                    hitTriggered = true;
                    Invoke(nameof(ResumeAfterHit), 0.3f);
                }
                break;

            case HeroState.Defeated:
                heroAnimationController.SetWalk(false);
                heroAnimationController.SetDefeated();
                enabled = false; // disable state machine setelah mati
                break;
        }
    }

    public void SetState(HeroState newState)
    {
        heroState = newState;
        attackTriggered = false;
        hitTriggered = false;
    }

    public void OnTakeDamage()
    {
        if (heroState != HeroState.Defeated)
        {
            isKnockedBack = true;
            Invoke(nameof(EndKnockback), 0.2f);
            SetState(HeroState.Hit);
        }
    }

    private void EndKnockback()
    {
        isKnockedBack = false;
    }

    private void ResumeAfterHit()
    {
        if (heroState != HeroState.Defeated)
        {
            SetState(HeroState.Walk);
        }
    }

    private void ResumeAfterAttack()
    {
        if (heroState != HeroState.Defeated)
        {
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position,
                Vector2.right,
                detectionRange,
                enemyLayer
            );
            if (hit.collider != null)
            {
                SetState(HeroState.Attack);
            }
            else
            {
                heroAnimationController.SetAttack(false);
                SetState(HeroState.Walk);
            }
        }
    }

    public void OnDefeated()
    {
        SetState(HeroState.Defeated);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * detectionRange);
    }
}

public enum HeroState
{
    Idle,
    Walk,
    Attack,
    Hit,
    Defeated,
}
