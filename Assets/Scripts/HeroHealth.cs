using UnityEngine;

public class HeroHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int health;
    public int maxHealth = 100;

    [Header("Sprites (Optional)")]
    public Sprite[] heroSprites;
    public SpriteRenderer heroSr;

    private HeroStateMachine stateMachine;
    private bool isDefeated = false;

    private void Awake()
    {
        health = maxHealth;
        stateMachine = GetComponent<HeroStateMachine>();
    }

    private void Update()
    {
       
        if (isDefeated)
            return;

       
        if (heroSprites != null && heroSprites.Length > 0 && heroSr != null)
        {
            float healthPercentage = (float)health / maxHealth;
            int spriteIndex = Mathf.Clamp(
                Mathf.FloorToInt(healthPercentage * heroSprites.Length - 1),
                0,
                heroSprites.Length - 1
            );
            heroSr.sprite = heroSprites[spriteIndex];
        }
    }

   
    public void TakeDamage(int amount)
    {
        if (isDefeated)
            return; 

        health -= amount;
        health = Mathf.Clamp(health, 0, maxHealth);
        Debug.Log($"{gameObject.name} HP: {health}/{maxHealth}");

      
        if (health <= 0)
        {
            isDefeated = true;
            if (stateMachine != null)
            {
                stateMachine.OnDefeated();
            }
            else
            {
             
                Destroy(gameObject);
            }
        }
    }
}
