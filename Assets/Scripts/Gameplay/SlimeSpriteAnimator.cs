using UnityEngine;

/// <summary>
/// Anime le slime 2D avec effet gélatineux (squash & stretch)
/// et retournement selon la direction de patrouille.
/// </summary>
public class SlimeSpriteAnimator : MonoBehaviour
{
    public Sprite idleSprite;
    public Sprite squashSprite;

    private SpriteRenderer sr;
    private SimpleEnemy enemy;
    private Vector3 originalScale;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        enemy = GetComponentInParent<SimpleEnemy>();
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (enemy != null && sr != null)
        {
            // Retourner selon le sens de marche
            sr.flipX = enemy.movingRight;
        }

        // Effet de rebond gélatineux cartoon
        float bounce = Mathf.Sin(Time.time * 5f);
        float scaleY = 1f + bounce * 0.08f;
        float scaleX = 1f - bounce * 0.08f;
        transform.localScale = new Vector3(originalScale.x * scaleX, originalScale.y * scaleY, originalScale.z);

        if (sr != null && idleSprite != null && squashSprite != null)
        {
            sr.sprite = (bounce < -0.6f) ? squashSprite : idleSprite;
        }
    }
}
