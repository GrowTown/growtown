using System.Collections.Generic;
using UnityEngine;

public class WaterRippleManager : MonoBehaviour
{
    public static WaterRippleManager Instance { get; private set; }

    [SerializeField] private int maxRipples = 16;
    [SerializeField] private float rippleLifetime = 2.5f;
    [SerializeField] private float defaultAmplitude = 0.4f;

    private struct Ripple
    {
        public Vector3 position;
        public float startTime;
        public float amplitude;
    }

    private readonly List<Ripple> _activeRipples = new List<Ripple>();
    private Vector4[] _rippleBuffer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _rippleBuffer = new Vector4[Mathf.Max(1, maxRipples)];
    }

    private void OnDisable()
    {
        Shader.SetGlobalInt("_RippleSourceCount", 0);
        if (_rippleBuffer != null)
        {
            System.Array.Clear(_rippleBuffer, 0, _rippleBuffer.Length);
            Shader.SetGlobalVectorArray("_RippleSources", _rippleBuffer);
        }
    }

    public void AddRipple(Vector3 position, float amplitudeMultiplier = 1f)
    {
        if (_activeRipples.Count >= maxRipples)
        {
            _activeRipples.RemoveAt(0);
        }

        _activeRipples.Add(new Ripple
        {
            position = position,
            startTime = Time.time,
            amplitude = defaultAmplitude * amplitudeMultiplier
        });
    }

    private void LateUpdate()
    {
        float time = Time.time;
        int count = 0;

        for (int i = 0; i < _rippleBuffer.Length; ++i)
        {
            _rippleBuffer[i] = Vector4.zero;
        }

        for (int i = 0; i < _activeRipples.Count;)
        {
            Ripple ripple = _activeRipples[i];
            float age = time - ripple.startTime;
            float life = Mathf.Clamp01(1f - age / rippleLifetime);

            if (life <= 0f)
            {
                _activeRipples.RemoveAt(i);
                continue;
            }

            if (count < _rippleBuffer.Length)
            {
                _rippleBuffer[count] = new Vector4(ripple.position.x, ripple.position.y, ripple.position.z, ripple.amplitude * life);
                count++;
            }

            i++;
        }

        Shader.SetGlobalInt("_RippleSourceCount", count);
        Shader.SetGlobalVectorArray("_RippleSources", _rippleBuffer);
    }
}
