using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Panneau modal d'instructions / guide des règles et commandes du jeu.
/// Permet au joueur de consulter à tout moment comment jouer sur PC et Mobile.
/// </summary>
public class InstructionsMenu : MonoBehaviour
{
    public Button btnClose;

    void Awake()
    {
        if (btnClose != null)
        {
            btnClose.onClick.AddListener(Close);
        }
    }

    public void Open()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlayButtonClick();
        gameObject.SetActive(true);
    }

    public void Close()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlayButtonClick();
        gameObject.SetActive(false);
    }
}
