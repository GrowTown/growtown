using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponHandler : MonoBehaviour
{
    [Header("References")]
    public GameObject gunObject;             // Reference to your gun prefab in hand
    public Transform firePoint;              // Where bullets come from
    public GameObject projectilePrefab;      // Player’s projectile
    public float shootCooldown = 0.3f;       // Fire rate
    public float projectileSpeed = 25f;      // Speed of bullet

    private bool gunEquipped = false;
    private float shootTimer = 0f;

    void Update()
    {
        // Cooldown timer
        if (shootTimer > 0f)
            shootTimer -= Time.deltaTime;

        // Fire if equipped + click or key
        if (gunEquipped && Input.GetKey(KeyCode.Mouse0) && shootTimer <= 0f)
        {
            Shoot();
            shootTimer = shootCooldown;
        }
    }

    public void EquipGun()
    {
        if (gunObject != null)
        {
            gunObject.SetActive(true);
            gunEquipped = true;
            Debug.Log("[PlayerWeaponHandler] Gun equipped.");
        }
    }

    public void UnequipGun()
    {
        if (gunObject != null)
        {
            gunObject.SetActive(false);
            gunEquipped = false;
        }
    }

    private void Shoot()
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
            rb.velocity = firePoint.forward * projectileSpeed;

        Destroy(proj, 2f); // clean up bullet
        Debug.Log("[PlayerWeaponHandler] Fired projectile!");
    }
}
