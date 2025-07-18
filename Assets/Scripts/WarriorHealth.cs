using UnityEngine;
using System.Collections;

public class WarriorHealth : MonoBehaviour

{
    public int health;
    public int maxHealth = 50;
    public float delayTime = .15f;
    public WarriorMovement warriorMovement;

    void Start()
    {
        health = maxHealth;

        // Prevent NullReferenceException if forgot to assign in Inspector
        if (warriorMovement == null)
        {
            warriorMovement = GetComponent<WarriorMovement>();
        }
    }

    public void TakeDamage(int damage) 
    {
        health -= damage;
        StartCoroutine(knockbackDelay());
    }

    IEnumerator knockbackDelay()
    {
        if (warriorMovement != null)
            warriorMovement.enabled = false;

        yield return new WaitForSeconds(delayTime);

        if (health <= 0)
        {
            Destroy(gameObject);
        }
        else 
        {
            if (warriorMovement != null)
                warriorMovement.enabled = true;
        }
    }
}
