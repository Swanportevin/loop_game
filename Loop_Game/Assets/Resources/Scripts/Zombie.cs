using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    private float OldSpeed; // Store the old speed when the zombie is hit
    public float speed = 2f;
    private GameObject target; // Reference to the player origin
    public Animator mAnimator;
    private Rigidbody rb;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player"); // Find the camera as the target
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        MovingTowardsPlayer();
        AnimBasedOnSpeed(speed);
    }

    public void MovingTowardsPlayer()
    {
        if (target == null || rb == null) return;

        Vector3 direction = (target.transform.position - transform.position).normalized;
        direction.y = 0f;

        if (direction.magnitude > 0.1f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * 5f);

            // Use MovePosition instead of transform.position += ...
            Vector3 moveTo = transform.position + transform.forward * speed * Time.fixedDeltaTime;
            rb.MovePosition(moveTo);
        }
    }
    public void AnimBasedOnSpeed(float speed)
    {
        // Update the animator parameters based on speed
        if (mAnimator != null)
        {
            if (speed <= 2 && speed > 0)
            {
                mAnimator.SetBool("Crawl", true);
            }
            else
            {
                mAnimator.SetBool("Crawl", false);
            }

            if (speed > 2 && speed < 5)
            {
                mAnimator.SetBool("Walk", true);
            }
            else
            {
                mAnimator.SetBool("Walk", false);
            }

            if (speed >= 5)
            {
                mAnimator.SetBool("Run", true);
            }
            else
            {
                mAnimator.SetBool("Run", false);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OldSpeed = speed; // Store the current speed
            speed = 0;
            mAnimator.SetBool("Crawl", false);
            mAnimator.SetBool("Walk", false);
            mAnimator.SetBool("Run", false);
            StartCoroutine(StartPunching()); // Start the punching animation
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopAllCoroutines(); // Stop any ongoing punch animations
            speed = OldSpeed; // Reset speed when exiting the camera trigger
            AnimBasedOnSpeed(speed);
        }
    }

    private IEnumerator StartPunching()
    {
        speed = 0;
        mAnimator.SetTrigger("Punch");
        yield return new WaitForSeconds(2f); // Wait for the punch animation to finish
        StartCoroutine(StartPunching());
    }
}