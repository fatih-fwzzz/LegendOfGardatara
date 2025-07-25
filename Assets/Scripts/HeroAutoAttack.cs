using UnityEngine;

public class HeroAutoAttack : MonoBehaviour
{
    public float attackRange = 2f;
    public float attackCooldown = 1f;
    public float damage = 50f;
    public LayerMask enemyLayer;
    public LayerMask towerLayer; 

    private HeroAnimationController heroAnimationController;
    private float lastAttackTime;

    private void Start()
    {
        heroAnimationController = GetComponent<HeroAnimationController>();
    }

    private void Update()
    {
        Collider2D enemy = Physics2D.OverlapCircle(transform.position, attackRange, enemyLayer);
        Collider2D tower = Physics2D.OverlapCircle(transform.position, attackRange, towerLayer);

        if (enemy != null)
        {
            heroAnimationController.SetWalk(false);

            if (Time.time - lastAttackTime >= attackCooldown)
            {
                Attack(enemy.gameObject);
                lastAttackTime = Time.time;
            }
        }
        else if (tower != null)
        {
            heroAnimationController.SetWalk(false);

            if (Time.time - lastAttackTime >= attackCooldown)
            {
                AttackTower(tower.gameObject);
                lastAttackTime = Time.time;
            }
        }
        else
        {
            heroAnimationController.SetAttack(false);
            heroAnimationController.SetWalk(true);
        }
    }

    private void Attack(GameObject enemyObject)
    {
        heroAnimationController.SetAttack(true);
        AudioManager.Instance.PlaySFX(AudioManager.Instance.hitSFX);
        


        EnemyHealth enemyHealth = enemyObject.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage((int)damage);
        }
    }

    private void AttackTower(GameObject towerObject)
    {

        heroAnimationController.SetAttack(true);

        AudioManager.Instance.PlaySFX(AudioManager.Instance.hitSFX);

        EnemyTowerHealth enemyTowerHealth = towerObject.GetComponent<EnemyTowerHealth>();
        if (enemyTowerHealth != null)
        {
            enemyTowerHealth.TakeDamage((int)damage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
