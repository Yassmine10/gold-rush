using UnityEngine;

/// <summary>
/// Anime la chauve-souris 2D avec battement d'ailes et trajectoire ondulante.
/// </summary>
public class BatSpriteAnimator : MonoBehaviour
{
    public Sprite fly1Sprite;
    public Sprite fly2Sprite;
    public float flapRate = 6f;
    public float waveAmplitude = 0.35f;
    public float waveSpeed = 4f;

    private SpriteRenderer sr;
    private SimpleEnemy enemy;
    private float flapTimer = 0f;
    private bool useFly1 = true;
    private float initialY;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        enemy = GetComponentInParent<SimpleEnemy>();
        initialY = transform.position.y;
    }

    void Update()
    {
        if (sr != null)
        {
            // Battement d'ailes
            flapTimer += Time.deltaTime;
            if (flapTimer >= 1f / flapRate)
            {
                flapTimer = 0f;
                useFly1 = !useFly1;
                sr.sprite = useFly1 ? fly1Sprite : fly2Sprite;
            }

            // Retourner selon le sens de vol
            if (enemy != null)
            {
                sr.flipX = enemy.movingRight;
            }

            // Ondulation sinusoïdale en l'air
            Vector3 pos = transform.position;
            pos.y = initialY + Mathf.Sin(Time.time * waveSpeed) * waveAmplitude;
            transform.position = pos;
        }
    }
}
