using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Interface Game Over pour Gold Rush / Coin Collector.
/// Affichée lorsque le joueur perd ses 3 cœurs.
/// Permet de réessayer le niveau immédiatement ou de retourner au menu principal.
/// </summary>
public class GameOverUI : MonoBehaviour
{
    [Header("Textes UI")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI subtitleText;
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI scoreText;

    [Header("Boutons")]
    public Button btnRetry;
    public Button btnMainMenu;

    [Header("Effets")]
    public CanvasGroup canvasGroup;
    private Vector3 baseScale;

    void Awake()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        baseScale = transform.localScale;
        if (btnRetry != null) btnRetry.onClick.AddListener(OnRetryClicked);
        if (btnMainMenu != null) btnMainMenu.onClick.AddListener(OnMainMenuClicked);
    }

    public void ShowGameOver(int coins, int maxCoins, int score)
    {
        gameObject.SetActive(true);

        if (titleText != null) titleText.text = "ÉCHEC !";
        if (subtitleText != null) subtitleText.text = "Vous êtes tombé. Réessayez pour terminer le niveau.";

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayHitSound();
        }

        if (coinsText != null) coinsText.text = $"PIÈCES {coins} / {maxCoins}";
        if (scoreText != null) scoreText.text = $"{score:N0}";

        canvasGroup.alpha = 0f;
        transform.localScale = baseScale * 0.9f;
        StartCoroutine(FadeInRoutine());

        Time.timeScale = 0f; // Mettre en pause le monde
    }

    private IEnumerator FadeInRoutine()
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * 3f;
            float eased = 1f - Mathf.Pow(1f - Mathf.Clamp01(t), 3f);
            canvasGroup.alpha = eased;
            transform.localScale = Vector3.Lerp(baseScale * 0.9f, baseScale, eased);
            yield return null;
        }
        canvasGroup.alpha = 1f;
        transform.localScale = baseScale;
    }

    public void OnRetryClicked()
    {
        Time.timeScale = 1f;
        if (SoundManager.Instance != null) SoundManager.Instance.PlayButtonClick();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnMainMenuClicked()
    {
        Time.timeScale = 1f;
        if (SoundManager.Instance != null) SoundManager.Instance.PlayButtonClick();
        SceneManager.LoadScene("MainMenu");
    }
}
