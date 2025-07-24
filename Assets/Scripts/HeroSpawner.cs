using UnityEngine;

public class HeroSpawner : MonoBehaviour
{
    [Header("Hero Spawn Settings")]
    public GameObject heroPrefab;
    public Transform[] spawnPoints; // Drag semua spawn points ke sini via Inspector

    /// <summary>
    /// Dipanggil dari tombol untuk spawn hero secara random.
    /// </summary>
    public void SummonHeroRandom()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("⚠️ Tidak ada spawn points terdaftar!");
            return;
        }

        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];

        Instantiate(heroPrefab, spawnPoint.position, Quaternion.identity);
    }
}
