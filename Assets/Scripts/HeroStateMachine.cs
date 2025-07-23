using UnityEngine;

public class HeroStateMachine : MonoBehaviour
{
    public HeroState heroState = HeroState.Idle;
    public float detectionRange = 6f;
    public LayerMask enemyLayer;
    public float moveSpeed = 2f;

    public float damage = 10f;
    public float knockbackForce = 5f;

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
                    Debug.Log("Attack not triggered by Kancil!");
                    heroAnimationController.SetAttack(true);
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
                    // Apply damage & knockback partway through the attack animation
                    Invoke(nameof(PerformAttack), 0.25f);

                    // Check what to do after the full attack cycle
                    Invoke(nameof(ResumeAfterHit), 0.3f);
                }
                break;

            case HeroState.Defeated:
                Debug.Log("Kancil Defeated State");
                heroAnimationController.SetWalk(false);

                gameObject.layer = LayerMask.NameToLayer("Dead");
                // ---------------------

                // Stop all movement and physics
                rb.bodyType = RigidbodyType2D.Static;
                rb.linearVelocity = Vector2.zero;

                GetComponent<Collider2D>().enabled = false;

                heroAnimationController.SetDefeated();
                Destroy(gameObject, 2);
                enabled = false; // disable state machine setelah mati
                break;
        }
    }

    private void PerformAttack()
    {
        // Find the enemy in front of us to hit
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.right,
            detectionRange,
            enemyLayer
        );

        // Make sure we are still in range of an enemy
        if (hit.collider != null)
        {
            // Get the enemy's components
            EnemyStateMachine enemyState = hit.collider.GetComponent<EnemyStateMachine>();
            Rigidbody2D enemyRb = hit.collider.GetComponent<Rigidbody2D>();
            EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>(); // Assuming enemy has this script

            // Apply damage
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage((int)damage);
            }

            // Apply knockback
            if (enemyState != null && enemyRb != null)
            {
                // Tell the enemy's state machine it was hit
                enemyState.OnTakeDamage();

                // Apply the knockback force to the enemy's Rigidbody2D
                enemyRb.AddForce(Vector2.right * knockbackForce, ForceMode2D.Impulse);
            }
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
