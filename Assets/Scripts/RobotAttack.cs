using UnityEngine;

public class RobotAttack : MonoBehaviour
{
   
    public int damage = 25;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.hitSFX);
        TowerHealthAnimated towerHealthAnimated = collision.gameObject.GetComponent<TowerHealthAnimated>();

        if (towerHealthAnimated != null)
        {
            towerHealthAnimated.TakeDamage(damage);
            // Destroy(gameObject); // hancurkan projectile robot setelah serangan
        }
    }
}
