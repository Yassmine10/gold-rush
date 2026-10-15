using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Applies the shared Gold Rush palette to menu, HUD and modal UI.
/// World sprites keep their level-specific colors.
/// </summary>
public static class GoldRushVisualTheme
{
    private static readonly Color32 Ink = new Color32(15, 23, 42, 255);
    private static readonly Color32 Panel = new Color32(27, 40, 62, 242);
    private static readonly Color32 PanelRaised = new Color32(39, 55, 79, 255);
    private static readonly Color32 Gold = new Color32(246, 190, 72, 255);
    private static readonly Color32 GoldLight = new Color32(255, 221, 145, 255);
    private static readonly Color32 Teal = new Color32(65, 190, 166, 255);
    private static readonly Color32 Cream = new Color32(246, 244, 237, 255);
    private static readonly Color32 Muted = new Color32(172, 187, 207, 255);
    private static readonly Color32 Coral = new Color32(226, 105, 97, 255);
    private static readonly Color32 Disabled = new Color32(102, 116, 135, 190);
    private static readonly Dictionary<int, float> BaseTextSizes = new Dictionary<int, float>();
    private static int cachedTextSceneHandle = -1;

    public static void ApplyToActiveScene()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        if (cachedTextSceneHandle != activeScene.handle)
        {
            BaseTextSizes.Clear();
            cachedTextSceneHandle = activeScene.handle;
        }
        CanvasScaler[] scalers = Object.FindObjectsByType<CanvasScaler>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (CanvasScaler scaler in scalers)
        {
            if (scaler == null || scaler.gameObject.scene != activeScene) continue;
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            // Keep edge controls inside narrow screens while the height grows naturally on portrait devices.
            scaler.matchWidthOrHeight = 0f;
        }

        Graphic[] graphics = Object.FindObjectsByType<Graphic>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (Graphic graphic in graphics)
        {
            if (graphic == null || graphic.gameObject.scene != activeScene || graphic is TextMeshProUGUI)
                continue;

            Image image = graphic as Image;
            if (image == null) continue;

            string objectName = image.gameObject.name.ToLowerInvariant();
            if (image.GetComponent<Button>() != null) continue;

            if (ContainsAny(objectName, "locked", "lock")) image.color = WithAlpha(Ink, 205);
            else if (ContainsAny(objectName, "panel", "modal", "card", "badge", "container"))
                image.color = objectName.Contains("card") ? PanelRaised : Panel;
            else if (objectName.Contains("fill")) image.color = Gold;
            else if (objectName.Contains("star")) image.color = Gold;
        }

        TextMeshProUGUI[] texts = Object.FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        bool highContrast = GameManager.GetSavedValue("Accessibility_HighContrast", 0) == 1;
        bool largeText = GameManager.GetSavedValue("Accessibility_LargeText", 0) == 1;
        Color mutedText = highContrast ? new Color32(225, 233, 244, 255) : Muted;
        foreach (TextMeshProUGUI text in texts)
        {
            if (text == null || text.gameObject.scene != activeScene) continue;

            int textId = text.GetInstanceID();
            if (!BaseTextSizes.ContainsKey(textId)) BaseTextSizes[textId] = text.fontSize;
            text.fontSize = BaseTextSizes[textId] * (largeText ? 1.18f : 1f);

            string objectName = text.gameObject.name.ToLowerInvariant();
            string content = text.text.ToLowerInvariant();
            Button parentButton = text.GetComponentInParent<Button>();

            if (parentButton != null)
                text.color = IsPrimaryButton(parentButton) ? Ink : Cream;
            else if (ContainsAny(objectName, "coin", "score", "star") || content.Contains("pièces"))
                text.color = GoldLight;
            else if (ContainsAny(objectName, "heart", "health", "vie"))
                text.color = Teal;
            else if (ContainsAny(objectName, "title", "header", "levelname"))
                text.color = GoldLight;
            else if (ContainsAny(objectName, "subtitle", "description", "hint", "footer", "caption"))
                text.color = mutedText;
            else
                text.color = Cream;
        }

        Button[] buttons = Object.FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Button button in buttons)
        {
            if (button == null || button.gameObject.scene != activeScene) continue;

            Color baseColor = ButtonColor(button);
            ColorBlock colors = button.colors;
            colors.normalColor = baseColor;
            colors.highlightedColor = Color.Lerp(baseColor, Color.white, 0.16f);
            colors.pressedColor = Color.Lerp(baseColor, Ink, 0.2f);
            colors.selectedColor = Color.Lerp(baseColor, Color.white, 0.1f);
            colors.disabledColor = Disabled;
            colors.colorMultiplier = 1f;
            button.colors = colors;

            if (button.targetGraphic != null) button.targetGraphic.color = baseColor;
        }
    }

    private static Color ButtonColor(Button button)
    {
        string name = button.gameObject.name.ToLowerInvariant();
        if (ContainsAny(name, "reset", "delete", "quit", "gameover")) return Coral;
        if (ContainsAny(name, "play", "next", "finish", "confirm")) return Gold;
        if (ContainsAny(name, "pause", "settings", "instructions", "resume", "back", "menu")) return Teal;
        return PanelRaised;
    }

    private static bool IsPrimaryButton(Button button)
    {
        string name = button.gameObject.name.ToLowerInvariant();
        return ContainsAny(name, "play", "next", "finish", "confirm");
    }

    private static bool ContainsAny(string value, params string[] terms)
    {
        foreach (string term in terms)
            if (value.Contains(term)) return true;
        return false;
    }

    private static Color WithAlpha(Color color, byte alpha)
    {
        color.a = alpha / 255f;
        return color;
    }
}
