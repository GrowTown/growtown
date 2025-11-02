using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class FadeOutAndDestroyWithSound : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private float delayBeforeFade = 0f;
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private bool destroyAfterFade = true;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource; // Assign in Inspector or auto-added
    [SerializeField] private AudioClip fadeSound;     // Optional: sound to play while fading
    [SerializeField] private bool playOnStart = true; // Toggle if sound should auto-play

    private Renderer objRenderer;
    private Material fadeMaterial;
    private Color startColor;

    void Start()
    {
        objRenderer = GetComponent<Renderer>();

        // Clone material so only this object fades
        fadeMaterial = new Material(objRenderer.material);
        objRenderer.material = fadeMaterial;
        startColor = fadeMaterial.color;

        // Ensure we have an AudioSource
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        // Play sound if needed
        if (playOnStart && fadeSound != null)
        {
            audioSource.clip = fadeSound;
            audioSource.Play();
        }

        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(delayBeforeFade);
        SetMaterialToFadeMode(fadeMaterial);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            fadeMaterial.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        // Wait for the sound to finish (if still playing)
        if (audioSource != null && audioSource.isPlaying)
            yield return new WaitForSeconds(audioSource.clip.length - audioSource.time);

        if (destroyAfterFade)
            Destroy(gameObject);
    }

    void SetMaterialToFadeMode(Material mat)
    {
        mat.SetFloat("_Mode", 2); // Transparent mode
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
    }
}
