using UnityEngine;

/// <summary>Petit éclat de sprite détaché, utilisé à la collecte et lors d'un impact.</summary>
public sealed class CollectibleBurstVFX : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color tint;
    private Vector3 startScale;
    private Vector3 drift;
    private float duration;
    private float elapsed;

    public static void Spawn(SpriteRenderer source, Color color, float lifetime = 0.3f, float scaleMultiplier = 1.35f)
    {
        if (source == null || source.sprite == null) return;
        GameObject burst = new GameObject("Sprite_Burst_VFX");
        burst.transform.position = source.transform.position;
        burst.transform.rotation = source.transform.rotation;
        burst.transform.localScale = source.transform.lossyScale;
        SpriteRenderer renderer = burst.AddComponent<SpriteRenderer>();
        renderer.sprite = source.sprite;
        renderer.flipX = source.flipX;
        renderer.flipY = source.flipY;
        renderer.sortingLayerID = source.sortingLayerID;
        renderer.sortingOrder = source.sortingOrder + 2;
        CollectibleBurstVFX effect = burst.AddComponent<CollectibleBurstVFX>();
        effect.Initialize(color, lifetime, scaleMultiplier);
    }

    private void Initialize(Color color, float lifetime, float scaleMultiplier)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        tint = color;
        duration = Mathf.Max(0.08f, lifetime);
        startScale = transform.localScale;
        transform.localScale = startScale * 0.7f;
        drift = new Vector3(Random.Range(-0.15f, 0.15f), Random.Range(0.25f, 0.55f), 0f);
        startScale *= scaleMultiplier;
    }

    private void Update()
    {
        float dt = Time.unscaledDeltaTime;
        elapsed += dt;
        float t = Mathf.Clamp01(elapsed / duration);
        float pop = t < 0.22f ? Mathf.Lerp(0.7f, 1.12f, t / 0.22f) : Mathf.Lerp(1.12f, 1f, (t - 0.22f) / 0.78f);
        transform.localScale = Vector3.Lerp(transform.localScale, startScale * pop, dt * 18f);
        transform.position += drift * dt;
        if (spriteRenderer != null)
        {
            Color c = tint;
            c.a = tint.a * (1f - t);
            spriteRenderer.color = c;
        }
        if (elapsed >= duration) Destroy(gameObject);
    }
}
