using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject zombiePrefab;       // Reference to the zombie prefab
    public int maxZombies = 10;           // Max zombies to spawn
    public float spawnAreaSize = 1000f;    // Size of the spawn area
    public GameObject PlayerOrigin;       // Player reference for offset
    public GameObject ParentObject;       // Parent object for spawned zombies
    public int zombieCount = 0;
    private int instanceCount = 0;
    
    void Start()
    {
        StartCoroutine(SpawnZombiesLoop());
    }

    private IEnumerator SpawnZombiesLoop()
    {
        while (zombieCount < maxZombies)
        {
            Vector3 spawnPosition = GetRandomPositionInArea();
            GameObject obj = Instantiate(zombiePrefab, spawnPosition, Quaternion.identity, ParentObject.transform);
            obj.name = "ZombieNumber"+instanceCount.ToString();
            zombieCount++;
            instanceCount++;
            yield return new WaitForSeconds(Random.Range(1f, 5f)); // Wait before spawning the next one
        }
    }

    Vector3 GetRandomPositionInArea()
    {
        float halfSize = spawnAreaSize / 2f;
        float x = Random.Range(-halfSize, halfSize);
        float z = Random.Range(-halfSize, halfSize);
        float y = 0f; // Slightly above ground

        return PlayerOrigin.transform.position + new Vector3(x, y, z);
    }
}