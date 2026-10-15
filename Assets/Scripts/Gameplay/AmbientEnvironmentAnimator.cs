using UnityEngine;
using System.Collections;

/// <summary>
/// Ambient environment animator for Gold Rush - Green Valley.
/// Spawns and animates floating birds, butterflies, and drifting clouds
/// purely in code — no extra assets required.
/// Attach this to a single empty GameObject in the scene called "AmbientAnimator".
/// </summary>
public class AmbientEnvironmentAnimator : MonoBehaviour
{
    [Header("Birds")]
    [Tooltip("How many birds to keep alive at a time.")]
    public int birdCount = 6;
    [Tooltip("Vertical range above the horizon where birds appear.")]
    public Vector2 birdYRange = new Vector2(4f, 9f);
    public float birdSpeed = 1.8f;

    [Header("Clouds")]
    public int extraCloudCount = 3;
    public Vector2 cloudYRange = new Vector2(5f, 9f);
    public float cloudSpeed = 0.3f;

    [Header("Camera Reference")]
    [Tooltip("Leave empty — auto-detected via Camera.main.")]
    public Transform cameraTransform;

    // Sprite colours used for procedurally-drawn birds
    private static readonly Color birdColour = new Color(0.18f, 0.18f, 0.25f, 1f);
    private static readonly Color cloudColour = new Color(1f, 1f, 1f, 0.88f);

    private Camera cam;
    private float camHalfWidth;

    // Runtime lists
    private GameObject[] birds;
    private float[] birdSpeeds;
    private bool[] birdGoingRight;

    private GameObject[] extraClouds;
    private float[] cloudSpeeds;

    void Start()
    {
        cam = Camera.main;
        if (cam == null) { enabled = false; return; }
        if (cameraTransform == null) cameraTransform = cam.transform;
        camHalfWidth = cam.orthographicSize * cam.aspect;

        SpawnBirds();
        SpawnExtraClouds();
    }

    void Update()
    {
        if (cam == null) return;
        MoveBirds();
        MoveClouds();
    }

    // -------------------------------------------------------------------------
    //  Birds
    // -------------------------------------------------------------------------

    private void SpawnBirds()
    {
        birds = new GameObject[birdCount];
        birdSpeeds = new float[birdCount];
        birdGoingRight = new bool[birdCount];

        for (int i = 0; i < birdCount; i++)
        {
            bool goRight = Random.value > 0.5f;
            birdGoingRight[i] = goRight;
            birdSpeeds[i] = birdSpeed * Random.Range(0.6f, 1.4f);

            float x = cameraTransform.position.x + Random.Range(-camHalfWidth * 2f, camHalfWidth * 2f);
            float y = Random.Range(birdYRange.x, birdYRange.y);

            birds[i] = CreateBirdObject(i, new Vector3(x, y, -8f), goRight);
            StartCoroutine(WingFlapRoutine(birds[i], i));
        }
    }

    private GameObject CreateBirdObject(int index, Vector3 pos, bool facingRight)
    {
        GameObject bird = new GameObject($"AmbientBird_{index}");
        bird.transform.position = pos;
        bird.transform.localScale = new Vector3(facingRight ? 1f : -1f, 1f, 1f);

        // Body (tiny dark sprite made from a SpriteRenderer with a square texture)
        SpriteRenderer body = bird.AddComponent<SpriteRenderer>();
        body.sprite = CreatePixelSprite(birdColour, 6, 3);
        body.sortingOrder = -15;

        // Left wing child
        GameObject wingL = CreateWingChild(bird.transform, "WingL", new Vector3(-0.07f, 0.02f, 0f), true);
        // Right wing child
        GameObject wingR = CreateWingChild(bird.transform, "WingR", new Vector3(0.07f, 0.02f, 0f), false);

        return bird;
    }

    private GameObject CreateWingChild(Transform parent, string name, Vector3 localPos, bool flipX)
    {
        GameObject wing = new GameObject(name);
        wing.transform.SetParent(parent, false);
        wing.transform.localPosition = localPos;
        SpriteRenderer sr = wing.AddComponent<SpriteRenderer>();
        sr.sprite = CreatePixelSprite(birdColour, 5, 2);
        sr.sortingOrder = -15;
        return wing;
    }

    private IEnumerator WingFlapRoutine(GameObject bird, int index)
    {
        while (bird != null)
        {
            if (bird == null) yield break;

            Transform wingL = bird.transform.Find("WingL");
            Transform wingR = bird.transform.Find("WingR");

            float flapDuration = Random.Range(0.12f, 0.22f);
            float elapsed = 0f;

            // Flap up
            while (elapsed < flapDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / flapDuration;
                if (wingL != null) wingL.localPosition = new Vector3(-0.07f, Mathf.Lerp(0.02f, 0.09f, t), 0f);
                if (wingR != null) wingR.localPosition = new Vector3(0.07f, Mathf.Lerp(0.02f, 0.09f, t), 0f);
                yield return null;
            }

            elapsed = 0f;
            // Flap down
            while (elapsed < flapDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / flapDuration;
                if (wingL != null) wingL.localPosition = new Vector3(-0.07f, Mathf.Lerp(0.09f, -0.04f, t), 0f);
                if (wingR != null) wingR.localPosition = new Vector3(0.07f, Mathf.Lerp(0.09f, -0.04f, t), 0f);
                yield return null;
            }

            yield return new WaitForSeconds(Random.Range(0.05f, 0.15f));
        }
    }

    private void MoveBirds()
    {
        float viewBound = camHalfWidth + 5f;

        for (int i = 0; i < birdCount; i++)
        {
            if (birds[i] == null) continue;

            float dir = birdGoingRight[i] ? 1f : -1f;
            birds[i].transform.position += new Vector3(dir * birdSpeeds[i] * Time.deltaTime, 0f, 0f);

            // Recycle when off-screen
            float relX = birds[i].transform.position.x - cameraTransform.position.x;
            if (Mathf.Abs(relX) > viewBound)
            {
                birdGoingRight[i] = !birdGoingRight[i];
                float newX = cameraTransform.position.x + (birdGoingRight[i] ? -viewBound : viewBound);
                float newY = Random.Range(birdYRange.x, birdYRange.y);
                birds[i].transform.position = new Vector3(newX, newY, -8f);
                birds[i].transform.localScale = new Vector3(birdGoingRight[i] ? 1f : -1f, 1f, 1f);
            }
        }
    }

    // -------------------------------------------------------------------------
    //  Extra Clouds (procedural, supplement the scene's existing Cloud_Decor)
    // -------------------------------------------------------------------------

    private void SpawnExtraClouds()
    {
        extraClouds = new GameObject[extraCloudCount];
        cloudSpeeds = new float[extraCloudCount];

        for (int i = 0; i < extraCloudCount; i++)
        {
            cloudSpeeds[i] = cloudSpeed * Random.Range(0.5f, 1.5f);
            float x = cameraTransform.position.x + Random.Range(-camHalfWidth * 2f, camHalfWidth * 2f);
            float y = Random.Range(cloudYRange.x, cloudYRange.y);

            extraClouds[i] = CreateCloudObject(i, new Vector3(x, y, -6f));
        }
    }

    private GameObject CreateCloudObject(int index, Vector3 pos)
    {
        GameObject cloud = new GameObject($"AmbientCloud_{index}");
        cloud.transform.position = pos;

        float scale = Random.Range(0.8f, 2.0f);
        cloud.transform.localScale = new Vector3(scale, scale * 0.55f, 1f);

        SpriteRenderer sr = cloud.AddComponent<SpriteRenderer>();
        sr.sprite = CreateCloudSprite();
        sr.color = cloudColour;
        sr.sortingOrder = -12;

        return cloud;
    }

    private void MoveClouds()
    {
        float viewBound = camHalfWidth + 8f;

        for (int i = 0; i < extraCloudCount; i++)
        {
            if (extraClouds[i] == null) continue;

            // Clouds drift slowly to the right
            extraClouds[i].transform.position += new Vector3(cloudSpeeds[i] * Time.deltaTime, 0f, 0f);

            float relX = extraClouds[i].transform.position.x - cameraTransform.position.x;
            if (relX > viewBound)
            {
                // Wrap to the left side
                float newX = cameraTransform.position.x - viewBound + Random.Range(-1f, 1f);
                float newY = Random.Range(cloudYRange.x, cloudYRange.y);
                extraClouds[i].transform.position = new Vector3(newX, newY, -6f);
            }
        }
    }

    // -------------------------------------------------------------------------
    //  Sprite factory helpers
    // -------------------------------------------------------------------------

    /// <summary>Creates a flat single-colour sprite from a tiny texture.</summary>
    private static Sprite CreatePixelSprite(Color col, int w, int h)
    {
        Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        Color[] pixels = new Color[w * h];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = col;
        tex.SetPixels(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 32f);
    }

    /// <summary>Creates a rounded cloud-shaped sprite procedurally.</summary>
    private static Sprite CreateCloudSprite()
    {
        int w = 64, h = 32;
        Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        Color[] pixels = new Color[w * h];

        // Simple circle union for cloud shape
        Vector2[] centres = { new Vector2(16, 14), new Vector2(28, 18), new Vector2(42, 14), new Vector2(52, 12) };
        float[] radii    = { 12f, 15f, 12f, 10f };

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                bool inside = false;
                for (int c = 0; c < centres.Length; c++)
                {
                    float d = Vector2.Distance(new Vector2(x, y), centres[c]);
                    if (d < radii[c]) { inside = true; break; }
                }
                float alpha = inside ? 1f : 0f;
                pixels[y * w + x] = new Color(1f, 1f, 1f, alpha);
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 16f);
    }
}
