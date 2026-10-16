using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Reflection;
using System.Linq;

/// <summary>
/// Editor tool for Gold Rush.
/// Menu: Gold Rush → Setup Parallax Background (Level 1)
///       Gold Rush → Setup Crystal Cave Background (Level 2)
///       Gold Rush → Fix Player Collider Alignment
/// </summary>
public class GoldRushParallaxSetup : EditorWindow
{
    // ── Menu items ────────────────────────────────────────────────────────────

    [MenuItem("Gold Rush/Setup Parallax Background (Level 1)")]
    public static void ShowWindow()
    {
        GetWindow<GoldRushParallaxSetup>("Parallax Setup");
    }

    [MenuItem("Gold Rush/Setup Crystal Cave Background (Level 2)")]
    public static void SetupLevel2()
    {
        AddSortingLayers();
        BuildCrystalCaveBackground();
        FixPlayerColliderAlignment();
        FixExistingObjectSortingLayersLevel2();
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);
        Debug.Log("[GoldRush] Crystal Cave background built and scene saved.");
        EditorUtility.DisplayDialog("Gold Rush – Level 2",
            "✅ Crystal Cave background setup complete!\n\nScene saved automatically.\nPress Play ▶️ to test.", "OK");
    }

    [MenuItem("Gold Rush/Setup Sky Temple Background (Level 3)")]
    public static void SetupLevel3()
    {
        AddSortingLayers();
        BuildSkyTempleBackground();
        FixPlayerColliderAlignment();
        FixExistingObjectSortingLayersLevel3();
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);
        Debug.Log("[GoldRush] Sky Temple background built and scene saved.");
        EditorUtility.DisplayDialog("Gold Rush – Level 3",
            "✅ Sky Temple background setup complete!\n\nScene saved automatically.\nPress Play ▶️ to test.", "OK");
    }

    [MenuItem("Gold Rush/Create Game Prefabs (Coins, Enemies, Hazards)")]
    public static void CreateGamePrefabs()
    {
        CreateStandardPrefabs();
        EditorUtility.DisplayDialog("Gold Rush – Prefabs",
            "✅ Prefabs successfully created in Assets/Prefabs/ !\n\n- GoldCoin.prefab\n- Enemy_Slime.prefab\n- SpikeTrap.prefab\n\nYou can now drag and drop them anywhere in your scenes.", "OK");
    }

    [MenuItem("Gold Rush/Fix Player Collider Alignment")]
    public static void FixPlayerColliderAlignment()
    {
        GameObject player = GameObject.Find("Player");
        if (player == null)
        {
            Debug.LogWarning("[GoldRush] Player not found in scene.");
            return;
        }

        UnityEngine.CharacterController cc = player.GetComponent<UnityEngine.CharacterController>();
        if (cc == null)
        {
            Debug.LogWarning("[GoldRush] Player has no CharacterController; collider alignment was not changed.");
            return;
        }

        Undo.RecordObject(cc, "Align Player Collider");
        cc.center = Vector3.zero;
        cc.height = 0.78f;
        cc.radius = 0.2f;
        cc.skinWidth = 0.02f;
        cc.stepOffset = 0.15f;
        EditorUtility.SetDirty(cc);

        Transform visual = player.transform.Find("Visual_Sprite");
        if (visual != null)
        {
            Undo.RecordObject(visual, "Center Player Sprite");
            visual.localPosition = new Vector3(0f, -0.08f, 0f);
            EditorUtility.SetDirty(visual);
        }

        Debug.Log("[GoldRush] Player collider aligned to the sprite; player position and gravity were left unchanged.");
        EditorUtility.SetDirty(player);
    }

    // ── EditorWindow GUI (Level 1) ────────────────────────────────────────────

    void OnGUI()
    {
        GUILayout.Label("Gold Rush – Background Setup", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Open the target scene before clicking a button.\n" +
            "Level 1: Level_01_GreenValley\n" +
            "Level 2: Level_02_CrystalCave",
            MessageType.Info);

        EditorGUILayout.Space(6);
        GUILayout.Label("── Level 1: Green Valley ──", EditorStyles.miniLabel);

        if (GUILayout.Button("➕ Add Sorting Layers", GUILayout.Height(28)))
            AddSortingLayers();

        if (GUILayout.Button("🌄 Build Parallax Background (Level 1)", GUILayout.Height(36)))
        {
            AddSortingLayers();
            BuildParallaxBackground();
        }

        EditorGUILayout.Space(6);
        GUILayout.Label("── Level 2: Crystal Cave ──", EditorStyles.miniLabel);

        if (GUILayout.Button("💎 Build Crystal Cave Background (Level 2)", GUILayout.Height(36)))
        {
            AddSortingLayers();
            BuildCrystalCaveBackground();
        }

        EditorGUILayout.Space(6);
        GUILayout.Label("── Level 3: Sky Temple ──", EditorStyles.miniLabel);

        if (GUILayout.Button("⛩️ Build Sky Temple Background (Level 3)", GUILayout.Height(36)))
        {
            AddSortingLayers();
            BuildSkyTempleBackground();
        }

        EditorGUILayout.Space(6);
        GUILayout.Label("── Shared ──", EditorStyles.miniLabel);

        if (GUILayout.Button("🧍 Fix Player Collider Alignment", GUILayout.Height(28)))
            FixPlayerColliderAlignment();

        EditorGUILayout.Space(6);

        if (GUILayout.Button("✅ Run All + Save Scene (Level 1 — Recommended)", GUILayout.Height(48)))
        {
            AddSortingLayers();
            BuildParallaxBackground();
            FixPlayerColliderAlignment();
            FixExistingObjectSortingLayersLevel1();
            SaveAndNotify("Level 1 – Green Valley");
        }

        if (GUILayout.Button("✅ Run All + Save Scene (Level 2 — Recommended)", GUILayout.Height(48)))
        {
            AddSortingLayers();
            BuildCrystalCaveBackground();
            FixPlayerColliderAlignment();
            FixExistingObjectSortingLayersLevel2();
            SaveAndNotify("Level 2 – Crystal Cave");
        }

        if (GUILayout.Button("✅ Run All + Save Scene (Level 3 — Recommended)", GUILayout.Height(48)))
        {
            AddSortingLayers();
            BuildSkyTempleBackground();
            FixPlayerColliderAlignment();
            FixExistingObjectSortingLayersLevel3();
            SaveAndNotify("Level 3 – Sky Temple");
        }

        EditorGUILayout.Space(6);
        GUILayout.Label("── Prefabs ──", EditorStyles.miniLabel);

        if (GUILayout.Button("📦 Create Game Prefabs (Coins, Slime, Spikes)", GUILayout.Height(36)))
        {
            CreateGamePrefabs();
        }
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static void SaveAndNotify(string levelName)
    {
        if (!Application.isPlaying)
        {
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);
            Debug.Log($"[GoldRush] {levelName} setup complete. Scene saved.");
        }
        EditorUtility.DisplayDialog("Gold Rush Setup",
            $"✅ {levelName} setup complete!\n\nScene saved. Press Play ▶️ to test.", "OK");
    }

    // ── Sorting Layers ────────────────────────────────────────────────────────

    private static void AddSortingLayers()
    {
        string[] desired = { "Background", "Midground", "Foreground", "Default", "UI" };
        SerializedObject tagManager = new SerializedObject(
            AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty sortingLayers = tagManager.FindProperty("m_SortingLayers");

        foreach (string layerName in desired)
        {
            bool exists = false;
            for (int i = 0; i < sortingLayers.arraySize; i++)
            {
                if (sortingLayers.GetArrayElementAtIndex(i)
                    .FindPropertyRelative("name").stringValue == layerName)
                { exists = true; break; }
            }
            if (!exists)
            {
                sortingLayers.InsertArrayElementAtIndex(sortingLayers.arraySize);
                var newLayer = sortingLayers.GetArrayElementAtIndex(sortingLayers.arraySize - 1);
                newLayer.FindPropertyRelative("name").stringValue = layerName;
                newLayer.FindPropertyRelative("uniqueID").intValue = UnityEngine.Random.Range(100000, 999999);
                Debug.Log($"[GoldRush] Sorting layer added: {layerName}");
            }
        }
        tagManager.ApplyModifiedProperties();
    }

    // ── Fix existing objects – Level 1 ────────────────────────────────────────

    private static void FixExistingObjectSortingLayersLevel1()
    {
        SetGroundTilesSortingLayer("Foreground", 0);
        foreach (SpriteRenderer sr in
            GameObject.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None))
        {
            if (sr.gameObject.name.StartsWith("Cloud_Decor"))
            {
                Undo.RecordObject(sr, "Fix Cloud Sorting Layer");
                sr.sortingLayerName = "Midground";
                sr.sortingOrder = 5;
                EditorUtility.SetDirty(sr);
            }
        }
        Debug.Log("[GoldRush] Level 1 object sorting layers fixed.");
    }

    // ── Fix existing objects – Level 2 ────────────────────────────────────────

    private static void FixExistingObjectSortingLayersLevel2()
    {
        SetGroundTilesSortingLayer("Foreground", 0);
        Debug.Log("[GoldRush] Level 2 object sorting layers fixed.");
    }

    // ── Fix existing objects – Level 3 ────────────────────────────────────────

    private static void FixExistingObjectSortingLayersLevel3()
    {
        SetGroundTilesSortingLayer("Foreground", 0);

        // Also fix SkyIsland platforms in Level 3
        foreach (GameObject go in GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        {
            if (go.name.StartsWith("SkyIsland"))
            {
                SpriteRenderer sr = go.GetComponentInChildren<SpriteRenderer>() ?? go.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    Undo.RecordObject(sr, "Fix SkyIsland Sorting Layer");
                    sr.sortingLayerName = "Foreground";
                    sr.sortingOrder = 0;
                    EditorUtility.SetDirty(sr);
                }
            }
        }
        Debug.Log("[GoldRush] Level 3 object sorting layers fixed.");
    }

    private static void SetGroundTilesSortingLayer(string layerName, int order)
    {
        for (int i = 0; i < 12; i++)
        {
            GameObject go = GameObject.Find($"Ground_Tile_{i}");
            if (go == null) continue;
            SpriteRenderer sr = go.GetComponentInChildren<SpriteRenderer>()
                             ?? go.GetComponent<SpriteRenderer>();
            if (sr == null) continue;
            Undo.RecordObject(sr, "Fix Ground Sorting Layer");
            sr.sortingLayerName = layerName;
            sr.sortingOrder = order;
            EditorUtility.SetDirty(sr);
        }
    }

    // =========================================================================
    //  LEVEL 1 – Green Valley parallax background
    // =========================================================================

    private static void BuildParallaxBackground()
    {
        // 1. Destroy any existing backgrounds from other levels in this scene
        DestroyRoot("CrystalCave_BG_Root");
        DestroyRoot("SkyTemple_BG_Root");
        DestroyRoot("ParallaxBackground_Root");

        GameObject root = CreateRoot("ParallaxBackground_Root");

        // Camera is at Z=-10, looking at +Z
        // Visible Z is -9.7 to 20
        var layers = new (string path, string sortLayer, int sortOrder,
                          float parallax, Vector3 pos, float sx, float sy)[]
        {
            // 1. Complete Sky & Mountain Panorama:
            // Perfectly seamless, bright blue sky with layered green mountains and sunny clouds
            ("Assets/Sprites/Background/BG_GreenValley_SkyMountains.png",
             "Background", -30, 0.05f,
             new Vector3(0f, 2.2f, 5f), 2.7f, 2.4f),

            // 2. Foreground Forest & Flowered Hills sitting right behind the platforms
            ("Assets/Sprites/Background/BG_Forest.png",
             "Midground", -10, 0.22f,
             new Vector3(0f, -1.0f, 3.5f), 2.8f, 2.3f),
        };

        foreach (var l in layers)
            CreateParallaxLayer(root, l.path, l.sortLayer, l.sortOrder,
                                l.parallax, l.pos, l.sx, l.sy);

        // Ambient birds + procedural clouds
        var ambient = new GameObject("AmbientAnimator");
        ambient.transform.SetParent(root.transform, false);
        ambient.AddComponent<AmbientEnvironmentAnimator>();
        EditorUtility.SetDirty(ambient);

        MarkDirty(root);
        Debug.Log("[GoldRush] Level 1 parallax background built.");
    }

    // =========================================================================
    //  LEVEL 2 – Crystal Cave background
    // =========================================================================

    private static void BuildCrystalCaveBackground()
    {
        DestroyRoot("CrystalCave_BG_Root");
        GameObject root = CreateRoot("CrystalCave_BG_Root");

        // Layer definitions
        // All at Z > -10 so the orthographic camera (Z=-10) can see them.
        // Sorting layers ensure correct draw order regardless of Z.
        var layers = new (string path, string sortLayer, int sortOrder,
                          float parallax, Vector3 pos, float sx, float sy)[]
        {
            // 1. Dark cave background — static, fills entire view
            ("Assets/Sprites/Background/CrystalCave/Cave_Deep.png",
             "Background", -30, 0.0f,
             new Vector3(0f, 2f, 6f), 2.0f, 2.0f),

            // 2. Stalactites ceiling — very slow parallax
            ("Assets/Sprites/Background/CrystalCave/Cave_Stalactites.png",
             "Background", -20, 0.04f,
             new Vector3(0f, 6.5f, 5f), 2.4f, 1.8f),

            // 3. Crystal ground formations — medium parallax
            ("Assets/Sprites/Background/CrystalCave/Cave_Crystals.png",
             "Midground", -10, 0.18f,
             new Vector3(0f, -2.0f, 4f), 2.6f, 2.0f),
        };

        SpriteRenderer crystalRenderer = null;

        foreach (var l in layers)
        {
            var sr = CreateParallaxLayer(root, l.path, l.sortLayer, l.sortOrder,
                                         l.parallax, l.pos, l.sx, l.sy);
            // Keep a reference to the crystal layer for the pulse effect
            if (l.path.Contains("Cave_Crystals") && sr != null)
                crystalRenderer = sr;
        }

        // Ambient cave animator (floating particles + crystal pulse)
        var ambient = new GameObject("CaveAmbientAnimator");
        ambient.transform.SetParent(root.transform, false);
        CaveAmbientAnimator caveAnim = ambient.AddComponent<CaveAmbientAnimator>();
        if (crystalRenderer != null)
            caveAnim.crystalLayerRenderer = crystalRenderer;
        EditorUtility.SetDirty(ambient);

        MarkDirty(root);
        Debug.Log("[GoldRush] Crystal Cave background built.");
    }

    // =========================================================================
    //  LEVEL 3 – Sky Temple background
    // =========================================================================

    private static void BuildSkyTempleBackground()
    {
        DestroyRoot("SkyTemple_BG_Root");
        GameObject root = CreateRoot("SkyTemple_BG_Root");

        // Camera at Z=-10, looking at +Z
        // Visible Z is -9.7 to 20
        //
        // 5 distinct depth layers:
        // 1. Sky_Base: pure light-blue gradient sky, static (parallax = 0.0)
        // 2. Sky_Mountains: distant misty mountains above clouds, very slow (parallax = 0.04)
        // 3. Sky_FloatingIslands: distant floating grassy islands, slow (parallax = 0.12)
        // 4. Sky_TempleRuins: ancient temple ruins with golden pillars and crystals (parallax = 0.22)
        var layers = new (string path, string sortLayer, int sortOrder,
                          float parallax, Vector3 pos, float sx, float sy)[]
        {
            // Layer 1: Sky base
            ("Assets/Sprites/Background/SkyTemple/Sky_Base.png",
             "Background", -30, 0.0f,
             new Vector3(0f, 3.5f, 6f), 2.6f, 2.4f),

            // Layer 2: Distant mountain peaks and sea of clouds
            ("Assets/Sprites/Background/SkyTemple/Sky_Mountains.png",
             "Background", -20, 0.04f,
             new Vector3(0f, 0.8f, 5f), 2.6f, 2.2f),

            // Layer 3: Distant floating islands
            ("Assets/Sprites/Background/SkyTemple/Sky_FloatingIslands.png",
             "Midground", -12, 0.12f,
             new Vector3(0f, 1.2f, 4f), 2.6f, 2.2f),

            // Layer 4: Ancient Temple Ruins with golden pillars and sacred crystal
            ("Assets/Sprites/Background/SkyTemple/Sky_TempleRuins.png",
             "Midground", -6, 0.22f,
             new Vector3(0f, 1.8f, 3f), 2.5f, 2.2f),
        };

        foreach (var l in layers)
        {
            CreateParallaxLayer(root, l.path, l.sortLayer, l.sortOrder,
                                l.parallax, l.pos, l.sx, l.sy);
        }

        // Layer 5 & Details: Ambient birds & magical sparkles
        var ambient = new GameObject("SkyTempleAmbientAnimator");
        ambient.transform.SetParent(root.transform, false);
        ambient.AddComponent<SkyTempleAmbientAnimator>();
        EditorUtility.SetDirty(ambient);

        // Also update Main Camera background clear color to sky blue instead of plain orange
        Camera cam = Camera.main;
        if (cam != null)
        {
            Undo.RecordObject(cam, "Set Sky Camera Background");
            cam.backgroundColor = new Color(0.51f, 0.85f, 0.96f, 1f); // #82D9F5
            EditorUtility.SetDirty(cam);
        }

        MarkDirty(root);
        Debug.Log("[GoldRush] Sky Temple background built successfully.");
    }

    // ── Shared helpers ────────────────────────────────────────────────────────

    /// <summary>Creates one parallax background layer and returns its SpriteRenderer.</summary>
    private static SpriteRenderer CreateParallaxLayer(
        GameObject root, string spritePath,
        string sortLayer, int sortOrder,
        float parallax, Vector3 pos, float sx, float sy)
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
        if (sprite == null)
        {
            Debug.LogWarning($"[GoldRush] Sprite not found: {spritePath} — layer skipped.");
            return null;
        }

        string goName = System.IO.Path.GetFileNameWithoutExtension(spritePath);
        GameObject go = new GameObject(goName);
        go.transform.SetParent(root.transform, false);
        go.transform.position = pos;
        go.transform.localScale = new Vector3(sx, sy, 1f);

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingLayerName = sortLayer;
        sr.sortingOrder = sortOrder;
        sr.drawMode = SpriteDrawMode.Simple;

        ParallaxBackground pb = go.AddComponent<ParallaxBackground>();
        pb.parallaxFactor = parallax;
        pb.infiniteScrollX = parallax > 0f;

        EditorUtility.SetDirty(go);
        Debug.Log($"[GoldRush] Layer created: {goName} | layer={sortLayer} | parallax={parallax}");
        return sr;
    }

    private static void DestroyRoot(string name)
    {
        foreach (GameObject go in GameObject.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (go.name == name || go.name.StartsWith(name))
            {
                Undo.DestroyObjectImmediate(go);
            }
        }
    }

    private static GameObject CreateRoot(string name)
    {
        var root = new GameObject(name);
        Undo.RegisterCreatedObjectUndo(root, $"Create {name}");
        return root;
    }

    private static void MarkDirty(GameObject go)
    {
        EditorUtility.SetDirty(go);
        if (!Application.isPlaying)
        {
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        }
    }

    // =========================================================================
    //  PREFAB GENERATOR
    // =========================================================================

    private static void CreateStandardPrefabs()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }

        // 1. GoldCoin Prefab
        CreateGoldCoinPrefab();

        // 2. Enemy_Slime Prefab
        CreateSlimeEnemyPrefab();

        // 3. SpikeTrap Prefab
        CreateSpikeTrapPrefab();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[GoldRush] All standard prefabs created in Assets/Prefabs/");
    }

    private static void CreateGoldCoinPrefab()
    {
        string path = "Assets/Prefabs/GoldCoin.prefab";

        GameObject coinRoot = new GameObject("GoldCoin");
        SphereCollider sc = coinRoot.AddComponent<SphereCollider>();
        sc.isTrigger = true;
        sc.radius = 0.45f;

        Coin coin = coinRoot.AddComponent<Coin>();
        coin.coinValue = 10;
        coin.scorePoints = 100;

        GameObject visual = new GameObject("Visual");
        visual.transform.SetParent(coinRoot.transform, false);
        visual.transform.localScale = new Vector3(0.18f, 0.18f, 1f);

        SpriteRenderer sr = visual.AddComponent<SpriteRenderer>();
        sr.sortingLayerName = "Foreground";
        sr.sortingOrder = 2;

        Sprite coinSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Collectibles/Coin_Idle.png");
        if (coinSprite != null) sr.sprite = coinSprite;

        CoinSpriteAnimator anim = visual.AddComponent<CoinSpriteAnimator>();
        anim.spinFrames = new Sprite[]
        {
            AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Collectibles/Coin_Spin_0.png") ?? coinSprite,
            AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Collectibles/Coin_Spin_1.png") ?? coinSprite,
            AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Collectibles/Coin_Spin_2.png") ?? coinSprite,
            AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Collectibles/Coin_Spin_3.png") ?? coinSprite
        };
        anim.frameRate = 9f;
        anim.bobAmplitude = 0.08f;
        anim.bobSpeed = 2.5f;

        PrefabUtility.SaveAsPrefabAsset(coinRoot, path);
        DestroyImmediate(coinRoot);
        Debug.Log($"[GoldRush] Created prefab: {path}");
    }

    private static void CreateSlimeEnemyPrefab()
    {
        string path = "Assets/Prefabs/Enemy_Slime.prefab";

        GameObject enemyRoot = new GameObject("Enemy_Slime");
        BoxCollider bc = enemyRoot.AddComponent<BoxCollider>();
        bc.isTrigger = true;
        bc.size = new Vector3(1.2f, 0.85f, 1.5f);

        SimpleEnemy enemy = enemyRoot.AddComponent<SimpleEnemy>();
        enemy.moveSpeed = 2.4f;
        enemy.patrolDistance = 3.5f;
        enemy.stompScorePoints = 200;
        enemy.is2DSprite = true;
        enemy.direction = 1;

        GameObject visual = new GameObject("Visual");
        visual.transform.SetParent(enemyRoot.transform, false);
        visual.transform.localScale = new Vector3(0.24f, 0.24f, 1f);

        SpriteRenderer sr = visual.AddComponent<SpriteRenderer>();
        sr.sortingLayerName = "Foreground";
        sr.sortingOrder = 1;

        Sprite slimeSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Characters/Enemy_Slime_Walk1.png")
                          ?? AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Characters/Enemy_Classic_Run1.png");
        if (slimeSprite != null) sr.sprite = slimeSprite;

        PrefabUtility.SaveAsPrefabAsset(enemyRoot, path);
        DestroyImmediate(enemyRoot);
        Debug.Log($"[GoldRush] Created prefab: {path}");
    }

    private static void CreateSpikeTrapPrefab()
    {
        string path = "Assets/Prefabs/SpikeTrap.prefab";

        GameObject spikeRoot = new GameObject("SpikeTrap");
        BoxCollider bc = spikeRoot.AddComponent<BoxCollider>();
        bc.isTrigger = true;
        bc.size = new Vector3(1.5f, 0.6f, 1f);

        spikeRoot.AddComponent<SpikeHazard>();

        GameObject visual = new GameObject("Visual");
        visual.transform.SetParent(spikeRoot.transform, false);
        visual.transform.localScale = new Vector3(0.35f, 0.35f, 1f);

        SpriteRenderer sr = visual.AddComponent<SpriteRenderer>();
        sr.sortingLayerName = "Foreground";
        sr.sortingOrder = 0;

        Sprite spikeSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Environment/Spikes.png")
                          ?? AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Environment/Platform_Spikes.png");
        if (spikeSprite != null) sr.sprite = spikeSprite;

        PrefabUtility.SaveAsPrefabAsset(spikeRoot, path);
        DestroyImmediate(spikeRoot);
        Debug.Log($"[GoldRush] Created prefab: {path}");
    }
}
