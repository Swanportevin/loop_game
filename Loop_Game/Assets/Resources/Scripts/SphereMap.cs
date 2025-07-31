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

    void Start()
    {
        if (raycastSource == null) return;

        // Ray from object to sphere center
        Vector3 sphereCenter = transform.position;
        Vector3 origin = raycastSource.transform.position;
        Vector3 direction = (sphereCenter - origin).normalized;

        // Sphere collider required for intersection
        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        if (sphereCollider == null)
        {
            Debug.LogWarning("SphereMap: No SphereCollider found on sphere.");
            return;
        }

        Ray ray = new Ray(origin, direction);
        RaycastHit hit;
        Vector3 intersection = Vector3.zero;
        bool hasIntersection = false;
        if (sphereCollider.Raycast(ray, out hit, Vector3.Distance(origin, sphereCenter) + sphereCollider.radius))
        {
            Debug.Log($"Ray hit sphere at: {hit.point}");
            intersection = hit.point;
            hasIntersection = true;
        }
        else
        {
            Debug.Log("Ray did not hit the sphere.");
        }

        if (!hasIntersection) return;

        Vector3 playerPosition = playerOrigin.transform.position;
        int object_count = objectContainer.transform.childCount;
        Debug.Log($"The object container has {object_count} children");

        // Get sphere center and radius
        Vector3 sphereCenterLocal = transform.position;
        SphereCollider sphereColliderLocal = GetComponent<SphereCollider>();
        float radius = sphereColliderLocal != null ? sphereColliderLocal.radius * transform.localScale.x : 1.0f;

        // Calculate plane bounds for normalization
        Bounds planeBounds = new Bounds(playerPosition, Vector3.zero);
        for (int i = 0; i < object_count; i++)
        {
            GameObject child = objectContainer.transform.GetChild(i).gameObject;
            planeBounds.Encapsulate(child.transform.position);
        }

        for (int i = 0; i < object_count; i++)
        {
            GameObject child = objectContainer.transform.GetChild(i).gameObject;
            Vector3 child_pos = child.transform.position;
            Vector3 relative_pos = child_pos - playerPosition;
            // remove y component:
            relative_pos.y = 0.0f;

            // Normalize relative position to [-1,1] range based on plane bounds
            float normX = 0f, normY = 0f;
            if (planeBounds.size.x > 0) normX = (child_pos.x - playerPosition.x) / (planeBounds.size.x / 2f);
            if (planeBounds.size.z > 0) normY = (child_pos.z - playerPosition.z) / (planeBounds.size.z / 2f);

            // Map normalized coordinates to latitude/longitude offsets
            float maxLat = 60f; // degrees from intersection point, adjust as needed
            float maxLon = 60f;
            float latOffset = normY * maxLat;
            float lonOffset = normX * maxLon;

            // Get intersection's spherical coordinates
            Vector3 sphereDir = (intersection - sphereCenterLocal).normalized;
            float baseLat = Mathf.Acos(sphereDir.y); // theta
            float baseLon = Mathf.Atan2(sphereDir.z, sphereDir.x); // phi

            // Apply offsets
            float newLat = baseLat + Mathf.Deg2Rad * latOffset;
            float newLon = baseLon + Mathf.Deg2Rad * lonOffset;

            // Convert spherical to cartesian
            float x = radius * Mathf.Sin(newLat) * Mathf.Cos(newLon);
            float y = radius * Mathf.Cos(newLat);
            float z = radius * Mathf.Sin(newLat) * Mathf.Sin(newLon);
            Vector3 mapped_position = sphereCenterLocal + new Vector3(x, y, z);

            GameObject duplicate_object = Instantiate(child);
            duplicate_object.transform.position = mapped_position;
            duplicate_object.transform.SetParent(transform); // Make sphere parent

            // Orient so up is normal to sphere at mapped position, preserving original local rotation
            Vector3 sphereNormal = (mapped_position - sphereCenterLocal).normalized;
            Quaternion alignToNormal = Quaternion.FromToRotation(duplicate_object.transform.up, sphereNormal);
            duplicate_object.transform.rotation = alignToNormal * duplicate_object.transform.rotation;
        }
    }

    void Update()
    {
        if (raycastSource == null) return;
        Vector3 sphereCenter = transform.position;
        Vector3 origin = raycastSource.transform.position;
        Vector3 direction = (sphereCenter - origin).normalized;
        Debug.DrawRay(origin, direction * Vector3.Distance(origin, sphereCenter), Color.red);
    }
}