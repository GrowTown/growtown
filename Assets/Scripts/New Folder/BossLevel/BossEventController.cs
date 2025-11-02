using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class BossEventController : MonoBehaviour
{
    [Header("References")]
    public BossHealth boss;               // Boss health script reference
    public Slider bossHealthBar;          // UI slider for boss HP
    public Image fadeImage;               // Optional fade overlay
    public AudioSource ambientMusic;      // Calm music
    public AudioSource combatMusic;       // Battle music
    public GameObject exitTeleport;       // Portal or exit
    public GameObject victoryEffect;      // Explosion / fireworks prefab

    [Header("Cinematic Camera")]
    public Camera mainCamera;             // Main player camera
    public Transform bossFocusPoint;      // Empty transform above the boss
    public Transform playerFocusPoint;    // Empty transform near player (for reset)
    public float cameraMoveSpeed = 2.5f;  // Camera pan speed
    public float cameraHoldTime = 2.0f;   // How long to hold on boss before fight

    [Header("Settings")]
    public float fadeDuration = 1.5f;

    private bool fightActive;
    private bool fightFinished;

    void Start()
    {
        if (bossHealthBar != null) bossHealthBar.gameObject.SetActive(false);
        if (exitTeleport != null) exitTeleport.SetActive(false);
        if (fadeImage != null) fadeImage.color = new Color(0, 0, 0, 0);
    }

    void Update()
    {
        if (fightActive && boss != null && bossHealthBar != null)
        {
            bossHealthBar.value = boss.CurrentHealthNormalized;
        }

        if (fightActive && !fightFinished && boss == null)
        {
            fightFinished = true;
            StartCoroutine(EndBossFight());
        }
    }

    public void StartBossFight()
    {
        if (fightActive) return;
        StartCoroutine(BossCinematicSequence());
    }

    private IEnumerator BossCinematicSequence()
    {
        fightActive = true;

        if (fadeImage != null)
        {
            // Fade out to black before cinematic
            for (float t = 0; t < 1f; t += Time.deltaTime / fadeDuration)
            {
                fadeImage.color = new Color(0, 0, 0, t);
                yield return null;
            }
            fadeImage.color = Color.black;
        }

        // Stop ambient music
        if (ambientMusic != null) ambientMusic.Stop();

        yield return new WaitForSeconds(0.5f);

        if (mainCamera != null && bossFocusPoint != null)
        {
            Vector3 startPos = mainCamera.transform.position;
            Quaternion startRot = mainCamera.transform.rotation;

            Vector3 targetPos = bossFocusPoint.position + (bossFocusPoint.forward * -6f) + Vector3.up * 3f;
            Quaternion targetRot = Quaternion.LookRotation(bossFocusPoint.position - targetPos);

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * cameraMoveSpeed;
                mainCamera.transform.position = Vector3.Lerp(startPos, targetPos, t);
                mainCamera.transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
                yield return null;
            }

            // Hold on the boss for dramatic effect
            yield return new WaitForSeconds(cameraHoldTime);
        }

        // Fade back in to gameplay
        if (fadeImage != null)
        {
            for (float t = 1f; t > 0f; t -= Time.deltaTime / fadeDuration)
            {
                fadeImage.color = new Color(0, 0, 0, t);
                yield return null;
            }
            fadeImage.color = Color.clear;
        }

        // ✅ Enable UI
        if (bossHealthBar != null)
        {
            bossHealthBar.gameObject.SetActive(true);
            bossHealthBar.maxValue = 1f;
            bossHealthBar.value = 1f;
        }

        // Start combat music
        if (combatMusic != null) combatMusic.Play();

        Debug.Log("[BossEventController] Boss fight started!");
    }

    private IEnumerator EndBossFight()
    {
        Debug.Log("[BossEventController] Boss defeated!");

        if (combatMusic != null) combatMusic.Stop();
        if (ambientMusic != null) ambientMusic.Play();

        if (victoryEffect != null && boss != null)
            Instantiate(victoryEffect, boss.transform.position, Quaternion.identity);

        yield return new WaitForSeconds(2f);

        // Fade to black for exit
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            Color c = fadeImage.color;
            for (float t = 0; t < 1f; t += Time.deltaTime / fadeDuration)
            {
                c.a = Mathf.Lerp(0f, 1f, t);
                fadeImage.color = c;
                yield return null;
            }
            fadeImage.color = Color.black;
        }

        if (exitTeleport != null)
            exitTeleport.SetActive(true);

        if (bossHealthBar != null)
            bossHealthBar.gameObject.SetActive(false);

        Debug.Log("[BossEventController] Boss event complete!");
    }
}
