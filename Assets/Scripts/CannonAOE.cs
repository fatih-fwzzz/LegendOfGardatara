using UnityEngine;

public class CannonProjectile : MonoBehaviour
{
    [Header("Behavior Settings")]
    public float speed = 10f;
    public float damage = 50f;
    public float arcHeight = 5f;

    [Header("Targeting")]
    public float searchRadius = 20f;
    public string towerTag = "TowerEnemy";
    public LayerMask enemyLayer; // Set this to your Enemy layer

    [Header("Effects")]
    public Transform spriteTransform;
    public GameObject explosionEffectPrefab;
    public float spinSpeed = 400f;

    // Internal State
    private Rigidbody2D rb;
    private Transform target;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float journeyDistance;
    private bool hasCollided = false;

    public TowerHealthAnimated towerHealthAnimated;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        FindTarget();
    }

    private void Update()
    {
        if (spriteTransform != null)
        {
            spriteTransform.Rotate(0, 0, spinSpeed * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        // Homing movement logic (this part remains the same)
        if (target != null)
        {
            // Vector2 direction = ((Vector3)target.position - transform.position).normalized;
            // rb.linearVelocity = direction * speed;
            // float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            // transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            // Update the target's position each frame in case it moves
            targetPosition = target.position;

            // If the target is extremely close, the arc math can fail.
            // In that case, just move directly towards it.
            if (journeyDistance < 0.1f)
            {
                Vector2 directDirection = (targetPosition - startPosition).normalized;
                rb.linearVelocity = directDirection * speed;
                return; // Skip the rest of the arc calculation
            }

            // --- ARC CALCULATION ---
            float distanceCovered = (transform.position - startPosition).magnitude;
            float fractionOfJourney = distanceCovered / journeyDistance;

            // Calculate the next point on the straight line path
            Vector3 direction = (targetPosition - startPosition).normalized;
            Vector3 nextLinearPoint =
                startPosition + direction * (distanceCovered + speed * Time.fixedDeltaTime);

            // Add the arc height
            // Mathf.Sin creates a smooth 0 -> 1 -> 0 curve over the journey
            float arc = arcHeight * Mathf.Sin(fractionOfJourney * Mathf.PI);
            nextLinearPoint.y += arc;

            // Calculate the velocity needed to move to the next point in the arc
            Vector2 velocityToNextPoint = (nextLinearPoint - transform.position).normalized * speed;
            rb.linearVelocity = velocityToNextPoint;

            // Optional: Rotate to face the direction of movement
            if (rb.linearVelocity != Vector2.zero)
            {
                float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
                if (spriteTransform != null)
                {
                    // Aim the parent empty object, not the spinning sprite
                    transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                }
            }
        }
        else
        {
            Destroy(gameObject, 3f);
        }
    }

    /// <summary>
    /// Finds the nearest enemy, or the nearest "TowerEnemy" if no enemies are in range.
    /// </summary>
    private void FindTarget()
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        // --- STEP 1: Find the nearest Enemy ---
        Collider2D[] enemyColliders = Physics2D.OverlapCircleAll(
            currentPosition,
            searchRadius,
            enemyLayer
        );

        foreach (Collider2D enemyCollider in enemyColliders)
        {
            Vector3 directionToTarget = enemyCollider.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = enemyCollider.transform;
            }
        }

        // If we found an enemy, lock on and finish.
        if (bestTarget != null)
        {
            target = bestTarget;
            return;
        }

        // --- STEP 2: If no enemy found, find the nearest "TowerEnemy" ---
        GameObject[] enemyTowers = GameObject.FindGameObjectsWithTag(towerTag);
        closestDistanceSqr = Mathf.Infinity; // Reset distance for the new search

        if (enemyTowers.Length > 0)
        {
            foreach (GameObject tower in enemyTowers)
            {
                Vector3 directionToTarget = tower.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = tower.transform;
                }
            }
        }

        // If we found an enemy tower, lock onto it.
        if (bestTarget != null)
        {
            target = bestTarget;
        }

        if (target != null)
        {
            targetPosition = target.position;
            journeyDistance = Vector3.Distance(startPosition, targetPosition);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasCollided)
            return;

        // Check if we hit an enemy
        if (((1 << collision.gameObject.layer) & enemyLayer) != 0)
        {
            hasCollided = true;
            // Assuming the enemy has an EnemyHealth script
            collision.gameObject.GetComponentInParent<EnemyHealth>()?.TakeDamage((int)damage);
            ExplodeAndDestroy();
        }
        // Check if we hit the enemy tower
        else if (collision.gameObject.CompareTag("TowerEnemy"))
        {
            hasCollided = true;

            // --- ADVANCED DEBUGGING ---
            Debug.Log(
                "Projectile collided with object named: '"
                    + collision.gameObject.name
                    + "' which has the correct tag."
            );

            EnemyTowerHealth towerHealth =
                collision.gameObject.GetComponentInParent<EnemyTowerHealth>();

            if (towerHealth != null)
            {
                // If this works, you will see this message
                Debug.Log(
                    "SUCCESS: Found TowerHealthAnimated script on parent: "
                        + towerHealth.gameObject.name
                );
                towerHealth.TakeDamage((int)damage);
            }
            else
            {
                // This is the error you are currently seeing.
                Debug.LogError(
                    "ERROR: Could not find TowerHealthAnimated script on '"
                        + collision.gameObject.name
                        + "' or any of its parents."
                );

                // This extra loop will show you every parent it checked.
                Transform parentToCheck = collision.transform;
                while (parentToCheck != null)
                {
                    Debug.LogWarning("Searched for script on: " + parentToCheck.name);
                    parentToCheck = parentToCheck.parent;
                }
            }

            ExplodeAndDestroy();
        }
        // If it hits anything else (like the ground or player tower), just explode
        else
        {
            hasCollided = true;
            ExplodeAndDestroy();
        }
    }

    private void ExplodeAndDestroy()
    {
        // if (explosionEffectPrefab != null)
        // {
        //     Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        // }
        Destroy(gameObject);
    }
}
