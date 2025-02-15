using StarterAssets;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponHandler : MonoBehaviour
{

    public static WeaponHandler instance;

    [SerializeField] private CharacterMovements characterMovements;
    [SerializeField] private ThirdPersonShooterController thirdPersonShooterController;
    [SerializeField] private ThirdPersonController thirdPersonController;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private StarterAssetsInputs starterAssetsInputs;

    [SerializeField] private SkinnedMeshRenderer shotGunSkinnedMesh;
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationEventTrigger animationEvents;

    [SerializeField] private GameObject[] crosHair;

    public float reloadTime = 2.5f;
    public float shootCooldown = 0.3f;

    // Variables for shoot logic
    private bool isTakeGun = false;
    private bool isReloading = false;
    private bool isShooting = false;
    private bool _canShoot = true;
    private bool _canReload = true;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && _canReload)
        {
            WeaponReload();
        }
    }


    public void OnCharacterWeaponMode(bool IsMouseBtnPressed)
    {
        if (IsMouseBtnPressed)
        {
            characterMovements.enabled = false;

            thirdPersonShooterController.enabled = true;
            thirdPersonController.enabled = true;

            var Cam = UI_Manager.Instance.CharacterMovements.gameObject.GetComponent<CamerasSwitch>();
            Cam.EnableShootCameraOnly();

            crosHair[0].gameObject.SetActive(true);
            //crosHair[1].gameObject.SetActive(true);

            Cursor.lockState = CursorLockMode.Locked;
            starterAssetsInputs.cursorInputForLook = true;

            shotGunSkinnedMesh.gameObject.SetActive(true);
            animator.SetLayerWeight(1, 1);
            isTakeGun = !isTakeGun;
            animator.SetBool("IsHoldingGun", isTakeGun);
            animationEvents.TriggerShotGunAnimationEvent();
        }
        else
        {
            characterMovements.enabled = true;

            thirdPersonShooterController.enabled = false;
            thirdPersonController.enabled = false;

            var Cam = UI_Manager.Instance.CharacterMovements.gameObject.GetComponent<CamerasSwitch>();
            //Cam.SwitchToCam(0);

            crosHair[0].gameObject.SetActive(false);
            //crosHair[1].gameObject.SetActive(false);

            Cursor.lockState = CursorLockMode.None;
            starterAssetsInputs.cursorInputForLook = false;


            starterAssetsInputs.cursorLocked = false;
            starterAssetsInputs.cursorInputForLook = false;
            shotGunSkinnedMesh.gameObject.SetActive(false);
            animator.SetLayerWeight(1, 0);
        }

    }



    #region ---- Handle Shooting ----

    public void WeaponReload()
    {
        StartCoroutine(PerformReload());

        IEnumerator PerformReload()
        {
            _canReload = false;
            isReloading = true;
            animator.SetTrigger("IsReloading");
            yield return new WaitForSeconds(reloadTime);
            isReloading = false;
            animator.SetBool("IsReloading", isReloading);
            _canReload = true;
        }
    }


    public void WeaponShootAnimation()
    {

        StartCoroutine(PerformShoot());

        IEnumerator PerformShoot()
        {
            _canShoot = false;
            isShooting = true;
            animator.SetTrigger("IsShooting");
            yield return new WaitForSeconds(shootCooldown);
            isShooting = false;
            animator.SetBool("IsShooting", isShooting);
            _canShoot = true;
        }
    }


    #endregion ---- Handle Shooting ----


    public void OnWeaponAvtive(float delay, string gunState)
    {

        StartCoroutine(WaitingForShotGunAnimation());

        IEnumerator WaitingForShotGunAnimation()
        {
            if (gunState == "ReloadStarted")
            {

                shotGunSkinnedMesh.SetBlendShapeWeight(0, 100);
            }
            else
            {
                shotGunSkinnedMesh.SetBlendShapeWeight(0, 0);
            }
            yield return new WaitForSeconds(delay);
        }
    }

}
