using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Gère la popup de victoire "NIVEAU TERMINÉ !" avec affichage des étoiles,
/// du nombre de pièces récoltées, du score, du temps, et des boutons d'action.
/// </summary>
public class LevelCompleteUI : MonoBehaviour
{
    [Header("Titres et Textes")]
    public TextMeshProUGUI levelNameHeaderText;
    public TextMeshProUGUI resultTitleText;
    public TextMeshProUGUI resultSubtitleText;
    public TextMeshProUGUI coinsCountText;
    public TextMeshProUGUI scoreCountText;
    public TextMeshProUGUI timeCountText;
    public TextMeshProUGUI footerBadgeText;

    [Header("Étoiles (3 étoiles animées)")]
    public GameObject star1;
    public GameObject star2;
    public GameObject star3;
    public Sprite earnedStarSprite;
    public Sprite unearnedStarSprite;

    [Header("Boutons")]
    public Button btnNextLevel;
    public Button btnReplay;
    public Button btnMenu;

    [Header("Particules Confettis")]
    public ParticleSystem confettiParticles;

    private int currentLevel = 1;
    private CanvasGroup victoryCanvasGroup;
    private Vector3 victoryBaseScale;

    void Awake()
    {
        victoryCanvasGroup = GetComponent<CanvasGroup>();
        if (victoryCanvasGroup == null) victoryCanvasGroup = gameObject.AddComponent<CanvasGroup>();
        victoryBaseScale = transform.localScale;
        ConfigureStarIcon(star1);
        ConfigureStarIcon(star2);
        ConfigureStarIcon(star3);
        if (btnNextLevel != null) btnNextLevel.onClick.AddListener(OnNextLevelClicked);
        if (btnReplay != null) btnReplay.onClick.AddListener(OnReplayClicked);
        if (btnMenu != null) btnMenu.onClick.AddListener(OnMenuClicked);
    }

    public void ShowVictory(int levelIndex, string levelName, int coins, int maxCoins, int score, float timeInSeconds)
    {
        currentLevel = levelIndex;
        gameObject.SetActive(true);
        if (btnNextLevel != null) btnNextLevel.gameObject.SetActive(levelIndex < 3);
        Time.timeScale = 0f;
        StartCoroutine(AnimateVictoryEntrance());

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayVictorySound();
        }

        // Sauvegarder dans le système de progression
        GameManager.SaveLevelResult(levelIndex, coins, maxCoins, score, timeInSeconds);

        // Mettre à jour les textes
        if (levelNameHeaderText != null) levelNameHeaderText.text = levelName.ToUpper();
        if (coinsCountText != null) coinsCountText.text = $"{coins} / {maxCoins}";
        if (scoreCountText != null) scoreCountText.text = $"{score:N0}";

        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        if (timeCountText != null) timeCountText.text = $"{minutes:00}:{seconds:00}";

        int stars = GameManager.CalculateStars(score, maxCoins);

        if (footerBadgeText != null)
        {
            footerBadgeText.text = stars switch
            {
                3 => "COLLECTEUR D'ÉLITE — 3 ÉTOILES",
                2 => "AVENTURIER ARGENT — 2 ÉTOILES",
                _ => "EXPLORATEUR BRONZE — 1 ÉTOILE"
            };
        }

        // Animation des étoiles
        StartCoroutine(AnimateStarsRoutine(stars));

        if (confettiParticles != null)
        {
            ParticleSystem.MainModule particleSettings = confettiParticles.main;
            particleSettings.useUnscaledTime = true;
            confettiParticles.Play();
        }
    }

    private IEnumerator AnimateVictoryEntrance()
    {
        victoryCanvasGroup.alpha = 0f;
        transform.localScale = victoryBaseScale * 0.88f;
        float elapsed = 0f;
        const float duration = 0.32f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = 1f - Mathf.Pow(1f - t, 3f);
            victoryCanvasGroup.alpha = eased;
            transform.localScale = Vector3.Lerp(victoryBaseScale * 0.88f, victoryBaseScale, eased);
            yield return null;
        }
        victoryCanvasGroup.alpha = 1f;
        transform.localScale = victoryBaseScale;
    }

    private IEnumerator AnimateStarsRoutine(int starCount)
    {
        SetStarSprite(star1, unearnedStarSprite);
        SetStarSprite(star2, unearnedStarSprite);
        SetStarSprite(star3, unearnedStarSprite);

        yield return new WaitForSecondsRealtime(0.3f);
        if (starCount >= 1 && star1 != null) { SetStarSprite(star1, earnedStarSprite); BumpScale(star1); }
        yield return new WaitForSecondsRealtime(0.3f);
        if (starCount >= 2 && star2 != null) { SetStarSprite(star2, earnedStarSprite); BumpScale(star2); }
        yield return new WaitForSecondsRealtime(0.3f);
        if (starCount >= 3 && star3 != null) { SetStarSprite(star3, earnedStarSprite); BumpScale(star3); }
    }

    private void ConfigureStarIcon(GameObject star)
    {
        if (star == null) return;

        Image image = star.GetComponent<Image>();
        if (image == null) image = star.AddComponent<Image>();
        image.preserveAspect = true;
        image.raycastTarget = false;

        TextMeshProUGUI placeholder = star.GetComponent<TextMeshProUGUI>();
        if (placeholder != null) placeholder.enabled = false;

        if (earnedStarSprite == null || unearnedStarSprite == null)
        {
            Debug.LogError($"{nameof(LevelCompleteUI)} on {name} needs both earned and unearned star sprites assigned.", this);
            image.enabled = false;
            return;
        }

        image.sprite = unearnedStarSprite;
    }

    private void SetStarSprite(GameObject star, Sprite sprite)
    {
        if (star == null) return;
        Image image = star.GetComponent<Image>();
        if (image != null && sprite != null)
        {
            image.sprite = sprite;
            image.enabled = true;
        }
    }

    private void BumpScale(GameObject obj)
    {
        obj.transform.localScale = Vector3.one * 1.5f;
        StartCoroutine(ShrinkBack(obj.transform));
    }

    private IEnumerator ShrinkBack(Transform tr)
    {
        float t = 0;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * 4f;
            tr.localScale = Vector3.Lerp(Vector3.one * 1.5f, Vector3.one, t);
            yield return null;
        }
    }

    public void OnNextLevelClicked()
    {
        Time.timeScale = 1f;
        if (SoundManager.Instance != null) SoundManager.Instance.PlayButtonClick();
        int next = currentLevel + 1;
        if (next > 3) { OnMenuClicked(); return; }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadLevel(next);
        }
        else
        {
            string scene = next switch
            {
                1 => "Level_01_GreenValley",
                2 => "Level_02_CrystalCave",
                3 => "Level_03_SkyTemple",
                _ => "Level_01_GreenValley"
            };
            SceneManager.LoadScene(scene);
        }
    }

    public void OnReplayClicked()
    {
        Time.timeScale = 1f;
        if (SoundManager.Instance != null) SoundManager.Instance.PlayButtonClick();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnMenuClicked()
    {
        Time.timeScale = 1f;
        if (SoundManager.Instance != null) SoundManager.Instance.PlayButtonClick();
        SceneManager.LoadScene("MainMenu");
    }
}
