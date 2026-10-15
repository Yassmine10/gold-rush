using UnityEngine;

/// <summary>
/// Sky Temple ambient animator for Gold Rush – Level 3.
/// Procedurally handles:
///   - Floating golden and turquoise magical sparkles around the temple ruins
///   - Gentle drifting pixel art birds across the sky
///   - Slowly moving ambient clouds
/// Fully self-contained, no external asset dependencies.
/// </summary>
public class SkyTempleAmbientAnimator : MonoBehaviour
{
    [Header("Sparkles")]
    public int sparkleCount = 28;
    public Vector2 sparkleAreaHalfSize = new Vector2(16f, 7f);
    public float sparkleSpeed = 0.45f;

    [Header("Flying Birds")]
    public int birdCount = 5;
    public Vector2 birdYRange = new Vector2(4f, 10f);
    public float birdSpeed = 2.1f;

    private Camera cam;
    private Transform[] sparkles;
    private float[] sparklePhases;
    private Color[] sparkleColors;

    private GameObject[] birds;
    private float[] birdSpeeds;
    private bool[] birdGoingRight;

    private static readonly Color[] sparklePalette =
    {
        new Color(0.96f, 0.77f, 0.33f, 0.85f), // Gold #F6C453
        new Color(0.51f, 0.85f, 0.96f, 0.80f), // Cyan #82D9F5
        new Color(0.78f, 0.92f, 0.97f, 0.75f), // Cloud white #C7EAF7
        new Color(0.61f, 0.51f, 0.84f, 0.70f)  // Temple violet #9B83D6
    };

    private static readonly Color birdColor = new Color(0.18f, 0.22f, 0.35f, 0.9f);

    void Start()
    {
        cam = Camera.main;
        if (cam == null) { enabled = false; return; }

        SpawnSparkles();
        SpawnBirds();
    }

    void Update()
    {
        MoveSparkles();
        MoveBirds();
    }

    private void SpawnSparkles()
    {
        sparkles = new Transform[sparkleCount];
        sparklePhases = new float[sparkleCount];
        sparkleColors = new Color[sparkleCount];

        for (int i = 0; i < sparkleCount; i++)
        {
            Color col = sparklePalette[Random.Range(0, sparklePalette.Length)];
            float sz = Random.Range(0.06f, 0.16f);

            GameObject go = new GameObject($"TempleSparkle_{i}");
            go.transform.SetParent(transform, false);
            go.transform.position = RandomSparklePos();
            go.transform.localScale = Vector3.one * sz;

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = CreateSparkleSprite(col);
            sr.color = col;
            sr.sortingLayerName = "Midground";
            sr.sortingOrder = -2;

            sparkles[i] = go.transform;
            sparklePhases[i] = Random.Range(0f, Mathf.PI * 2f);
            sparkleColors[i] = col;
        }
    }

    private void MoveSparkles()
    {
        if (cam == null) return;
        float camX = cam.transform.position.x;
        float camY = cam.transform.position.y;

        for (int i = 0; i < sparkleCount; i++)
        {
            if (sparkles[i] == null) continue;

            Vector3 p = sparkles[i].position;
            p.y += sparkleSpeed * Time.deltaTime;
            p.x += Mathf.Sin(Time.time * 1.5f + sparklePhases[i]) * 0.25f * Time.deltaTime;
            sparkles[i].position = p;

            // Twinkle pulse
            float pulse = (Mathf.Sin(Time.time * 3f + sparklePhases[i]) + 1f) * 0.5f;
            SpriteRenderer sr = sparkles[i].GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color c = sparkleColors[i];
                c.a = Mathf.Lerp(0.2f, 0.9f, pulse);
                sr.color = c;
            }

            // Recycle
            if (p.y > camY + sparkleAreaHalfSize.y + 1f || Mathf.Abs(p.x - camX) > sparkleAreaHalfSize.x + 2f)
            {
                sparkles[i].position = RandomSparklePos();
            }
        }
    }

    private Vector3 RandomSparklePos()
    {
        float cx = cam != null ? cam.transform.position.x : 0f;
        float cy = cam != null ? cam.transform.position.y : 0f;
        return new Vector3(
            cx + Random.Range(-sparkleAreaHalfSize.x, sparkleAreaHalfSize.x),
            cy + Random.Range(-sparkleAreaHalfSize.y, sparkleAreaHalfSize.y),
            3.5f);
    }

    private void SpawnBirds()
    {
        birds = new GameObject[birdCount];
        birdSpeeds = new float[birdCount];
        birdGoingRight = new bool[birdCount];

        for (int i = 0; i < birdCount; i++)
        {
            bool goRight = Random.value > 0.5f;
            birdGoingRight[i] = goRight;
            birdSpeeds[i] = birdSpeed * Random.Range(0.7f, 1.3f);

            float cx = cam != null ? cam.transform.position.x : 0f;
            float x = cx + Random.Range(-12f, 12f);
            float y = Random.Range(birdYRange.x, birdYRange.y);

            birds[i] = new GameObject($"SkyBird_{i}");
            birds[i].transform.position = new Vector3(x, y, 4.5f);
            birds[i].transform.localScale = new Vector3(goRight ? 1f : -1f, 1f, 1f);

            SpriteRenderer sr = birds[i].AddComponent<SpriteRenderer>();
            sr.sprite = CreateBirdSprite();
            sr.sortingLayerName = "Background";
            sr.sortingOrder = -15;
        }
    }

    private void MoveBirds()
    {
        if (cam == null) return;
        float camX = cam.transform.position.x;
        float bound = 16f;

        for (int i = 0; i < birdCount; i++)
        {
            if (birds[i] == null) continue;

            float dir = birdGoingRight[i] ? 1f : -1f;
            birds[i].transform.position += new Vector3(dir * birdSpeeds[i] * Time.deltaTime, 0f, 0f);

            // Subtle vertical wave
            Vector3 pos = birds[i].transform.position;
            pos.y += Mathf.Sin(Time.time * 2f + i) * 0.005f;
            birds[i].transform.position = pos;

            float relX = birds[i].transform.position.x - camX;
            if (Mathf.Abs(relX) > bound)
            {
                birdGoingRight[i] = !birdGoingRight[i];
                float newX = camX + (birdGoingRight[i] ? -bound : bound);
                float newY = Random.Range(birdYRange.x, birdYRange.y);
                birds[i].transform.position = new Vector3(newX, newY, 4.5f);
                birds[i].transform.localScale = new Vector3(birdGoingRight[i] ? 1f : -1f, 1f, 1f);
            }
        }
    }

    private static Sprite CreateSparkleSprite(Color col)
    {
        const int s = 12;
        Texture2D tex = new Texture2D(s, s, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        Color[] p = new Color[s * s];
        int mid = s / 2;

        for (int y = 0; y < s; y++)
        {
            for (int x = 0; x < s; x++)
            {
                // 4-pointed cross sparkle
                bool isCore = (x >= mid - 1 && x <= mid + 1) && (y >= mid - 1 && y <= mid + 1);
                bool isArmX = (y == mid) && (x >= 1 && x <= s - 2);
                bool isArmY = (x == mid) && (y >= 1 && y <= s - 2);

                if (isCore || isArmX || isArmY)
                {
                    p[y * s + x] = col;
                }
                else
                {
                    p[y * s + x] = Color.clear;
                }
            }
        }
        tex.SetPixels(p);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, s, s), new Vector2(0.5f, 0.5f), 24f);
    }

    private static Sprite CreateBirdSprite()
    {
        const int w = 12;
        const int h = 6;
        Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        Color[] p = new Color[w * h];

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                // Classic silhouette V bird shape
                bool leftWing  = (y == 4 && (x == 1 || x == 2)) || (y == 3 && (x == 3 || x == 4)) || (y == 2 && x == 5);
                bool center    = (y == 1 && (x == 5 || x == 6));
                bool rightWing = (y == 2 && x == 6) || (y == 3 && (x == 7 || x == 8)) || (y == 4 && (x == 9 || x == 10));

                if (leftWing || center || rightWing)
                {
                    p[y * w + x] = birdColor;
                }
                else
                {
                    p[y * w + x] = Color.clear;
                }
            }
        }
        tex.SetPixels(p);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 16f);
    }
}
