using UnityEngine;

public class FishingBobber : MonoBehaviour
{
    [Header("Float")]
    public Rigidbody rb;
    public float waterLevel = 0f;
    public float floatForce = 12f;
    public float bobAmplitude = 0.05f;
    public float bobFrequency = 2.2f;
    public float surfaceAlignSpeed = 6f;
    public float maxBuoyancyDepth = 0.75f;
    public float alignActivationRange = 1.5f;

    [Header("FX")]
    [Tooltip("Existing particle system to reuse; leave empty to spawn the prefab when needed.")]
    [SerializeField] private ParticleSystem splashFx;
    [Tooltip("Particle system prefab spawned on first water contact if no existing system is assigned.")]
    [SerializeField] private ParticleSystem splashPrefab;

    float seed;
    float restHeight;
    float surfaceOffset;
    bool hasTouchedWater;

    float baseAmplitude;
    float baseFrequency;
    float amplitudeMultiplier = 1f;
    float frequencyMultiplier = 1f;
    float wiggleTimer;

    void Awake()
    {
        if (!rb)
            rb = GetComponent<Rigidbody>();

        seed = Random.value * 10f;
        baseAmplitude = bobAmplitude;
        baseFrequency = bobFrequency;
        restHeight = waterLevel;
        surfaceOffset = 0f;
    }

    void OnEnable()
    {
        if (splashFx != null)
        {
            splashFx.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    void FixedUpdate()
    {
        if (rb == null)
            return;

        if (wiggleTimer > 0f)
        {
            wiggleTimer -= Time.fixedDeltaTime;
            if (wiggleTimer <= 0f)
            {
                wiggleTimer = 0f;
                amplitudeMultiplier = 1f;
                frequencyMultiplier = 1f;
            }
        }

        float alignFactor = Mathf.Clamp01(surfaceAlignSpeed * Time.fixedDeltaTime);
        float desiredRestHeight = waterLevel + surfaceOffset;
        restHeight = Mathf.Lerp(restHeight, desiredRestHeight, alignFactor);

        Vector3 pos = rb.position;
        bool nearSurface = Mathf.Abs(pos.y - restHeight) <= alignActivationRange;
        bool belowRest = pos.y < restHeight;

        if (!hasTouchedWater && nearSurface)
        {
            hasTouchedWater = true;
            rb.useGravity = false;

            waterLevel = pos.y - surfaceOffset;
            restHeight = pos.y;

            pos.y = restHeight;
            rb.position = pos;

            Vector3 snappedVelocity = rb.velocity;
            snappedVelocity.y = Mathf.Max(0f, snappedVelocity.y);
            rb.velocity = snappedVelocity;

            SpawnSplashIfNeeded();
            PlaySplash();
            if (WaterRippleManager.Instance != null)
                WaterRippleManager.Instance.AddRipple(rb.position);
        }

        if (hasTouchedWater)
        {
            rb.useGravity = false;

            if (belowRest)
            {
                float depth = Mathf.Clamp(restHeight - pos.y, 0f, maxBuoyancyDepth);
                float normalizedDepth = maxBuoyancyDepth > 0.001f ? depth / maxBuoyancyDepth : 1f;
                float lift = Physics.gravity.magnitude + floatForce * normalizedDepth;
                rb.AddForce(Vector3.up * lift, ForceMode.Acceleration);
            }

            float amplitude = baseAmplitude * amplitudeMultiplier;
            float frequency = baseFrequency * frequencyMultiplier;
            float bobOffset = Mathf.Sin((Time.time + seed) * frequency) * amplitude;

            pos.y = Mathf.Lerp(pos.y, restHeight + bobOffset, alignFactor);
            rb.MovePosition(pos);

            Vector3 velocity = rb.velocity;
            velocity.y = Mathf.Lerp(velocity.y, 0f, alignFactor);
            velocity.x = Mathf.Lerp(velocity.x, 0f, alignFactor * 0.5f);
            velocity.z = Mathf.Lerp(velocity.z, 0f, alignFactor * 0.5f);
            rb.velocity = velocity;

            rb.drag = Mathf.Lerp(rb.drag, 2.5f, alignFactor);
            rb.angularDrag = Mathf.Lerp(rb.angularDrag, 1.5f, alignFactor);
        }
        else
        {
            rb.useGravity = true;
            rb.drag = Mathf.Lerp(rb.drag, 0.5f, alignFactor);
            rb.angularDrag = Mathf.Lerp(rb.angularDrag, 0.05f, alignFactor);
        }
    }

    public void BiteNudge()
    {
        if (rb == null)
            return;

        rb.AddForce(Vector3.down * 0.75f, ForceMode.VelocityChange);
        rb.AddTorque(Random.onUnitSphere * 0.5f, ForceMode.VelocityChange);

        if (hasTouchedWater)
        {
            SpawnSplashIfNeeded();
            PlaySplash();
            if (WaterRippleManager.Instance != null)
                WaterRippleManager.Instance.AddRipple(rb.position);
        }
    }

    public void TriggerBite(float amplitudeBoost = 2f, float frequencyBoost = 1.6f, float duration = 1.2f, bool addImpulse = true)
    {
        amplitudeMultiplier = Mathf.Max(0.1f, amplitudeBoost);
        frequencyMultiplier = Mathf.Max(0.1f, frequencyBoost);
        wiggleTimer = Mathf.Max(0f, duration);

        if (addImpulse)
            BiteNudge();
    }

    public void ConfigureSurface(float surfaceHeight, float offset)
    {
        waterLevel = surfaceHeight;
        surfaceOffset = offset;
        restHeight = surfaceHeight + offset;
        baseAmplitude = bobAmplitude;
        baseFrequency = bobFrequency;

        hasTouchedWater = false;
        amplitudeMultiplier = 1f;
        frequencyMultiplier = 1f;
        wiggleTimer = 0f;

        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.useGravity = true;
            rb.drag = 0.5f;
            rb.angularDrag = 0.05f;
        }
    }

    void SpawnSplashIfNeeded()
    {
        if (splashFx == null && splashPrefab != null)
        {
            splashFx = Instantiate(splashPrefab, transform);
            splashFx.transform.localPosition = Vector3.zero;
            splashFx.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    void PlaySplash()
    {
        if (splashFx == null)
            return;

        splashFx.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        splashFx.Play();
    }
}

