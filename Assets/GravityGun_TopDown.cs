using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityGun_TopDown : MonoBehaviour
{
    public float pickupRange = 5f;
    public float moveSpeed = 15f;
    public float throwForce = 20f;
    public float throwHoldDuration = 0.35f;
    public float fullChargeDuration = 1.1f;
    public Color hoverHighlightColor = Color.white;
    public Color throwChargeStartColor = Color.green;
    public Color throwChargeMidColor = new Color(1f, 0.55f, 0f); // Orange
    public Color throwChargeMaxColor = Color.red;

    public Transform holdPoint;
    public LayerMask pickupLayer;
    [Header("Throw Camera Shake")]
    [SerializeField] private CamerasSwitch camerasSwitch;
    public float throwShakeAmplitude = 1.1f;
    public float throwShakeFrequency = 2.1f;
    public float throwShakeDuration = 0.2f;

    private Rigidbody heldObject;
    private Camera cam;
    private bool isHoldingThrowInput;
    private float throwInputStartTime;

    private Renderer highlightedRenderer;
    private Material[] highlightedMaterials;
    private string[] highlightedColorProperties;
    private Color[] highlightedOriginalColors;

    void Start()
    {
        cam = Camera.main;
        if (camerasSwitch == null)
            camerasSwitch = GetComponent<CamerasSwitch>();
        if (camerasSwitch == null)
            camerasSwitch = GetComponentInParent<CamerasSwitch>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (heldObject == null)
                TryPickup();
            else
                BeginThrowHold();
        }

        if (Input.GetMouseButtonUp(0) && heldObject != null && isHoldingThrowInput)
        {
            EndThrowHold();
        }

        UpdateHighlightVisuals();
    }

    void FixedUpdate()
    {
        if (heldObject != null)
        {
            MoveObject();
        }
    }

    void TryPickup()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, pickupLayer))
        {
            float distance = Vector3.Distance(transform.position, hit.point);
            if (distance <= pickupRange)
            {
                Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    heldObject = rb;
                    heldObject.useGravity = false;
                    heldObject.drag = 8f;
                    isHoldingThrowInput = false;
                }
            }
        }
    }

    void MoveObject()
    {
        Vector3 direction = holdPoint.position - heldObject.position;
        heldObject.velocity = direction * moveSpeed;
        heldObject.angularVelocity = Vector3.zero;
    }

    void DropObject()
    {
        heldObject.useGravity = true;
        heldObject.drag = 1f;
        heldObject = null;
        isHoldingThrowInput = false;
    }

    void ThrowObject()
    {
        heldObject.useGravity = true;
        heldObject.drag = 1f;

        Vector3 mouseWorld = GetMouseWorldPosition();
        Vector3 throwDir = (mouseWorld - transform.position).normalized;

        heldObject.AddForce(throwDir * throwForce, ForceMode.Impulse);

        heldObject = null;
        isHoldingThrowInput = false;

        TriggerThrowCameraShake();
    }

    void TriggerThrowCameraShake()
    {
        if (camerasSwitch == null)
            return;

        camerasSwitch.ShakeCamera(throwShakeAmplitude, throwShakeFrequency, throwShakeDuration);
    }

    void BeginThrowHold()
    {
        isHoldingThrowInput = true;
        throwInputStartTime = Time.time;
    }

    void EndThrowHold()
    {
        float heldDuration = Time.time - throwInputStartTime;
        if (heldDuration >= throwHoldDuration)
            ThrowObject();
        else
            DropObject();
    }

    void UpdateHoverHighlight()
    {
        Renderer targetRenderer = GetHoveredPickupRenderer();

        if (targetRenderer == highlightedRenderer)
            return;

        ClearHoverHighlight();

        if (targetRenderer != null)
            ApplyHoverHighlight(targetRenderer);
    }

    void UpdateHighlightVisuals()
    {
        if (heldObject != null && isHoldingThrowInput)
        {
            UpdateThrowChargeHighlight();
            return;
        }

        UpdateHoverHighlight();
    }

    void UpdateThrowChargeHighlight()
    {
        Renderer heldRenderer = GetRendererFromCollider(heldObject.GetComponent<Collider>());
        if (heldRenderer == null)
        {
            ClearHoverHighlight();
            return;
        }

        if (heldRenderer != highlightedRenderer)
        {
            ClearHoverHighlight();
            ApplyHoverHighlight(heldRenderer);
        }

        float chargeDuration = Mathf.Max(0.01f, fullChargeDuration);
        float rawT = Mathf.Clamp01((Time.time - throwInputStartTime) / chargeDuration);
        float t = Mathf.SmoothStep(0f, 1f, rawT);
        Color chargeColor = EvaluateThrowChargeColor(t);
        SetHighlightColor(chargeColor);
    }

    Color EvaluateThrowChargeColor(float t)
    {
        if (t <= 0.5f)
            return Color.Lerp(throwChargeStartColor, throwChargeMidColor, t / 0.5f);

        return Color.Lerp(throwChargeMidColor, throwChargeMaxColor, (t - 0.5f) / 0.5f);
    }

    Renderer GetHoveredPickupRenderer()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit, 100f, pickupLayer))
            return null;

        float distance = Vector3.Distance(transform.position, hit.point);
        if (distance > pickupRange)
            return null;

        return GetRendererFromCollider(hit.collider);
    }

    Renderer GetRendererFromCollider(Collider sourceCollider)
    {
        if (sourceCollider == null)
            return null;

        Renderer renderer = sourceCollider.GetComponent<Renderer>();
        if (renderer == null)
            renderer = sourceCollider.GetComponentInChildren<Renderer>();
        if (renderer == null)
            renderer = sourceCollider.GetComponentInParent<Renderer>();
        return renderer;
    }

    void ApplyHoverHighlight(Renderer renderer)
    {
        if (renderer == null)
            return;

        Material[] materials = renderer.materials;
        if (materials == null || materials.Length == 0)
            return;

        highlightedRenderer = renderer;
        highlightedMaterials = materials;
        highlightedColorProperties = new string[materials.Length];
        highlightedOriginalColors = new Color[materials.Length];

        for (int i = 0; i < materials.Length; i++)
        {
            Material mat = materials[i];
            if (mat == null)
                continue;

            string colorProperty = null;
            if (mat.HasProperty("_BaseColor"))
                colorProperty = "_BaseColor";
            else if (mat.HasProperty("_Color"))
                colorProperty = "_Color";

            if (string.IsNullOrEmpty(colorProperty))
                continue;

            highlightedColorProperties[i] = colorProperty;
            highlightedOriginalColors[i] = mat.GetColor(colorProperty);
        }

        SetHighlightColor(hoverHighlightColor);
    }

    void SetHighlightColor(Color color)
    {
        if (highlightedMaterials == null || highlightedColorProperties == null)
            return;

        for (int i = 0; i < highlightedMaterials.Length; i++)
        {
            Material mat = highlightedMaterials[i];
            string colorProperty = highlightedColorProperties[i];

            if (mat != null && !string.IsNullOrEmpty(colorProperty))
                mat.SetColor(colorProperty, color);
        }
    }

    void ClearHoverHighlight()
    {
        if (highlightedMaterials != null && highlightedColorProperties != null && highlightedOriginalColors != null)
        {
            for (int i = 0; i < highlightedMaterials.Length; i++)
            {
                Material mat = highlightedMaterials[i];
                string colorProperty = highlightedColorProperties[i];

                if (mat != null && !string.IsNullOrEmpty(colorProperty))
                    mat.SetColor(colorProperty, highlightedOriginalColors[i]);
            }
        }

        highlightedRenderer = null;
        highlightedMaterials = null;
        highlightedColorProperties = null;
        highlightedOriginalColors = null;
    }

    void OnDisable()
    {
        ClearHoverHighlight();
    }

    Vector3 GetMouseWorldPosition()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, Vector3.zero);

        if (ground.Raycast(ray, out float enter))
        {
            return ray.GetPoint(enter);
        }

        return transform.position;
    }
}
