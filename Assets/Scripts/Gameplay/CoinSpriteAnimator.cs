using UnityEngine;

/// <summary>
/// Fait tourner la pièce d'or 2D avec ses 4 frames d'animation
/// et ajoute un léger flottement vertical pour un effet vivant.
/// </summary>
public class CoinSpriteAnimator : MonoBehaviour
{
    [Header("Frames de rotation")]
    public Sprite[] spinFrames;
    public float frameRate = 9f;
    public float bobAmplitude = 0.08f;
    public float bobSpeed = 2.5f;

    private SpriteRenderer sr;
    private int currentFrame = 0;
    private float frameTimer = 0f;
    private Vector3 initialPos;
    private float randomOffset;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        initialPos = transform.position;
        randomOffset = Random.Range(0f, 6.28f);
    }

    void Update()
    {
        // 1. Animation des frames de rotation
        if (sr != null && spinFrames != null && spinFrames.Length > 0)
        {
            frameTimer += Time.deltaTime;
            if (frameTimer >= 1f / frameRate)
            {
                frameTimer = 0f;
                currentFrame = (currentFrame + 1) % spinFrames.Length;
                sr.sprite = spinFrames[currentFrame];
            }
        }

        // 2. Flottement vertical doux
        transform.position = initialPos + new Vector3(0, Mathf.Sin(Time.time * bobSpeed + randomOffset) * bobAmplitude, 0);
    }
}
