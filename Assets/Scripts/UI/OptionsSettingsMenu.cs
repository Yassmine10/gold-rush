using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Accessible runtime options shared by the main menu and in-game pause menu.</summary>
public class OptionsSettingsMenu : MonoBehaviour
{
    public Slider sfxVolumeSlider;
    public Slider musicVolumeSlider;
    public Button btnClose;
    public Button btnResetProgress;
    public Action closedCallback;

    private Button bindLeftButton;
    private Button bindRightButton;
    private Button bindJumpButton;
    private Button contrastButton;
    private Button largeTextButton;
    private TextMeshProUGUI contrastLabel;
    private TextMeshProUGUI largeTextLabel;
    private bool built;
    private bool listeningForKey;

    private void Start()
    {
        BuildIfNeeded();
        InitializeSliders();
        RefreshControlLabels();
        RefreshAccessibilityLabels();
        GoldRushVisualTheme.ApplyToActiveScene();
    }

    private void BuildIfNeeded()
    {
        if (built) return;
        built = true;
        RectTransform root = transform as RectTransform;
        if (root != null) root.sizeDelta = new Vector2(760f, 900f);

        TMP_FontAsset font = GetComponentInChildren<TextMeshProUGUI>(true)?.font;
        if (font == null)
        {
            TextMeshProUGUI[] all = FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            if (all.Length > 0) font = all[0].font;
        }

        TextMeshProUGUI title = FindText("Txt_Title");
        if (title == null) title = CreateLabel("Txt_Title", "OPTIONS", new Vector2(0, 325), new Vector2(620, 64), 34, font);
        else Place(title.rectTransform, new Vector2(0, 325), new Vector2(620, 64));

        TextMeshProUGUI sfxLabel = FindText("Txt_SFX");
        if (sfxLabel == null) sfxLabel = CreateLabel("Txt_SFX", "VOLUME DES EFFETS", new Vector2(0, 245), new Vector2(580, 34), 19, font);
        else { sfxLabel.text = "VOLUME DES EFFETS"; Place(sfxLabel.rectTransform, new Vector2(0, 245), new Vector2(580, 34)); }
        TextMeshProUGUI musicLabel = FindText("Txt_Mus");
        if (musicLabel == null) musicLabel = CreateLabel("Txt_Mus", "VOLUME DE LA MUSIQUE", new Vector2(0, 145), new Vector2(580, 34), 19, font);
        else { musicLabel.text = "VOLUME DE LA MUSIQUE"; Place(musicLabel.rectTransform, new Vector2(0, 145), new Vector2(580, 34)); }

        if (sfxVolumeSlider == null) sfxVolumeSlider = FindSlider("Slider_SFX");
        if (musicVolumeSlider == null) musicVolumeSlider = FindSlider("Slider_Mus");
        ConfigureSlider(sfxVolumeSlider, new Vector2(0, 200));
        ConfigureSlider(musicVolumeSlider, new Vector2(0, 100));

        bindLeftButton = EnsureButton("Btn_BindLeft", "GAUCHE  ·  Q", new Vector2(0, 15), font);
        bindRightButton = EnsureButton("Btn_BindRight", "DROITE  ·  D", new Vector2(0, -45), font);
        bindJumpButton = EnsureButton("Btn_BindJump", "SAUT  ·  ESPACE", new Vector2(0, -105), font);
        contrastButton = EnsureButton("Btn_HighContrast", "CONTRASTE ÉLEVÉ", new Vector2(0, -175), font);
        largeTextButton = EnsureButton("Btn_LargeText", "TEXTE AGRANDI", new Vector2(0, -235), font);
        contrastLabel = contrastButton.GetComponentInChildren<TextMeshProUGUI>(true);
        largeTextLabel = largeTextButton.GetComponentInChildren<TextMeshProUGUI>(true);
        bindLeftButton.onClick.RemoveAllListeners(); bindLeftButton.onClick.AddListener(() => StartBinding("left"));
        bindRightButton.onClick.RemoveAllListeners(); bindRightButton.onClick.AddListener(() => StartBinding("right"));
        bindJumpButton.onClick.RemoveAllListeners(); bindJumpButton.onClick.AddListener(() => StartBinding("jump"));
        contrastButton.onClick.RemoveAllListeners(); contrastButton.onClick.AddListener(ToggleHighContrast);
        largeTextButton.onClick.RemoveAllListeners(); largeTextButton.onClick.AddListener(ToggleLargeText);

        btnResetProgress = FindButton("Btn_Reset");
        if (btnResetProgress != null)
        {
            Place(btnResetProgress.GetComponent<RectTransform>(), new Vector2(0, -305), new Vector2(540, 54));
            btnResetProgress.onClick.RemoveAllListeners();
            btnResetProgress.onClick.AddListener(ResetProgress);
        }
        btnClose = FindButton("Btn_CloseOpt");
        if (btnClose == null) btnClose = EnsureButton("Btn_CloseOpt", "FERMER", new Vector2(0, -370), font);
        else Place(btnClose.GetComponent<RectTransform>(), new Vector2(0, -370), new Vector2(540, 58));
        btnClose.onClick.RemoveAllListeners(); btnClose.onClick.AddListener(CloseMenu);
    }

    private void InitializeSliders()
    {
        float sfx = GameManager.GetSavedSetting("SFXVolume", 1f);
        float music = GameManager.GetSavedSetting("MusicVolume", 0.55f);
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.minValue = 0; sfxVolumeSlider.maxValue = 1;
            sfxVolumeSlider.SetValueWithoutNotify(sfx);
            sfxVolumeSlider.onValueChanged.RemoveListener(OnSFXChanged);
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXChanged);
        }
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.minValue = 0; musicVolumeSlider.maxValue = 1;
            musicVolumeSlider.SetValueWithoutNotify(music);
            musicVolumeSlider.onValueChanged.RemoveListener(OnMusicChanged);
            musicVolumeSlider.onValueChanged.AddListener(OnMusicChanged);
        }
        OnSFXChanged(sfx); OnMusicChanged(music);
    }

    private void ConfigureSlider(Slider slider, Vector2 position)
    {
        if (slider == null) return;
        RectTransform rect = slider.GetComponent<RectTransform>();
        Place(rect, position, new Vector2(540, 30));
        slider.minValue = 0; slider.maxValue = 1; slider.wholeNumbers = false;
        Image track = slider.GetComponent<Image>();
        if (track == null) track = slider.gameObject.AddComponent<Image>();
        track.color = new Color32(15, 23, 42, 255);
        slider.targetGraphic = track;
        if (slider.fillRect != null)
        {
            slider.fillRect.anchorMin = new Vector2(0, 0);
            slider.fillRect.anchorMax = new Vector2(0, 1);
            slider.fillRect.pivot = new Vector2(0, 0.5f);
            slider.fillRect.offsetMin = new Vector2(4, 4);
            slider.fillRect.offsetMax = new Vector2(-4, -4);
            Image fill = slider.fillRect.GetComponent<Image>();
            if (fill != null) { fill.color = new Color32(246, 190, 72, 255); fill.raycastTarget = false; }
        }
        if (slider.handleRect == null)
        {
            GameObject handle = new GameObject("Handle", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            handle.transform.SetParent(slider.transform, false);
            slider.handleRect = handle.GetComponent<RectTransform>();
            slider.handleRect.sizeDelta = new Vector2(22, 22);
        }
        Image handleImage = slider.handleRect.GetComponent<Image>();
        if (handleImage != null) { handleImage.color = new Color32(255, 221, 145, 255); handleImage.raycastTarget = true; }
    }

    private Button EnsureButton(string name, string label, Vector2 position, TMP_FontAsset font)
    {
        Button button = FindButton(name);
        if (button == null)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            go.transform.SetParent(transform, false);
            button = go.GetComponent<Button>(); button.targetGraphic = go.GetComponent<Image>();
            go.GetComponent<Image>().color = new Color32(39, 55, 79, 255);
            TextMeshProUGUI text = CreateLabel("Label", label, Vector2.zero, new Vector2(520, 56), 17, font, go.transform);
            text.color = Color.white;
        }
        TextMeshProUGUI child = button.GetComponentInChildren<TextMeshProUGUI>(true);
        if (child == null) child = CreateLabel("Label", label, Vector2.zero, new Vector2(520, 56), 17, font, button.transform);
        child.text = label;
        Place(button.GetComponent<RectTransform>(), position, new Vector2(540, 56));
        return button;
    }

    private TextMeshProUGUI CreateLabel(string name, string value, Vector2 position, Vector2 size, float fontSize, TMP_FontAsset font, Transform parent = null)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent != null ? parent : transform, false);
        TextMeshProUGUI text = go.GetComponent<TextMeshProUGUI>();
        text.text = value; text.font = font; text.fontSize = fontSize; text.fontStyle = FontStyles.Bold;
        text.alignment = TextAlignmentOptions.Center; text.color = new Color32(246, 244, 237, 255);
        text.enableAutoSizing = true; text.fontSizeMin = 12; text.fontSizeMax = fontSize;
        text.raycastTarget = false;
        Place(text.rectTransform, position, size);
        return text;
    }

    private void Place(RectTransform rect, Vector2 position, Vector2 size)
    {
        if (rect == null) return;
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position; rect.sizeDelta = size;
    }

    private TextMeshProUGUI FindText(string name) => FindNamed<TextMeshProUGUI>(name);
    private Button FindButton(string name) => FindNamed<Button>(name);
    private Slider FindSlider(string name) => FindNamed<Slider>(name);
    private T FindNamed<T>(string objectName) where T : Component
    {
        T[] items = GetComponentsInChildren<T>(true);
        foreach (T item in items) if (item.gameObject.name == objectName) return item;
        return null;
    }

    private void OnSFXChanged(float value)
    {
        GameManager.SaveSetting("SFXVolume", value);
        if (SoundManager.Instance != null) SoundManager.Instance.SetSFXVolume(value);
    }
    private void OnMusicChanged(float value)
    {
        GameManager.SaveSetting("MusicVolume", value);
        if (SoundManager.Instance != null) SoundManager.Instance.SetMusicVolume(value);
    }

    private void StartBinding(string action)
    {
        if (listeningForKey) return;
        StartCoroutine(WaitForBinding(action));
    }

    private IEnumerator WaitForBinding(string action)
    {
        listeningForKey = true;
        TextMeshProUGUI label = action == "left" ? bindLeftButton.GetComponentInChildren<TextMeshProUGUI>() : action == "right" ? bindRightButton.GetComponentInChildren<TextMeshProUGUI>() : bindJumpButton.GetComponentInChildren<TextMeshProUGUI>();
        string previous = label != null ? label.text : "";
        if (label != null) label.text = "APPUYEZ SUR UNE TOUCHE…  ·  ÉCHAP POUR ANNULER";
        yield return null;
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) break;
            if (Input.anyKeyDown)
            {
                KeyCode key = ReadPressedKey();
                if (key != KeyCode.None && IsBindable(key) && !IsUsedByOtherAction(action, key))
                {
                    if (action == "left") PlayerKeyBindings.SetLeft(key);
                    else if (action == "right") PlayerKeyBindings.SetRight(key);
                    else PlayerKeyBindings.SetJump(key);
                    break;
                }
            }
            yield return null;
        }
        listeningForKey = false;
        if (label != null) label.text = previous;
        RefreshControlLabels();
    }

    private KeyCode ReadPressedKey()
    {
        foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            if (key != KeyCode.None && Input.GetKeyDown(key)) return key;
        return KeyCode.None;
    }

    private bool IsBindable(KeyCode key)
    {
        return (key >= KeyCode.A && key <= KeyCode.Z) || (key >= KeyCode.Alpha0 && key <= KeyCode.Alpha9) ||
               key == KeyCode.Space || key == KeyCode.LeftShift || key == KeyCode.RightShift ||
               key == KeyCode.LeftControl || key == KeyCode.RightControl;
    }

    private bool IsUsedByOtherAction(string action, KeyCode key)
    {
        return (action != "left" && PlayerKeyBindings.Left == key) ||
               (action != "right" && PlayerKeyBindings.Right == key) ||
               (action != "jump" && PlayerKeyBindings.Jump == key);
    }

    private void RefreshControlLabels()
    {
        SetButtonLabel(bindLeftButton, "GAUCHE  ·  " + PlayerKeyBindings.Left);
        SetButtonLabel(bindRightButton, "DROITE  ·  " + PlayerKeyBindings.Right);
        SetButtonLabel(bindJumpButton, "SAUT  ·  " + PlayerKeyBindings.Jump);
    }

    private void SetButtonLabel(Button button, string value)
    {
        if (button == null) return;
        TextMeshProUGUI label = button.GetComponentInChildren<TextMeshProUGUI>(true);
        if (label != null) label.text = value;
    }

    private void ToggleHighContrast()
    {
        int enabled = GameManager.GetSavedValue("Accessibility_HighContrast", 0) == 1 ? 0 : 1;
        GameManager.SaveValue("Accessibility_HighContrast", enabled);
        RefreshAccessibilityLabels(); GoldRushVisualTheme.ApplyToActiveScene();
    }
    private void ToggleLargeText()
    {
        int enabled = GameManager.GetSavedValue("Accessibility_LargeText", 0) == 1 ? 0 : 1;
        GameManager.SaveValue("Accessibility_LargeText", enabled);
        RefreshAccessibilityLabels(); GoldRushVisualTheme.ApplyToActiveScene();
    }
    private void RefreshAccessibilityLabels()
    {
        if (contrastLabel != null) contrastLabel.text = "CONTRASTE ÉLEVÉ  ·  " + (GameManager.GetSavedValue("Accessibility_HighContrast", 0) == 1 ? "ACTIVÉ" : "DÉSACTIVÉ");
        if (largeTextLabel != null) largeTextLabel.text = "TEXTE AGRANDI  ·  " + (GameManager.GetSavedValue("Accessibility_LargeText", 0) == 1 ? "ACTIVÉ" : "DÉSACTIVÉ");
    }
    private void ResetProgress()
    {
        GameManager.ResetAllProgress();
        OnSFXChanged(1f); OnMusicChanged(0.55f);
        RefreshControlLabels(); RefreshAccessibilityLabels();
    }

    public void CloseMenu()
    {
        gameObject.SetActive(false);
        closedCallback?.Invoke();
    }
}
