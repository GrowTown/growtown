using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TeleportZone : MonoBehaviour
{
    [Header("References")]
    public Transform player;                 // Player to teleport
    public Transform destination;            // Where to move the player
    public ParticleSystem teleportEffect;    // Effect played before teleport
    public AudioSource audioSource;          // Audio source for teleport sound
    public AudioClip teleportSound;          // Sound to play

    [Header("Fade Settings")]
    public Image fadeImage;                  // UI Image (full-screen black overlay)
    public float fadeDuration = 1f;          // Time to fade in/out
    public float delayBeforeTeleport = 1.5f; // Time before teleport happens

    [Header("Optional")]
    public bool disableMovement = true;      // Disable player movement during teleport
    public MonoBehaviour playerMovementScript; // Reference to your movement script

    private bool isTeleporting = false;

   private void OnTriggerEnter(Collider other)
{
    Debug.Log("Something entered trigger: " + other.name);

    if (isTeleporting) return;

    if (other.CompareTag("Player"))
    {
        Debug.Log("Player entered teleport zone!");
        isTeleporting = true;
        StartCoroutine(TeleportSequence());
    }
}

 private IEnumerator TeleportSequence()
{
    // Optional: disable player movement during teleport
    if (disableMovement && playerMovementScript != null)
        playerMovementScript.enabled = false;

    // 1️⃣ Play teleport particles and sound first
  // 1️⃣ Play teleport particle and sound
if (teleportEffect != null)
{
    // Spawn particle at player position (for departure)
    ParticleSystem effectInstance = Instantiate(teleportEffect, player.position, Quaternion.identity);
    effectInstance.Play();
    Destroy(effectInstance.gameObject, effectInstance.main.duration + 0.5f);
    Debug.Log("Teleport effect played at player position.");
}

if (audioSource != null && teleportSound != null)
{
    audioSource.PlayOneShot(teleportSound);
    Debug.Log("Teleport sound played.");
}

    // 2️⃣ Wait for particle duration OR custom delay
    float waitTime = delayBeforeTeleport;

    // if the particle system has its own duration, use that if longer
    if (teleportEffect != null)
    {
        float particleDuration = teleportEffect.main.duration;
        if (particleDuration > waitTime)
            waitTime = particleDuration;
    }

    yield return new WaitForSeconds(waitTime);

    // 3️⃣ Fade out to black
    if (fadeImage != null)
        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));

    // 4️⃣ Teleport player
// 4️⃣ Teleport player safely and align to ground
if (player != null && destination != null)
{
    CharacterController cc = player.GetComponent<CharacterController>();
    if (cc != null)
        cc.enabled = false;

    // Start above destination to ensure raycast hits something below
    Vector3 targetPos = destination.position + Vector3.up * 2f;
    Vector3 finalPos = destination.position;

    // Cast downward to find bridge or terrain
    if (Physics.Raycast(targetPos, Vector3.down, out RaycastHit hit, 10f, ~0, QueryTriggerInteraction.Ignore))
    {
        finalPos = hit.point;
        Debug.Log($"Ground hit: {hit.collider.name}, height={hit.point.y}");
    }
    else
    {
        Debug.LogWarning("TeleportZone: no ground detected! Using destination Y.");
        finalPos = destination.position;
    }

    // Adjust for CharacterController height so feet sit on ground
    float skin = 0.05f;
    if (cc != null)
        finalPos += Vector3.up * (cc.skinWidth + skin);

    // Move player and sync physics
    player.position = finalPos;
    Physics.SyncTransforms();

    // Re-enable controller and push down slightly to ground firmly
    if (cc != null)
    {
        cc.enabled = true;
        yield return null;
        cc.Move(Vector3.down * 0.2f);
    }

    // Optional: rotate player to face same way as destination
    player.rotation = destination.rotation;

    Debug.Log("Player teleported and perfectly grounded.");
}



    // 5️⃣ Optional: play effect again at destination
  // Arrival effect at destination
if (teleportEffect != null)
{
    ParticleSystem arrivalEffect = Instantiate(teleportEffect, destination.position, Quaternion.identity);
    arrivalEffect.Play();
    Destroy(arrivalEffect.gameObject, arrivalEffect.main.duration + 0.5f);
}

    // 6️⃣ Fade back in
    if (fadeImage != null)
        yield return StartCoroutine(Fade(1f, 0f, fadeDuration));

    // 7️⃣ Re-enable player movement
    if (disableMovement && playerMovementScript != null)
        playerMovementScript.enabled = true;

    isTeleporting = false;
}


    private IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            fadeImage.color = color;
            yield return null;
        }
    }
    private IEnumerator ReGroundCharacter(CharacterController cc)
{
    // Wait one frame so physics catches up
    yield return null;

    // Push slightly downward to refresh ground detection
    cc.Move(Vector3.down * 0.05f);
}

}
