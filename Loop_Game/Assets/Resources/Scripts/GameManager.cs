using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject zombiePrefab; // Reference to the zombie prefab
    private Vector3 spawnPoint; // Point where zombies will spawn
    private int zombieCount = 0; // Counter for spawned zombies
    public int maxZombies = 10; // Maximum number of zombies allowed
    public float spawnAreaSize = 20f;
    public GameObject PlayerOrigin;

    void Start()
    {
        SpawnZombies();
    }

    void SpawnZombies()
    {
        for (int i = 0; i < maxZombies; i++)
        {
            Vector3 spawnPosition = GetRandomPositionInArea();
            Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);
            zombieCount++;
        }
    }

    Vector3 GetRandomPositionInArea()
    {
        float halfSize = spawnAreaSize / 2f;
        float x = Random.Range(-halfSize, halfSize);
        float z = Random.Range(-halfSize, halfSize);
        float y = 0f; // Ground level

        return PlayerOrigin.transform.position + new Vector3(x, y+3, z);
    }
}
