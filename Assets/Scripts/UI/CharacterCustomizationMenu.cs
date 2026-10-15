using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Gère la fenêtre d'achat / choix du personnage (Bouton 🧍 PERSO).
/// Permet de débloquer et sélectionner différentes tenues pour le joueur.
/// </summary>
public class CharacterCustomizationMenu : MonoBehaviour
{
    [Header("Bouton Fermer")]
    public Button btnClose;

    [Header("Boutons de Sélection de Skins")]
    public Button btnSkinClassic;
    public Button btnSkinAdventurer;
    public Button btnSkinNinja;
    public Button btnSkinMage;
    public Button btnSkinKing;

    [Header("Textes d'état")]
    public TextMeshProUGUI txtSelectedSkinName;

    void Start()
    {
        if (btnClose != null) btnClose.onClick.AddListener(() => gameObject.SetActive(false));

        if (btnSkinClassic != null) btnSkinClassic.onClick.AddListener(() => SelectSkin(0, "Héros Classic"));
        if (btnSkinAdventurer != null) btnSkinAdventurer.onClick.AddListener(() => SelectSkin(1, "Aventurier"));
        if (btnSkinNinja != null) btnSkinNinja.onClick.AddListener(() => SelectSkin(2, "Ninja Shinobi"));
        if (btnSkinMage != null) btnSkinMage.onClick.AddListener(() => SelectSkin(3, "Mage Arcaniste"));
        if (btnSkinKing != null) btnSkinKing.onClick.AddListener(() => SelectSkin(4, "Roi des Pièces"));

        int activeSkin = GameManager.GetSavedValue("SelectedSkinIndex", 0);
        UpdateSkinSelectionUI(activeSkin);
    }

    public void SelectSkin(int skinIndex, string skinName)
    {
        GameManager.SaveValue("SelectedSkinIndex", skinIndex);

        if (SoundManager.Instance != null) SoundManager.Instance.PlayButtonClick();
        UpdateSkinSelectionUI(skinIndex, skinName);
    }

    private void UpdateSkinSelectionUI(int index, string name = "")
    {
        if (txtSelectedSkinName != null)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = index switch
                {
                    1 => "Aventurier",
                    2 => "Ninja Shinobi",
                    3 => "Mage Arcaniste",
                    4 => "Roi des Pièces",
                    _ => "Héros Classic"
                };
            }
            txtSelectedSkinName.text = $"PERSO ÉQUIPÉ : {name.ToUpper()}";
        }
    }
}
