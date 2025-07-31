using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    public float speed = 2f;
    private Transform target;
    private Animator animator;

    void Start()
    {
        target = Camera.main.transform;
        animator = GetComponent<Animator>();
        animator.SetBool("Walk", true);
    }

    void Update()
    {
        if (target == null) return;

        // Move toward the camera
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0f; // Keep movement flat

        // Rotate toward the camera
        if (direction.magnitude > 0.1f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

            // Move forward
            transform.position += transform.forward * speed * Time.deltaTime;
        }
    }
}
