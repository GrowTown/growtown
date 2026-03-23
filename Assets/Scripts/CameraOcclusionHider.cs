using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraOcclusionHider : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Vector3 _playerOffset = Vector3.up;

    [Header("Occlusion")]
    [SerializeField] private LayerMask _occluderLayers = ~0;
    [SerializeField] [Range(1f, 60f)] private float _horizontalConeAngle = 14f;
    [SerializeField] [Range(1f, 60f)] private float _verticalConeAngle = 10f;
    [SerializeField] private float _coneBaseRadiusX = 0.2f;
    [SerializeField] private float _coneBaseRadiusY = 0.12f;
    [SerializeField] private int _maxOverlapColliders = 256;
    [SerializeField] private int _maxLineHits = 64;
    [SerializeField] [Range(0.05f, 1f)] private float _minAlpha = 0.25f;
    [SerializeField] private float _fadeSpeed = 8f;
    [SerializeField] private float _coneFalloffPower = 1.5f;
    [SerializeField] private float _playerDepthPadding = 0.15f;
    [SerializeField] private bool _requireDirectOcclusion = true;
    [SerializeField] private bool _useFallbackTransparentMaterial = true;

    private Collider[] _overlapColliders;
    private RaycastHit[] _lineHits;
    private readonly HashSet<Renderer> _frameOccluders = new HashSet<Renderer>();
    private readonly HashSet<Renderer> _directOccluders = new HashSet<Renderer>();
    private readonly List<Renderer> _stateBuffer = new List<Renderer>();
    private readonly Dictionary<Renderer, FadeState> _fadeStates = new Dictionary<Renderer, FadeState>();

    private sealed class FadeState
    {
        public Renderer Renderer;
        public Material[] OriginalMaterials;
        public Material[] FadedMaterials;
        public float CurrentAlpha = 1f;
        public float TargetAlpha = 1f;
        public bool IsUsingFadedMaterials;
    }

    private void Awake()
    {
        if (_cameraTransform == null)
        {
            Camera mainCamera = Camera.main;
            _cameraTransform = mainCamera != null ? mainCamera.transform : transform;
        }

        if (_maxOverlapColliders < 1)
        {
            _maxOverlapColliders = 1;
        }
        if (_maxLineHits < 1)
        {
            _maxLineHits = 1;
        }

        if (_playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _playerTransform = player.transform;
            }
        }

        _overlapColliders = new Collider[_maxOverlapColliders];
        _lineHits = new RaycastHit[_maxLineHits];
    }

    private void LateUpdate()
    {
        if (_cameraTransform == null || _playerTransform == null)
        {
            return;
        }

        Vector3 from = _cameraTransform.position;
        Vector3 to = _playerTransform.position + _playerOffset;
        Vector3 delta = to - from;
        float distance = delta.magnitude;

        if (distance <= 0.001f)
        {
            ResetTargets();
            UpdateFades(Time.deltaTime);
            return;
        }

        Vector3 direction = delta / distance;
        Vector3 right = _cameraTransform.right;
        Vector3 up = _cameraTransform.up;

        _frameOccluders.Clear();
        ResetTargets();
        RebuildDirectOccluders(from, direction, distance);

        int hitCount = Physics.OverlapSphereNonAlloc(
            from,
            distance,
            _overlapColliders,
            _occluderLayers,
            QueryTriggerInteraction.Ignore
        );

        for (int i = 0; i < hitCount; i++)
        {
            Collider hitCollider = _overlapColliders[i];
            if (hitCollider == null)
            {
                continue;
            }

            Transform candidateTransform = hitCollider.transform;
            if (IsIgnored(candidateTransform))
            {
                continue;
            }

            Renderer rendererToFade = hitCollider.GetComponentInParent<Renderer>();
            if (rendererToFade == null)
            {
                continue;
            }

            if (_requireDirectOcclusion && !_directOccluders.Contains(rendererToFade))
            {
                continue;
            }

            Vector3 closestPoint = hitCollider.ClosestPoint(from);
            Vector3 toPoint = closestPoint - from;
            float projection = Vector3.Dot(toPoint, direction);

            // Only affect blockers that are between camera and player.
            if (projection <= 0f || projection >= distance)
            {
                continue;
            }

            // Prevent fading objects that are effectively beyond player depth.
            float centerProjection = Vector3.Dot(rendererToFade.bounds.center - from, direction);
            if (centerProjection >= distance - _playerDepthPadding)
            {
                continue;
            }

            Vector3 projectedPoint = direction * projection;
            Vector3 lateral = toPoint - projectedPoint;
            float lateralX = Vector3.Dot(lateral, right);
            float lateralY = Vector3.Dot(lateral, up);

            float radiusX = Mathf.Tan(_horizontalConeAngle * Mathf.Deg2Rad) * projection + _coneBaseRadiusX;
            float radiusY = Mathf.Tan(_verticalConeAngle * Mathf.Deg2Rad) * projection + _coneBaseRadiusY;
            radiusX = Mathf.Max(0.0001f, radiusX);
            radiusY = Mathf.Max(0.0001f, radiusY);

            float ellipse = (lateralX * lateralX) / (radiusX * radiusX) + (lateralY * lateralY) / (radiusY * radiusY);
            if (ellipse > 1f)
            {
                continue;
            }

            float normalized = Mathf.Clamp01(Mathf.Sqrt(ellipse));
            float falloff = Mathf.Pow(normalized, Mathf.Max(0.01f, _coneFalloffPower));
            float targetAlpha = Mathf.Lerp(_minAlpha, 1f, falloff);

            _frameOccluders.Add(rendererToFade);

            FadeState state = EnsureFadeState(rendererToFade);
            state.TargetAlpha = Mathf.Min(state.TargetAlpha, targetAlpha);
        }

        foreach (KeyValuePair<Renderer, FadeState> pair in _fadeStates)
        {
            if (pair.Key == null)
            {
                continue;
            }

            if (!_frameOccluders.Contains(pair.Key))
            {
                pair.Value.TargetAlpha = 1f;
            }
        }

        UpdateFades(Time.deltaTime);
    }

    private void RebuildDirectOccluders(Vector3 from, Vector3 direction, float distance)
    {
        _directOccluders.Clear();
        if (!_requireDirectOcclusion)
        {
            return;
        }

        int lineHitCount = Physics.RaycastNonAlloc(
            from,
            direction,
            _lineHits,
            distance,
            _occluderLayers,
            QueryTriggerInteraction.Ignore
        );

        for (int i = 0; i < lineHitCount; i++)
        {
            Collider hitCollider = _lineHits[i].collider;
            if (hitCollider == null)
            {
                continue;
            }

            Renderer renderer = hitCollider.GetComponentInParent<Renderer>();
            if (renderer == null)
            {
                continue;
            }

            if (IsIgnored(renderer.transform))
            {
                continue;
            }

            _directOccluders.Add(renderer);
        }
    }

    private bool IsIgnored(Transform candidate)
    {
        if (candidate == null)
        {
            return true;
        }

        if (_playerTransform != null && (candidate == _playerTransform || candidate.IsChildOf(_playerTransform)))
        {
            return true;
        }

        if (_cameraTransform != null && (candidate == _cameraTransform || candidate.IsChildOf(_cameraTransform)))
        {
            return true;
        }

        return false;
    }

    private void OnDisable()
    {
        RestoreAll();
    }

    private void OnDestroy()
    {
        RestoreAll();
    }

    private void ResetTargets()
    {
        foreach (FadeState state in _fadeStates.Values)
        {
            state.TargetAlpha = 1f;
        }
    }

    private FadeState EnsureFadeState(Renderer renderer)
    {
        if (_fadeStates.TryGetValue(renderer, out FadeState existing))
        {
            return existing;
        }

        Material[] original = renderer.sharedMaterials;
        Material[] faded = new Material[original.Length];

        for (int i = 0; i < original.Length; i++)
        {
            Material source = original[i];
            if (source == null)
            {
                continue;
            }

            Material instance = CreateFadedMaterial(source);
            faded[i] = instance;
        }

        FadeState state = new FadeState
        {
            Renderer = renderer,
            OriginalMaterials = original,
            FadedMaterials = faded
        };

        _fadeStates.Add(renderer, state);
        return state;
    }

    private void UpdateFades(float deltaTime)
    {
        _stateBuffer.Clear();

        foreach (KeyValuePair<Renderer, FadeState> pair in _fadeStates)
        {
            FadeState state = pair.Value;
            Renderer renderer = state.Renderer;

            if (renderer == null)
            {
                _stateBuffer.Add(pair.Key);
                continue;
            }

            state.CurrentAlpha = Mathf.MoveTowards(state.CurrentAlpha, state.TargetAlpha, _fadeSpeed * deltaTime);
            bool shouldFade = state.CurrentAlpha < 0.999f || state.TargetAlpha < 0.999f;

            if (shouldFade && !state.IsUsingFadedMaterials)
            {
                renderer.materials = state.FadedMaterials;
                state.IsUsingFadedMaterials = true;
            }

            if (state.IsUsingFadedMaterials)
            {
                for (int i = 0; i < state.FadedMaterials.Length; i++)
                {
                    Material material = state.FadedMaterials[i];
                    if (material != null)
                    {
                        SetAlpha(material, state.CurrentAlpha);
                    }
                }

                if (state.TargetAlpha >= 0.999f && state.CurrentAlpha >= 0.999f)
                {
                    renderer.materials = state.OriginalMaterials;
                    state.IsUsingFadedMaterials = false;
                }
            }
        }

        for (int i = 0; i < _stateBuffer.Count; i++)
        {
            _fadeStates.Remove(_stateBuffer[i]);
        }
    }

    private static void MakeTransparent(Material material)
    {
        if (material == null)
        {
            return;
        }

        if (material.HasProperty("_Surface"))
        {
            material.SetFloat("_Surface", 1f);
        }

        if (material.HasProperty("_Mode"))
        {
            material.SetFloat("_Mode", 3f);
        }

        material.SetOverrideTag("RenderType", "Transparent");
        material.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        material.renderQueue = (int)RenderQueue.Transparent;
    }

    private Material CreateFadedMaterial(Material source)
    {
        Material instance = new Material(source);
        MakeTransparent(instance);
        SetAlpha(instance, 1f);

        if (_useFallbackTransparentMaterial && !CanFadeAlpha(instance))
        {
            Destroy(instance);
            instance = CreateFallbackTransparentMaterial(source);
        }

        return instance;
    }

    private static bool CanFadeAlpha(Material material)
    {
        return material.HasProperty("_BaseColor") || material.HasProperty("_Color");
    }

    private static Material CreateFallbackTransparentMaterial(Material source)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
        {
            shader = Shader.Find("Standard");
        }
        if (shader == null)
        {
            shader = Shader.Find("Legacy Shaders/Transparent/Diffuse");
        }

        Material fallback = new Material(shader);
        MakeTransparent(fallback);

        if (source != null)
        {
            Texture baseMap = null;
            if (source.HasProperty("_BaseMap"))
            {
                baseMap = source.GetTexture("_BaseMap");
            }
            else if (source.HasProperty("_MainTex"))
            {
                baseMap = source.GetTexture("_MainTex");
            }

            if (baseMap != null)
            {
                if (fallback.HasProperty("_BaseMap"))
                {
                    fallback.SetTexture("_BaseMap", baseMap);
                }
                if (fallback.HasProperty("_MainTex"))
                {
                    fallback.SetTexture("_MainTex", baseMap);
                }
            }

            Color color = Color.white;
            if (source.HasProperty("_BaseColor"))
            {
                color = source.GetColor("_BaseColor");
            }
            else if (source.HasProperty("_Color"))
            {
                color = source.GetColor("_Color");
            }

            if (fallback.HasProperty("_BaseColor"))
            {
                fallback.SetColor("_BaseColor", color);
            }
            if (fallback.HasProperty("_Color"))
            {
                fallback.SetColor("_Color", color);
            }
        }

        SetAlpha(fallback, 1f);
        return fallback;
    }

    private static void SetAlpha(Material material, float alpha)
    {
        if (material == null)
        {
            return;
        }

        if (material.HasProperty("_BaseColor"))
        {
            Color c = material.GetColor("_BaseColor");
            c.a = alpha;
            material.SetColor("_BaseColor", c);
        }

        if (material.HasProperty("_Color"))
        {
            Color c = material.GetColor("_Color");
            c.a = alpha;
            material.SetColor("_Color", c);
        }
    }

    private void RestoreAll()
    {
        foreach (FadeState state in _fadeStates.Values)
        {
            if (state.Renderer != null)
            {
                state.Renderer.materials = state.OriginalMaterials;
            }

            if (state.FadedMaterials == null)
            {
                continue;
            }

            for (int i = 0; i < state.FadedMaterials.Length; i++)
            {
                if (state.FadedMaterials[i] != null)
                {
                    Destroy(state.FadedMaterials[i]);
                }
            }
        }

        _fadeStates.Clear();
        _frameOccluders.Clear();
        _directOccluders.Clear();
        _stateBuffer.Clear();
    }
}
