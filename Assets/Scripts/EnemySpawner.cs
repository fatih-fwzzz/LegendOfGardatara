using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoints; // drag semua spawn point untuk enemy
    public float initialDelay = 3f; // 3 detik setelah game mulai
    public float waveInterval = 15f; // interval antar wave

    private void Start()
    {
        StartCoroutine(SpawnWaveSequence());
    }

    IEnumerator SpawnWaveSequence()
    {
        // Wave 1: 1 enemy, spawn langsung setelah initialDelay
        yield return new WaitForSeconds(initialDelay);
        SpawnEnemy();

        // Wave 2: 2 enemy, dalam 15 detik setelah wave 1
        yield return new WaitForSeconds(waveInterval);
        yield return StartCoroutine(SpawnWave(2, waveInterval));

        // Wave 3: 3 enemy, dalam 15 detik setelah wave 2
        yield return new WaitForSeconds(waveInterval);
        yield return StartCoroutine(SpawnWave(3, waveInterval));

        // Wave 4: 4 enemy, dalam 15 detik setelah wave 3
        yield return new WaitForSeconds(waveInterval);
        yield return StartCoroutine(SpawnWave(4, waveInterval));

        Debug.Log("✅ Semua wave spawn selesai.");
    }

    IEnumerator SpawnWave(int enemyCount, float duration)
    {
        float interval = duration / enemyCount;

        for (int i = 0; i < enemyCount; i++)
        {
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
}
