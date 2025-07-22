using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoints; // drag semua spawn point untuk enemy
    public float initialDelay = 3f; // 3 detik setelah game mulai
    public float waveInterval = 15f; // interval antar wave

    [Header("Dependencies")]
    public EnemyTowerHealth enemyTowerHealth; // drag EnemyTowerHealth dari scene ke sini

    private void Start()
    {
        if (enemyTowerHealth == null)
        {
            enemyTowerHealth = FindObjectOfType<EnemyTowerHealth>();
        }

        StartCoroutine(SpawnWaveSequence());
    }

    IEnumerator SpawnWaveSequence()
    {
        yield return new WaitForSeconds(initialDelay);
        if (IsTowerDestroyed()) yield break;

        SpawnEnemy();

        yield return new WaitForSeconds(waveInterval);
        if (IsTowerDestroyed()) yield break;

        yield return StartCoroutine(SpawnWave(2, waveInterval));

        yield return new WaitForSeconds(waveInterval);
        if (IsTowerDestroyed()) yield break;

        yield return StartCoroutine(SpawnWave(3, waveInterval));

        yield return new WaitForSeconds(waveInterval);
        if (IsTowerDestroyed()) yield break;

        yield return StartCoroutine(SpawnWave(4, waveInterval));

        Debug.Log("✅ Semua wave spawn selesai.");
    }

    IEnumerator SpawnWave(int enemyCount, float duration)
    {
        float interval = duration / enemyCount;

        for (int i = 0; i < enemyCount; i++)
        {
            if (IsTowerDestroyed()) yield break;

            SpawnEnemy();
            yield return new WaitForSeconds(interval);
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefab != null && spawnPoints.Length > 0)
        {
            Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(enemyPrefab, randomPoint.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("[EnemySpawner] EnemyPrefab atau SpawnPoints belum diatur!");
        }
    }

    bool IsTowerDestroyed()
    {
        if (enemyTowerHealth != null && enemyTowerHealth.currentHealth <= 0)
        {
            Debug.Log("⛔ Enemy tower destroyed. Stop spawning waves.");
            return true;
        }
        return false;
    }
}
