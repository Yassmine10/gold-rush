using UnityEngine;

/// <summary>
/// Parallax background controller – Gold Rush.
/// Compatible with CameraFollow25D.
///
/// CORRECT APPROACH:
///   position = startWorldPos + (camera - startCameraPos) * (1 - parallaxFactor)
///
///   - parallaxFactor = 0  → sprite is fully locked to camera (pure background, never moves)
///   - parallaxFactor = 1  → sprite moves exactly with camera (no parallax effect)
///   - Values 0.05–0.3     → realistic depth layers
///
/// This avoids the "drift" problem of the old delta-based approach, where the
/// background would slide away from the camera over time.
/// </summary>
public class ParallaxBackground : MonoBehaviour
{
    [Header("Parallax")]
    [Tooltip("0 = locked to camera (sky). 1 = follows world. 0.05–0.3 = depth layers.")]
    [Range(0f, 1f)]
    public float parallaxFactor = 0.1f;

    [Tooltip("Tile seamlessly in X so there are never horizontal gaps.")]
    public bool infiniteScrollX = true;

    [Tooltip("Also apply parallax vertically (usually false for sky, true for cave layers).")]
    public bool parallaxY = false;

    // ── private ───────────────────────────────────────────────────────────────
    private Camera  cam;
    private Vector3 startWorldPos;     // where this layer started
    private Vector3 startCameraPos;    // where the camera was at Start
    private float   spriteWorldWidth;  // used for tiling

    void Start()
    {
        cam = Camera.main;
        if (cam == null) { enabled = false; return; }

        startWorldPos  = transform.position;
        startCameraPos = cam.transform.position;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && sr.sprite != null)
        {
            spriteWorldWidth = (sr.sprite.texture.width / sr.sprite.pixelsPerUnit)
                               * Mathf.Abs(transform.localScale.x);
        }
        if (spriteWorldWidth < 0.1f) spriteWorldWidth = 40f;
    }

    void LateUpdate()
    {
        if (cam == null) return;

        // Camera displacement since scene start
        Vector3 camDelta = cam.transform.position - startCameraPos;

        // New position: start + (how much camera moved) * (1 - factor)
        //   factor=0 → layer moves as much as camera → locked to camera view
        //   factor=1 → layer doesn't move → far distance parallax
        float newX = startWorldPos.x + camDelta.x * (1f - parallaxFactor);
        float newY = parallaxY
            ? startWorldPos.y + camDelta.y * (1f - parallaxFactor)
            : startWorldPos.y + camDelta.y;   // Y always follows camera unless overridden

        transform.position = new Vector3(newX, newY, transform.position.z);

        // ── Seamless X tiling ────────────────────────────────────────────────
        if (infiniteScrollX && spriteWorldWidth > 0.1f)
        {
            float distX = cam.transform.position.x - transform.position.x;
            if (Mathf.Abs(distX) >= spriteWorldWidth)
            {
                float steps = Mathf.Floor(Mathf.Abs(distX) / spriteWorldWidth);
                startWorldPos.x += Mathf.Sign(distX) * steps * spriteWorldWidth;
            }
        }
    }
}
