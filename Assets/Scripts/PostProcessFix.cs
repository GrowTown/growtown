using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessFix : MonoBehaviour
{
    void Start()
    {
        #if UNITY_ANDROID
        Volume volume = GetComponent<Volume>() ?? FindObjectOfType<Volume>();
        if (volume != null && volume.profile.TryGet<DepthOfField>(out var dof))
        {
            dof.active = false;
            Debug.Log("✅ Disabled Depth of Field on Android to avoid texture format issue.");
        }
        #endif
    }
}