using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Gère l'écran "CHOISIR UN NIVEAU" avec les 3 cartes de niveaux :
/// - Niveau 1 : GREEN VALLEY (Facile)
/// - Niveau 2 : CRYSTAL CAVE (Moyen)
/// - Niveau 3 : SKY TEMPLE (Difficile)
/// Gère les verrous, les étoiles et les scores sauvegardés.
/// </summary>
public class LevelSelectMenu : MonoBehaviour
{
    [Header("Bouton Retour")]
    public Button btnBack;

    [Header("Carte Niveau 1 - Green Valley")]
    public Button btnPlayLevel1;
    public TextMeshProUGUI level1CoinsText;
    public TextMeshProUGUI level1ScoreText;
    public GameObject[] level1Stars; // 3 étoiles

    [Header("Carte Niveau 2 - Crystal Cave")]
    public Button btnPlayLevel2;
    public GameObject level2LockedOverlay;
    public TextMeshProUGUI level2CoinsText;
    public TextMeshProUGUI level2ScoreText;
    public GameObject[] level2Stars; // 3 étoiles

    [Header("Carte Niveau 3 - Sky Temple")]
    public Button btnPlayLevel3;
    public GameObject level3LockedOverlay;
    public TextMeshProUGUI level3CoinsText;
    public TextMeshProUGUI level3ScoreText;
    public GameObject[] level3Stars; // 3 étoiles

    void Start()
    {
        GoldRushVisualTheme.ApplyToActiveScene();
        if (btnBack != null) btnBack.onClick.AddListener(OnBackClicked);

        if (btnPlayLevel1 != null) btnPlayLevel1.onClick.AddListener(() => LaunchLevel(1));
        if (btnPlayLevel2 != null) btnPlayLevel2.onClick.AddListener(() => LaunchLevel(2));
        if (btnPlayLevel3 != null) btnPlayLevel3.onClick.AddListener(() => LaunchLevel(3));

        RefreshLevelCards();
    }

    public void RefreshLevelCards()
    {
        ApplyObjectiveToCard(btnPlayLevel1, 1);
        ApplyObjectiveToCard(btnPlayLevel2, 2);
        ApplyObjectiveToCard(btnPlayLevel3, 3);

        // Niveau 1 (Toujours débloqué)
        int l1Stars = GameManager.GetLevelStars(1);
        int l1Score = GameManager.GetLevelHighScore(1);
        int l1Coins = GameManager.GetLevelMaxCoinsCollected(1);
        if (level1CoinsText != null) level1CoinsText.text = $"PIÈCES {l1Coins}/{LevelBalance.GetMaximumCoins(1)}";
        if (level1ScoreText != null) level1ScoreText.text = l1Score > 0 ? $"{l1Score:N0}" : "---";
        UpdateStarsUI(level1Stars, l1Stars);

        // Niveau 2
        bool l2Unlocked = GameManager.IsLevelUnlocked(2);
        if (level2LockedOverlay != null) level2LockedOverlay.SetActive(!l2Unlocked);
        UpdateLockedObjective(level2LockedOverlay, 2);
        if (btnPlayLevel2 != null) btnPlayLevel2.interactable = l2Unlocked;
        if (l2Unlocked)
        {
            int l2Stars = GameManager.GetLevelStars(2);
            int l2Score = GameManager.GetLevelHighScore(2);
            int l2Coins = GameManager.GetLevelMaxCoinsCollected(2);
            if (level2CoinsText != null) level2CoinsText.text = $"PIÈCES {l2Coins}/{LevelBalance.GetMaximumCoins(2)}";
            if (level2ScoreText != null) level2ScoreText.text = l2Score > 0 ? $"{l2Score:N0}" : "---";
            UpdateStarsUI(level2Stars, l2Stars);
        }

        // Niveau 3
        bool l3Unlocked = GameManager.IsLevelUnlocked(3);
        if (level3LockedOverlay != null) level3LockedOverlay.SetActive(!l3Unlocked);
        UpdateLockedObjective(level3LockedOverlay, 3);
        if (btnPlayLevel3 != null) btnPlayLevel3.interactable = l3Unlocked;
        if (l3Unlocked)
        {
            int l3Stars = GameManager.GetLevelStars(3);
            int l3Score = GameManager.GetLevelHighScore(3);
            int l3Coins = GameManager.GetLevelMaxCoinsCollected(3);
            if (level3CoinsText != null) level3CoinsText.text = $"PIÈCES {l3Coins}/{LevelBalance.GetMaximumCoins(3)}";
            if (level3ScoreText != null) level3ScoreText.text = l3Score > 0 ? $"{l3Score:N0}" : "---";
            UpdateStarsUI(level3Stars, l3Stars);
        }
    }

    private static void ApplyObjectiveToCard(Button playButton, int level)
    {
        if (playButton == null || playButton.transform.parent == null) return;
        Transform card = playButton.transform.parent;
        Transform difficultyTransform = card.Find("Tag_Diff");
        TextMeshProUGUI difficulty = difficultyTransform != null ? difficultyTransform.GetComponent<TextMeshProUGUI>() : null;
        if (difficulty != null) difficulty.text = LevelBalance.GetDifficulty(level);

        Transform descriptionTransform = card.Find("Desc");
        if (descriptionTransform == null) return;

        TextMeshProUGUI description = descriptionTransform.GetComponent<TextMeshProUGUI>();
        if (description == null) return;

        string existingDescription = description.text;
        int objectiveStart = existingDescription.IndexOf("\nObjectif :");
        if (objectiveStart >= 0) existingDescription = existingDescription.Substring(0, objectiveStart);
        description.text = $"{existingDescription}\nObjectif : {LevelBalance.GetMinimumCoins(level)} pièces sur {LevelBalance.GetMaximumCoins(level)}.";
        description.enableAutoSizing = true;
        description.fontSizeMax = 16;
        description.fontSizeMin = 12;
    }

    private static void UpdateLockedObjective(GameObject overlay, int level)
    {
        if (overlay == null) return;
        Transform lockTextTransform = overlay.transform.Find("LockText");
        TextMeshProUGUI lockText = lockTextTransform != null ? lockTextTransform.GetComponent<TextMeshProUGUI>() : null;
        if (lockText != null)
            lockText.text = $"Terminez le niveau précédent.\nObjectif : {LevelBalance.GetMinimumCoins(level)} pièces.";
    }

    private void UpdateStarsUI(GameObject[] starObjs, int starCount)
    {
        if (starObjs == null) return;
        for (int i = 0; i < starObjs.Length; i++)
        {
            if (starObjs[i] != null)
            {
                starObjs[i].SetActive(i < starCount);
            }
        }
    }

    public void LaunchLevel(int levelNum)
    {
        if (!GameManager.IsLevelUnlocked(levelNum)) return;
        if (SoundManager.Instance != null) SoundManager.Instance.PlayButtonClick();
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadLevel(levelNum);
        }
        else
        {
            string scene = levelNum switch
            {
                1 => "Level_01_GreenValley",
                2 => "Level_02_CrystalCave",
                3 => "Level_03_SkyTemple",
                _ => "Level_01_GreenValley"
            };
            SceneManager.LoadScene(scene);
        }
    }

    public void OnBackClicked()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlayButtonClick();
        SceneManager.LoadScene("MainMenu");
    }
}
