using UnityEngine;
using TMPro;

/// <summary>
/// Gère la collecte des pièces dorées, l'effet de glow, le feedback visuel (+10) et le score.
/// </summary>
public class Coin : MonoBehaviour
{
    [Header("Valeur")]
    public int coinValue = 10;
    public int scorePoints = 100;

    [Header("Effets & Préfabs")]
    [Tooltip("Préfab de texte flottant '+10'")]
    public GameObject floatingTextPrefab;
    [Tooltip("Particules d'explosion dorée / glow lors de la collecte")]
    public GameObject collectParticlePrefab;
    [Tooltip("Son joué lors de la collecte")]
    public AudioClip collectSound;

    private bool isCollected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isCollected) return;

        // Vérifier si c'est le joueur qui touche la pièce
        if (other.CompareTag("Player") || other.GetComponent<PlayerController25D>() != null)
        {
            CollectCoin();
        }
    }

    public void CollectCoin()
    {
        isCollected = true;

        SpriteRenderer coinRenderer = GetComponent<SpriteRenderer>();
        if (coinRenderer != null)
            CollectibleBurstVFX.Spawn(coinRenderer, new Color(1f, 0.78f, 0.22f, 0.95f), 0.34f, 1.55f);

        // 1. Ajouter au gestionnaire de HUD / Game Manager
        if (HUDManager.Instance != null)
        {
            HUDManager.Instance.AddCoin(1, scorePoints);
        }

        // 2. Jouer le son
        if (collectSound != null)
        {
            if (SoundManager.Instance != null) SoundManager.Instance.PlayCustomSFX(collectSound);
            else AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }
        else if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayCoinSound();
        }

        // 3. Spawner le feedback visuel "+10"
        if (floatingTextPrefab != null)
        {
            GameObject popup = Instantiate(floatingTextPrefab, transform.position + Vector3.up * 0.8f, Quaternion.identity);
            FloatingFeedbackText feedback = popup.GetComponent<FloatingFeedbackText>();
            if (feedback != null)
            {
                feedback.SetText($"+{coinValue}");
            }
        }

        // 4. Spawner l'effet de particules Glow
        if (collectParticlePrefab != null)
        {
            Instantiate(collectParticlePrefab, transform.position, Quaternion.identity);
        }

        // 5. Détruire la pièce
        Destroy(gameObject);
    }
}
