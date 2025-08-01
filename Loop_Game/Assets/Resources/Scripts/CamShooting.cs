using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamShooting : MonoBehaviour
{
    // Start is called before the first frame update
    public LayerMask ZombieLayer; // Layer for zombies

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            Shoot();
        }

    }
    void Shoot()
    {
        Ray ray = new Ray(gameObject.transform.position, gameObject.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, ZombieLayer))
        {
            string originalName = hitInfo.collider.name.Replace("(Clone)", "").Trim();
            GameObject OriginalZombie = GameObject.Find(originalName);
            OriginalZombie.GetComponent<ZombieAnimation>().health -= 1;
            StartCoroutine(PlayHitAnimation(OriginalZombie));
        }
    }

    public IEnumerator PlayHitAnimation(GameObject OriginalZombie)
    {
        Debug.Log("Playing hit animation on: " + OriginalZombie.name);
        OriginalZombie.GetComponent<ZombieAnimation>().mAnimator.SetBool("Crawl", false);
        OriginalZombie.GetComponent<ZombieAnimation>().mAnimator.SetBool("Walk", false);
        OriginalZombie.GetComponent<ZombieAnimation>().mAnimator.SetBool("Run", false);
        OriginalZombie.GetComponent<ZombieAnimation>().mAnimator.SetTrigger("Hit");
        yield return new WaitForSeconds(0.3f); // Wait for the hit animation to finish
        OriginalZombie.GetComponent<ZombieAnimation>().AnimBasedOnSpeed(OriginalZombie.GetComponent<ZombieMovement>().OldSpeed);
    }
}