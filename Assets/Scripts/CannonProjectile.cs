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
    public LayerMask enemyLayer;

    [Header("Effects")]
    public Transform spriteTransform;
    public GameObject explosionEffectPrefab;
    public float spinSpeed = 400f;

   
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
        if (target != null)
        {
        
            targetPosition = target.position;

          
            if (journeyDistance < 0.1f)
            {
                Vector2 directDirection = (targetPosition - startPosition).normalized;
                rb.linearVelocity = directDirection * speed;
                return; 
            }

            float distanceCovered = (transform.position - startPosition).magnitude;
            float fractionOfJourney = distanceCovered / journeyDistance;

            Vector3 direction = (targetPosition - startPosition).normalized;
            Vector3 nextLinearPoint =
                startPosition + direction * (distanceCovered + speed * Time.fixedDeltaTime);

          
            float arc = arcHeight * Mathf.Sin(fractionOfJourney * Mathf.PI);
            nextLinearPoint.y += arc;

            Vector2 velocityToNextPoint = (nextLinearPoint - transform.position).normalized * speed;
            rb.linearVelocity = velocityToNextPoint;

            if (rb.linearVelocity != Vector2.zero)
            {
                float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
                if (spriteTransform != null)
                {
                    
                    transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                }
            }
        }
        else
        {
            Destroy(gameObject, 3f);
        }
    }

   
    private void FindTarget()
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        // deteksi enemy terdekat
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

      
        if (bestTarget != null)
        {
            target = bestTarget;
            return;
        }

        // jika tidak ada enemy, serang tower enemy
        GameObject[] enemyTowers = GameObject.FindGameObjectsWithTag(towerTag);
        closestDistanceSqr = Mathf.Infinity; 

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

        
        if (((1 << collision.gameObject.layer) & enemyLayer) != 0)
        {
            hasCollided = true;
            
            collision.gameObject.GetComponentInParent<EnemyHealth>()?.TakeDamage((int)damage);
            ExplodeAndDestroy();
        }
       
        else if (collision.gameObject.CompareTag("TowerEnemy"))
        {
            hasCollided = true;

          
            Debug.Log(
                "Projectile collided with object named: '"
                    + collision.gameObject.name
                    + "' which has the correct tag."
            );

            EnemyTowerHealth towerHealth =
                collision.gameObject.GetComponentInParent<EnemyTowerHealth>();

            if (towerHealth != null)
            {
               
                Debug.Log(
                    "SUCCESS: Found TowerHealthAnimated script on parent: "
                        + towerHealth.gameObject.name
                );
                towerHealth.TakeDamage((int)damage);
            }
            else
            {
              
                Debug.LogError(
                    "ERROR: Could not find TowerHealthAnimated script on '"
                        + collision.gameObject.name
                        + "' or any of its parents."
                );

           
                Transform parentToCheck = collision.transform;
                while (parentToCheck != null)
                {
                    Debug.LogWarning("Searched for script on: " + parentToCheck.name);
                    parentToCheck = parentToCheck.parent;
                }
            }

            ExplodeAndDestroy();
        }
      
        else
        {
            hasCollided = true;
            ExplodeAndDestroy();
        }
    }

    private void ExplodeAndDestroy()
    {
      
        Destroy(gameObject);
    }
}
