using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.EventSystems;


public class WeaponAttackEvent : MonoBehaviour, IPointerClickHandler
{


    [SerializeField] private GameObject _backGround;
    [SerializeField] private GameObject Gungo;
    [Header("Aim Constraints")]
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform debugTransform;
    [SerializeField] internal GameObject crossHair;
    [SerializeField] internal Transform targetForCrossHair;


    [SerializeField] internal TwoBoneIKConstraint leftHandPos;
    [SerializeField] internal Rig aim;
    public float maxDistance = 100f;
    private Camera mainCamera;
    Vector3 worldAimTarget;
    private bool _isSelected;
    internal bool isGunActive;
    public float rotationSpeed = 10f;

    public float reloadTime = 2.5f;
    internal bool isReloading = false;
    private bool _canReload = true;
    private float maxFireDistance = 100f;
    public float rotationDamping = 5f;

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            _backGround.SetActive(value);
        }
    }

    private void Start()
    {
        mainCamera = Camera.main;

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!UI_Manager.Instance.isPlayerInField && !UI_Manager.Instance.starterPackInfoPopUpPanel.activeSelf)
        {
            isGunActive = !isGunActive;
            Gungo.SetActive(isGunActive);
            IsSelected = isGunActive;
        }
    }

    internal void ToMakeHammerInactive()
    {
        isGunActive = false;
        Gungo.SetActive(isGunActive);
        IsSelected = isGunActive;
    }

    private void Update()
    {
        if (isGunActive)
        {
           
            ActivatingTheGun();
          
            Aiming();
        }
        else
        {
            DeActivatingTheGun();
        }

        if (Input.GetKeyDown(KeyCode.R) && _canReload&& isGunActive)
        {
            WeaponReload();
        }
    }

    private void ActivatingTheGun()
    {
        var animator = UI_Manager.Instance.CharacterMovements.animator;
        if (animator != null)
        {
            animator.SetLayerWeight(1, 1);
            crossHair.SetActive(true);
            targetForCrossHair.gameObject.SetActive(true);
            leftHandPos.weight = 1;
            aim.weight = 0.4f;
            UI_Manager.Instance.CharacterMovements.gameObject.GetComponent<CamerasSwitch>().EnableShootCameraOnly();
        }
    }

    /*internal void Aiming()
    {
        if (IsPointerOverUI()) return;

        var player = UI_Manager.Instance.CharacterMovements.transform;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Transform hitTransform = null;

        Debug.DrawRay(ray.origin, ray.direction * maxFireDistance, Color.red, 1f);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, maxFireDistance))
        {
            hitTransform = raycastHit.transform;
            Vector3 aimTarget = raycastHit.point;

            //  **Prevent crosshair from coming too close to the player**
            float minAimDistance = 2.0f; // Adjust this value
            float distanceToPlayer = Vector3.Distance(player.position, aimTarget);
            if (distanceToPlayer < minAimDistance)
            {
                aimTarget = player.position + (ray.direction * minAimDistance);
            }

            //  **Prevent crosshair from going too high**
            float maxAimHeight = player.position.y + 1.5f; // Adjust this value
            if (aimTarget.y > maxAimHeight)
            {
                aimTarget.y = maxAimHeight;
            }

            //  Apply the clamped position to the crosshair target
            targetForCrossHair.position = aimTarget;

            //  Update crosshair UI position
            Vector3 screenPoint = mainCamera.WorldToScreenPoint(targetForCrossHair.position);
            crossHair.GetComponent<RectTransform>().position = screenPoint;

            //  **PLAYER ROTATION LOGIC**
            Vector3 directionToTarget = aimTarget - player.position;

            // Prevent aiming too far behind the player
            float angle = Vector3.Angle(player.forward, directionToTarget);
            if (angle > 80f) return;

            // Calculate the rotation smoothly
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(aimTarget.x, player.position.y, aimTarget.z) - player.position);
            player.rotation = Quaternion.Slerp(player.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }*/

    public float crosshairDistance = 5f;
    /*    internal void Aiming()
        {
            if (IsPointerOverUI()) return;

            var player = UI_Manager.Instance.CharacterMovements.transform;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Transform hitTransform = null;

            Debug.DrawRay(ray.origin, ray.direction * maxFireDistance, Color.red, 1f);

            if (Physics.Raycast(ray, out RaycastHit raycastHit, maxFireDistance))
            {
                hitTransform = raycastHit.transform;
                Vector3 aimTarget = raycastHit.point;

                // Prevent crosshair from coming too close to the player
                float minAimDistance = 2.0f; // Adjust this value
                float distanceToPlayer = Vector3.Distance(player.position, aimTarget);
                if (distanceToPlayer < minAimDistance)
                {
                    aimTarget = player.position + (ray.direction * minAimDistance);
                }

                // Prevent crosshair from going too high
                float maxAimHeight = player.position.y + 2f; // Adjust this value
                if (aimTarget.y > maxAimHeight)
                {
                    aimTarget.y = maxAimHeight;
                }

                // Apply the clamped position to the crosshair target
                targetForCrossHair.position = aimTarget;

                // Update crosshair UI position
                Vector3 screenPoint = mainCamera.WorldToScreenPoint(targetForCrossHair.position);
                crossHair.GetComponent<RectTransform>().position = screenPoint;


                // Player rotation logic
                Vector3 directionToTarget = new Vector3(aimTarget.x, player.position.y, aimTarget.z);

                // Prevent aiming too far behind the player
                float angle = Vector3.Angle(player.forward, directionToTarget);



                // Calculate the target rotation (horizontal only)
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

                // Smoothly rotate the player towards the target (horizontal rotation only)
                player.rotation = Quaternion.Slerp(player.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }*/

    internal void Aiming()
    {
        if (IsPointerOverUI()) return;

        var player = UI_Manager.Instance.CharacterMovements.transform;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Transform hitTransform = null;

        Debug.DrawRay(ray.origin, ray.direction * maxFireDistance, Color.red, 1f);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, maxFireDistance))
        {
            hitTransform = raycastHit.transform;
            Vector3 aimTarget = raycastHit.point;

            // Calculate the direction from the player to the aim target
            Vector3 directionToTarget = (aimTarget - player.position).normalized;

            // Calculate the angle between the player's forward direction and the aim direction
            float angle = Vector3.Angle(player.forward, directionToTarget);

            // Prevent aiming behind the player (e.g., beyond 90 degrees)
            if (angle > 90f)
            {
                // Option 1: Stop aiming entirely
                return;

                // Option 2: Clamp the aim direction to the edge of the front hemisphere
                // directionToTarget = Vector3.RotateTowards(player.forward, directionToTarget, Mathf.Deg2Rad * 90f, 0f);
            }

            // Set the crosshair position at a fixed distance from the player
            targetForCrossHair.position = player.position + directionToTarget * crosshairDistance;

            // Update crosshair UI position
            Vector3 screenPoint = mainCamera.WorldToScreenPoint(targetForCrossHair.position);
            crossHair.GetComponent<RectTransform>().position = screenPoint;

            // Player rotation logic
            Vector3 horizontalDirectionToTarget = new Vector3(directionToTarget.x, 0f, directionToTarget.z).normalized;

            // Calculate the target rotation (horizontal only)
            Quaternion targetRotation = Quaternion.LookRotation(horizontalDirectionToTarget);

            // Smoothly interpolate the player's rotation towards the target
            player.rotation = Quaternion.Slerp(player.rotation, targetRotation, Time.deltaTime * rotationDamping);
        }
    }
    private void DeActivatingTheGun()
    {
        var animator = UI_Manager.Instance.CharacterMovements.animator;
        if (animator != null)
        {
            animator.SetLayerWeight(1, 0);
            crossHair.SetActive(false);
            targetForCrossHair.gameObject.SetActive(false);
            leftHandPos.weight = 0;
            aim.weight = 0;
            UI_Manager.Instance.CharacterMovements.iscameraReset = false;
            if (!UI_Manager.Instance.isPlayerInField)
           UI_Manager.Instance.CharacterMovements.gameObject.GetComponent<CamerasSwitch>().SwitchToCam(0);
        }
    }

    public void WeaponReload()
    {
        StartCoroutine(PerformReload());

        IEnumerator PerformReload()
        {
            leftHandPos.weight = 0;
            aim.weight = 0;
            _canReload = false;
            isReloading = true;
            UI_Manager.Instance.CharacterMovements.animator.SetTrigger("IsReloading");
            yield return new WaitForSeconds(reloadTime);
            isReloading = false;
            UI_Manager.Instance.CharacterMovements.animator.SetBool("IsReloading", isReloading);
            leftHandPos.weight = 0.4f;
            aim.weight = 0.4f;
            _canReload = true;
        }
    }

    private bool IsPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

}


