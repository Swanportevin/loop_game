using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float lifetime = 5f; // How long the bullet exists before destroying itself
    public bool destroyOnImpact = true; // Whether to destroy bullet on collision
    
    [Header("Effects")]
    public GameObject impactEffect; // Optional impact effect

    private void Start()
    {
        // Destroy the bullet after its lifetime expires
        Destroy(gameObject, lifetime);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        // Ignore collisions with the player
        if (collision.gameObject.CompareTag("Player"))
        {
            return;
        }
        
        // Handle collision with objects
        if (collision.gameObject.CompareTag("zombie"))
        {
            // Handle zombie damage and animation
            HandleZombieHit(collision.gameObject);
        }
        
        // Spawn impact effect if available
        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }

        // Destroy bullet on impact if enabled
        if (destroyOnImpact)
        {
            Destroy(gameObject);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        
        // Alternative collision detection using triggers
        if (other.CompareTag("zombie"))
        {
            // Handle zombie damage and animation
            HandleZombieHit(other.gameObject);
        }
        
        // Spawn impact effect if available
        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }
        
        // Destroy bullet on impact if enabled
        if (destroyOnImpact)
        {
            Destroy(gameObject);
        }
    }
    
    private void HandleZombieHit(GameObject hitZombie)
    {
        Debug.Log("Bullet hit zombie: " + hitZombie.name);
        
        // Get the original zombie name (remove clone suffix if present)
        string originalName = hitZombie.name.Replace("(Clone)", "").Trim();
        GameObject originalZombie = GameObject.Find(originalName);
        
        if (originalZombie != null)
        {
            // Reduce zombie health
            ZombieAnimation zombieAnim = originalZombie.GetComponent<ZombieAnimation>();
            if (zombieAnim != null)
            {
                zombieAnim.health -= 1;
                StartCoroutine(PlayHitAnimation(originalZombie));
            }
            else
            {
                Debug.LogWarning("ZombieAnimation component not found on " + originalZombie.name);
            }
        }
        else
        {
            Debug.LogWarning("Original zombie not found for " + hitZombie.name);
        }
    }
    
    public IEnumerator PlayHitAnimation(GameObject originalZombie)
    {
        Debug.Log("Playing hit animation on: " + originalZombie.name);
        ZombieAnimation zombieAnim = originalZombie.GetComponent<ZombieAnimation>();
        if (zombieAnim != null && zombieAnim.mAnimator != null)
        {
            zombieAnim.mAnimator.SetBool("Crawl", false);
            zombieAnim.mAnimator.SetBool("Walk", false);
            zombieAnim.mAnimator.SetBool("Run", false);
            zombieAnim.mAnimator.SetTrigger("Hit");
            yield return new WaitForSeconds(0.3f); // Wait for the hit animation to finish
            
            ZombieMovement zombieMovement = originalZombie.GetComponent<ZombieMovement>();
            if (zombieMovement != null)
            {
                zombieAnim.AnimBasedOnSpeed(zombieMovement.OldSpeed);
            }
        }
    }
}
