using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position : MonoBehaviour
{
    public Transform raycastOrigin;
    public Transform sphereTransform;
    public float height = 2f;
    [Range(-180f, 180f)]
    public float yawOffset = 0f;   // degrees, rotation around tangent normal
    [Range(-90f, 90f)]
    public float pitchOffset = 0f; // degrees, up/down tilt on tangent plane
    [Range(-180f, 180f)]
    public float rollOffset = 0f;  // degrees, roll around look direction

    void Update()
    {
        if (raycastOrigin == null || sphereTransform == null) return;
        Vector3 sphereCenter = sphereTransform.position;
        SphereCollider sphereCollider = sphereTransform.GetComponent<SphereCollider>();
        if (sphereCollider == null) return;

        // Raycast from raycastOrigin to sphere center
        Vector3 origin = raycastOrigin.position;
        Vector3 direction = (sphereCenter - origin).normalized;
        Ray ray = new Ray(origin, direction);
        RaycastHit hit;
        float maxDist = Vector3.Distance(origin, sphereCenter) + sphereCollider.radius * sphereTransform.localScale.x;
        if (sphereCollider.Raycast(ray, out hit, maxDist))
        {
            // Place camera at tangent point with height offset
            Vector3 tangentNormal = (hit.point - sphereCenter).normalized;
            Vector3 newPos = hit.point + tangentNormal * height;
            transform.position = newPos;

            // Stable reference direction: use tangentRight (perpendicular to tangentNormal and world up)
            Vector3 tangentRight = Vector3.Cross(Vector3.up, tangentNormal).normalized;
            if (tangentRight.sqrMagnitude < 0.001f)
            {
                tangentRight = Vector3.Cross(Vector3.forward, tangentNormal).normalized;
            }

            // Apply yaw (around tangent normal)
            Vector3 lookDir = Quaternion.AngleAxis(yawOffset, tangentNormal) * tangentRight;

            // Apply pitch (rotate around axis perpendicular to lookDir and tangentNormal)
            Vector3 pitchAxis = Vector3.Cross(lookDir, tangentNormal).normalized;
            lookDir = Quaternion.AngleAxis(pitchOffset, pitchAxis) * lookDir;

            // Apply roll (rotate up vector around lookDir)
            Vector3 upDir = tangentNormal;
            upDir = Quaternion.AngleAxis(rollOffset, lookDir) * upDir;

            transform.rotation = Quaternion.LookRotation(lookDir, upDir);
        }
    }
}
