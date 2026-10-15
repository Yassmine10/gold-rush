using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Gestionnaire complet du HUD en jeu pour "Gold Rush" :
/// - Compteur de pièces (ex: 🪙 12 / 30)
/// - Score (ex: 1 250)
/// - Titre du niveau (ex: 🌿 NIVEAU 1 / 💎 NIVEAU 2 / ⛩️ NIVEAU 3)
/// - Barre de progression du niveau (START -> SORTIE)
/// - Boutons Pause, Paramètres et Bouton de Fin de test (WIN FIN)
/// - Intégration de la fenêtre de Victoire (LevelCompleteUI)
/// </summary>
public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    [Header("Paramètres du Niveau Actuel")]
    public int currentLevelNumber = 1;
    public string currentLevelName = "Green Valley";
    public int maxCoinsInLevel = 30;
    [Min(0)] public int minimumCoinsToWin = 8;

    [Header("En haut à gauche : Pièces, Score & Santé")]
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI heartsText;

    [Header("En haut au centre : Progression")]
    public TextMeshProUGUI levelTitleText;
    public Slider progressBar;
    public Transform playerTransform;
    public Transform levelStartTransform;
    public Transform levelEndTransform;

    [Header("En haut à droite : Boutons & Actions")]
    public Button btnFinishTest;
    public Button btnPause;
    public Button btnSettings;
    public Button btnInstructions;

    [Header("Panneaux & Modals")]
    public GameObject pauseMenuPanel;
    public GameObject settingsPanel;
    public LevelCompleteUI levelCompleteModal;
    public GameOverUI gameOverModal;
    public InstructionsMenu instructionsModal;

    [Header("Boutons de Pause")]
    public Button btnResume;
    public Button btnRestart;
    public Button btnQuitToMenu;

    private int currentCoins = 0;
    private int currentScore = 0;
    private float levelTimer = 0f;
    private bool isPaused = false;
    private bool isFinished = false;
    private CanvasGroup pauseCanvasGroup;
    private Coroutine pauseTransition;
    private float settingsReturnTimeScale = 1f;
    private bool settingsReturnToPause;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        minimumCoinsToWin = LevelBalance.GetMinimumCoins(currentLevelNumber);
        maxCoinsInLevel = LevelBalance.GetMaximumCoins(currentLevelNumber);
        EnsurePauseMenu();
        EnsureSettingsMenu();
        GoldRushVisualTheme.ApplyToActiveScene();
        UpdateUI();
        UpdateHeartsUI(3);

        if (btnPause != null) btnPause.onClick.AddListener(TogglePause);
        if (btnSettings != null) btnSettings.onClick.AddListener(OpenSettings);
        if (btnFinishTest != null) btnFinishTest.onClick.AddListener(TriggerFinishLevel);
        if (btnInstructions != null) btnInstructions.onClick.AddListener(OpenInstructions);

        if (btnResume != null) btnResume.onClick.AddListener(TogglePause);
        if (btnRestart != null) btnRestart.onClick.AddListener(RestartCurrentLevel);
        if (btnQuitToMenu != null) btnQuitToMenu.onClick.AddListener(QuitToMenu);

        if (levelTitleText != null)
        {
            string levelName = currentLevelName;
            levelTitleText.text = $"NIVEAU {currentLevelNumber} · {levelName}";
        }
    }

    void Update()
    {
        if (!isPaused && !isFinished)
        {
            levelTimer += Time.deltaTime;
        }

        UpdateProgressBar();
    }

    public void AddCoin(int amount = 1, int scoreGain = 100)
    {
        currentCoins += amount;
        currentScore += scoreGain;
        UpdateUI();

        if (SoundManager.Instance != null && amount > 0)
        {
            SoundManager.Instance.PlayCoinSound();
        }
    }

    public bool HasMinimumCoins(int minimum) => currentCoins >= minimum;

    public void ShowCoinsRequired(int minimum)
    {
        if (coinsText == null) return;
        coinsText.text = $"PIÈCES {currentCoins}/{minimum} REQUISES";
        CancelInvoke(nameof(UpdateUI));
        Invoke(nameof(UpdateUI), 2f);
    }

    public void UpdateHeartsUI(int currentHearts)
    {
        if (heartsText != null)
        {
            heartsText.text = $"VIES {Mathf.Clamp(currentHearts, 0, 3)} / 3";
        }
    }

    private void UpdateUI()
    {
        if (coinsText != null)
        {
            coinsText.text = $"PIÈCES {currentCoins} / {maxCoinsInLevel}";
        }

        if (scoreText != null)
        {
            scoreText.text = $"{currentScore:N0}";
        }
    }

    private void UpdateProgressBar()
    {
        if (progressBar != null && playerTransform != null && levelStartTransform != null && levelEndTransform != null)
        {
            float totalDist = levelEndTransform.position.x - levelStartTransform.position.x;
            if (totalDist > 0.1f)
            {
                float currentDist = playerTransform.position.x - levelStartTransform.position.x;
                progressBar.value = Mathf.Clamp01(currentDist / totalDist);
            }
        }
    }

    public void TriggerFinishLevel()
    {
        if (isFinished) return;
        if (currentCoins < minimumCoinsToWin)
        {
            ShowCoinsRequired(minimumCoinsToWin);
            return;
        }
        isFinished = true;

        if (levelCompleteModal != null)
        {
            levelCompleteModal.ShowVictory(currentLevelNumber, currentLevelName, currentCoins, maxCoinsInLevel, currentScore, levelTimer);
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;

        if (pauseMenuPanel != null)
        {
            if (pauseTransition != null) StopCoroutine(pauseTransition);
            if (isPaused)
            {
                pauseMenuPanel.SetActive(true);
                if (pauseCanvasGroup != null)
                {
                    pauseCanvasGroup.interactable = true;
                    pauseCanvasGroup.blocksRaycasts = true;
                }
                pauseTransition = StartCoroutine(AnimatePausePanel(true));
            }
            else
            {
                pauseTransition = StartCoroutine(AnimatePausePanel(false));
            }
        }
    }

    private void EnsurePauseMenu()
    {
        if (pauseMenuPanel != null) return;

        Transform canvas = btnPause != null ? btnPause.transform.parent : null;
        if (canvas == null)
        {
            Canvas foundCanvas = Object.FindFirstObjectByType<Canvas>();
            if (foundCanvas != null) canvas = foundCanvas.transform;
        }
        if (canvas == null) return;

        GameObject overlay = new GameObject("PauseOverlay_Backdrop", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(CanvasGroup));
        overlay.transform.SetParent(canvas, false);
        RectTransform overlayRect = overlay.GetComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.offsetMin = Vector2.zero;
        overlayRect.offsetMax = Vector2.zero;
        Image backdrop = overlay.GetComponent<Image>();
        backdrop.color = new Color(0.035f, 0.055f, 0.09f, 0.82f);
        pauseCanvasGroup = overlay.GetComponent<CanvasGroup>();
        pauseCanvasGroup.alpha = 0f;
        pauseCanvasGroup.interactable = true;
        pauseCanvasGroup.blocksRaycasts = true;
        pauseMenuPanel = overlay;

        GameObject panel = new GameObject("Pause_Panel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panel.transform.SetParent(overlay.transform, false);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(560f, 460f);
        panel.GetComponent<Image>().color = new Color32(27, 40, 62, 255);

        CreatePauseLabel(panel.transform, "Pause_Title", "JEU EN PAUSE", new Vector2(0, 145), new Vector2(500, 70), 42, new Color32(255, 221, 145, 255));
        CreatePauseLabel(panel.transform, "Pause_Subtitle", "Reprends quand tu es prêt", new Vector2(0, 92), new Vector2(480, 36), 20, new Color32(172, 187, 207, 255));
        btnResume = CreatePauseButton(panel.transform, "Btn_Resume", "REPRENDRE", new Vector2(0, 15), new Color32(65, 190, 166, 255));
        btnRestart = CreatePauseButton(panel.transform, "Btn_Restart", "RECOMMENCER", new Vector2(0, -75), new Color32(39, 55, 79, 255));
        btnQuitToMenu = CreatePauseButton(panel.transform, "Btn_Menu", "MENU PRINCIPAL", new Vector2(0, -165), new Color32(39, 55, 79, 255));
        overlay.SetActive(false);
    }

    private void EnsureSettingsMenu()
    {
        if (settingsPanel != null)
        {
            OptionsSettingsMenu existingOptions = settingsPanel.GetComponent<OptionsSettingsMenu>();
            if (existingOptions == null) existingOptions = settingsPanel.AddComponent<OptionsSettingsMenu>();
            existingOptions.closedCallback = RestoreAfterSettings;
            return;
        }
        Transform canvas = btnSettings != null ? btnSettings.transform.parent : null;
        if (canvas == null)
        {
            Canvas foundCanvas = Object.FindFirstObjectByType<Canvas>();
            if (foundCanvas != null) canvas = foundCanvas.transform;
        }
        if (canvas == null) return;

        GameObject modal = new GameObject("Modal_OptionsRuntime", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        modal.transform.SetParent(canvas, false);
        RectTransform rect = modal.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(760f, 900f);
        modal.GetComponent<Image>().color = new Color32(27, 40, 62, 250);
        OptionsSettingsMenu options = modal.AddComponent<OptionsSettingsMenu>();
        options.closedCallback = RestoreAfterSettings;
        settingsPanel = modal;
        modal.SetActive(false);
    }

    private void RestoreAfterSettings()
    {
        Time.timeScale = settingsReturnTimeScale;
        isPaused = settingsReturnTimeScale == 0f;
        if (settingsReturnToPause && pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        settingsReturnToPause = false;
    }

    private void CreatePauseLabel(Transform parent, string objectName, string content, Vector2 position, Vector2 size, float fontSize, Color color)
    {
        GameObject labelObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        labelObject.transform.SetParent(parent, false);
        RectTransform rect = labelObject.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        TextMeshProUGUI label = labelObject.GetComponent<TextMeshProUGUI>();
        label.text = content;
        label.font = levelTitleText != null ? levelTitleText.font : null;
        label.fontSize = fontSize;
        label.fontStyle = FontStyles.Bold;
        label.alignment = TextAlignmentOptions.Center;
        label.color = color;
        label.raycastTarget = false;
    }

    private Button CreatePauseButton(Transform parent, string objectName, string content, Vector2 position, Color color)
    {
        GameObject buttonObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);
        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(420f, 68f);
        Image image = buttonObject.GetComponent<Image>();
        image.color = color;
        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;

        ColorBlock colors = button.colors;
        colors.normalColor = color;
        colors.highlightedColor = Color.Lerp(color, Color.white, 0.16f);
        colors.pressedColor = Color.Lerp(color, Color.black, 0.18f);
        colors.disabledColor = new Color(0.4f, 0.45f, 0.52f, 0.75f);
        button.colors = colors;

        CreatePauseLabel(buttonObject.transform, "Label", content, Vector2.zero, rect.sizeDelta, 22, Color.white);
        return button;
    }

    private IEnumerator AnimatePausePanel(bool opening)
    {
        if (pauseCanvasGroup == null) pauseCanvasGroup = pauseMenuPanel.GetComponent<CanvasGroup>();
        float start = pauseCanvasGroup != null ? pauseCanvasGroup.alpha : 1f;
        float end = opening ? 1f : 0f;
        float elapsed = 0f;
        const float duration = 0.18f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            if (pauseCanvasGroup != null) pauseCanvasGroup.alpha = Mathf.Lerp(start, end, Mathf.Clamp01(elapsed / duration));
            yield return null;
        }
        if (pauseCanvasGroup != null) pauseCanvasGroup.alpha = end;
        if (!opening && pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
            if (pauseCanvasGroup != null)
            {
                pauseCanvasGroup.interactable = false;
                pauseCanvasGroup.blocksRaycasts = false;
            }
        }
        pauseTransition = null;
    }

    public void TriggerGameOver()
    {
        if (isFinished) return;
        isFinished = true;

        if (gameOverModal != null)
        {
            gameOverModal.ShowGameOver(currentCoins, maxCoinsInLevel, currentScore);
        }
    }

    public void OpenInstructions()
    {
        if (instructionsModal != null)
        {
            instructionsModal.Open();
        }
    }

    public void OpenSettings()
    {
        if (settingsPanel == null) EnsureSettingsMenu();
        if (settingsPanel == null) return;
        settingsReturnTimeScale = Time.timeScale;
        settingsReturnToPause = isPaused;
        Time.timeScale = 0f;
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
        OptionsSettingsMenu options = settingsPanel.GetComponent<OptionsSettingsMenu>();
        if (options != null) options.closedCallback = RestoreAfterSettings;
    }

    public void RestartCurrentLevel()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
