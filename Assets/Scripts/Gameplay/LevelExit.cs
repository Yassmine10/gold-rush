using UnityEngine;

/// <summary>
/// Détecte quand le joueur atteint le drapeau / portail de sortie.
/// Déclenche la victoire et affiche la fenêtre "NIVEAU TERMINÉ !".
/// </summary>
public class LevelExit : MonoBehaviour
{
    public ParticleSystem victoryParticles;
    [Min(0)] public int minimumCoinsToWin = 8;
    private bool isTriggered = false;

    private void Start()
    {
        if (HUDManager.Instance != null)
            minimumCoinsToWin = LevelBalance.GetMinimumCoins(HUDManager.Instance.currentLevelNumber);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;

        PlayerController25D player = other.GetComponent<PlayerController25D>()
                                  ?? other.GetComponentInParent<PlayerController25D>();
        if (player == null && other.CompareTag("Player"))
        {
            player = FindFirstObjectByType<PlayerController25D>();
        }

        if (player != null)
        {
            if (HUDManager.Instance != null && !HUDManager.Instance.HasMinimumCoins(minimumCoinsToWin))
            {
                HUDManager.Instance.ShowCoinsRequired(minimumCoinsToWin);
                return;
            }
            isTriggered = true;

            player.TriggerVictory();
            SpriteRenderer playerSprite = player.GetComponentInChildren<SpriteRenderer>();
            if (playerSprite != null)
            {
                CollectibleBurstVFX.Spawn(playerSprite, new Color(1f, 0.82f, 0.3f, 0.95f), 0.48f, 1.45f);
                CollectibleBurstVFX.Spawn(playerSprite, new Color(0.35f, 0.9f, 0.75f, 0.8f), 0.58f, 1.75f);
            }

            if (victoryParticles != null)
            {
                ParticleSystem.MainModule particleSettings = victoryParticles.main;
                particleSettings.useUnscaledTime = true;
                victoryParticles.Play();
            }

            if (HUDManager.Instance != null)
            {
                HUDManager.Instance.TriggerFinishLevel();
            }
        }
    }
}
