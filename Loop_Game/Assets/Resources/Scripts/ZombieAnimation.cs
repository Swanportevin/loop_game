using UnityEngine;

public class ZombieAnimation : MonoBehaviour
{
    public Animator mAnimator;

    void Start()
    {
        mAnimator = gameObject.GetComponent<Animator>();
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
