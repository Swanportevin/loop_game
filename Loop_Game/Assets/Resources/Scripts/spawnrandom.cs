using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnrandom : MonoBehaviour
{
    // Quad to spawn spheres on
    public GameObject quad;
    // Sphere prefab to spawn
    public GameObject Prefab;
    // Number of objects to spawn
    public int sphereCount = 10;

    void Start()
    {
        if (quad == null || Prefab == null)
        {
            Debug.LogWarning("spawnrandom: Quad or Prefab not assigned.");
            return;
        }

        MeshRenderer quadRenderer = quad.GetComponent<MeshRenderer>();
        if (quadRenderer == null)
        {
            Debug.LogWarning("spawnrandom: Quad does not have a MeshRenderer.");
            return;
        }

        Bounds bounds = quadRenderer.bounds;
        for (int i = 0; i < sphereCount; i++)
        {
            // Random position within quad bounds
            float x = Random.Range(bounds.min.x, bounds.max.x);
            float z = Random.Range(bounds.min.z, bounds.max.z);
            float y = bounds.max.y + 0.5f; // Slightly above quad
            Vector3 spawnPos = new Vector3(x, y, z);
            GameObject onbject = Instantiate(Prefab, spawnPos, Quaternion.identity, transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
