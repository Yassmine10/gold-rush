using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Gère les contrôles tactiles à l'écran pour smartphone :
/// - Bouton Gauche (Maintenir appuyé)
/// - Bouton Droite (Maintenir appuyé)
/// - Gros Bouton SAUT (Tap)
/// </summary>
public class MobileTouchControls : MonoBehaviour
{
    public PlayerController25D playerController;

    private bool isMovingLeft = false;
    private bool isMovingRight = false;

    void Update()
    {
        if (playerController == null) return;

        if (isMovingLeft && !isMovingRight)
        {
            playerController.SetHorizontalInput(-1f);
        }
        else if (isMovingRight && !isMovingLeft)
        {
            playerController.SetHorizontalInput(1f);
        }
        else
        {
            playerController.SetHorizontalInput(0f);
        }
    }

    // Événements appelés par EventTrigger sur les boutons UI

    public void OnPointerDownLeft()
    {
        isMovingLeft = true;
    }

    public void OnPointerUpLeft()
    {
        isMovingLeft = false;
    }

    public void OnPointerDownRight()
    {
        isMovingRight = true;
    }

    public void OnPointerUpRight()
    {
        isMovingRight = false;
    }

    public void OnJumpClicked()
    {
        if (playerController != null)
        {
            playerController.TriggerJump();
        }
    }
}
