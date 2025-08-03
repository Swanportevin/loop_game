using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobUpDownSpin : MonoBehaviour
{
    [Header("Bobbing Settings")]
    public float bobHeight = 1f;        // How high/low the object bobs
    public float bobSpeed = 2f;         // Speed of the bobbing motion
    
    [Header("Spinning Settings")]
    public float spinSpeed = 90f;       // Degrees per second for spinning
    public Vector3 spinAxis = Vector3.up; // Axis to spin around (Y-axis by default)
    
    private Vector3 startPosition;      // Store the initial position
    
    // Start is called before the first frame update
    void Start()
    {
        // Remember the starting position
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Bobbing motion using sine wave
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
        
        // Spinning motion
        transform.Rotate(spinAxis * spinSpeed * Time.deltaTime);
    }
}
