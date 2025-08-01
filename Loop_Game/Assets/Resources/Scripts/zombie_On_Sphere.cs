using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class zombie_On_Sphere : MonoBehaviour
{
    public Animator mAnimator;
    private Rigidbody rb;
    private float speed;
    void Start()
    {
        AnimBasedOnSpeed(agent.speed);
    }

    // Update is called once per frame
    void Update()
    {

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
            OldSpeed = agent.speed; // Store the current speed
            agent.speed = 0;
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
            agent.speed = OldSpeed; // Reset speed when exiting the camera trigger
            AnimBasedOnSpeed(agent.speed);
        }
    }

    private IEnumerator StartPunching()
    {
        agent.speed = 0;
        mAnimator.SetTrigger("Punch");
        yield return new WaitForSeconds(2f); // Wait for the punch animation to finish
        StartCoroutine(StartPunching());
    }
}
