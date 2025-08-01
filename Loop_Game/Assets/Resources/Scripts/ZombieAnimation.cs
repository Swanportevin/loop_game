using System.Collections;
using UnityEngine;

public class ZombieAnimation : MonoBehaviour
{
    public Animator mAnimator;
    public int health = 5;

    void Start()
    {
        mAnimator = gameObject.GetComponent<Animator>();
    }

    void LateUpdate()
    {
        if (health <= 0) StartCoroutine(Die());
    }

    public IEnumerator Die()
    {
        mAnimator.SetTrigger("Die");
        FindObjectOfType<GameManager>().zombieCount--;
        yield return new WaitForSeconds(0.8f); // Wait for the death animation to finish
        Destroy(gameObject); // Destroy the zombie after death animation   
    }

    public void AnimBasedOnSpeed(float speed)
    {
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

    public void SetPunch()
    {
        if (mAnimator != null)
        {
            mAnimator.SetTrigger("Punch");
        }
    }

    public void StopAll()
    {
        if (mAnimator != null)
        {
            mAnimator.SetBool("Crawl", false);
            mAnimator.SetBool("Walk", false);
            mAnimator.SetBool("Run", false);
        }
    }
}