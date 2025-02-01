using System.Collections;
using UnityEngine;

public class Shotgun : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.2f;
    public float bulletForce = 1000f;

    private bool canShoot = true;

    void Update()
    {
        if (Input.GetKey(KeyCode.F) && canShoot)
        {
            StartCoroutine(FireShotgun());
        }
    }

    private IEnumerator FireShotgun()
    {
        canShoot = false;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.AddForce(firePoint.forward * bulletForce);

        yield return new WaitForSeconds(fireRate);
        canShoot = true;
    }
}
