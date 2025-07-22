using UnityEngine;

public class GameManager : MonoBehaviour
{
    public EnemyTowerHealth enemyTowerHealth;
    public HeroStateMachine heroStateMachine; // drag hero utama
    public GameObject heroPrefab; // prefab hero baru yang akan di-spawn
    public Transform enemyTowerSpawnPoint; // posisi spawn hero baru
    public BossController bossController; // script boss

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

        Debug.Log("🏆 Victory!");

        // 1️⃣ Hero berhenti dan idle
        heroStateMachine.SetState(HeroState.Idle);

        // 2️⃣ Spawn hero baru di tower enemy
        Instantiate(heroPrefab, enemyTowerSpawnPoint.position, Quaternion.identity);

        // 3️⃣ Boss kabur
        bossController.Flee();
    }
}