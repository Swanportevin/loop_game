using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractBuilding : MonoBehaviour
{
    [Header("Raycast Settings")]
    public LayerMask targetLayer = 1; // Layer to check for intersections
    public float rayDistance = 3f; // Maximum distance for the ray
    public bool showRayInScene = true; // Show ray in scene view for debugging
    
    [Header("UI Settings")]
    public GameObject uiElement; // UI element to show/hide on hit
    
    private Camera cam;
    
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("InteractBuilding script requires a Camera component!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (cam != null)
        {
            ShootRay();
        }
    }
    
    void ShootRay()
    {
        // Create a ray from the camera's position forward
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;
        
        // Perform the raycast
        if (Physics.Raycast(ray, out hit, rayDistance, targetLayer))
        {
            // Hit detected - show UI element
            if (uiElement != null)
            {
                uiElement.SetActive(true);
            }

            // Debug.Log($"Hit object: {hit.collider.gameObject.name}, Distance: {hit.distance}");

            if (Input.GetKeyDown(KeyCode.E))
            {
                GameObject hitObject = hit.collider.gameObject;
                InteractDialouge interaction = hitObject.GetComponent<InteractDialouge>();
                if (interaction != null)
                {
                    interaction.Interact();
                }
                else
                {
                    Debug.LogWarning($"No Interaction");
                }
            }
        }
        else
        {
            // No hit - hide UI element
            if (uiElement != null)
            {
                uiElement.SetActive(false);
            }
        }
        
        // Draw the ray in scene view for debugging
        if (showRayInScene)
        {
            if (Physics.Raycast(ray, out hit, rayDistance, targetLayer))
            {
                Debug.DrawRay(cam.transform.position, cam.transform.forward * hit.distance, Color.red);
            }
            else
            {
                Debug.DrawRay(cam.transform.position, cam.transform.forward * rayDistance, Color.green);
            }
        }
    }
}
