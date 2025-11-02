using System.Collections;
using System.Collections.Generic;
// SciFiGun.cs
using UnityEngine;

public class SciFiGun : MonoBehaviour
{
    public Transform firePoint;
    public GameObject projectilePrefab;
    public float fireRate = 0.25f;
    private float lastFire;

    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time > lastFire + fireRate)
        {
            lastFire = Time.time;
            Shoot();
        }
    }

    void Shoot()
    {
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
    }
}
