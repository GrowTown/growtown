using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class WeaponShoot : MonoBehaviour
{

    [Header("References")]
    public Camera cam;
    public Transform firePoint;
    public GameObject bulletPrefab;
    public LayerMask aimColliderLayerMask;
    public Button FireBT;
 
    [Header("Settings")]
    public float fireRate = 0.2f;
    public float bulletSpeed = 50f;
    public float maxFireDistance = 100f;

    private float lastFireTime = 0f;

    public Transform upperBodyTarget;
    public Transform lowerBody;
    public float rotationWeight = 0.5f;
    Vector3 worldAimTarget;

    public float rotationSpeed = 10f;

    private void Start()
    {
        FireBT.onClick.AddListener(()=> { ForFire();});
    }
    private void Update()
    {
       
        if (Input.GetKey(KeyCode.F) && !UI_Manager.Instance.WeaponAttackEvent.isReloading)
        {
            Fire();
        }
        else
        {
            UI_Manager.Instance.CharacterMovements.animator.SetBool("IsShooting", false);
        }
     
    }

    /* public void Fire()
     {

         if (Time.time - lastFireTime < fireRate) return;
         lastFireTime = Time.time;


         Vector3 crosshairWorldPosition = UI_Manager.Instance.WeaponAttackEvent.crossHair.transform.position;

         Vector3 fireDirection = (crosshairWorldPosition - firePoint.position).normalized;
         Debug.DrawRay(firePoint.position, fireDirection * maxFireDistance, Color.red, 1f);

         if (Physics.Raycast(firePoint.position, fireDirection, out RaycastHit hit, maxFireDistance, aimColliderLayerMask))
         {
             if (UI_Manager.Instance.CharacterMovements.animator != null)
             {
                 UI_Manager.Instance.CharacterMovements.animator.SetBool("IsShooting", true);
                 UI_Manager.Instance.WeaponAttackEvent.leftHandPos.weight = 0.1f;
             }

             if (impulseSource != null)
             {
                 impulseSource.GenerateImpulse();
             }
             Debug.Log("Hit: " + hit.collider.gameObject.name);

             if (bulletPrefab != null)
             {
                 ShootBullet(hit.point);
             }
         }
     }*/

    void ForFire()
    {
        if (!UI_Manager.Instance.WeaponAttackEvent.isReloading&& UI_Manager.Instance.WeaponAttackEvent.isGunActive)
        {
            Fire();
        }
        else
        {
            UI_Manager.Instance.CharacterMovements.animator.SetBool("IsShooting", false);
        }
    }
    float crosshairDistance = 5f;
    public void Fire()
    {
        if (Time.time - lastFireTime < fireRate) return;
        lastFireTime = Time.time;

        var player = this.transform;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = cam.ScreenPointToRay(screenCenter);

        Transform hitTransform = null;

        Debug.DrawRay(ray.origin, ray.direction * maxFireDistance, Color.red, 1f);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, maxFireDistance))
        {
            hitTransform = raycastHit.transform;
            Vector3 aimTarget = raycastHit.point;
            // Calculate direction from player to aim target
            Vector3 directionToTarget = (aimTarget - player.position).normalized;

            Vector3 screenPoint = cam.WorldToScreenPoint(aimTarget);
            UI_Manager.Instance.WeaponAttackEvent.crossHair.GetComponent<RectTransform>().position = screenPoint;
            UI_Manager.Instance.WeaponAttackEvent.targetForCrossHair.position = aimTarget;
            // Update crosshair UI position

            if (UI_Manager.Instance.CharacterMovements.animator != null)
            {
                UI_Manager.Instance.CharacterMovements.animator.SetBool("IsShooting", true);
                UI_Manager.Instance.WeaponAttackEvent.leftHandPos.weight = 0.1f;
            }


            Debug.Log("Hit: " + raycastHit.collider.gameObject.name);

            if (bulletPrefab != null)
            {
                ShootBullet(UI_Manager.Instance.WeaponAttackEvent.targetForCrossHair.position);
            }
        }
/*        worldAimTarget = UI_Manager.Instance.WeaponAttackEvent.targetForCrossHair.position;
        worldAimTarget.y=transform.position.y;
        Vector3 aimDirection=(worldAimTarget-transform.position).normalized;

        transform.forward = Vector3.Slerp(transform.forward, aimDirection, 20f * Time.deltaTime);*/
    }

    void ShootBullet(Vector3 targetPoint)
    {
       
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Bullet prefab is missing Rigidbody component!");
            return;
        }
        Vector3 direction = (targetPoint - firePoint.position).normalized;
        bullet.transform.rotation = Quaternion.LookRotation(direction);
        rb.velocity = direction * bulletSpeed;
        Destroy(bullet, 3f);
    }

    void ShootBullet(Vector3 targetPoint,Vector2 t)
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        Vector3 direction = (targetPoint - firePoint.position).normalized;
        bullet.transform.rotation = Quaternion.LookRotation(direction);
        rb.velocity = direction * bulletSpeed;

        Destroy(bullet, 3f);
    }


}
