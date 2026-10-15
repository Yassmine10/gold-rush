using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Gestionnaire central de sauvegarde, progression et score pour GOLD RUSH.
/// Gère le déblocage des 3 niveaux, les étoiles obtenues et les meilleurs scores.
/// </summary>
public class GameManager : MonoBehaviour
{
    [Serializable]
    private class ProgressEntry { public string key; public int value; }
    [Serializable]
    private class ProgressFile { public List<ProgressEntry> entries = new List<ProgressEntry>(); }

    private static readonly Dictionary<string, int> fallbackProgress = new Dictionary<string, int>();
    private static bool fallbackLoaded;
    private static bool fallbackWarningLogged;
    private static string FallbackPath
    {
        get
        {
            string directory = Application.persistentDataPath;
            if (string.IsNullOrEmpty(directory)) directory = Path.GetDirectoryName(Application.dataPath);
            if (string.IsNullOrEmpty(directory)) directory = Application.dataPath;
            return Path.Combine(directory, "gold-rush-progress.json");
        }
    }

    public static GameManager Instance { get; private set; }

    [Header("Progression")]
    public int currentLevelIndex = 1; // 1 = Green Valley, 2 = Crystal Cave, 3 = Sky Temple
    public int totalCoinsCollectedAllGame = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialiser le niveau 1 comme débloqué par défaut
        if (!HasProgressKey("Level_1_Unlocked")) SetProgressInt("Level_1_Unlocked", 1);
    }

    public static bool IsLevelUnlocked(int levelIndex)
    {
        if (levelIndex < 1 || levelIndex > 3) return false;
        if (levelIndex == 1) return true;
        return GetProgressInt($"Level_{levelIndex}_Unlocked", 0) == 1;
    }

    public static void UnlockLevel(int levelIndex)
    {
        if (levelIndex < 1 || levelIndex > 3) return;
        SetProgressInt($"Level_{levelIndex}_Unlocked", 1);
    }

    public static int GetLevelStars(int levelIndex)
    {
        return GetProgressInt($"Level_{levelIndex}_Stars", 0);
    }

    public static int GetLevelHighScore(int levelIndex)
    {
        return GetProgressInt($"Level_{levelIndex}_HighScore", 0);
    }

    public static int GetLevelMaxCoinsCollected(int levelIndex)
    {
        return GetProgressInt($"Level_{levelIndex}_Coins", 0);
    }

    public static int CalculateStars(int score, int maxCoins)
    {
        float scoreRatio = (float)score / Mathf.Max(1, maxCoins * 100);
        if (scoreRatio >= 0.85f) return 3;
        if (scoreRatio >= 0.5f) return 2;
        return 1;
    }

    public static int GetSavedValue(string key, int defaultValue = 0) => GetProgressInt(key, defaultValue);

    public static void SaveValue(string key, int value) => SetProgressInt(key, value);

    public static float GetSavedSetting(string key, float defaultValue)
    {
        EnsureFallbackLoaded();
        try { if (PlayerPrefs.HasKey(key)) return PlayerPrefs.GetFloat(key, defaultValue); }
        catch (PlayerPrefsException) { }
        return fallbackProgress.TryGetValue(key, out int value) ? value / 1000f : defaultValue;
    }

    public static void SaveSetting(string key, float value)
    {
        EnsureFallbackLoaded();
        fallbackProgress[key] = Mathf.RoundToInt(value * 1000f);
        try { PlayerPrefs.SetFloat(key, value); PlayerPrefs.Save(); }
        catch (PlayerPrefsException) { }
        SaveFallbackProgress();
    }

    public static void SaveLevelResult(int levelIndex, int coins, int maxCoins, int score, float timeInSeconds)
    {
        int stars = CalculateStars(score, maxCoins);

        int prevStars = GetLevelStars(levelIndex);
        if (stars > prevStars)
        {
            SetProgressInt($"Level_{levelIndex}_Stars", stars);
        }

        int prevScore = GetLevelHighScore(levelIndex);
        if (score > prevScore)
        {
            SetProgressInt($"Level_{levelIndex}_HighScore", score);
        }

        int prevCoins = GetLevelMaxCoinsCollected(levelIndex);
        if (coins > prevCoins)
        {
            SetProgressInt($"Level_{levelIndex}_Coins", coins);
        }

        // Débloquer le niveau suivant
        if (levelIndex < 3)
        {
            UnlockLevel(levelIndex + 1);
        }

    }

    public static void ResetAllProgress()
    {
        EnsureFallbackLoaded();
        fallbackProgress.Clear();
        try { PlayerPrefs.DeleteAll(); PlayerPrefs.Save(); }
        catch (PlayerPrefsException) { }
        SaveFallbackProgress();
        SetProgressInt("Level_1_Unlocked", 1);
    }

    private static void EnsureFallbackLoaded()
    {
        if (fallbackLoaded) return;
        fallbackLoaded = true;
        try
        {
            if (!File.Exists(FallbackPath)) return;
            ProgressFile data = JsonUtility.FromJson<ProgressFile>(File.ReadAllText(FallbackPath));
            if (data?.entries == null) return;
            foreach (ProgressEntry entry in data.entries)
                if (entry != null && !string.IsNullOrEmpty(entry.key)) fallbackProgress[entry.key] = entry.value;
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"Impossible de lire la sauvegarde de progression locale : {exception.Message}");
        }
    }

    private static bool HasProgressKey(string key)
    {
        EnsureFallbackLoaded();
        try { if (PlayerPrefs.HasKey(key)) return true; }
        catch (PlayerPrefsException) { }
        return fallbackProgress.ContainsKey(key);
    }

    private static int GetProgressInt(string key, int defaultValue)
    {
        EnsureFallbackLoaded();
        try { if (PlayerPrefs.HasKey(key)) return PlayerPrefs.GetInt(key, defaultValue); }
        catch (PlayerPrefsException) { }
        return fallbackProgress.TryGetValue(key, out int value) ? value : defaultValue;
    }

    private static void SetProgressInt(string key, int value)
    {
        EnsureFallbackLoaded();
        fallbackProgress[key] = value;
        try
        {
            PlayerPrefs.SetInt(key, value);
            PlayerPrefs.Save();
        }
        catch (PlayerPrefsException) { }
        SaveFallbackProgress();
    }

    private static void SaveFallbackProgress()
    {
        try
        {
            ProgressFile data = new ProgressFile();
            foreach (KeyValuePair<string, int> entry in fallbackProgress)
                data.entries.Add(new ProgressEntry { key = entry.Key, value = entry.Value });
            string path = FallbackPath;
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, JsonUtility.ToJson(data, true));
        }
        catch (Exception exception)
        {
            if (!fallbackWarningLogged)
            {
                fallbackWarningLogged = true;
                Debug.LogWarning($"La progression ne peut pas être sauvegardée sur cet appareil : {exception.Message}");
            }
        }
    }

    public void LoadLevel(int levelNumber)
    {
        currentLevelIndex = levelNumber;
        Time.timeScale = 1f;

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
            Debug.Log($"Chargement de la scène: {sceneName}");
            SceneManager.LoadScene(sceneName);
        }
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadLevelSelect()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("LevelSelect");
    }
}
