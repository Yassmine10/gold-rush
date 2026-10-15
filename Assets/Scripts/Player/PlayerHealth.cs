using UnityEngine;
using System.Collections;

/// <summary>
/// Gère la santé du joueur (3 cœurs 💖 💖 💖).
/// Gère le clignotement d'invincibilité après dégât et le respawn.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [Header("Santé")]
    public int maxHealth = 3;
    public int currentHealth = 3;
    public float invincibilityDuration = 1.5f;

    [Header("Respawn")]
    public Vector3 respawnPoint = new Vector3(0, 1.5f, 0);

    private bool isInvincible = false;
    private CharacterController controller;
    private Renderer[] renderers;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        renderers = GetComponentsInChildren<Renderer>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount = 1)
    {
        if (isInvincible) return;

        currentHealth -= damageAmount;

        CharacterSpriteAnimator spriteAnim = GetComponentInChildren<CharacterSpriteAnimator>();
        if (spriteAnim != null) spriteAnim.TriggerHurt();

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayHitSound();
        }

        if (currentHealth <= 0)
        {
            if (HUDManager.Instance != null && HUDManager.Instance.gameOverModal != null)
            {
                HUDManager.Instance.TriggerGameOver();
            }
            else
            {
                DieAndRespawn();
            }
        }
        else
        {
            StartCoroutine(InvincibilityRoutine());
        }

        if (HUDManager.Instance != null)
        {
            HUDManager.Instance.UpdateHeartsUI(currentHealth);
        }
    }

    public void DieAndRespawn()
    {
        currentHealth = maxHealth;
        if (HUDManager.Instance != null)
        {
            HUDManager.Instance.UpdateHeartsUI(currentHealth);
        }

        if (controller != null) controller.enabled = false;
        transform.position = respawnPoint;
        PlayerController25D playerController = GetComponent<PlayerController25D>();
        if (playerController != null) playerController.ResetMotionAfterRespawn();
        if (controller != null) controller.enabled = true;

        StartCoroutine(InvincibilityRoutine());
    }

    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;
        float elapsed = 0f;

        while (elapsed < invincibilityDuration)
        {
            elapsed += 0.15f;
            SetRenderersEnabled(false);
            yield return new WaitForSeconds(0.075f);
            SetRenderersEnabled(true);
            yield return new WaitForSeconds(0.075f);
        }

        SetRenderersEnabled(true);
        isInvincible = false;
    }

    private void SetRenderersEnabled(bool state)
    {
        if (renderers == null) return;
        foreach (var r in renderers)
        {
            if (r != null) r.enabled = state;
        }
    }

    public void SetRespawnPoint(Vector3 newPoint)
    {
        respawnPoint = newPoint;
    }
}
