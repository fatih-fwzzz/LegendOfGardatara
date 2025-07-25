using UnityEngine;

public class GameManager : MonoBehaviour
{
    public EnemyTowerHealth enemyTowerHealth;
    public HeroStateMachine heroStateMachine;
    public GameObject heroPrefab;

    public BossController bossController;
    public Vector3 bawangSpawnPosition = new Vector3(10.55f, -0.36f, 0f);

    private bool hasWon = false;

    void Update()
    {
        if (!hasWon && enemyTowerHealth.currentHealth <= 0)
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
        Instantiate(heroPrefab, bawangSpawnPosition, Quaternion.identity);

        //Boss kabur
        bossController.Flee();
    }
}
