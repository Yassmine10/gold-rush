using UnityEngine;
using System.Collections;

/// <summary>
/// Crystal Cave ambient animator for Gold Rush – Level 2.
/// Procedurally generates:
///   - Floating magical dust particles (cyan, violet, magenta)
///   - Pulsing glow on crystal layers (colour modulation via SpriteRenderer.color)
///   - Slow-drifting spore particles
/// Attach to a single GameObject called "CaveAmbientAnimator" inside the
/// CrystalCave_BG_Root hierarchy.
/// No external assets required — everything is drawn in code.
/// </summary>
public class CaveAmbientAnimator : MonoBehaviour
{
    // ── Inspector ─────────────────────────────────────────────────────────────
    [Header("Dust Particles")]
    public int dustCount = 40;
    public Vector2 dustSpawnAreaHalfSize = new Vector2(14f, 6f);
    public float dustRiseSpeed = 0.25f;
    public float dustWobbleFrequency = 1.2f;
    public float dustWobbleAmplitude = 0.18f;

    [Header("Crystal Pulse")]
    [Tooltip("Drag in the SpriteRenderer of the Crystal layer so it can pulse.")]
    public SpriteRenderer crystalLayerRenderer;
    public float pulseSpeed = 0.7f;
    public float pulseMin = 0.82f;
    public float pulseMax = 1.0f;

    // ── private ───────────────────────────────────────────────────────────────
    private Camera cam;
    private Transform[] dustParticles;
    private float[]    dustPhases;      // per-particle wobble offset
    private Color[]    dustColours;
    private float[]    dustSizes;
    private float      pulseTimer;

    // Crystal cave colour palette
    private static readonly Color[] palette =
    {
        new Color(0.20f, 0.95f, 0.90f, 0.75f),   // cyan
        new Color(0.55f, 0.15f, 0.95f, 0.65f),   // violet
        new Color(0.90f, 0.10f, 0.80f, 0.70f),   // magenta
        new Color(0.10f, 0.85f, 0.65f, 0.60f),   // turquoise
        new Color(0.75f, 0.40f, 1.00f, 0.55f),   // lavender
    };

    // ── Unity lifecycle ───────────────────────────────────────────────────────

    void Start()
    {
        cam = Camera.main;
        if (cam == null) { enabled = false; return; }
        SpawnDustParticles();
    }

    void Update()
    {
        MoveDust();
        PulseCrystalLayer();
    }

    // ── Dust particles ────────────────────────────────────────────────────────

    private void SpawnDustParticles()
    {
        dustParticles = new Transform[dustCount];
        dustPhases    = new float[dustCount];
        dustColours   = new Color[dustCount];
        dustSizes     = new float[dustCount];

        for (int i = 0; i < dustCount; i++)
        {
            Color col    = palette[Random.Range(0, palette.Length)];
            float sz     = Random.Range(0.04f, 0.12f);
            Vector3 pos  = RandomSpawnPos();

            GameObject go = new GameObject($"CaveDust_{i}");
            go.transform.SetParent(transform, false);
            go.transform.position = pos;
            go.transform.localScale = Vector3.one * sz;

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite       = CreateGlowDotSprite(col);
            sr.color        = col;
            sr.sortingLayerName = "Background";
            sr.sortingOrder = -2;

            dustParticles[i] = go.transform;
            dustPhases[i]    = Random.Range(0f, Mathf.PI * 2f);
            dustColours[i]   = col;
            dustSizes[i]     = sz;
        }
    }

    private void MoveDust()
    {
        if (cam == null) return;
        float camX = cam.transform.position.x;
        float camY = cam.transform.position.y;

        for (int i = 0; i < dustCount; i++)
        {
            if (dustParticles[i] == null) continue;

            // Rise slowly upward with horizontal wobble
            Vector3 p = dustParticles[i].position;
            p.y += dustRiseSpeed * Time.deltaTime;
            p.x += Mathf.Sin(Time.time * dustWobbleFrequency + dustPhases[i])
                   * dustWobbleAmplitude * Time.deltaTime;
            dustParticles[i].position = p;

            // Fade in alpha as particle rises, fade out near top
            float relY  = p.y - (camY - dustSpawnAreaHalfSize.y);
            float ratio = Mathf.Clamp01(relY / (dustSpawnAreaHalfSize.y * 2f));
            SpriteRenderer sr = dustParticles[i].GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color c = dustColours[i];
                c.a = Mathf.Sin(ratio * Mathf.PI) * dustColours[i].a;
                sr.color = c;
            }

            // Recycle when particle drifts too far up or off screen
            bool offTop    = p.y > camY + dustSpawnAreaHalfSize.y + 1f;
            bool offSide   = Mathf.Abs(p.x - camX) > dustSpawnAreaHalfSize.x + 2f;
            if (offTop || offSide)
            {
                dustParticles[i].position = RandomSpawnPos();
            }
        }
    }

    private Vector3 RandomSpawnPos()
    {
        float cx = cam != null ? cam.transform.position.x : 0f;
        float cy = cam != null ? cam.transform.position.y : 0f;
        return new Vector3(
            cx + Random.Range(-dustSpawnAreaHalfSize.x, dustSpawnAreaHalfSize.x),
            cy + Random.Range(-dustSpawnAreaHalfSize.y, -dustSpawnAreaHalfSize.y * 0.3f),
            4f);
    }

    // ── Crystal pulse ─────────────────────────────────────────────────────────

    private void PulseCrystalLayer()
    {
        if (crystalLayerRenderer == null) return;
        pulseTimer += Time.deltaTime * pulseSpeed;
        float t = (Mathf.Sin(pulseTimer) + 1f) * 0.5f;   // 0..1
        float brightness = Mathf.Lerp(pulseMin, pulseMax, t);
        crystalLayerRenderer.color = new Color(brightness, brightness, brightness, 1f);
    }

    // ── Sprite factory ────────────────────────────────────────────────────────

    /// <summary>Tiny radial glow dot for the dust particles.</summary>
    private static Sprite CreateGlowDotSprite(Color col)
    {
        const int size = 16;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        Color[] pixels = new Color[size * size];
        float center = size * 0.5f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(center, center));
                float alpha = Mathf.Clamp01(1f - dist / center);
                alpha = alpha * alpha;   // sharper falloff
                pixels[y * size + x] = new Color(col.r, col.g, col.b, alpha * col.a);
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 32f);
    }
}
