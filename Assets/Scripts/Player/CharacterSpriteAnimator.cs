using UnityEngine;

/// <summary>Animations 2D procédurales pour la course, le saut, les dégâts et la victoire.</summary>
public class CharacterSpriteAnimator : MonoBehaviour
{
    [Header("Sprites 2D")]
    public Sprite idleSprite;
    public Sprite run1Sprite;
    public Sprite run2Sprite;
    public Sprite jumpSprite;
    public Sprite hurtSprite;

    [Header("Skins alternatifs")]
    public Sprite skinAdventurer;
    public Sprite skinNinja;
    public Sprite skinMage;
    public Sprite skinKing;

    [Header("Réglages d'animation")]
    [Min(0.04f)] public float runStepDuration = 0.12f;
    [Min(0f)] public float runBounce = 0.045f;
    [Min(0f)] public float breathingAmount = 0.022f;
    [Min(0.05f)] public float hurtDuration = 0.42f;

    private SpriteRenderer sr;
    private PlayerController25D playerCtrl;
    private float runTimer;
    private float stateTimer;
    private float hurtTimer;
    private bool useRun1 = true;
    private Vector3 baseScale;
    private Vector3 basePosition;
    private Quaternion baseRotation;
    private Color baseColor;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        playerCtrl = GetComponentInParent<PlayerController25D>();
        baseScale = transform.localScale;
        basePosition = transform.localPosition;
        baseRotation = transform.localRotation;
        if (sr != null) baseColor = sr.color;
    }

    private void Start()
    {
        int skin = GameManager.GetSavedValue("SelectedSkinIndex", 0);
        if (skin == 1 && skinAdventurer != null) idleSprite = skinAdventurer;
        else if (skin == 2 && skinNinja != null) idleSprite = skinNinja;
        else if (skin == 3 && skinMage != null) idleSprite = skinMage;
        else if (skin == 4 && skinKing != null) idleSprite = skinKing;
        if (sr != null && idleSprite != null) sr.sprite = idleSprite;
    }

    private void Update()
    {
        if (sr == null || playerCtrl == null) return;

        float dt = Time.deltaTime;
        Vector3 move = playerCtrl.GetLastMoveDirection();
        float speed = Mathf.Abs(move.x);
        bool grounded = playerCtrl.IsGrounded();
        bool victorious = playerCtrl.isVictorious;
        stateTimer += dt;
        hurtTimer = Mathf.Max(0f, hurtTimer - dt);

        // Le sprite regarde la direction du déplacement et s'incline légèrement dans les virages.
        if (move.x < -0.05f) sr.flipX = true;
        else if (move.x > 0.05f) sr.flipX = false;
        float lean = grounded && speed > 0.2f ? Mathf.Clamp(-move.x * 1.8f, -7f, 7f) : 0f;
        transform.localRotation = Quaternion.Slerp(transform.localRotation,
            baseRotation * Quaternion.Euler(0f, 0f, lean), dt * 12f);

        Vector3 targetScale = baseScale;
        Vector3 targetPosition = basePosition;

        if (hurtTimer > 0f)
        {
            if (hurtSprite != null) sr.sprite = hurtSprite;
            float pulse = Mathf.Sin((1f - hurtTimer / hurtDuration) * Mathf.PI * 8f);
            sr.color = Color.Lerp(baseColor, new Color(1f, 0.45f, 0.38f, baseColor.a), Mathf.Clamp01(0.35f + pulse * 0.25f));
            targetScale = Vector3.Scale(baseScale, new Vector3(1.1f, 0.9f, 1f));
        }
        else if (victorious)
        {
            if (idleSprite != null) sr.sprite = idleSprite;
            float bounce = Mathf.Abs(Mathf.Sin(stateTimer * 6f));
            targetPosition.y += bounce * 0.09f;
            targetScale = Vector3.Scale(baseScale, new Vector3(1f - bounce * 0.06f, 1f + bounce * 0.08f, 1f));
            sr.color = Color.Lerp(baseColor, new Color(1f, 0.88f, 0.42f, baseColor.a), (Mathf.Sin(stateTimer * 8f) + 1f) * 0.22f);
        }
        else if (!grounded)
        {
            if (jumpSprite != null) sr.sprite = jumpSprite;
            else if (idleSprite != null) sr.sprite = idleSprite;

            float riseStretch = playerCtrl.GetVelocity().y > 0.1f ? 1.12f : 0.94f;
            targetScale = Vector3.Scale(baseScale, new Vector3(0.92f, riseStretch, 1f));
            targetPosition.y += playerCtrl.GetVelocity().y > 0.1f ? 0.025f : -0.015f;
            sr.color = baseColor;
            runTimer = 0f; // reset run timer while airborne
        }
        else if (speed > 0.2f && run1Sprite != null && run2Sprite != null)
        {
            runTimer += dt;
            if (runTimer >= runStepDuration)
            {
                runTimer %= runStepDuration;
                useRun1 = !useRun1;
            }
            sr.sprite = useRun1 ? run1Sprite : run2Sprite;
            float phase = stateTimer * 15f;
            float bounce = Mathf.Sin(phase);
            targetScale = Vector3.Scale(baseScale, new Vector3(1f - bounce * runBounce * 0.75f, 1f, 1f));
            sr.color = baseColor;
        }
        else
        {
            if (idleSprite != null) sr.sprite = idleSprite;
            float breath = Mathf.Sin(stateTimer * 2.1f) * breathingAmount;
            targetScale = Vector3.Scale(baseScale, new Vector3(1f - breath * 0.45f, 1f, 1f));
            sr.color = baseColor;
        }

        float smoothing = hurtTimer > 0f ? 18f : 12f;
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, dt * smoothing);
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, dt * smoothing);
    }

    public void TriggerHurt()
    {
        hurtTimer = hurtDuration;
        if (sr != null && hurtSprite != null) sr.sprite = hurtSprite;
        if (sr != null) CollectibleBurstVFX.Spawn(sr, new Color(1f, 0.35f, 0.3f, 0.85f), 0.34f, 1.28f);
    }
}
