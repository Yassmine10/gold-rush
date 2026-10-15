using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Contrôleur complet du menu principal pour "GOLD RUSH".
/// Gère les boutons Jouer, Niveaux, Personnage, Options et les effets sonores/visuels.
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    [Header("Boutons Principaux")]
    public Button btnPlay;
    public Button btnLevels;
    public Button btnCharacter;
    public Button btnOptions;
    public Button btnInstructions;

    [Header("Popups / Panneaux Optionnels")]
    public GameObject levelsPanel;
    public GameObject characterPanel;
    public GameObject optionsPanel;
    public GameObject instructionsPanel;

    [Header("Paramètres de Scène")]
    [Tooltip("Nom de la première scène de jeu")]
    public string firstLevelSceneName = "Level_01_GreenValley";

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip clickSound;

    void Awake()
    {
        GoldRushVisualTheme.ApplyToActiveScene();
        if (btnPlay != null) btnPlay.onClick.AddListener(OnPlayClicked);
        if (btnLevels != null) btnLevels.onClick.AddListener(OnLevelsClicked);
        if (btnCharacter != null) btnCharacter.onClick.AddListener(OnCharacterClicked);
        if (btnOptions != null) btnOptions.onClick.AddListener(OnOptionsClicked);
        if (btnInstructions != null) btnInstructions.onClick.AddListener(OnInstructionsClicked);

        // Fermer tous les panels au démarrage
        CloseAllPanels();
    }

    public void OnPlayClicked()
    {
        PlayClickSound();
        Debug.Log("Lancement du niveau : " + firstLevelSceneName);
        
        if (Application.CanStreamedLevelBeLoaded(firstLevelSceneName))
        {
            SceneManager.LoadScene(firstLevelSceneName);
        }
        else if (Application.CanStreamedLevelBeLoaded("Level_01"))
        {
            SceneManager.LoadScene("Level_01");
        }
        else
        {
            Debug.LogWarning($"Scène '{firstLevelSceneName}' non trouvée dans Build Settings. Créez la scène de gameplay !");
        }
    }

    public void OnLevelsClicked()
    {
        PlayClickSound();
        if (levelsPanel != null)
        {
            levelsPanel.SetActive(!levelsPanel.activeSelf);
        }
        else if (Application.CanStreamedLevelBeLoaded("LevelSelect"))
        {
            SceneManager.LoadScene("LevelSelect");
        }
    }

    public void OnCharacterClicked()
    {
        PlayClickSound();
        if (characterPanel != null)
        {
            characterPanel.SetActive(!characterPanel.activeSelf);
        }
    }

    public void OnOptionsClicked()
    {
        PlayClickSound();
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(!optionsPanel.activeSelf);
        }
    }

    public void OnInstructionsClicked()
    {
        PlayClickSound();
        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(!instructionsPanel.activeSelf);
        }
    }

    public void CloseAllPanels()
    {
        if (levelsPanel != null) levelsPanel.SetActive(false);
        if (characterPanel != null) characterPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (instructionsPanel != null) instructionsPanel.SetActive(false);
    }

    public void LoadSpecificLevel(int levelNumber)
    {
        PlayClickSound();
        string sceneName = levelNumber switch
        {
            1 => "Level_01_GreenValley",
            2 => "Level_02_CrystalCave",
            3 => "Level_03_SkyTemple",
            _ => "Level_01_GreenValley"
        };
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.Log($"Chargement du niveau {levelNumber} ({sceneName})");
        }
    }

    private void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}
