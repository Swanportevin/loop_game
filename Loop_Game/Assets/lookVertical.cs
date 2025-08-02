using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lookVertical : MonoBehaviour
{
    [Header("Mouse Settings")]
    public float mouseSensitivity = 100f;
    public float pitchClampMin = -90f;
    public float pitchClampMax = 90f;
    
    private float xRotation = 0f;
    
    // Start is called before the first frame update
    void Start()
    {
        // Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // Get mouse Y movement (vertical movement)
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        
        // Apply vertical rotation (pitch)
        xRotation -= mouseY;
        
        // Clamp the pitch to prevent over-rotation
        xRotation = Mathf.Clamp(xRotation, pitchClampMin, pitchClampMax);
        
        // Apply the rotation to the camera
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
