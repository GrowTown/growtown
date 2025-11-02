using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 300f;
    private float currentHealth;

    [Header("UI")]
    public Slider bossHealthBar;          // World-space or screen-space slider
    public Image fillImage;               // Optional fill image for color
    public Gradient healthColor;          // Green → Yellow → Red gradient

    [Header("Effects")]
    public ParticleSystem hitEffect;
    public ParticleSystem deathEffect;
    public AudioSource audioSource;
    public AudioClip hitSound;
    public AudioClip deathSound;

    [Header("References")]
    public Animator anim;
    public BossController bossController; // To disable on death
    public GameObject victoryTrigger;     // Optional trigger or teleporter

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        if (anim == null)
            anim = GetComponent<Animator>();
        if (bossController == null)
            bossController = GetComponent<BossController>();

        UpdateHealthUI();
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (hitEffect != null)
            hitEffect.Play();

        if (audioSource != null && hitSound != null)
            audioSource.PlayOneShot(hitSound);

        UpdateHealthUI();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void UpdateHealthUI()
    {
        if (bossHealthBar != null)
        {
            bossHealthBar.value = currentHealth / maxHealth;
        }

        if (fillImage != null)
        {
            fillImage.color = healthColor.Evaluate(currentHealth / maxHealth);
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("[BossHealth] Boss defeated!");

        // Disable boss logic
        if (bossController != null)
            bossController.enabled = false;

        // Play death animation
        if (anim != null)
            anim.Play("Die");

        // Audio
        if (audioSource != null && deathSound != null)
            audioSource.PlayOneShot(deathSound);

        // Visuals
        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        // Trigger victory / exit
        if (victoryTrigger != null)
            victoryTrigger.SetActive(true);

        // Cleanup
        Destroy(gameObject, 4f);
    }

    // ✅ Property for other scripts (like BossEventController)
    public float CurrentHealthNormalized
    {
        get { return currentHealth / maxHealth; }
    }

    // Optional: raw health getter
    public float CurrentHealth
    {
        get { return currentHealth; }
    }
}
