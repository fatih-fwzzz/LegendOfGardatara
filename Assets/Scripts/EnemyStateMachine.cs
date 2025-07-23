using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    public EnemyState enemyState = EnemyState.Idle;
    public float detectionRange = 6f;
    public LayerMask heroLayer;
    public float moveSpeed = 2f;

    public float damage = 10f;
    public float knockbackForce = 5f;

    private EnemyAnimationController enemyAnimationController;
    private Rigidbody2D rb;

    private bool attackTriggered = false;
    private bool hitTriggered = false;
    private bool isKnockedBack = false;

    private void Start()
    {
        enemyAnimationController = GetComponent<EnemyAnimationController>();
        rb = GetComponent<Rigidbody2D>();
        SetState(EnemyState.Walk); // langsung jalan saat spawn
    }

    private void FixedUpdate()
    {
        if (enemyState == EnemyState.Walk && !isKnockedBack)
        {
            rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y);
        }
        else if (!isKnockedBack)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    private void Update()
    {
        switch (enemyState)
        {
            case EnemyState.Walk:
                enemyAnimationController.SetWalk(true);
                RaycastHit2D hit = Physics2D.Raycast(
                    transform.position,
                    Vector2.left,
                    detectionRange,
                    heroLayer
                );
                if (hit.collider != null)
                {
                    SetState(EnemyState.Attack);
                }
                break;

            case EnemyState.Attack:
                enemyAnimationController.SetWalk(false);
                if (!attackTriggered)
                {
                    Debug.Log("Attack not triggered by Octopus!");
                    enemyAnimationController.SetAttack(true);
                    attackTriggered = true;
                    Invoke(nameof(PerformAttack), 0.25f);
                    // Decide what to do after the attack cycle
                    Invoke(nameof(ResumeAfterAttack), 0.5f);
                }
                break;

            case EnemyState.Hit:
                enemyAnimationController.SetWalk(false);
                if (!hitTriggered)
                {
                    enemyAnimationController.TriggerHit();
                    hitTriggered = true;

                    // Check what to do after the full attack cycle
                    Invoke(nameof(ResumeAfterHit), 0.3f);
                }
                break;

            case EnemyState.Defeated:
                Debug.Log("Octopus Defeated State");
                enemyAnimationController.SetWalk(false);

                gameObject.layer = LayerMask.NameToLayer("Dead");
                // ---------------------

                // Stop all movement and physics
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Static;

                enemyAnimationController.SetDefeated();
                Destroy(gameObject, 2f);
                enabled = false;
                break;
        }
    }

    private void PerformAttack()
    {
        // Find the enemy in front of us to hit
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.left,
            detectionRange,
            heroLayer
        );

        if (hit.collider != null)
        {
            // CORRECT: Get HERO components, not enemy components
            HeroStateMachine heroState = hit.collider.GetComponent<HeroStateMachine>();
            Rigidbody2D heroRb = hit.collider.GetComponent<Rigidbody2D>();
            HeroHealth heroHealth = hit.collider.GetComponent<HeroHealth>(); // Assuming hero has this script

            // Apply damage to hero
            if (heroHealth != null)
            {
                heroHealth.TakeDamage((int)damage);
            }

            // Apply knockback to hero
            if (heroState != null && heroRb != null)
            {
                heroState.OnTakeDamage();
                // CORRECT: Push hero LEFT (away from the enemy)
                heroRb.AddForce(Vector2.left * knockbackForce, ForceMode2D.Impulse);
            }
        }
    }

    public void SetState(EnemyState newState)
    {
        enemyState = newState;
        attackTriggered = false;
        hitTriggered = false;
    }

    public void OnTakeDamage()
    {
        if (enemyState != EnemyState.Defeated)
        {
            isKnockedBack = true;
            Invoke(nameof(EndKnockback), 0.2f);
            SetState(EnemyState.Hit);
        }
    }

    private void EndKnockback()
    {
        isKnockedBack = false;
    }

    private void ResumeAfterHit()
    {
        if (enemyState != EnemyState.Defeated)
        {
            // After being hit, go back to walking
            SetState(EnemyState.Walk);
        }
    }

    private void ResumeAfterAttack()
    {
        if (enemyState != EnemyState.Defeated)
        {
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position,
                Vector2.left,
                detectionRange,
                heroLayer
            );
            if (hit.collider != null)
            {
                SetState(EnemyState.Attack);
            }
            else
            {
                enemyAnimationController.SetAttack(false);
                SetState(EnemyState.Walk);
            }
        }
    }

    public void OnDefeated()
    {
        SetState(EnemyState.Defeated);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.left * detectionRange);
    }
}

public enum EnemyState
{
    Idle,
    Walk,
    Attack,
    Defeated,
    Hit,
}
