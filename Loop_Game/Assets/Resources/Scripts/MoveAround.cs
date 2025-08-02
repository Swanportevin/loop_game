using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAround : MonoBehaviour
{
    public float moveSpeed = 50f;
    public float rotationSpeed = 5f;
    public bool movementEnabled = true; // Optionally disable movement

    private float yaw;

    void Update()
    {
        // --- Movement (WASD relative to object's forward/right) ---
        if (movementEnabled)
        {
            float h = Input.GetAxis("Horizontal"); // A/D
            float v = Input.GetAxis("Vertical");   // W/S
            Vector3 input = new Vector3(h, 0f, -v);
            Vector3 move = transform.TransformDirection(input);
            transform.position += move * moveSpeed * Time.deltaTime;
        }

        // --- Rotation (trackpad or mouse X) ---
        float mouseX = Input.GetAxis("Mouse X"); // Trackpad works as mouse
        yaw += mouseX * rotationSpeed;
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
    }
}
