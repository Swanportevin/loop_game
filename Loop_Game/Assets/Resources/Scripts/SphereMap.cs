using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class SphereMap : MonoBehaviour
{
    // The object from which to cast the ray, selectable in Inspector
    public GameObject raycastSource;
    public GameObject staticObjectContainerObject;
    public GameObject aiObjectContainer;
    public GameObject buildingsContainer;
    public GameObject bulletsContainer;
    public GameObject playerOrigin;


    Vector3 GetPointOnSmallCircle(Vector3 hitpoint, Vector3 sphereCenter, float radius, float geodesicDistance, Vector3 right, float angleDegrees)
    {
        // Step 1: Normal from center to hitpoint (unit)
        Vector3 A = (hitpoint - sphereCenter).normalized;

        // Step 2: Tangent basis vectors (u and v) in hitpoint's tangent plane
        Vector3 u = Vector3.ProjectOnPlane(right, A).normalized;
        Vector3 v = Vector3.Cross(A, u); // perpendicular in tangent plane

        // Step 3: Convert geodesic distance to central angle θ
        float theta = geodesicDistance / radius;

        // Step 4: Convert angleDegrees to radians and rotate
        float angleRad = angleDegrees * Mathf.Deg2Rad;

        // Step 5: Compute point on sphere
        Vector3 direction = Mathf.Cos(angleRad) * u + Mathf.Sin(angleRad) * v;
        Vector3 point = Mathf.Cos(theta) * A + Mathf.Sin(theta) * direction;

        // Step 6: Scale back to sphere radius and translate to world space
        return sphereCenter + point * radius;
    }

    

    void PlaceObjectsOnSphere(GameObject container, System.Action<GameObject> ModifyObject, bool face_player)
    {

        if (raycastSource == null || container == null || playerOrigin == null) return;

        Vector3 sphereCenter = transform.position;
        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        float radius = sphereCollider != null ? sphereCollider.radius * transform.localScale.x : 1.0f;

        Vector3 raycastOrigin = raycastSource.transform.position;


        // Calculate a point on a circle around the raycast origin hitpoint
        // First, raycast from raycastOrigin to sphere center to get hitpoint
        Vector3 toCenter = (sphereCenter - raycastOrigin).normalized;
        Ray ray = new Ray(raycastOrigin, toCenter);
        RaycastHit hit;
        Vector3 hitpoint = sphereCenter;
        if (sphereCollider != null && sphereCollider.Raycast(ray, out hit, Vector3.Distance(raycastOrigin, sphereCenter) + radius))
        {
            hitpoint = hit.point;
        }

        // Get player yaw (rotation around y axis)
        float playerYaw = playerOrigin.transform.eulerAngles.y * Mathf.Deg2Rad;
        Vector3 playerPosition = playerOrigin.transform.position;
        int object_count = container.transform.childCount;

        float diameter = radius * 2f;
        for (int i = 0; i < object_count; i++)
        {
            GameObject child = container.transform.GetChild(i).gameObject;
            Vector3 child_pos = child.transform.position;
            float height_offset = child_pos.y;

            // 1. Distance and angle from player
            Vector3 _rel = child_pos - playerPosition;
            Vector2 rel = new Vector2(_rel.x, _rel.z);
            float dist = rel.magnitude;
            if (dist > diameter)
            {
                // Don't clone if further than 1 diameter
                continue;
            }

            Vector3 _right_player = playerOrigin.transform.right;
            Vector2 right_player = new Vector2(_right_player.x, _right_player.z);
            float angle = Vector2.SignedAngle(right_player, rel); // angle in xz-plane


            Vector3 _forward_dir = child.transform.forward;
            Vector2 forward_dir = new Vector2(_forward_dir.x, _forward_dir.z);

            float angle_child = Vector2.SignedAngle(forward_dir, rel);

            Vector3 _right_raycast = raycastSource.transform.right;
            Vector2 right_raycast = new Vector2(_right_raycast.x, _right_raycast.z);

            // 4. Raycast from offset position (slightly off sphere) to center
            Vector3 newPoint = GetPointOnSmallCircle(hitpoint, sphereCenter, radius, dist, right_raycast, -angle);
            Vector3 normal = (newPoint - sphereCenter).normalized;
            Vector3 offsetRayOrigin = newPoint + normal * 2f; // offset a bit off sphere

            Ray offsetRay = new Ray(offsetRayOrigin, sphereCenter - offsetRayOrigin);
            RaycastHit offsetHit;
            Vector3 mapped_position = newPoint;

            if (sphereCollider != null && sphereCollider.Raycast(offsetRay, out offsetHit, Vector3.Distance(offsetRayOrigin, sphereCenter) + radius))
            {
                mapped_position = offsetHit.point;
            }


            // 5. Place object at hitpoint with y offset
            Vector3 sphereNormal = (mapped_position - sphereCenter).normalized;
            mapped_position += sphereNormal * height_offset;

            Quaternion finalRot;


            Vector3 childForward = hitpoint - mapped_position;

            if (face_player)
            {
                finalRot = Quaternion.LookRotation(childForward, sphereNormal);
            }
            else
            {
                finalRot = Quaternion.FromToRotation(Vector3.up, sphereNormal) * child.transform.rotation;
            }
            

            GameObject duplicate_object = Instantiate(child, mapped_position, finalRot);

            ModifyObject(duplicate_object);

            Debug.DrawRay(duplicate_object.transform.position, duplicate_object.transform.forward * 2f, Color.red, 0.1f);

            duplicate_object.transform.SetParent(transform);
        }
    }

    void Update()
    {

        // Remove previous duplicates
        foreach (Transform t in transform)
        {
            if (t.gameObject != this.gameObject)
                Destroy(t.gameObject);
        }
        // Loop through staticObjectContainer array and place each on the sphere
        if (staticObjectContainerObject != null && playerOrigin != null)
        {
            float maxDistance = 300f; // Set your desired max distance here
            Vector3 playerPos = playerOrigin.transform.position;
            foreach (Transform child in staticObjectContainerObject.transform)
            {
            if (child != null)
            {
                float dist = Vector3.Distance(playerPos, child.position);
                if (dist <= maxDistance)
                {
                PlaceObjectsOnSphere(child.gameObject, (_gameobject) => { }, false);
                }
            }
            }
        }
        PlaceObjectsOnSphere(buildingsContainer, (_gameobject) => { }, false);
        PlaceObjectsOnSphere(bulletsContainer, (_gameobject) => { }, false);
        PlaceObjectsOnSphere(aiObjectContainer, (gameobject) =>
        {
            Destroy(gameobject.GetComponent<ZombieMovement>());
            Destroy(gameobject.GetComponent<NavMeshAgent>());
            
        }, true);

        if (raycastSource == null) return;
        Vector3 sphereCenter = transform.position;
        Vector3 origin = raycastSource.transform.position;
        Vector3 direction = (sphereCenter - origin).normalized;
        Debug.DrawRay(origin, direction * Vector3.Distance(origin, sphereCenter), Color.red);
    }
}