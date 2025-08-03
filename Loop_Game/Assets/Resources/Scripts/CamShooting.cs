using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum WeaponType
{
    Pistol,
    Flamethrower
}

public class CamShooting : MonoBehaviour
{
    [Header("Weapon Selection")]
    public bool useFlamethrower = false; // Toggle this to switch weapons
    private WeaponType currentWeapon = WeaponType.Pistol;

    [Header("Pistol Settings")]
    //bullet 
    public GameObject bullet;
    //bullet force
    public float shootForce, upwardForce;

    [Header("Flamethrower Settings")]
    public GameObject flamethrowerParticleSystem;
    public float flamethrowerRange = 10f;
    public float flamethrowerConeAngle = 30f;
    public int flamethrowerRaycastCount = 10;
    public int flamethrowerDamagePerRay = 1; // Changed to int to avoid casting issues
    public float flamethrowerTickRate = 0.1f; // How often damage is applied
    private float lastFlamethrowerTick;

    //Gun stats
    public float timeBetweenShooting, spread, timeBetweenShots;
    public int bulletsPerTap;
    public bool allowButtonHold;

    int bulletsShot;

    //bools
    bool shooting, readyToShoot;

    //Reference
    public Camera fpsCam;
    public Transform attackPoint;

    //Graphics
    public GameObject muzzleFlash;
    public TextMeshProUGUI ammunitionDisplay;

    //bug fixing :D
    public bool allowInvoke = true;

    // Helper method to get ParticleSystem from flamethrower prefab
    private ParticleSystem GetFlamethrowerParticleSystem()
    {
        if (flamethrowerParticleSystem == null)
            return null;
            
        // Try to get ParticleSystem from the GameObject itself
        ParticleSystem ps = flamethrowerParticleSystem.GetComponent<ParticleSystem>();
        
        // If not found, try to get it from children
        if (ps == null)
            ps = flamethrowerParticleSystem.GetComponentInChildren<ParticleSystem>();
            
        return ps;
    }

    private void Awake()
    {
        readyToShoot = true;
        currentWeapon = WeaponType.Pistol;
        
        // Make sure flamethrower starts OFF
        ParticleSystem ps = GetFlamethrowerParticleSystem();
        if (ps != null)
        {
            ps.Stop();
            Debug.Log("Flamethrower particle system found and stopped on startup");
        }
        else if (flamethrowerParticleSystem != null)
        {
            Debug.LogWarning("No ParticleSystem component found on flamethrower prefab or its children!");
        }
    }

    void Update()
    {
        MyInput();

        //Set ammo display based on current weapon
        if (ammunitionDisplay != null)
        {
            if (currentWeapon == WeaponType.Pistol)
            {
                ammunitionDisplay.SetText("PISTOL - UNLIMITED");
            }
            else
            {
                ammunitionDisplay.SetText("FLAMETHROWER");
            }
        }

        // Stop flamethrower particle system when not shooting
        if (currentWeapon == WeaponType.Flamethrower && !shooting)
        {
            ParticleSystem ps = GetFlamethrowerParticleSystem();
            if (ps != null && ps.isPlaying)
                ps.Stop();
        }
    }

    private void MyInput()
    {
        // Weapon switching based on boolean
        WeaponType targetWeapon = useFlamethrower ? WeaponType.Flamethrower : WeaponType.Pistol;
        if (currentWeapon != targetWeapon)
        {
            currentWeapon = targetWeapon;
            SwitchWeaponEffects();
        }

        //Check if allowed to hold down button and take corresponding input
        if (currentWeapon == WeaponType.Flamethrower)
        {
            // Flamethrower should always use hold-down behavior
            shooting = Input.GetKey(KeyCode.Mouse0);
        }
        else
        {
            // Pistol uses the configured behavior
            if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
            else shooting = Input.GetKeyDown(KeyCode.Mouse0);
        }

        //Shooting
        if (shooting)
        {
            if (currentWeapon == WeaponType.Pistol && readyToShoot)
            {
                //Set bullets shot to 0
                bulletsShot = 0;
                Shoot();
            }
            else if (currentWeapon == WeaponType.Flamethrower)
            {
                // Flamethrower shoots continuously while held, but damage ticks are rate-limited
                ShootFlamethrower();
            }
        }
    }
    void Shoot()
    {
        readyToShoot = false;

        //Find the exact hit position using a raycast
        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); //Just a ray through the middle of your current view
        RaycastHit hit;

        //check if ray hits something
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
            targetPoint = ray.GetPoint(75); //Just a point far away from the player

        //Calculate direction from attackPoint to targetPoint
        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

        //Calculate spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        //Calculate new direction with spread
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0); //Just add spread to last direction

        //Instantiate bullet/projectile
        if (bullet != null)
        {
            GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity); //store instantiated bullet in currentBullet
            //Rotate bullet to shoot direction
            currentBullet.transform.forward = directionWithSpread.normalized;

            //Add forces to bullet if it has a Rigidbody
            Rigidbody bulletRb = currentBullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                bulletRb.AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
                bulletRb.AddForce(fpsCam.transform.up * upwardForce, ForceMode.Impulse);
            }
            else
            {
                Debug.LogWarning("Bullet prefab is missing Rigidbody component!");
            }
        }
        else
        {
            Debug.LogError("Bullet prefab is not assigned in CamShooting script!");
        }

        //Instantiate muzzle flash, if you have one
        if (muzzleFlash != null)
            Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);

        bulletsShot++;

        //Invoke resetShot function (if not already invoked), with your timeBetweenShooting
        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;
        }

        //if more than one bulletsPerTap make sure to repeat shoot function
        if (bulletsShot < bulletsPerTap)
            Invoke("Shoot", timeBetweenShots);
    }

    private void ResetShot()
    {
        //Allow shooting and invoking again
        readyToShoot = true;
        allowInvoke = true;
    }

    private void SwitchWeaponEffects()
    {
        Debug.Log("Switched to " + currentWeapon.ToString());

        // Handle flamethrower particle system effects
        ParticleSystem ps = GetFlamethrowerParticleSystem();
        if (ps != null)
        {
            // Always stop first, then start if needed
            ps.Stop();
            if (currentWeapon == WeaponType.Flamethrower)
            {
                // Don't auto-start, only start when shooting
                Debug.Log("Flamethrower ready - will activate when shooting");
            }
        }
        else if (flamethrowerParticleSystem != null)
        {
            Debug.LogWarning("No ParticleSystem found on flamethrower prefab or its children during weapon switch!");
        }
    }

    private void ShootFlamethrower()
    {
        // Always activate particle system when shooting (continuous flames)
        ParticleSystem ps = GetFlamethrowerParticleSystem();
        if (ps != null && !ps.isPlaying)
        {
            ps.Play();
            Debug.Log("Flamethrower particle system activated");
        }
        else if (ps == null && flamethrowerParticleSystem != null)
        {
            Debug.LogWarning("No ParticleSystem found on flamethrower prefab or its children during shooting!");
        }

        // Only apply damage at the specified tick rate
        if (Time.time - lastFlamethrowerTick < flamethrowerTickRate)
            return;

        lastFlamethrowerTick = Time.time;

        // Cast multiple rays in a systematic grid pattern for reliable coverage
        List<GameObject> hitZombies = new List<GameObject>();
        
        // Create a grid of rays for consistent coverage
        int raysPerAxis = Mathf.RoundToInt(Mathf.Sqrt(flamethrowerRaycastCount));
        float angleStep = flamethrowerConeAngle / (raysPerAxis - 1);
        
        for (int x = 0; x < raysPerAxis; x++)
        {
            for (int y = 0; y < raysPerAxis; y++)
            {
                // Calculate systematic direction within cone
                float angleX = -flamethrowerConeAngle / 2 + (x * angleStep);
                float angleY = -flamethrowerConeAngle / 2 + (y * angleStep);
                
                Vector3 rayDirection = fpsCam.transform.forward;
                rayDirection = Quaternion.AngleAxis(angleX, fpsCam.transform.up) * rayDirection;
                rayDirection = Quaternion.AngleAxis(angleY, fpsCam.transform.right) * rayDirection;
                
                Ray ray = new Ray(attackPoint.position, rayDirection);
                
                if (Physics.Raycast(ray, out RaycastHit hit, flamethrowerRange))
                {
                    if (hit.collider.CompareTag("zombie"))
                    {
                        // Add to hit list if not already there (prevent multiple hits per frame)
                        if (!hitZombies.Contains(hit.collider.gameObject))
                        {
                            hitZombies.Add(hit.collider.gameObject);
                        }
                    }
                    
                    // Debug visualization - make rays more visible
                    Debug.DrawRay(ray.origin, rayDirection * hit.distance, Color.red, 0.5f);
                }
                else
                {
                    // Debug visualization - make rays more visible
                    Debug.DrawRay(ray.origin, rayDirection * flamethrowerRange, Color.yellow, 0.5f);
                }
            }
        }

        // Apply damage to all hit zombies
        foreach (GameObject zombie in hitZombies)
        {
            ApplyFlamethrowerDamage(zombie);
        }
    }

    private void ApplyFlamethrowerDamage(GameObject hitZombie)
    {
        Debug.Log("Flamethrower hit zombie: " + hitZombie.name);
        
        // Get the original zombie name (remove clone suffix if present)
        string originalName = hitZombie.name.Replace("(Clone)", "").Trim();
        Debug.Log("Looking for original zombie: '" + originalName + "'");
        
        GameObject originalZombie = GameObject.Find(originalName);
        
        if (originalZombie != null)
        {
            Debug.Log("Found original zombie: " + originalZombie.name);
            
            // Reduce zombie health (same as bullet script)
            ZombieAnimation zombieAnim = originalZombie.GetComponent<ZombieAnimation>();
            if (zombieAnim != null)
            {
                int oldHealth = zombieAnim.health;
                zombieAnim.health -= flamethrowerDamagePerRay; // No cast needed now
                Debug.Log("Zombie health: " + oldHealth + " -> " + zombieAnim.health + " (damage: " + flamethrowerDamagePerRay + ")");
                
                // Trigger hit animation same as bullet script (but less frequently)
                StartCoroutine(PlayFlamethrowerHitAnimation(originalZombie));
            }
            else
            {
                Debug.LogWarning("ZombieAnimation component not found on " + originalZombie.name);
            }
        }
        else
        {
            Debug.LogWarning("Original zombie not found for '" + originalName + "' (hit zombie was: " + hitZombie.name + ")");
        }
    }

    public IEnumerator PlayFlamethrowerHitAnimation(GameObject originalZombie)
    {
        Debug.Log("Playing flamethrower hit animation on: " + originalZombie.name);
        ZombieAnimation zombieAnim = originalZombie.GetComponent<ZombieAnimation>();
        if (zombieAnim != null && zombieAnim.mAnimator != null)
        {
            zombieAnim.mAnimator.SetBool("Crawl", false);
            zombieAnim.mAnimator.SetBool("Walk", false);
            zombieAnim.mAnimator.SetBool("Run", false);
            zombieAnim.mAnimator.SetTrigger("Hit");
            yield return new WaitForSeconds(0.1f); // Shorter wait for flamethrower
            
            ZombieMovement zombieMovement = originalZombie.GetComponent<ZombieMovement>();
            if (zombieMovement != null)
            {
                zombieAnim.AnimBasedOnSpeed(zombieMovement.OldSpeed);
            }
        }
    }
}