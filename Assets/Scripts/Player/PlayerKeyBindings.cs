using UnityEngine;

/// <summary>Lecture et sauvegarde des touches personnalisables du joueur.</summary>
public static class PlayerKeyBindings
{
    private const string LeftKeySetting = "Control_MoveLeft";
    private const string RightKeySetting = "Control_MoveRight";
    private const string JumpKeySetting = "Control_Jump";

    public static KeyCode Left => ReadKey(LeftKeySetting, KeyCode.Q);
    public static KeyCode Right => ReadKey(RightKeySetting, KeyCode.D);
    public static KeyCode Jump => ReadKey(JumpKeySetting, KeyCode.Space);

    public static void SetLeft(KeyCode key) => GameManager.SaveValue(LeftKeySetting, (int)key);
    public static void SetRight(KeyCode key) => GameManager.SaveValue(RightKeySetting, (int)key);
    public static void SetJump(KeyCode key) => GameManager.SaveValue(JumpKeySetting, (int)key);

    private static KeyCode ReadKey(string setting, KeyCode fallback)
    {
        int value = GameManager.GetSavedValue(setting, (int)fallback);
        KeyCode key = (KeyCode)value;
        return System.Enum.IsDefined(typeof(KeyCode), key) ? key : fallback;
    }
}
