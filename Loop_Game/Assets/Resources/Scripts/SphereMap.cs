using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SphereMap : MonoBehaviour
{
    // The object from which to cast the ray, selectable in Inspector
    public GameObject raycastSource;
    public GameObject objectContainer;
    public GameObject playerOrigin;

    void PlaceObjectsOnSphere()
    {
        if (raycastSource == null || objectContainer == null || playerOrigin == null) return;

        // Remove previous duplicates
        foreach (Transform t in transform)
        {
            if (t.gameObject != this.gameObject)
                Destroy(t.gameObject);
        }

        Vector3 sphereCenter = transform.position;
        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        float radius = sphereCollider != null ? sphereCollider.radius * transform.localScale.x : 1.0f;

        Vector3 raycastOrigin = raycastSource.transform.position;
        Vector3 playerPosition = playerOrigin.transform.position;
        int object_count = objectContainer.transform.childCount;

        // Get player yaw (rotation around y axis)
        float playerYaw = playerOrigin.transform.eulerAngles.y * Mathf.Deg2Rad;

        float diameter = radius * 2f;
        for (int i = 0; i < object_count; i++)
        {
            GameObject child = objectContainer.transform.GetChild(i).gameObject;
            Vector3 child_pos = child.transform.position;
            float height_offset = child_pos.y;

            // 1. Distance and angle from player
            Vector3 rel = child_pos - playerPosition;
            rel.y = 0f;
            float dist = rel.magnitude;
            if (dist > diameter)
            {
                // Don't clone if further than 1 diameter
                continue;
            }
            float angle = Mathf.Atan2(rel.z, rel.x); // angle in xz-plane

            // Add player yaw to angle so projection rotates with player view
            float rotatedAngle = angle + playerYaw;

            // 2. Calculate a point on a circle around the raycast origin hitpoint
            // First, raycast from raycastOrigin to sphere center to get hitpoint
            Vector3 toCenter = (sphereCenter - raycastOrigin).normalized;
            Ray ray = new Ray(raycastOrigin, toCenter);
            RaycastHit hit;
            Vector3 hitpoint = sphereCenter;
            if (sphereCollider != null && sphereCollider.Raycast(ray, out hit, Vector3.Distance(raycastOrigin, sphereCenter) + radius))
            {
                hitpoint = hit.point;
            }

            // 3. Calculate offset position on tangent plane at hitpoint
            Vector3 tangentRight = Vector3.Cross(Vector3.up, (hitpoint - sphereCenter).normalized).normalized;
            Vector3 tangentForward = Vector3.Cross((hitpoint - sphereCenter).normalized, tangentRight).normalized;
            Vector3 offsetOnPlane = hitpoint + tangentRight * Mathf.Cos(rotatedAngle) * dist + tangentForward * Mathf.Sin(rotatedAngle) * dist;

            // 4. Raycast from offset position (slightly off sphere) to center
            Vector3 normal = (offsetOnPlane - sphereCenter).normalized;
            Vector3 offsetRayOrigin = offsetOnPlane + normal * 0.1f; // offset a bit off sphere
            Ray offsetRay = new Ray(offsetRayOrigin, sphereCenter - offsetRayOrigin);
            RaycastHit offsetHit;
            Vector3 mapped_position = offsetOnPlane;
            if (sphereCollider != null && sphereCollider.Raycast(offsetRay, out offsetHit, Vector3.Distance(offsetRayOrigin, sphereCenter) + radius))
            {
                mapped_position = offsetHit.point;
            }

            // 5. Place object at hitpoint with y offset
            Vector3 sphereNormal = (mapped_position - sphereCenter).normalized;
            mapped_position += sphereNormal * height_offset;

            // Orient so up is normal to sphere at mapped position, and forward is original child forward projected onto tangent plane
            Vector3 childForward = child.transform.forward;
            Vector3 projectedForward = Vector3.ProjectOnPlane(childForward, sphereNormal).normalized;
            if (projectedForward.sqrMagnitude < 0.001f)
            {
                projectedForward = Vector3.Cross(Vector3.up, sphereNormal).normalized;
            }
            Quaternion rot = Quaternion.LookRotation(sphereNormal, projectedForward);
            GameObject duplicate_object = Instantiate(child, mapped_position, rot);
            duplicate_object.transform.SetParent(transform);
        }
    }

    void Update()
    {
        PlaceObjectsOnSphere();
        if (raycastSource == null) return;
        Vector3 sphereCenter = transform.position;
        Vector3 origin = raycastSource.transform.position;
        Vector3 direction = (sphereCenter - origin).normalized;
        Debug.DrawRay(origin, direction * Vector3.Distance(origin, sphereCenter), Color.red);
    }
}