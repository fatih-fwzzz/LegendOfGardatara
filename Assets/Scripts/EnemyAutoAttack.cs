using UnityEngine;

public class EnemyAutoAttack : MonoBehaviour
{
    public float attackRange = 2f;
    public float attackCooldown = 1f;
    public float damage = 50f;
    public LayerMask heroLayer;
    public LayerMask towerLayer; 

    private EnemyAnimationController enemyAnimationController;
    private float lastAttackTime;

    private void Start()
    {
        enemyAnimationController = GetComponent<EnemyAnimationController>();
    }

    private void Update()
    {
        Collider2D enemy = Physics2D.OverlapCircle(transform.position, attackRange, heroLayer);
        Collider2D tower = Physics2D.OverlapCircle(transform.position, attackRange, towerLayer);

        if (enemy != null)
        {
            enemyAnimationController.SetWalk(false);

            if (Time.time - lastAttackTime >= attackCooldown)
            {
                Attack(enemy.gameObject);
                lastAttackTime = Time.time;
            }
        }
        else if (tower != null)
        {
            enemyAnimationController.SetWalk(false);

            if (Time.time - lastAttackTime >= attackCooldown)
            {
                AttackTower(tower.gameObject);
                lastAttackTime = Time.time;
            }
        }
        else
        {
            enemyAnimationController.SetAttack(false);
            enemyAnimationController.SetWalk(true);
        }
    }

    private void Attack(GameObject enemyObject)
    {
        enemyAnimationController.SetAttack(true);

        EnemyHealth enemyHealth = enemyObject.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage((int)damage);
        }
    }

    private void AttackTower(GameObject towerObject)
    {
        enemyAnimationController.SetAttack(true);

        TowerHealthAnimated heroTowerHealth = towerObject.GetComponent<TowerHealthAnimated>();
        if (heroTowerHealth != null)
        {
            heroTowerHealth.TakeDamage((int)damage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

