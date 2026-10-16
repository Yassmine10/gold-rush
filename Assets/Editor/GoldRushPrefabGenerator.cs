using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Générateur et gestionnaire officiel des Prefabs pour Coin Collector / Gold Rush.
/// Crée et enregistre tous les composants modulaires du jeu sous forme d'assets .prefab réutilisables.
/// </summary>
public static class GoldRushPrefabGenerator
{
    public const string ROOT_DIR = "Assets/Prefabs";
    public const string CHAR_DIR = ROOT_DIR + "/Characters";
    public const string COIN_DIR = ROOT_DIR + "/Collectibles";
    public const string ENEMY_DIR = ROOT_DIR + "/Enemies";
    public const string ENV_DIR = ROOT_DIR + "/Environment";
    public const string HAZARD_DIR = ROOT_DIR + "/Hazards";
    public const string EXIT_DIR = ROOT_DIR + "/Exits";
    public const string MGR_DIR = ROOT_DIR + "/Managers";

    [MenuItem("Gold Rush/🏗️ 8. Générer Tous les Prefabs (.prefab)", false, 18)]
    public static void GenerateAllPrefabs()
    {
        EnsureFolders();

        CreatePlayerPrefab();
        CreateMenuPlayerPrefab();
        CreateCoinPrefabs();
        CreateEnemyPrefabs();
        CreateHazardPrefabs();
        CreatePlatformPrefabs();
        CreateExitPrefabs();
        CreateCloudPrefab();
        CreateManagersPrefab();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("🎉 [GoldRush] Tous les Prefabs modulaires ont été générés avec succès dans Assets/Prefabs/ !");
    }

    private static void EnsureFolders()
    {
        string[] dirs = new string[] { ROOT_DIR, CHAR_DIR, COIN_DIR, ENEMY_DIR, ENV_DIR, HAZARD_DIR, EXIT_DIR, MGR_DIR };
        foreach (string d in dirs)
        {
            if (!Directory.Exists(d)) Directory.CreateDirectory(d);
        }
    }

    private static Sprite LoadSprite(string path)
    {
        Sprite sp = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (sp == null)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null && importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.SaveAndReimport();
                sp = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            }
        }
        return sp;
    }

    // 1. JOUEUR & MENU
    public static void CreatePlayerPrefab()
    {
        GameObject root = new GameObject("Player");

        CharacterController cc = root.AddComponent<CharacterController>();
        cc.center = Vector3.zero;
        cc.height = 0.78f;
        cc.radius = 0.2f;
        cc.skinWidth = 0.02f;
        cc.stepOffset = 0.15f;
        cc.minMoveDistance = 0f;

        PlayerController25D ctrl = root.AddComponent<PlayerController25D>();
        ctrl.moveSpeed = 8f;
        ctrl.jumpForce = 13.5f;
        ctrl.is2DSprite = true;

        PlayerHealth health = root.AddComponent<PlayerHealth>();
        health.maxHealth = 3;
        health.currentHealth = 3;

        GameObject visual = new GameObject("Visual_Sprite");
        visual.transform.SetParent(root.transform, false);
        visual.transform.localPosition = new Vector3(0, -0.08f, 0);
        visual.transform.localScale = new Vector3(0.38f, 0.38f, 1f);

        Sprite idle = LoadSprite("Assets/Sprites/Characters/Player_Classic_Idle.png");
        Sprite run1 = LoadSprite("Assets/Sprites/Characters/Player_Classic_Run1.png");
        Sprite run2 = LoadSprite("Assets/Sprites/Characters/Player_Classic_Run2.png");
        Sprite jump = LoadSprite("Assets/Sprites/Characters/Player_Classic_Jump.png");
        Sprite hurt = LoadSprite("Assets/Sprites/Characters/Player_Classic_Hurt.png");

        SpriteRenderer sr = visual.AddComponent<SpriteRenderer>();
        sr.sprite = idle;
        sr.sortingOrder = 10;

        CharacterSpriteAnimator anim = visual.AddComponent<CharacterSpriteAnimator>();
        anim.scaleMultiplier = 1.20f;
        anim.idleSprite = idle;
        anim.run1Sprite = run1;
        anim.run2Sprite = run2;
        anim.jumpSprite = jump;
        anim.hurtSprite = hurt;
        anim.skinAdventurer = LoadSprite("Assets/Sprites/Characters/Player_Adventurer_Idle.png");
        anim.skinNinja = LoadSprite("Assets/Sprites/Characters/Player_Ninja_Idle.png");
        anim.skinMage = LoadSprite("Assets/Sprites/Characters/Player_Mage_Idle.png");
        anim.skinKing = LoadSprite("Assets/Sprites/Characters/Player_King_Idle.png");

        SaveAndDestroy(root, CHAR_DIR + "/Player.prefab");
    }

    public static void CreateMenuPlayerPrefab()
    {
        GameObject root = new GameObject("MenuPlayerCharacter");

        FloatingCharacter floatC = root.AddComponent<FloatingCharacter>();
        floatC.verticalAmplitude = 0.08f;
        floatC.verticalSpeed = 2.5f;

        GameObject visual = new GameObject("Visual_Sprite");
        visual.transform.SetParent(root.transform, false);
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localScale = new Vector3(0.38f, 0.38f, 1f);

        Sprite idle = LoadSprite("Assets/Sprites/Characters/Player_Classic_Idle.png");
        SpriteRenderer sr = visual.AddComponent<SpriteRenderer>();
        sr.sprite = idle;
        sr.sortingOrder = 10;

        CharacterSpriteAnimator anim = visual.AddComponent<CharacterSpriteAnimator>();
        anim.scaleMultiplier = 1.20f;
        anim.idleSprite = idle;
        anim.skinAdventurer = LoadSprite("Assets/Sprites/Characters/Player_Adventurer_Idle.png");
        anim.skinNinja = LoadSprite("Assets/Sprites/Characters/Player_Ninja_Idle.png");
        anim.skinMage = LoadSprite("Assets/Sprites/Characters/Player_Mage_Idle.png");
        anim.skinKing = LoadSprite("Assets/Sprites/Characters/Player_King_Idle.png");

        SaveAndDestroy(root, CHAR_DIR + "/MenuPlayerCharacter.prefab");
    }

    // 2. PIÈCES & COLLECTIBLES
    public static void CreateCoinPrefabs()
    {
        // GoldCoin de gameplay
        GameObject coin = new GameObject("GoldCoin");

        SphereCollider col = coin.AddComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = 0.45f;

        coin.AddComponent<Coin>();

        GameObject visual = new GameObject("Visual");
        visual.transform.SetParent(coin.transform, false);
        visual.transform.localScale = new Vector3(0.18f, 0.18f, 1f);

        SpriteRenderer sr = visual.AddComponent<SpriteRenderer>();
        Sprite s0 = LoadSprite("Assets/Sprites/Items/Coin_Frame0.png");
        Sprite s1 = LoadSprite("Assets/Sprites/Items/Coin_Frame1.png");
        Sprite s2 = LoadSprite("Assets/Sprites/Items/Coin_Frame2.png");
        Sprite s3 = LoadSprite("Assets/Sprites/Items/Coin_Frame3.png");
        sr.sprite = s0;
        sr.sortingOrder = 5;

        CoinSpriteAnimator anim = visual.AddComponent<CoinSpriteAnimator>();
        anim.spinFrames = new Sprite[] { s0, s1, s2, s3 };
        anim.frameRate = 9f;
        anim.bobAmplitude = 0.08f;
        anim.bobSpeed = 2.5f;

        SaveAndDestroy(coin, COIN_DIR + "/GoldCoin.prefab");

        // MenuSpinningCoin de décor
        GameObject menuCoin = new GameObject("MenuSpinningCoin");
        GameObject mVisual = new GameObject("Visual");
        mVisual.transform.SetParent(menuCoin.transform, false);
        mVisual.transform.localScale = new Vector3(0.16f, 0.16f, 1f);

        SpriteRenderer msr = mVisual.AddComponent<SpriteRenderer>();
        msr.sprite = s0;
        msr.sortingOrder = 3;

        CoinSpriteAnimator mAnim = mVisual.AddComponent<CoinSpriteAnimator>();
        mAnim.spinFrames = new Sprite[] { s0, s1, s2, s3 };
        mAnim.frameRate = 8f;
        mAnim.bobAmplitude = 0.12f;
        mAnim.bobSpeed = 2.0f;

        SaveAndDestroy(menuCoin, COIN_DIR + "/MenuSpinningCoin.prefab");
    }

    // 3. ENNEMIS (Slimes & Bat)
    public static void CreateEnemyPrefabs()
    {
        CreateSlimePrefab("Enemy_Slime_Green", "Assets/Sprites/Enemies/Slime_Green_Idle.png", 2.4f);
        CreateSlimePrefab("Enemy_Slime_Purple", "Assets/Sprites/Enemies/Slime_Purple_Idle.png", 2.6f);
        CreateSlimePrefab("Enemy_Slime_Fire", "Assets/Sprites/Enemies/Slime_Fire_Idle.png", 2.8f);

        // Bat
        GameObject bat = new GameObject("Enemy_Bat");
        BoxCollider col = bat.AddComponent<BoxCollider>();
        col.isTrigger = true;
        col.size = new Vector3(1.2f, 0.8f, 1.5f);
        col.center = Vector3.zero;

        SimpleEnemy se = bat.AddComponent<SimpleEnemy>();
        se.patrolDistance = 3.0f;
        se.moveSpeed = 3.0f;
        se.is2DSprite = true;

        GameObject visual = new GameObject("Visual");
        visual.transform.SetParent(bat.transform, false);
        visual.transform.localScale = new Vector3(0.24f, 0.24f, 1f);

        SpriteRenderer sr = visual.AddComponent<SpriteRenderer>();
        Sprite f1 = LoadSprite("Assets/Sprites/Enemies/Bat_Fly1.png");
        Sprite f2 = LoadSprite("Assets/Sprites/Enemies/Bat_Fly2.png");
        sr.sprite = f1;
        sr.sortingOrder = 6;

        BatSpriteAnimator anim = visual.AddComponent<BatSpriteAnimator>();
        anim.fly1Sprite = f1;
        anim.fly2Sprite = f2;
        anim.flapRate = 6f;
        anim.waveAmplitude = 0.35f;

        SaveAndDestroy(bat, ENEMY_DIR + "/Enemy_Bat.prefab");
    }

    private static void CreateSlimePrefab(string name, string spritePath, float speed)
    {
        GameObject enemy = new GameObject(name);
        BoxCollider col = enemy.AddComponent<BoxCollider>();
        col.isTrigger = true;
        col.size = new Vector3(1.2f, 0.85f, 1.5f);
        col.center = Vector3.zero;

        SimpleEnemy se = enemy.AddComponent<SimpleEnemy>();
        se.patrolDistance = 3.0f;
        se.moveSpeed = speed;
        se.is2DSprite = true;

        GameObject visual = new GameObject("Visual");
        visual.transform.SetParent(enemy.transform, false);
        visual.transform.localScale = new Vector3(0.24f, 0.24f, 1f);

        SpriteRenderer sr = visual.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 6;
        Sprite idle = LoadSprite(spritePath);
        Sprite squash = LoadSprite("Assets/Sprites/Enemies/Slime_Green_Squash.png");
        sr.sprite = idle;

        SlimeSpriteAnimator anim = visual.AddComponent<SlimeSpriteAnimator>();
        anim.idleSprite = idle;
        anim.squashSprite = squash;

        SaveAndDestroy(enemy, ENEMY_DIR + "/" + name + ".prefab");
    }

    // 4. PIÈGES (Spikes)
    public static void CreateHazardPrefabs()
    {
        GameObject trap = new GameObject("SpikeTrap");

        BoxCollider col = trap.AddComponent<BoxCollider>();
        col.isTrigger = true;
        col.size = new Vector3(1.5f, 0.75f, 1.5f);
        col.center = new Vector3(0, 0.2f, 0);

        trap.AddComponent<SpikeHazard>();

        GameObject visual = new GameObject("Visual");
        visual.transform.SetParent(trap.transform, false);
        visual.transform.localScale = new Vector3(0.32f, 0.32f, 1f);

        SpriteRenderer sr = visual.AddComponent<SpriteRenderer>();
        sr.sprite = LoadSprite("Assets/Sprites/Environment/Spikes_Hazard.png");
        sr.sortingOrder = 4;

        SaveAndDestroy(trap, HAZARD_DIR + "/SpikeTrap.prefab");
    }

    // 5. PLATEFORMES
    public static void CreatePlatformPrefabs()
    {
        CreatePlatformSingle("Platform_Grass", "Assets/Sprites/Environment/Platform_Grass.png", new Vector3(4.5f, 0.9f, 2.5f));
        CreatePlatformSingle("Platform_Crystal", "Assets/Sprites/Environment/Platform_Crystal.png", new Vector3(4.5f, 0.9f, 2.5f));
        CreatePlatformSingle("Platform_Wood", "Assets/Sprites/Environment/Platform_Wood.png", new Vector3(5.0f, 1.2f, 2.5f));
    }

    private static void CreatePlatformSingle(string name, string spritePath, Vector3 size)
    {
        GameObject p = new GameObject(name);
        BoxCollider col = p.AddComponent<BoxCollider>();
        col.size = new Vector3(size.x, size.y, 2.5f);

        GameObject visual = new GameObject("Visual");
        visual.transform.SetParent(p.transform, false);
        visual.transform.localScale = new Vector3(size.x / 5.12f, size.y / 1.6f, 1f);

        SpriteRenderer sr = visual.AddComponent<SpriteRenderer>();
        sr.sprite = LoadSprite(spritePath);
        sr.sortingOrder = 2;

        SaveAndDestroy(p, ENV_DIR + "/" + name + ".prefab");
    }

    // 6. SORTIES (Exits)
    public static void CreateExitPrefabs()
    {
        // Drapeau
        GameObject flag = new GameObject("Exit_Flag");
        BoxCollider colF = flag.AddComponent<BoxCollider>();
        colF.isTrigger = true;
        colF.size = new Vector3(2f, 2.6f, 2f);
        colF.center = new Vector3(0, 0.3f, 0);
        flag.AddComponent<LevelExit>().minimumCoinsToWin = 8;

        GameObject vF = new GameObject("Visual");
        vF.transform.SetParent(flag.transform, false);
        vF.transform.localScale = new Vector3(0.48f, 0.48f, 1f);
        SpriteRenderer srF = vF.AddComponent<SpriteRenderer>();
        srF.sprite = LoadSprite("Assets/Sprites/Environment/Exit_Flag.png");
        srF.sortingOrder = 5;
        SaveAndDestroy(flag, EXIT_DIR + "/Exit_Flag.prefab");

        // Portail
        GameObject portal = new GameObject("Exit_Portal");
        BoxCollider colP = portal.AddComponent<BoxCollider>();
        colP.isTrigger = true;
        colP.size = new Vector3(2.5f, 2.8f, 2f);
        portal.AddComponent<LevelExit>().minimumCoinsToWin = 10;

        GameObject vP = new GameObject("Visual");
        vP.transform.SetParent(portal.transform, false);
        vP.transform.localScale = new Vector3(0.55f, 0.55f, 1f);
        SpriteRenderer srP = vP.AddComponent<SpriteRenderer>();
        srP.sprite = LoadSprite("Assets/Sprites/Environment/Exit_Portal.png");
        srP.sortingOrder = 5;
        SaveAndDestroy(portal, EXIT_DIR + "/Exit_Portal.prefab");

        // Arche dorée
        GameObject gate = new GameObject("Exit_GoldenGate");
        BoxCollider colG = gate.AddComponent<BoxCollider>();
        colG.isTrigger = true;
        colG.size = new Vector3(2.6f, 3.2f, 2f);
        gate.AddComponent<LevelExit>().minimumCoinsToWin = 12;

        GameObject vG = new GameObject("Visual");
        vG.transform.SetParent(gate.transform, false);
        vG.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
        SpriteRenderer srG = vG.AddComponent<SpriteRenderer>();
        srG.sprite = LoadSprite("Assets/Sprites/Environment/Exit_GoldenGate.png");
        srG.sortingOrder = 5;
        SaveAndDestroy(gate, EXIT_DIR + "/Exit_GoldenGate.prefab");
    }

    // 7. DÉCOR & NUAGES
    public static void CreateCloudPrefab()
    {
        GameObject cloud = new GameObject("Cloud_Decor");
        cloud.transform.localScale = new Vector3(0.45f, 0.45f, 1f);

        SpriteRenderer sr = cloud.AddComponent<SpriteRenderer>();
        sr.sprite = LoadSprite("Assets/Sprites/Environment/Cloud_Fluffy.png");
        sr.sortingOrder = 1;

        SaveAndDestroy(cloud, ENV_DIR + "/Cloud_Decor.prefab");
    }

    // 8. MANAGERS
    public static void CreateManagersPrefab()
    {
        GameObject mgr = new GameObject("Managers_Master");
        mgr.AddComponent<GameManager>();
        mgr.AddComponent<SoundManager>();

        SaveAndDestroy(mgr, MGR_DIR + "/Managers_Master.prefab");
    }

    private static void SaveAndDestroy(GameObject obj, string path)
    {
        PrefabUtility.SaveAsPrefabAsset(obj, path);
        Object.DestroyImmediate(obj);
    }
}
