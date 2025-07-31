using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject chunk;
    private List<GameObject> trees;
    private List<GameObject> rocks;
    private List<GameObject> grass;
    public List<GameObject> zombies;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public GameObject CreateNewChunk(Vector3 MiddlePoint, Vector3 Vector1, Vector3 Vector2, Vector3 Vector3)
    {
        GameObject chunk = new GameObject("Chunk");

        // For grass
        int grassCount = Random.Range(0, 6);
        for (int i = 0; i < grassCount; i++)
        {
            float a = Random.Range(0, 1);
            float b = Random.Range(0, 1);
            float c = Random.Range(0, 1);
            Vector3 treePosition = MiddlePoint + a*Vector1 + b*Vector2 + c*Vector3; // Y = 0 for flat terrain

            // 4. Instantiate the tree and parent it to the chunk
            GameObject tree = Instantiate(grass[Random.Range(0, grass.Count)], treePosition, Quaternion.identity);
            tree.transform.SetParent(chunk.transform);
        }

        // For rocks
        int rockCount = Random.Range(0, 3);
        for (int i = 0; i < rockCount; i++)
        {
            float a = Random.Range(0, 1);
            float b = Random.Range(0, 1);
            float c = Random.Range(0, 1);
            Vector3 treePosition = MiddlePoint + a*Vector1 + b*Vector2 + c*Vector3; // Y = 0 for flat terrain

            // 4. Instantiate the tree and parent it to the chunk
            GameObject tree = Instantiate(rocks[Random.Range(0, rocks.Count)], treePosition, Quaternion.identity);
            tree.transform.SetParent(chunk.transform);
        }

        // For trees
        int treeCount = Random.Range(0, 3);
        for (int i = 0; i < treeCount; i++)
        {
            float a = Random.Range(0, 1);
            float b = Random.Range(0, 1);
            float c = Random.Range(0, 1);
            Vector3 treePosition = MiddlePoint + a*Vector1 + b*Vector2 + c*Vector3; // Y = 0 for flat terrain

            // 4. Instantiate the tree and parent it to the chunk
            GameObject tree = Instantiate(trees[Random.Range(0, trees.Count)], treePosition, Quaternion.identity);
            tree.transform.SetParent(chunk.transform);
        }

        // 5. Return the completed chunk GameObject
        return chunk;
    }
}
