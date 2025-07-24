using UnityEngine;

public class GameManager : MonoBehaviour
{
    public EnemyTowerHealth enemyTowerHealth;
    public HeroStateMachine heroStateMachine; 
    public GameObject heroPrefab; 
    public Transform enemyTowerSpawnPoint; 
    public BossController bossController; 

    private bool hasWon = false;

    void Update()
    {
        if (!hasWon && enemyTowerHealth.currentHealth
 <= 0)
        {
            WinCondition();
        }
    }

    void WinCondition()
    {
        hasWon = true;

        Debug.Log("Victory!");

        // Hero berhenti dan idle
        heroStateMachine.SetState(HeroState.Idle);

        // Spawn hero baru di tower enemy
        Instantiate(heroPrefab, enemyTowerSpawnPoint.position, Quaternion.identity);

        //Boss kabur
        bossController.Flee();
    }
}