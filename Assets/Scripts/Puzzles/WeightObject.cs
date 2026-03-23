using UnityEngine;

[DisallowMultipleComponent]
public class WeightObject : MonoBehaviour
{
    [SerializeField, Min(0f)] private float weight = 1f;

    // Pressure plates read this value when calculating total plate load.
    public float Weight => weight;

    public void SetWeight(float value)
    {
        weight = Mathf.Max(0f, value);
    }
}
