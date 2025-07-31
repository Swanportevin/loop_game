using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject zombiePrefab;       // Reference to the zombie prefab
    public int maxZombies = 10;           // Max zombies to spawn
    public float spawnAreaSize = 1000f;    // Size of the spawn area
    public GameObject PlayerOrigin;       // Player reference for offset
    public GameObject ParentObject;       // Parent object for spawned zombies

    private int zombieCount = 0;

    void Start()
    {
        StartCoroutine(SpawnZombiesLoop());
    }

    private IEnumerator SpawnZombiesLoop()
    {
        while (zombieCount < maxZombies)
        {
            Vector3 spawnPosition = GetRandomPositionInArea();
            Instantiate(zombiePrefab, spawnPosition, Quaternion.identity, ParentObject.transform);
            zombieCount++;

            yield return new WaitForSeconds(Random.Range(1f, 5f)); // Wait before spawning the next one
        }
    }

    Vector3 GetRandomPositionInArea()
    {
        float halfSize = spawnAreaSize / 2f;
        float x = Random.Range(-halfSize, halfSize);
        float z = Random.Range(-halfSize, halfSize);
        float y = 2.5f; // Slightly above ground
        Debug.Log(x);
        Debug.Log(z);

        return PlayerOrigin.transform.position + new Vector3(x, y, z);
    }
}
