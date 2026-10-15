using UnityEngine;

/// <summary>
/// Contrôleur 2D/2.5D du joueur pour Coin Collector.
/// Prend en charge les contrôles Clavier :
/// - [ESPACE] : Saut principal
/// - [FLÈCHE HAUT] : Saut alternatif
/// - [Z] : Saut (clavier AZERTY)
/// - [W] : Saut (clavier QWERTY)
/// - [Q] / [D] et [FLÈCHES GAUCHE / DROITE] : Déplacement
/// Ainsi que les boutons tactiles Mobile à l'écran.
/// Intègre Jump Buffer, Coyote Time et détection du sol par collision pour un saut 100% réactif.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController25D : MonoBehaviour
{
    [Header("Mouvement")]
    public float moveSpeed = 8f;
    public float acceleration = 30f;
    public float rotationSpeed = 15f;

    [Header("Saut et Gravité")]
    public float jumpForce = 13.5f;
    public float gravity = 32f;
    public float fallMultiplier = 1.6f;
    public float coyoteTimeDuration = 0.18f;
    public float jumpBufferDuration = 0.18f;

    [Header("Respiration & Animations Procédurales (3D)")]
    public bool enableProceduralBreathing = false;
    public float breathingSpeed = 3f;
    public float breathingIntensity = 0.04f;
    public float runBobbingSpeed = 14f;
    public float runBobbingAmount = 0.06f;
    [Tooltip("Le joueur perd la partie s'il tombe sous cette hauteur.")]
    public float fallDeathY = -12f;
    [Min(0f)] public float offscreenGracePeriod = 0.35f;

    [Header("Mode Visuel 2D")]
    public bool is2DSprite = true;

    [Header("États d'animation")]
    public Animator animator;
    public bool isGrounded;
    public bool isRunning;
    public bool isJumping;
    public bool isFalling;
    public bool isVictorious;

    private CharacterController controller;
    private Vector3 velocity;
    private float horizontalInput;
    private bool jumpRequested;
    private float currentSpeed;

    private float coyoteTimer = 0f;
    private float jumpBufferTimer = 0f;

    private Vector3 originalScale;
    private float stepCycleTimer;
    private float offscreenTimer;
    private bool failureTriggered;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        originalScale = transform.localScale;
    }

    void Update()
    {
        bool belowLevel = transform.position.y < fallDeathY;

        if (!failureTriggered && belowLevel)
        {
            failureTriggered = true;
            if (HUDManager.Instance != null) HUDManager.Instance.TriggerGameOver();
            enabled = false;
            return;
        }
        if (isVictorious)
        {
            velocity.x = 0;
            velocity.y = 0;
            UpdateAnimations();
            return;
        }


        // 1. Lire les inputs de saut (Clavier & Mobile)
        KeyCode jumpKey = PlayerKeyBindings.Jump;
        bool jumpKeyDown = Input.GetKeyDown(jumpKey)
            || Input.GetKeyDown(KeyCode.UpArrow)
            || jumpRequested;

        bool jumpKeyHeld = Input.GetKey(jumpKey)
            || Input.GetKey(KeyCode.UpArrow)
            || jumpRequested;

        if (jumpKeyDown)
        {
            jumpBufferTimer = jumpBufferDuration;
            jumpRequested = false;
        }
        else
        {
            jumpBufferTimer -= Time.deltaTime;
        }

        // 2. Déclenchement du saut avec Jump Buffer et Coyote Time
        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            velocity.y = jumpForce;
            isJumping = true;
            isFalling = false;
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayJumpSound();
            }
        }
        else if (!isGrounded)
        {
            coyoteTimer -= Time.deltaTime;
        }

        // 3. Lire le déplacement horizontal (Clavier flèches, Q/D, A/D, ou boutons tactiles)
        KeyCode leftKey = PlayerKeyBindings.Left;
        KeyCode rightKey = PlayerKeyBindings.Right;
        bool movingLeft = Input.GetKey(leftKey) || Input.GetKey(KeyCode.LeftArrow);
        bool movingRight = Input.GetKey(rightKey) || Input.GetKey(KeyCode.RightArrow);
        float keyboardInput = movingLeft == movingRight ? 0f : movingLeft ? -1f : 1f;
        if (Input.GetJoystickNames().Length > 0)
        {
            float joystickInput = Input.GetAxisRaw("Horizontal");
            if (Mathf.Abs(joystickInput) > Mathf.Abs(keyboardInput)) keyboardInput = joystickInput;
        }

        float finalInput = Mathf.Abs(keyboardInput) > 0.01f ? keyboardInput : horizontalInput;

        // Vitesse horizontale fluide (gardée en l'air et au sol)
        float targetSpeed = finalInput * moveSpeed;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
        velocity.x = currentSpeed;

        // 4. Gravité dynamique (chute plus rapide si la touche saut est relâchée)
        if (velocity.y > 0 && !jumpKeyHeld)
        {
            velocity.y -= gravity * fallMultiplier * Time.deltaTime;
        }
        else
        {
            velocity.y -= gravity * Time.deltaTime;
        }

        // 5. Application du déplacement physique via CharacterController
        Vector3 moveVector = new Vector3(velocity.x, velocity.y, 0f);
        CollisionFlags flags = controller.Move(moveVector * Time.deltaTime);

        // Correction de dérive sur Z uniquement si nécessaire
        if (Mathf.Abs(transform.position.z) > 0.005f)
        {
            Vector3 pos = transform.position;
            pos.z = 0;
            transform.position = pos;
        }

        // A proximity ray caused early grounding; only a real collision counts.
        isGrounded = (flags & CollisionFlags.Below) != 0;

        if (isGrounded)
        {
            coyoteTimer = coyoteTimeDuration;
            if (velocity.y < 0f)
            {
                velocity.y = -2.5f; // Légère force de placage au sol
                isJumping = false;
                isFalling = false;
            }
        }
        else
        {
            if (velocity.y < -0.2f)
            {
                isFalling = true;
                isJumping = false;
            }
        }

        // 7. Animations procédurales 3D (bypassed si sprite 2D)
        ApplyProceduralBreathing();

        // 8. Mettre à jour les paramètres de l'Animator si présent
        UpdateAnimations();
    }

    private void ApplyProceduralBreathing()
    {
        if (!enableProceduralBreathing || is2DSprite) return;

        bool inAir = !isGrounded;
        bool isMoving = Mathf.Abs(currentSpeed) > 0.2f;

        if (inAir)
        {
            float stretch = velocity.y > 0 ? 1.15f : 0.9f;
            float squash = velocity.y > 0 ? 0.88f : 1.1f;
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(originalScale.x * squash, originalScale.y * stretch, originalScale.z * squash), Time.deltaTime * 12f);
        }
        else if (isMoving)
        {
            stepCycleTimer += Time.deltaTime * runBobbingSpeed;
            float scaleY = 1f + Mathf.Sin(stepCycleTimer) * runBobbingAmount;
            float scaleXZ = 1f - Mathf.Sin(stepCycleTimer) * (runBobbingAmount * 0.5f);
            transform.localScale = new Vector3(originalScale.x * scaleXZ, originalScale.y * scaleY, originalScale.z * scaleXZ);
        }
        else
        {
            stepCycleTimer += Time.deltaTime * breathingSpeed;
            float breathY = 1f + Mathf.Sin(stepCycleTimer) * breathingIntensity;
            float breathXZ = 1f - Mathf.Sin(stepCycleTimer) * (breathingIntensity * 0.5f);
            Vector3 targetScale = new Vector3(originalScale.x * breathXZ, originalScale.y * breathY, originalScale.z * breathXZ);
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * 8f);
        }
    }

    private void UpdateAnimations()
    {
        isRunning = Mathf.Abs(currentSpeed) > 0.1f && isGrounded;

        if (animator != null)
        {
            animator.SetBool("IsRunning", isRunning);
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetBool("IsJumping", isJumping);
            animator.SetBool("IsFalling", isFalling);
            animator.SetBool("IsVictorious", isVictorious);
            animator.SetFloat("Speed", Mathf.Abs(currentSpeed));
        }
    }

    // Méthodes pour les boutons tactiles Mobile & Stomp
    public void SetHorizontalInput(float direction)
    {
        horizontalInput = direction;
    }

    public void TriggerJump()
    {
        jumpRequested = true;
        jumpBufferTimer = jumpBufferDuration;
        coyoteTimer = coyoteTimeDuration; // Permet de rebondir immédiatement après avoir écrasé un slime
    }

    public void BounceFromStomp()
    {
        velocity.y = jumpForce;
        isGrounded = false;
        isJumping = true;
        isFalling = false;
        jumpRequested = false;
        jumpBufferTimer = 0f;
        coyoteTimer = 0f;
    }

    public bool IsStomping(Collider target)
    {
        if (target == null || controller == null || velocity.y > 0f)
        {
            return false;
        }

        return controller.bounds.min.y >= target.bounds.center.y;
    }

    public void ResetMotionAfterRespawn()
    {
        velocity = Vector3.zero;
        currentSpeed = 0f;
        horizontalInput = 0f;
        jumpRequested = false;
        jumpBufferTimer = 0f;
        coyoteTimer = 0f;
        isGrounded = false;
        isRunning = false;
        isJumping = false;
        isFalling = false;
        transform.localScale = originalScale;
    }

    public void TriggerVictory()
    {
        isVictorious = true;
    }

    public bool IsGrounded() => isGrounded;
    public Vector3 GetVelocity() => velocity;
    public float GetCurrentSpeed() => currentSpeed;
    public Vector3 GetLastMoveDirection() => new Vector3(velocity.x, 0, 0);
}
