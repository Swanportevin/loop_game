using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Store the face vertices for Gizmos
    private Vector3[] gizmoFaceVertices = null;
    private Mesh cachedOriginalMesh = null;

    [Range(0, 100)]
    public int triangleIndex = 0; // Select which triangle to display

    private bool needsMeshUpdate = true;

    [ContextMenu("Create Triangle Prefab Object")]
    public void CreateTrianglePrefabObject()
    {
        if (cachedOriginalMesh == null)
        {
            MeshFilter mf = GetComponent<MeshFilter>();
            if (mf != null && mf.sharedMesh != null)
            {
                cachedOriginalMesh = mf.sharedMesh;
            }
        }
        if (cachedOriginalMesh == null)
        {
            Debug.LogError("No cached original mesh available.");
            return;
        }

        int[] triangles = cachedOriginalMesh.triangles;
        Vector3[] vertices = cachedOriginalMesh.vertices;

        if (triangles.Length < 3)
        {
            Debug.LogError("Original mesh does not have enough triangles.");
            return;
        }

        int triangleCount = triangles.Length / 3;
        int clampedIndex = Mathf.Clamp(triangleIndex, 0, triangleCount - 1);
        int triStart = clampedIndex * 3;
        int[] singleTriangle = new int[3] { triangles[triStart], triangles[triStart + 1], triangles[triStart + 2] };

        Vector3[] faceVertices = new Vector3[3]
        {
            vertices[singleTriangle[0]],
            vertices[singleTriangle[1]],
            vertices[singleTriangle[2]]
        };

        // Create a new GameObject
        GameObject triangleObj = new GameObject($"Triangle_{triangleIndex}");
        var meshFilter = triangleObj.AddComponent<MeshFilter>();
        var meshRenderer = triangleObj.AddComponent<MeshRenderer>();

        // Create the triangle mesh
        Mesh triangleMesh = new Mesh();
        triangleMesh.vertices = faceVertices;
        triangleMesh.triangles = new int[3] { 0, 1, 2 };
        triangleMesh.RecalculateNormals();
        meshFilter.sharedMesh = triangleMesh;

        // Optionally, assign a default material
        meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));

        // Select the new object in the editor
        #if UNITY_EDITOR
        UnityEditor.Selection.activeGameObject = triangleObj;
        #endif
    }

    private void UpdateSingleTriangleMesh()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null || cachedOriginalMesh == null)
            return;

        int[] triangles = cachedOriginalMesh.triangles;
        Vector3[] vertices = cachedOriginalMesh.vertices;

        if (triangles.Length < 3)
            return;

        int triangleCount = triangles.Length / 3;
        int clampedIndex = Mathf.Clamp(triangleIndex, 0, triangleCount - 1);
        int triStart = clampedIndex * 3;
        int[] singleTriangle = new int[3] { triangles[triStart], triangles[triStart + 1], triangles[triStart + 2] };

        Vector3[] faceVertices = new Vector3[3]
        {
            vertices[singleTriangle[0]],
            vertices[singleTriangle[1]],
            vertices[singleTriangle[2]]
        };

        gizmoFaceVertices = faceVertices;

        Mesh faceMesh = new Mesh();
        faceMesh.vertices = faceVertices;
        faceMesh.triangles = new int[3] { 0, 1, 2 };
        faceMesh.RecalculateNormals();

        meshFilter.sharedMesh = faceMesh;
    }

    void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null && meshFilter.sharedMesh != null)
        {
            cachedOriginalMesh = meshFilter.sharedMesh;
        }
        UpdateSingleTriangleMesh();
        needsMeshUpdate = false;
    }

    void Update()
    {
        if (needsMeshUpdate)
        {
            UpdateSingleTriangleMesh();
            needsMeshUpdate = false;
        }
    }

    void OnValidate()
    {
        needsMeshUpdate = true;
    }

    // Draw triangle and dimensions in the Scene view
    void OnDrawGizmos()
    {
        if (gizmoFaceVertices == null || gizmoFaceVertices.Length != 3)
            return;

        // Transform vertices to world space
        Vector3 v0 = transform.TransformPoint(gizmoFaceVertices[0]);
        Vector3 v1 = transform.TransformPoint(gizmoFaceVertices[1]);
        Vector3 v2 = transform.TransformPoint(gizmoFaceVertices[2]);

        // Draw triangle edges
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(v0, v1);
        Gizmos.DrawLine(v1, v2);
        Gizmos.DrawLine(v2, v0);

        // Calculate side lengths
        float len01 = Vector3.Distance(v0, v1);
        float len12 = Vector3.Distance(v1, v2);
        float len20 = Vector3.Distance(v2, v0);

        // Draw labels for each side
        #if UNITY_EDITOR
        UnityEditor.Handles.Label((v0 + v1) / 2, $"{len01:F3}");
        UnityEditor.Handles.Label((v1 + v2) / 2, $"{len12:F3}");
        UnityEditor.Handles.Label((v2 + v0) / 2, $"{len20:F3}");
        #endif
    }
}