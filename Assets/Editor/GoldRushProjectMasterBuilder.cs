using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.IO;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

public static class GoldRushProjectMasterBuilder
{
    [MenuItem("Gold Rush/🚀 GÉNÉRER LE PROJET COMPLET (Tous les Niveaux & Menus 2D)", false, 0)]
    public static void GenerateFullGameProject()
    {
        EditorUtility.DisplayProgressBar("Coin Collector Master Builder", "Configuration des sprites 2D...", 0.1f);
        CreateProjectFolders();
        ConfigureAllSpritesAs2D();

        EditorUtility.DisplayProgressBar("Coin Collector Master Builder", "Génération du Menu Principal 2D...", 0.25f);
        BuildMainMenuScene();

        EditorUtility.DisplayProgressBar("Coin Collector Master Builder", "Génération de l'Écran de Sélection des Niveaux...", 0.45f);
        BuildLevelSelectScene();

        EditorUtility.DisplayProgressBar("Coin Collector Master Builder", "Génération du Niveau 1 - Green Valley...", 0.65f);
        BuildLevel1GreenValleyScene();

        EditorUtility.DisplayProgressBar("Coin Collector Master Builder", "Génération du Niveau 2 - Crystal Cave...", 0.85f);
        BuildLevel2CrystalCaveScene();

        EditorUtility.DisplayProgressBar("Coin Collector Master Builder", "Génération du Niveau 3 - Sky Temple...", 0.95f);
        BuildLevel3SkyTempleScene();

        SetupAllScenesInBuildSettings();

        EditorUtility.ClearProgressBar();
        Debug.Log("🎉 LE PROJET COIN COLLECTOR 2D COMPLET A ÉTÉ GÉNÉRÉ AVEC SUCCÈS !");
        EditorUtility.DisplayDialog("Coin Collector - Terminé !", "Toutes les scènes du jeu (Menu Principal, Sélection des niveaux, Niveau 1, Niveau 2, Niveau 3, HUD, Victoire et Game Over) ont été générées avec de véritables sprites 2D professionnels !\n\nOuvrez 'Assets/Scenes/MainMenu.unity' et cliquez sur Play ▶ pour tester !", "C'est parti ! 🎮");
    }

    public static void GenerateFullGameProjectBatch()
    {
        CreateProjectFolders();
        ConfigureAllSpritesAs2D();
        BuildMainMenuScene();
        BuildLevelSelectScene();
        BuildLevel1GreenValleyScene();
        BuildLevel2CrystalCaveScene();
        BuildLevel3SkyTempleScene();
        SetupAllScenesInBuildSettings();
        Debug.Log("🎉 LE PROJET COIN COLLECTOR 2D A ÉTÉ GÉNÉRÉ AVEC SUCCÈS (BATCH) !");
    }

    public static void BuildWindowsPlayer()
    {
        string[] scenes = new string[]
        {
            "Assets/Scenes/MainMenu.unity",
            "Assets/Scenes/LevelSelect.unity",
            "Assets/Scenes/Level_01_GreenValley.unity",
            "Assets/Scenes/Level_02_CrystalCave.unity",
            "Assets/Scenes/Level_03_SkyTemple.unity"
        };
        string dir = "Builds/Windows";
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        string buildPath = dir + "/CoinCollector.exe";

        Debug.Log("🚀 Démarrage du Build Windows 64-bit...");
        var report = BuildPipeline.BuildPlayer(scenes, buildPath, BuildTarget.StandaloneWindows64, BuildOptions.None);
        if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.Log($"🎉 BUILD WINDOWS RÉUSSI DANS {buildPath} ({report.summary.totalSize / (1024 * 1024)} Mo)");
        }
        else
        {
            Debug.LogError($"❌ ÉCHEC DU BUILD WINDOWS : {report.summary.result}");
        }
    }

    private static void CreateProjectFolders()
    {
        string[] dirs = new string[] { "Assets/Scenes", "Assets/Materials", "Assets/Prefabs", "Assets/Resources", "Assets/Sprites" };
        foreach (var d in dirs)
        {
            if (!Directory.Exists(d)) Directory.CreateDirectory(d);
        }
    }

    #region SPRITES 2D & CONFIGURATION

    public static void ConfigureAllSpritesAs2D()
    {
        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new string[] { "Assets/Sprites" });
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                bool dirty = false;
                if (importer.textureType != TextureImporterType.Sprite)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    dirty = true;
                }
                if (importer.spriteImportMode != SpriteImportMode.Single)
                {
                    importer.spriteImportMode = SpriteImportMode.Single;
                    dirty = true;
                }
                if (!importer.alphaIsTransparency)
                {
                    importer.alphaIsTransparency = true;
                    dirty = true;
                }
                if (importer.mipmapEnabled)
                {
                    importer.mipmapEnabled = false;
                    dirty = true;
                }
                if (importer.filterMode != FilterMode.Bilinear)
                {
                    importer.filterMode = FilterMode.Bilinear;
                    dirty = true;
                }
                if (dirty)
                {
                    importer.SaveAndReimport();
                }
            }
        }
        AssetDatabase.Refresh();
    }

    private static Sprite LoadSprite(string path)
    {
        Sprite sp = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (sp == null)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.alphaIsTransparency = true;
                importer.mipmapEnabled = false;
                importer.SaveAndReimport();
                sp = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            }
        }
        return sp;
    }

    #endregion

    #region 1. SCÈNE : MENU PRINCIPAL (Title Screen)

    [MenuItem("Gold Rush/🌟 1. Générer Menu Principal (MainMenu)", false, 11)]
    public static void BuildMainMenuScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Caméra & Fond Nuit
        GameObject camObj = new GameObject("Main Camera");
        camObj.tag = "MainCamera";
        Camera cam = camObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color32(16, 14, 44, 255); // Nuit bleu foncé profond
        cam.transform.position = new Vector3(0, 0, -10f);
        camObj.AddComponent<AudioListener>();

        // Lumières
        CreateDirectionalLight(new Vector3(30, -30, 0), Color.white, 1.0f);

        // Décors Arrière-plan
        GameObject bgGroup = new GameObject("Background_Decor");

        // Nuages de nuit
        CreateCloud(bgGroup.transform, new Vector3(-5.5f, 3.8f, 2f), 1.1f);
        CreateCloud(bgGroup.transform, new Vector3(5.2f, 3.5f, 2f), 1.0f);

        // Collines vertes en bas
        CreateBackdropHills(bgGroup.transform);

        // Personnage mignon au centre (Sprite 2D)
        GameObject player = CreateCuteCharacter(new Vector3(0, 0.45f, 0), true);
        FloatingCharacter floatC = player.AddComponent<FloatingCharacter>();
        floatC.verticalAmplitude = 0.08f;
        floatC.verticalSpeed = 2.5f;

        // Pièces d'or tournantes dans le fond (Sprites 2D)
        GameObject coinsGroup = new GameObject("Background_Spinning_Coins");
        Vector3[] coinPos = new Vector3[]
        {
            new Vector3(-4.8f, 2.5f, 0.5f), new Vector3(-4.2f, -0.2f, 0.5f), new Vector3(-3.2f, -1.5f, 0.5f),
            new Vector3(4.5f, 2.8f, 0.5f), new Vector3(4.8f, 0.6f, 0.5f), new Vector3(3.8f, -1.2f, 0.5f),
            new Vector3(3.2f, 3.2f, 0.5f), new Vector3(-2.8f, 3.0f, 0.5f)
        };
        for (int i = 0; i < coinPos.Length; i++)
        {
            CreateSpinningCoinVisual(coinsGroup.transform, coinPos[i], 100f + i * 15f);
        }

        // Canvas UI
        CreateEventSystem();
        Canvas canvas = CreateCanvas("Canvas_MainMenu");

        // Titre "COIN COLLECTOR"
        CreateTMPText(canvas.transform, "Title_GoldRush", "COIN COLLECTOR", 80, FontStyles.Bold, new Color32(255, 204, 0, 255), new Vector2(0, 380), new Vector2(900, 100));
        CreateTMPText(canvas.transform, "Subtitle", "3 NIVEAUX   •   COLLECTE   •   AVENTURE 2D", 22, FontStyles.Bold, new Color32(220, 225, 245, 230), new Vector2(0, 310), new Vector2(900, 50));

        // Boutons Menu
        Button btnPlay = CreateMenuButton(canvas.transform, "Btn_Play", "JOUER", new Vector2(0, -115), new Vector2(480, 85), new Color32(255, 185, 0, 255), Color.white, 36);
        Button btnLevels = CreateMenuButton(canvas.transform, "Btn_Levels", "NIVEAUX", new Vector2(0, -210), new Vector2(480, 75), new Color32(125, 75, 230, 255), Color.white, 30);
        Button btnChar = CreateMenuButton(canvas.transform, "Btn_Character", "PERSONNAGE", new Vector2(-125, -295), new Vector2(230, 65), new Color32(40, 110, 80, 240), Color.white, 22);
        Button btnOpt = CreateMenuButton(canvas.transform, "Btn_Options", "OPTIONS", new Vector2(125, -295), new Vector2(230, 65), new Color32(40, 110, 80, 240), Color.white, 22);
        Button btnInstructions = CreateMenuButton(canvas.transform, "Btn_Instructions", "RÈGLES & COMMANDES", new Vector2(0, -370), new Vector2(480, 60), new Color32(50, 70, 130, 240), Color.white, 22);

        CreateTMPText(canvas.transform, "Footer", "v2.0 — ÉDITION 2D PROFESSIONNELLE", 18, FontStyles.Normal, new Color32(180, 255, 180, 180), new Vector2(0, 30), new Vector2(500, 40), true);

        // Modals : Instructions, Personnage (Skins), et Options
        InstructionsMenu instructionsModal = CreateInstructionsModal(canvas.transform);
        CharacterCustomizationMenu characterModal = CreateCharacterCustomizationModal(canvas.transform);
        OptionsSettingsMenu optionsModal = CreateOptionsModal(canvas.transform);

        // Menu Manager
        GameObject mgr = new GameObject("MainMenuManager");
        MainMenuManager menuMgr = mgr.AddComponent<MainMenuManager>();
        menuMgr.btnPlay = btnPlay;
        menuMgr.btnLevels = btnLevels;
        menuMgr.btnCharacter = btnChar;
        menuMgr.btnOptions = btnOpt;
        menuMgr.btnInstructions = btnInstructions;
        menuMgr.instructionsPanel = instructionsModal.gameObject;
        menuMgr.characterPanel = characterModal.gameObject;
        menuMgr.optionsPanel = optionsModal.gameObject;
        menuMgr.firstLevelSceneName = "Level_01_GreenValley";

        // SoundManager & GameManager
        CreateManagersInScene();

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/MainMenu.unity");
    }

    private static void CreateBackdropHills(Transform parent)
    {
        // Collines vertes en bas réalisées avec le sprite de plateforme herbe étiré doucement
        GameObject hillLeft = new GameObject("Hill_Left");
        hillLeft.transform.SetParent(parent, false);
        hillLeft.transform.position = new Vector3(-4.5f, -3.2f, 1f);
        hillLeft.transform.localScale = new Vector3(2.4f, 1.8f, 1f);
        SpriteRenderer srL = hillLeft.AddComponent<SpriteRenderer>();
        srL.sprite = LoadSprite("Assets/Sprites/Environment/Platform_Grass.png");
        srL.color = new Color(0.2f, 0.45f, 0.25f, 0.9f);
        srL.sortingOrder = 0;

        GameObject hillRight = new GameObject("Hill_Right");
        hillRight.transform.SetParent(parent, false);
        hillRight.transform.position = new Vector3(4.5f, -3.5f, 1f);
        hillRight.transform.localScale = new Vector3(2.6f, 1.9f, 1f);
        SpriteRenderer srR = hillRight.AddComponent<SpriteRenderer>();
        srR.sprite = LoadSprite("Assets/Sprites/Environment/Platform_Grass.png");
        srR.color = new Color(0.25f, 0.55f, 0.3f, 0.9f);
        srR.sortingOrder = 0;

        GameObject hillCenter = new GameObject("Hill_Center");
        hillCenter.transform.SetParent(parent, false);
        hillCenter.transform.position = new Vector3(0f, -4.0f, 0.5f);
        hillCenter.transform.localScale = new Vector3(3.2f, 2.0f, 1f);
        SpriteRenderer srC = hillCenter.AddComponent<SpriteRenderer>();
        srC.sprite = LoadSprite("Assets/Sprites/Environment/Platform_Grass.png");
        srC.color = new Color(0.18f, 0.4f, 0.22f, 1.0f);
        srC.sortingOrder = 0;
    }

    #endregion

    #region 2. SCÈNE : CHOISIR UN NIVEAU (Level Selection)

    [MenuItem("Gold Rush/🗺️ 2. Générer Choix des Niveaux (LevelSelect)", false, 12)]
    public static void BuildLevelSelectScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Caméra
        GameObject camObj = new GameObject("Main Camera");
        camObj.tag = "MainCamera";
        Camera cam = camObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color32(20, 12, 38, 255);
        cam.transform.position = new Vector3(0, 0, -10f);
        camObj.AddComponent<AudioListener>();

        CreateEventSystem();
        Canvas canvas = CreateCanvas("Canvas_LevelSelect");

        // Bouton RETOUR
        Button btnBack = CreateMenuButton(canvas.transform, "Btn_Back", "← RETOUR", new Vector2(-750, 450), new Vector2(200, 60), new Color32(45, 40, 70, 255), Color.white, 24);

        // Titre "CHOISIR UN NIVEAU"
        CreateTMPText(canvas.transform, "Title_LevelSelect", "CHOISIR UN NIVEAU", 54, FontStyles.Bold, Color.white, new Vector2(0, 450), new Vector2(700, 80));

        // 3 Cartes de Niveau (Côte à côte)
        // Carte 1: Green Valley
        var card1 = CreateLevelCard(canvas.transform, "Card_Level1", "01", "FACILE", "GREEN VALLEY", "Prairies fleuries, plateformes en bois et slimes bondissants.", new Vector2(-540, -40), new Color32(35, 30, 65, 255), false);
        // Carte 2: Crystal Cave
        var card2 = CreateLevelCard(canvas.transform, "Card_Level2", "02", "MOYEN", "💎 CRYSTAL CAVE", "Grottes mystiques, cristaux acérés, chauves-souris et portails magiques.", new Vector2(0, -40), new Color32(30, 25, 55, 255), true);
        // Carte 3: Sky Temple
        var card3 = CreateLevelCard(canvas.transform, "Card_Level3", "03", "DIFFICILE", "SKY TEMPLE", "Îlots célestes sacrés, slimes enflammés et porte dorée légendaire.", new Vector2(540, -40), new Color32(30, 25, 55, 255), true);

        // LevelSelectMenu Manager
        GameObject mgr = new GameObject("LevelSelectMenu");
        LevelSelectMenu lsm = mgr.AddComponent<LevelSelectMenu>();
        lsm.btnBack = btnBack;

        lsm.btnPlayLevel1 = card1.btnPlay;
        lsm.level1CoinsText = card1.txtCoins;
        lsm.level1ScoreText = card1.txtScore;
        lsm.level1Stars = card1.stars;

        lsm.btnPlayLevel2 = card2.btnPlay;
        lsm.level2CoinsText = card2.txtCoins;
        lsm.level2ScoreText = card2.txtScore;
        lsm.level2Stars = card2.stars;
        lsm.level2LockedOverlay = card2.lockOverlay;

        lsm.btnPlayLevel3 = card3.btnPlay;
        lsm.level3CoinsText = card3.txtCoins;
        lsm.level3ScoreText = card3.txtScore;
        lsm.level3Stars = card3.stars;
        lsm.level3LockedOverlay = card3.lockOverlay;

        CreateManagersInScene();

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/LevelSelect.unity");
    }

    #endregion

    #region 3. SCÈNE : NIVEAU 1 - GREEN VALLEY (Screenshot 3)

    [MenuItem("Gold Rush/🌿 3. Générer Niveau 1 - Green Valley", false, 13)]
    public static void BuildLevel1GreenValleyScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Lumière
        CreateDirectionalLight(new Vector3(40, -30, 0), Color.white, 1.1f);

        // Caméra Ciel Bleu
        GameObject camObj = new GameObject("Main Camera");
        camObj.tag = "MainCamera";
        Camera cam = camObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color32(95, 190, 255, 255); // Ciel bleu radieux
        cam.transform.position = new Vector3(0, 2, -10f);
        camObj.AddComponent<AudioListener>();

        // Nuages en arrière-plan (Sprites 2D)
        GameObject bgGroup = new GameObject("Background_Decor");
        CreateCloud(bgGroup.transform, new Vector3(-6, 7, 2), 1.2f);
        CreateCloud(bgGroup.transform, new Vector3(10, 8, 2), 1.4f);
        CreateCloud(bgGroup.transform, new Vector3(25, 7.5f, 2), 1.1f);
        CreateCloud(bgGroup.transform, new Vector3(38, 8.2f, 2), 1.3f);

        // Sol Principal avec Sprite 2D Grass Platform
        CreateMainGround(new Vector3(18, -0.6f, 0), 46f, 1.4f, "Assets/Sprites/Environment/Platform_Grass.png");

        // Plateformes surélevées flottantes en herbe et bois
        CreateGrassPlatform(new Vector3(4f, 1.6f, 0), new Vector3(4.5f, 0.9f, 2.5f));
        CreateGrassPlatform(new Vector3(12f, 3.2f, 0), new Vector3(5.5f, 0.9f, 2.5f));
        CreateGrassPlatform(new Vector3(20f, 4.8f, 0), new Vector3(5.0f, 0.9f, 2.5f));
        CreateGrassPlatform(new Vector3(27f, 2.4f, 0), new Vector3(4.5f, 0.9f, 2.5f));

        // Pièges : Roches / Piques au sol
        CreateSpikeTrap(new Vector3(8.5f, 0.35f, 0));
        CreateSpikeTrap(new Vector3(16f, 0.35f, 0));

        // Joueur 2D
        GameObject playerObj = CreateCuteCharacter(new Vector3(0, 0.47f, 0));
        CharacterController cc = playerObj.AddComponent<CharacterController>();
        cc.center = new Vector3(0, 0, 0);
        cc.height = 0.78f;
        cc.radius = 0.2f;
        cc.skinWidth = 0.02f;
        cc.stepOffset = 0.15f;
        cc.minMoveDistance = 0f;
        PlayerController25D playerCtrl = playerObj.AddComponent<PlayerController25D>();
        playerCtrl.moveSpeed = 8f;
        playerCtrl.jumpForce = 13.5f;
        playerCtrl.is2DSprite = true;
        PlayerHealth playerHealth = playerObj.AddComponent<PlayerHealth>();
        playerHealth.respawnPoint = playerObj.transform.position;

        // Ennemis slimes verts
        CreateSimpleEnemy(new Vector3(14f, 0.5f, 0), 3f, "slime_green");
        CreateSimpleEnemy(new Vector3(25f, 0.5f, 0), 3.5f, "slime_green");

        // Suivi Caméra
        CameraFollow25D camFollow = camObj.AddComponent<CameraFollow25D>();
        camFollow.target = playerObj.transform;
        camFollow.offset = new Vector3(2.5f, 2f, -10f);

        // 30 Pièces dorées à collecter (Sprites 2D animés)
        GameObject coinsGroup = new GameObject("Coins_Group");
        List<Vector3> coinList = new List<Vector3>();
        for (float x = 2; x <= 6; x += 1.4f) coinList.Add(new Vector3(x, 2.7f, 0));
        for (float x = 10; x <= 14; x += 1.3f) coinList.Add(new Vector3(x, 4.3f, 0));
        for (float x = 18; x <= 22; x += 1.3f) coinList.Add(new Vector3(x, 5.9f, 0));
        for (float x = 25; x <= 29; x += 1.3f) coinList.Add(new Vector3(x, 3.5f, 0));
        for (float x = 7; x <= 15; x += 2f) coinList.Add(new Vector3(x, 1.2f, 0));
        for (float x = 22; x <= 33; x += 2f) coinList.Add(new Vector3(x, 1.2f, 0));

        foreach (var cPos in coinList)
        {
            CreateCollectibleCoin(coinsGroup.transform, cPos);
        }

        // Sortie : Drapeau de Fin 2D (SORTIE)
        GameObject exitObj = CreateFlagExit(new Vector3(36f, 1.5f, 0), 8);
        GameObject startMarker = new GameObject("StartMarker"); startMarker.transform.position = new Vector3(0, 0, 0);
        GameObject endMarker = new GameObject("EndMarker"); endMarker.transform.position = new Vector3(36f, 0, 0);

        // UI HUD & Victory & Game Over Modals
        CreateGameplayHUDAndVictory(1, "Green Valley", coinList.Count, playerObj.transform, startMarker.transform, endMarker.transform, playerCtrl);

        CreateManagersInScene();
        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Level_01_GreenValley.unity");
    }

    #endregion

    #region 4. SCÈNE : NIVEAU 2 - CRYSTAL CAVE (Screenshot 5)

    [MenuItem("Gold Rush/💎 4. Générer Niveau 2 - Crystal Cave", false, 14)]
    public static void BuildLevel2CrystalCaveScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Lumière violette d'ambiance
        CreateDirectionalLight(new Vector3(45, -30, 0), new Color32(180, 130, 255, 255), 1.0f);

        // Caméra Grotte Sombre Violette
        GameObject camObj = new GameObject("Main Camera");
        camObj.tag = "MainCamera";
        Camera cam = camObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color32(18, 10, 35, 255);
        cam.transform.position = new Vector3(0, 2, -10f);
        camObj.AddComponent<AudioListener>();

        // Sol de Grotte avec Sprite Crystal Platform
        CreateMainGround(new Vector3(18, -0.6f, 0), 46f, 1.4f, "Assets/Sprites/Environment/Platform_Crystal.png");

        // Plateformes flottantes de cristal
        CreateCavePlatform(new Vector3(4f, 1.6f, 0), new Vector3(4.5f, 0.9f, 2.5f));
        CreateCavePlatform(new Vector3(12f, 3.4f, 0), new Vector3(5.5f, 0.9f, 2.5f));
        CreateCavePlatform(new Vector3(20f, 4.8f, 0), new Vector3(5.0f, 0.9f, 2.5f));
        CreateCavePlatform(new Vector3(28f, 3.0f, 0), new Vector3(5.0f, 0.9f, 2.5f));

        // Pièges : Spikes acérés au sol
        CreateSpikeTrap(new Vector3(7.5f, 0.35f, 0));
        CreateSpikeTrap(new Vector3(16f, 0.35f, 0));
        CreateSpikeTrap(new Vector3(24.5f, 0.35f, 0));

        // Joueur 2D
        GameObject playerObj = CreateCuteCharacter(new Vector3(0, 0.47f, 0));
        CharacterController cc = playerObj.AddComponent<CharacterController>();
        cc.center = new Vector3(0, 0, 0);
        cc.height = 0.78f;
        cc.radius = 0.2f;
        cc.skinWidth = 0.02f;
        cc.stepOffset = 0.15f;
        cc.minMoveDistance = 0f;
        PlayerController25D playerCtrl = playerObj.AddComponent<PlayerController25D>();
        playerCtrl.moveSpeed = 8f;
        playerCtrl.jumpForce = 13.5f;
        playerCtrl.is2DSprite = true;
        PlayerHealth playerHealth = playerObj.AddComponent<PlayerHealth>();
        playerHealth.respawnPoint = playerObj.transform.position;

        // Ennemis de Grotte : Slime Violet et Chauves-souris
        CreateSimpleEnemy(new Vector3(12f, 4.2f, 0), 2.5f, "slime_purple", 2.6f);
        CreateSimpleEnemy(new Vector3(28f, 3.8f, 0), 2.5f, "slime_purple", 2.6f);
        CreateBatEnemy(new Vector3(6f, 3.2f, 0), 3.0f, 3.1f);
        CreateBatEnemy(new Vector3(22f, 3.0f, 0), 3.5f, 3.1f);

        // Suivi Caméra
        CameraFollow25D camFollow = camObj.AddComponent<CameraFollow25D>();
        camFollow.target = playerObj.transform;
        camFollow.offset = new Vector3(2.5f, 2f, -10f);

        // 35 Pièces dorées
        GameObject coinsGroup = new GameObject("Coins_Group");
        List<Vector3> coinList = new List<Vector3>();
        for (float x = 2; x <= 6; x += 1.2f) coinList.Add(new Vector3(x, 2.7f, 0));
        for (float x = 10; x <= 14; x += 1.1f) coinList.Add(new Vector3(x, 4.5f, 0));
        for (float x = 18; x <= 22; x += 1.1f) coinList.Add(new Vector3(x, 5.9f, 0));
        for (float x = 26; x <= 30; x += 1.1f) coinList.Add(new Vector3(x, 4.1f, 0));
        for (float x = 6; x <= 18; x += 2f) coinList.Add(new Vector3(x, 1.2f, 0));
        for (float x = 23; x <= 35; x += 1.8f) coinList.Add(new Vector3(x, 1.2f, 0));

        foreach (var cPos in coinList)
        {
            CreateCollectibleCoin(coinsGroup.transform, cPos);
        }

        // Sortie : Portail Magique Lumineux 2D
        CreatePortalExit(new Vector3(36f, 1.8f, 0), 10);
        GameObject startMarker = new GameObject("StartMarker"); startMarker.transform.position = new Vector3(0, 0, 0);
        GameObject endMarker = new GameObject("EndMarker"); endMarker.transform.position = new Vector3(36f, 0, 0);

        // HUD & Victoire & Game Over Modals
        CreateGameplayHUDAndVictory(2, "Crystal Cave", coinList.Count, playerObj.transform, startMarker.transform, endMarker.transform, playerCtrl);

        CreateManagersInScene();
        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Level_02_CrystalCave.unity");
    }

    #endregion

    #region 5. SCÈNE : NIVEAU 3 - SKY TEMPLE

    [MenuItem("Gold Rush/⛩️ 5. Générer Niveau 3 - Sky Temple", false, 15)]
    public static void BuildLevel3SkyTempleScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Lumière Dorée du Crépuscule
        CreateDirectionalLight(new Vector3(35, -40, 0), new Color32(255, 220, 160, 255), 1.2f);

        // Caméra Ciel Couchant
        GameObject camObj = new GameObject("Main Camera");
        camObj.tag = "MainCamera";
        Camera cam = camObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color32(230, 140, 95, 255); // Coucher de soleil
        cam.transform.position = new Vector3(0, 2, -10f);
        camObj.AddComponent<AudioListener>();

        // Nuages flottants 2D
        GameObject bgGroup = new GameObject("Clouds_Decor");
        for (int i = 0; i < 7; i++)
        {
            CreateCloud(bgGroup.transform, new Vector3(i * 6f - 3f, 4f + (i % 3) * 1.5f, 2f), 1.1f + (i % 2) * 0.3f);
        }

        // Îlots de temple flottants
        CreateSkyIsland(new Vector3(0, 0, 0), new Vector3(6, 1.2f, 2.5f));
        CreateSkyIsland(new Vector3(8, 2.0f, 0), new Vector3(5, 1.2f, 2.5f));
        CreateSkyIsland(new Vector3(16, 4.2f, 0), new Vector3(6, 1.2f, 2.5f));
        CreateSkyIsland(new Vector3(25, 2.8f, 0), new Vector3(5, 1.2f, 2.5f));
        CreateSkyIsland(new Vector3(34, 5.2f, 0), new Vector3(7, 1.4f, 2.5f));

        // Piques volants / pièges
        CreateSpikeTrap(new Vector3(8f, 2.8f, 0));
        CreateSpikeTrap(new Vector3(25f, 3.6f, 0));

        // Joueur 2D
        GameObject playerObj = CreateCuteCharacter(new Vector3(0, 0.97f, 0));
        CharacterController cc = playerObj.AddComponent<CharacterController>();
        cc.center = new Vector3(0, 0, 0);
        cc.height = 0.78f;
        cc.radius = 0.2f;
        cc.skinWidth = 0.02f;
        cc.stepOffset = 0.15f;
        cc.minMoveDistance = 0f;
        PlayerController25D playerCtrl = playerObj.AddComponent<PlayerController25D>();
        playerCtrl.moveSpeed = 8f;
        playerCtrl.jumpForce = 14f;
        playerCtrl.is2DSprite = true;
        PlayerHealth playerHealth = playerObj.AddComponent<PlayerHealth>();
        playerHealth.respawnPoint = playerObj.transform.position;

        // Ennemis Célestes : Slimes de feu et Chauves-souris
        CreateSimpleEnemy(new Vector3(16f, 5.0f, 0), 2.2f, "slime_fire", 2.8f);
        CreateBatEnemy(new Vector3(11f, 3.5f, 0), 2.5f, 3.3f);
        CreateBatEnemy(new Vector3(20f, 4.0f, 0), 3.0f, 3.3f);
        CreateBatEnemy(new Vector3(30f, 4.5f, 0), 2.5f, 3.3f);

        // Suivi Caméra
        CameraFollow25D camFollow = camObj.AddComponent<CameraFollow25D>();
        camFollow.target = playerObj.transform;
        camFollow.offset = new Vector3(2.5f, 2f, -10f);

        // 40 Pièces
        GameObject coinsGroup = new GameObject("Coins_Group");
        List<Vector3> coinList = new List<Vector3>();
        for (float x = -1; x <= 2; x += 1f) coinList.Add(new Vector3(x, 1.5f, 0));
        for (float x = 6; x <= 10; x += 1f) coinList.Add(new Vector3(x, 3.5f, 0));
        for (float x = 14; x <= 18; x += 1f) coinList.Add(new Vector3(x, 5.6f, 0));
        for (float x = 23; x <= 27; x += 1f) coinList.Add(new Vector3(x, 4.2f, 0));
        for (float x = 32; x <= 36; x += 1f) coinList.Add(new Vector3(x, 6.6f, 0));

        foreach (var cPos in coinList)
        {
            CreateCollectibleCoin(coinsGroup.transform, cPos);
        }

        // Sortie : Porte Dorée Royale 2D
        CreateGoldenGateExit(new Vector3(36f, 6.8f, 0), 12);
        GameObject startMarker = new GameObject("StartMarker"); startMarker.transform.position = new Vector3(0, 0, 0);
        GameObject endMarker = new GameObject("EndMarker"); endMarker.transform.position = new Vector3(36f, 0, 0);

        // HUD & Victoire & Game Over Modals
        CreateGameplayHUDAndVictory(3, "Sky Temple", coinList.Count, playerObj.transform, startMarker.transform, endMarker.transform, playerCtrl);

        CreateManagersInScene();
        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Level_03_SkyTemple.unity");
    }

    #endregion

    #region HELPERS UI & HUD

    private static void CreateGameplayHUDAndVictory(int levelNum, string levelName, int maxCoins, Transform player, Transform startT, Transform endT, PlayerController25D playerCtrl)
    {
        CreateEventSystem();
        Canvas canvas = CreateCanvas("Canvas_Gameplay_HUD");

        // En haut à gauche : Badge Pièces & Score & Cœurs
        GameObject coinBadge = new GameObject("CoinScoreBadge");
        coinBadge.transform.SetParent(canvas.transform, false);
        Image badgeImg = coinBadge.AddComponent<Image>();
        badgeImg.color = new Color32(20, 20, 40, 200);
        RectTransform bRect = coinBadge.GetComponent<RectTransform>();
        bRect.anchorMin = new Vector2(0, 1);
        bRect.anchorMax = new Vector2(0, 1);
        bRect.pivot = new Vector2(0, 1);
        bRect.anchoredPosition = new Vector2(40, -30);
        bRect.sizeDelta = new Vector2(260, 65);

        TextMeshProUGUI txtCoins = CreateTMPText(coinBadge.transform, "Txt_Coins", $"PIÈCES  0 / {maxCoins}", 22, FontStyles.Bold, new Color32(255, 215, 0, 255), new Vector2(-70, 0), new Vector2(110, 40));
        TextMeshProUGUI txtScore = CreateTMPText(coinBadge.transform, "Txt_Score", "0", 22, FontStyles.Bold, Color.white, new Vector2(25, 0), new Vector2(80, 40));
        TextMeshProUGUI txtHearts = CreateTMPText(coinBadge.transform, "Txt_Hearts", "VIES  3/3", 16, FontStyles.Bold, Color.white, new Vector2(110, 0), new Vector2(90, 40));

        // En haut au centre : Niveau & Barre de progression
        GameObject progObj = new GameObject("LevelProgressCenter");
        progObj.transform.SetParent(canvas.transform, false);
        Image progBg = progObj.AddComponent<Image>();
        progBg.color = new Color32(20, 25, 45, 200);
        RectTransform prRect = progObj.GetComponent<RectTransform>();
        prRect.anchorMin = new Vector2(0.5f, 1);
        prRect.anchorMax = new Vector2(0.5f, 1);
        prRect.pivot = new Vector2(0.5f, 1);
        prRect.anchoredPosition = new Vector2(0, -30);
        prRect.sizeDelta = new Vector2(380, 65);

        TextMeshProUGUI txtLevelHeader = CreateTMPText(progObj.transform, "Txt_LevelHeader", $"NIVEAU {levelNum}", 20, FontStyles.Bold, Color.white, new Vector2(0, 14), new Vector2(250, 30));

        // Slider Barre de Progression
        GameObject sliderObj = new GameObject("Slider_Progress");
        sliderObj.transform.SetParent(progObj.transform, false);
        Slider slider = sliderObj.AddComponent<Slider>();
        RectTransform slRect = sliderObj.GetComponent<RectTransform>();
        slRect.anchoredPosition = new Vector2(0, -12);
        slRect.sizeDelta = new Vector2(260, 12);

        GameObject fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(sliderObj.transform, false);
        Image fillImg = fillObj.AddComponent<Image>();
        fillImg.color = new Color32(255, 185, 0, 255);
        slider.fillRect = fillObj.GetComponent<RectTransform>();

        CreateTMPText(progObj.transform, "Txt_Start", "START", 13, FontStyles.Bold, new Color32(180, 190, 220, 200), new Vector2(-155, -12), new Vector2(50, 20));
        CreateTMPText(progObj.transform, "Txt_Sortie", "SORTIE", 13, FontStyles.Bold, new Color32(180, 190, 220, 200), new Vector2(155, -12), new Vector2(60, 20));

        // En haut à droite : Bouton FIN, Pause, Settings, Instructions
        Button btnInstructions = CreateMenuButton(canvas.transform, "Btn_Instructions_HUD", "INF", new Vector2(630, 480), new Vector2(55, 50), new Color32(50, 70, 130, 220), Color.white, 20);
        Button btnFin = CreateMenuButton(canvas.transform, "Btn_FinTest", "FIN", new Vector2(705, 480), new Vector2(90, 50), new Color32(255, 185, 0, 255), Color.white, 19);
        Button btnPause = CreateMenuButton(canvas.transform, "Btn_Pause", "II", new Vector2(805, 480), new Vector2(55, 50), new Color32(60, 120, 120, 220), Color.white, 20);
        Button btnSettings = CreateMenuButton(canvas.transform, "Btn_Settings", "OPT", new Vector2(875, 480), new Vector2(55, 50), new Color32(60, 120, 120, 220), Color.white, 20);

        // Contrôles Tactiles Mobile (En bas)
        GameObject controlsObj = new GameObject("MobileTouchControls");
        MobileTouchControls touchCtrl = controlsObj.AddComponent<MobileTouchControls>();
        touchCtrl.playerController = playerCtrl;

        Button btnLeft = CreateMenuButton(canvas.transform, "Btn_Left", "←", new Vector2(-820, -450), new Vector2(90, 80), new Color32(255, 255, 255, 60), Color.white, 36);
        Button btnRight = CreateMenuButton(canvas.transform, "Btn_Right", "→", new Vector2(-710, -450), new Vector2(90, 80), new Color32(255, 255, 255, 60), Color.white, 36);
        Button btnJump = CreateMenuButton(canvas.transform, "Btn_Jump", "↑\nSAUT", new Vector2(820, -430), new Vector2(110, 110), new Color32(255, 190, 0, 180), Color.white, 22);

        AddEventTrigger(btnLeft.gameObject, EventTriggerType.PointerDown, touchCtrl.OnPointerDownLeft);
        AddEventTrigger(btnLeft.gameObject, EventTriggerType.PointerUp, touchCtrl.OnPointerUpLeft);
        AddEventTrigger(btnRight.gameObject, EventTriggerType.PointerDown, touchCtrl.OnPointerDownRight);
        AddEventTrigger(btnRight.gameObject, EventTriggerType.PointerUp, touchCtrl.OnPointerUpRight);
        btnJump.onClick.AddListener(touchCtrl.OnJumpClicked);

        // Modals : Victoire, Game Over, Instructions
        LevelCompleteUI victoryModal = CreateVictoryModal(canvas.transform);
        GameOverUI gameOverModal = CreateGameOverModal(canvas.transform);
        InstructionsMenu instructionsModal = CreateInstructionsModal(canvas.transform);

        // HUDManager
        GameObject hudObj = new GameObject("HUDManager");
        HUDManager hud = hudObj.AddComponent<HUDManager>();
        hud.currentLevelNumber = levelNum;
        hud.currentLevelName = levelName;
        hud.maxCoinsInLevel = maxCoins;
        hud.minimumCoinsToWin = LevelBalance.GetMinimumCoins(levelNum);
        hud.coinsText = txtCoins;
        hud.scoreText = txtScore;
        hud.heartsText = txtHearts;
        hud.levelTitleText = txtLevelHeader;
        hud.progressBar = slider;
        hud.playerTransform = player;
        hud.levelStartTransform = startT;
        hud.levelEndTransform = endT;
        hud.btnFinishTest = btnFin;
        hud.btnPause = btnPause;
        hud.btnSettings = btnSettings;
        hud.btnInstructions = btnInstructions;
        hud.levelCompleteModal = victoryModal;
        hud.gameOverModal = gameOverModal;
        hud.instructionsModal = instructionsModal;
    }

    private static GameOverUI CreateGameOverModal(Transform parent)
    {
        GameObject modalObj = new GameObject("Modal_GameOver");
        modalObj.transform.SetParent(parent, false);

        Image bg = modalObj.AddComponent<Image>();
        bg.color = new Color32(25, 10, 15, 250);
        RectTransform mRect = modalObj.GetComponent<RectTransform>();
        mRect.sizeDelta = new Vector2(560, 620);

        CanvasGroup cg = modalObj.AddComponent<CanvasGroup>();
        GameOverUI ui = modalObj.AddComponent<GameOverUI>();
        ui.canvasGroup = cg;

        CreateTMPText(modalObj.transform, "Txt_Skull", "💀", 55, FontStyles.Bold, Color.white, new Vector2(0, 220), new Vector2(100, 70));
        ui.titleText = CreateTMPText(modalObj.transform, "Txt_Title", "GAME OVER", 44, FontStyles.Bold, new Color32(239, 68, 68, 255), new Vector2(0, 150), new Vector2(500, 55));
        ui.subtitleText = CreateTMPText(modalObj.transform, "Txt_Subtitle", "Vous n'avez plus de vies !", 20, FontStyles.Bold, new Color32(250, 200, 200, 220), new Vector2(0, 95), new Vector2(450, 35));

        GameObject statsPanel = new GameObject("Stats_Panel_GameOver");
        statsPanel.transform.SetParent(modalObj.transform, false);
        Image statsBg = statsPanel.AddComponent<Image>();
        statsBg.color = new Color32(35, 15, 20, 200);
        RectTransform stRect = statsPanel.GetComponent<RectTransform>();
        stRect.anchoredPosition = new Vector2(0, -10);
        stRect.sizeDelta = new Vector2(440, 120);

        CreateTMPText(statsPanel.transform, "L_Coins", "PIÈCES RAMASSÉES", 18, FontStyles.Bold, Color.white, new Vector2(-100, 25), new Vector2(200, 30));
        ui.coinsText = CreateTMPText(statsPanel.transform, "V_Coins", "0", 22, FontStyles.Bold, new Color32(255, 215, 0, 255), new Vector2(120, 25), new Vector2(150, 30));

        CreateTMPText(statsPanel.transform, "L_Score", "SCORE OBTENU", 18, FontStyles.Bold, Color.white, new Vector2(-100, -25), new Vector2(200, 30));
        ui.scoreText = CreateTMPText(statsPanel.transform, "V_Score", "0", 22, FontStyles.Bold, new Color32(255, 200, 50, 255), new Vector2(120, -25), new Vector2(150, 30));

        ui.btnRetry = CreateMenuButton(modalObj.transform, "Btn_Retry", "🔄  RÉESSAYER", new Vector2(0, -135), new Vector2(440, 70), new Color32(239, 68, 68, 255), Color.white, 26);
        ui.btnMainMenu = CreateMenuButton(modalObj.transform, "Btn_Menu", "🏠  MENU PRINCIPAL", new Vector2(0, -220), new Vector2(440, 65), new Color32(60, 50, 80, 240), Color.white, 22);

        modalObj.SetActive(false);
        return ui;
    }

    private static InstructionsMenu CreateInstructionsModal(Transform parent)
    {
        GameObject modalObj = new GameObject("Modal_Instructions");
        modalObj.transform.SetParent(parent, false);

        Image bg = modalObj.AddComponent<Image>();
        bg.color = new Color32(18, 14, 32, 250);
        RectTransform mRect = modalObj.GetComponent<RectTransform>();
        mRect.sizeDelta = new Vector2(700, 760);

        InstructionsMenu ui = modalObj.AddComponent<InstructionsMenu>();

        CreateTMPText(modalObj.transform, "Txt_HeaderIcon", "INF", 42, FontStyles.Bold, Color.white, new Vector2(0, 320), new Vector2(80, 50));
        CreateTMPText(modalObj.transform, "Txt_Title", "COMMENT JOUER & INSTRUCTIONS", 32, FontStyles.Bold, new Color32(255, 215, 0, 255), new Vector2(0, 275), new Vector2(650, 45));

        // Boite 1 : Objectif
        GameObject boxObj = new GameObject("Box_Objectif");
        boxObj.transform.SetParent(modalObj.transform, false);
        Image b1 = boxObj.AddComponent<Image>(); b1.color = new Color32(30, 25, 55, 200);
        RectTransform r1 = boxObj.GetComponent<RectTransform>();
        r1.anchoredPosition = new Vector2(0, 180); r1.sizeDelta = new Vector2(620, 110);
        CreateTMPText(boxObj.transform, "T1", "OBJECTIF DU JEU", 20, FontStyles.Bold, new Color32(255, 200, 50, 255), new Vector2(0, 28), new Vector2(580, 30));
        CreateTMPText(boxObj.transform, "D1", "Ramassez des pièces d'or, évitez les monstres et franchissez la sortie pour débloquer les niveaux.", 15, FontStyles.Normal, Color.white, new Vector2(0, -18), new Vector2(580, 60));

        // Boite 2 : Commandes PC & Mobile
        GameObject boxCtrl = new GameObject("Box_Controls");
        boxCtrl.transform.SetParent(modalObj.transform, false);
        Image b2 = boxCtrl.AddComponent<Image>(); b2.color = new Color32(30, 25, 55, 200);
        RectTransform r2 = boxCtrl.GetComponent<RectTransform>();
        r2.anchoredPosition = new Vector2(0, 30); r2.sizeDelta = new Vector2(620, 150);
        CreateTMPText(boxCtrl.transform, "T2", "COMMANDES PC & MOBILE", 20, FontStyles.Bold, new Color32(96, 165, 250, 255), new Vector2(0, 48), new Vector2(580, 30));
        CreateTMPText(boxCtrl.transform, "D2", "• Flèches ◀ ▶ ou [A] [D] / [Q] [D] : Déplacement gauche / droite\n• [ESPACE] ou [↑] / [Z] : Sauter (saut fluide 2D)\n• [ÉCHAP] : Pause\n• Mobile : Boutons tactiles virtuels réactifs intégrés !", 15, FontStyles.Normal, Color.white, new Vector2(0, -18), new Vector2(580, 95));

        // Boite 3 : Astuces & Vies
        GameObject boxTips = new GameObject("Box_Tips");
        boxTips.transform.SetParent(modalObj.transform, false);
        Image b3 = boxTips.AddComponent<Image>(); b3.color = new Color32(30, 25, 55, 200);
        RectTransform r3 = boxTips.GetComponent<RectTransform>();
        r3.anchoredPosition = new Vector2(0, -130); r3.sizeDelta = new Vector2(620, 130);
        CreateTMPText(boxTips.transform, "T3", "💡  VIES & ASTUCES", 20, FontStyles.Bold, new Color32(52, 211, 153, 255), new Vector2(0, 35), new Vector2(580, 30));
        CreateTMPText(boxTips.transform, "D3", "• Vous avez 3 vies. Toucher un monstre par le côté retire 1 vie.\n• Écrasement : sautez sur les slimes pour les éliminer (+200 pts).\n• Personnalisation : changez de tenue dans le menu Personnage.", 15, FontStyles.Normal, Color.white, new Vector2(0, -20), new Vector2(580, 75));

        ui.btnClose = CreateMenuButton(modalObj.transform, "Btn_CloseInstructions", "C'EST PARTI !", new Vector2(0, -280), new Vector2(400, 65), new Color32(255, 185, 0, 255), Color.white, 24);

        modalObj.SetActive(false);
        return ui;
    }

    private static CharacterCustomizationMenu CreateCharacterCustomizationModal(Transform parent)
    {
        GameObject modalObj = new GameObject("Modal_CharacterCustomization");
        modalObj.transform.SetParent(parent, false);

        Image bg = modalObj.AddComponent<Image>();
        bg.color = new Color32(18, 22, 38, 250);
        RectTransform mRect = modalObj.GetComponent<RectTransform>();
        mRect.sizeDelta = new Vector2(720, 750);

        CharacterCustomizationMenu ui = modalObj.AddComponent<CharacterCustomizationMenu>();

        CreateTMPText(modalObj.transform, "Txt_HeaderIcon", "PERSONNAGE", 44, FontStyles.Bold, Color.white, new Vector2(0, 310), new Vector2(80, 50));
        CreateTMPText(modalObj.transform, "Txt_Title", "CHOIX DU PERSONNAGE", 32, FontStyles.Bold, new Color32(255, 215, 0, 255), new Vector2(0, 260), new Vector2(650, 45));

        ui.txtSelectedSkinName = CreateTMPText(modalObj.transform, "Txt_SelectedSkin", "PERSO ÉQUIPÉ : HÉROS CLASSIC", 18, FontStyles.Bold, new Color32(180, 240, 255, 255), new Vector2(0, 205), new Vector2(550, 35));

        // 5 Boutons de skins
        ui.btnSkinClassic = CreateMenuButton(modalObj.transform, "Btn_Skin0", "🔵  HÉROS CLASSIC  (Bleu)", new Vector2(0, 135), new Vector2(550, 65), new Color32(35, 110, 225, 255), Color.white, 22);
        ui.btnSkinAdventurer = CreateMenuButton(modalObj.transform, "Btn_Skin1", "🧭  AVENTURIER  (Boussole & Sac)", new Vector2(0, 55), new Vector2(550, 65), new Color32(215, 140, 30, 255), Color.white, 22);
        ui.btnSkinNinja = CreateMenuButton(modalObj.transform, "Btn_Skin2", "🥷  NINJA SHINOBI  (Ombre & Vitesse)", new Vector2(0, -25), new Vector2(550, 65), new Color32(50, 50, 65, 255), Color.white, 22);
        ui.btnSkinMage = CreateMenuButton(modalObj.transform, "Btn_Skin3", "🔮  MAGE ARCANISTE  (Magie Violette)", new Vector2(0, -105), new Vector2(550, 65), new Color32(130, 50, 210, 255), Color.white, 22);
        ui.btnSkinKing = CreateMenuButton(modalObj.transform, "Btn_Skin4", "👑  ROI DES PIÈCES  (Couronne Dorée)", new Vector2(0, -185), new Vector2(550, 65), new Color32(220, 180, 20, 255), Color.white, 22);

        ui.btnClose = CreateMenuButton(modalObj.transform, "Btn_CloseChar", "VALIDER & FERMER", new Vector2(0, -280), new Vector2(400, 65), new Color32(40, 160, 90, 255), Color.white, 24);

        modalObj.SetActive(false);
        return ui;
    }

    private static OptionsSettingsMenu CreateOptionsModal(Transform parent)
    {
        GameObject modalObj = new GameObject("Modal_Options");
        modalObj.transform.SetParent(parent, false);

        Image bg = modalObj.AddComponent<Image>();
        bg.color = new Color32(20, 20, 35, 250);
        RectTransform mRect = modalObj.GetComponent<RectTransform>();
        mRect.sizeDelta = new Vector2(620, 640);

        OptionsSettingsMenu ui = modalObj.AddComponent<OptionsSettingsMenu>();

        CreateTMPText(modalObj.transform, "Txt_HeaderIcon", "OPTIONS", 44, FontStyles.Bold, Color.white, new Vector2(0, 250), new Vector2(80, 50));
        CreateTMPText(modalObj.transform, "Txt_Title", "OPTIONS & SONS", 32, FontStyles.Bold, new Color32(255, 215, 0, 255), new Vector2(0, 195), new Vector2(550, 45));

        // Volume Effets Sonores (SFX)
        CreateTMPText(modalObj.transform, "Txt_SFX", "VOLUME DES BRUITAGES", 18, FontStyles.Bold, Color.white, new Vector2(0, 125), new Vector2(500, 30));
        GameObject sfxObj = new GameObject("Slider_SFX");
        sfxObj.transform.SetParent(modalObj.transform, false);
        Slider sfxSlider = sfxObj.AddComponent<Slider>();
        RectTransform sfxRt = sfxObj.GetComponent<RectTransform>();
        sfxRt.anchoredPosition = new Vector2(0, 85);
        sfxRt.sizeDelta = new Vector2(400, 15);
        GameObject sfxFill = new GameObject("Fill");
        sfxFill.transform.SetParent(sfxObj.transform, false);
        Image sfImg = sfxFill.AddComponent<Image>();
        sfImg.color = new Color32(255, 185, 0, 255);
        sfxSlider.fillRect = sfxFill.GetComponent<RectTransform>();
        ui.sfxVolumeSlider = sfxSlider;

        // Volume Musique
        CreateTMPText(modalObj.transform, "Txt_Mus", "VOLUME DE LA MUSIQUE", 18, FontStyles.Bold, Color.white, new Vector2(0, 30), new Vector2(500, 30));
        GameObject musObj = new GameObject("Slider_Mus");
        musObj.transform.SetParent(modalObj.transform, false);
        Slider musSlider = musObj.AddComponent<Slider>();
        RectTransform musRt = musObj.GetComponent<RectTransform>();
        musRt.anchoredPosition = new Vector2(0, -10);
        musRt.sizeDelta = new Vector2(400, 15);
        GameObject musFill = new GameObject("Fill");
        musFill.transform.SetParent(musObj.transform, false);
        Image mfImg = musFill.AddComponent<Image>();
        mfImg.color = new Color32(120, 180, 255, 255);
        musSlider.fillRect = musFill.GetComponent<RectTransform>();
        ui.musicVolumeSlider = musSlider;

        // Réinitialiser la progression
        ui.btnResetProgress = CreateMenuButton(modalObj.transform, "Btn_Reset", "🗑️  RÉINITIALISER LA PROGRESSION", new Vector2(0, -95), new Vector2(440, 55), new Color32(180, 45, 55, 240), Color.white, 18);

        ui.btnClose = CreateMenuButton(modalObj.transform, "Btn_CloseOpt", "FERMER", new Vector2(0, -200), new Vector2(360, 60), new Color32(60, 70, 100, 240), Color.white, 22);

        modalObj.SetActive(false);
        return ui;
    }

    private static LevelCompleteUI CreateVictoryModal(Transform parent)
    {
        GameObject modalObj = new GameObject("Modal_LevelComplete");
        modalObj.transform.SetParent(parent, false);

        Image bg = modalObj.AddComponent<Image>();
        bg.color = new Color32(22, 28, 25, 245);
        RectTransform mRect = modalObj.GetComponent<RectTransform>();
        mRect.sizeDelta = new Vector2(580, 720);

        LevelCompleteUI ui = modalObj.AddComponent<LevelCompleteUI>();

        ui.levelNameHeaderText = CreateTMPText(modalObj.transform, "Txt_Header", "GREEN VALLEY", 20, FontStyles.Bold, new Color32(150, 220, 150, 255), new Vector2(0, 290), new Vector2(400, 30));
        ui.resultTitleText = CreateTMPText(modalObj.transform, "Txt_Title", "NIVEAU TERMINÉ !", 38, FontStyles.Bold, Color.white, new Vector2(0, 245), new Vector2(500, 50));
        ui.resultSubtitleText = CreateTMPText(modalObj.transform, "Txt_Subtitle", "PARFAIT !", 22, FontStyles.Bold, new Color32(255, 200, 50, 255), new Vector2(0, 200), new Vector2(300, 30));

        // 3 Étoiles
        GameObject starsGroup = new GameObject("Stars_Group");
        starsGroup.transform.SetParent(modalObj.transform, false);
        starsGroup.transform.localPosition = new Vector2(0, 130);
        ui.star1 = CreateVictoryStarUI(starsGroup.transform, new Vector2(-75, 0));
        ui.star2 = CreateVictoryStarUI(starsGroup.transform, new Vector2(0, 15), 1.2f);
        ui.star3 = CreateVictoryStarUI(starsGroup.transform, new Vector2(75, 0));
        ui.earnedStarSprite = LoadSprite("Assets/Sprites/UI/UI_Star_Gold.png");
        ui.unearnedStarSprite = LoadSprite("Assets/Sprites/UI/UI_Star_Empty.png");

        // Panneau Stats (Pièces, Score, Temps)
        GameObject statsPanel = new GameObject("Stats_Panel");
        statsPanel.transform.SetParent(modalObj.transform, false);
        Image statsBg = statsPanel.AddComponent<Image>();
        statsBg.color = new Color32(15, 18, 22, 200);
        RectTransform stRect = statsPanel.GetComponent<RectTransform>();
        stRect.anchoredPosition = new Vector2(0, -20);
        stRect.sizeDelta = new Vector2(480, 160);

        CreateTMPText(statsPanel.transform, "L_Coins", "PIÈCES", 20, FontStyles.Bold, Color.white, new Vector2(-120, 45), new Vector2(200, 30));
        ui.coinsCountText = CreateTMPText(statsPanel.transform, "V_Coins", "28 / 30", 22, FontStyles.Bold, new Color32(255, 215, 0, 255), new Vector2(140, 45), new Vector2(150, 30));

        CreateTMPText(statsPanel.transform, "L_Score", "SCORE", 20, FontStyles.Bold, Color.white, new Vector2(-120, 0), new Vector2(200, 30));
        ui.scoreCountText = CreateTMPText(statsPanel.transform, "V_Score", "2 450", 22, FontStyles.Bold, new Color32(255, 200, 50, 255), new Vector2(140, 0), new Vector2(150, 30));

        CreateTMPText(statsPanel.transform, "L_Time", "TEMPS", 20, FontStyles.Bold, Color.white, new Vector2(-120, -45), new Vector2(200, 30));
        ui.timeCountText = CreateTMPText(statsPanel.transform, "V_Time", "01:32", 22, FontStyles.Bold, Color.white, new Vector2(140, -45), new Vector2(150, 30));

        // Boutons
        ui.btnNextLevel = CreateMenuButton(modalObj.transform, "Btn_NextLevel", "NIVEAU SUIVANT →", new Vector2(0, -170), new Vector2(480, 70), new Color32(255, 185, 0, 255), Color.white, 26);
        ui.btnReplay = CreateMenuButton(modalObj.transform, "Btn_Replay", "REJOUER", new Vector2(-125, -255), new Vector2(225, 60), new Color32(130, 70, 230, 255), Color.white, 22);
        ui.btnMenu = CreateMenuButton(modalObj.transform, "Btn_Menu", "MENU", new Vector2(125, -255), new Vector2(225, 60), new Color32(40, 110, 80, 240), Color.white, 22);

        ui.footerBadgeText = CreateTMPText(modalObj.transform, "Footer_Badge", "COLLECTEUR D'ÉLITE — 3 ÉTOILES", 18, FontStyles.Bold, new Color32(255, 215, 0, 220), new Vector2(0, -320), new Vector2(450, 30));

        modalObj.SetActive(false);
        return ui;
    }

    private static GameObject CreateStarUI(Transform parent, Vector2 pos, float scale = 1f)
    {
        GameObject starObj = new GameObject("Star");
        starObj.transform.SetParent(parent, false);
        TextMeshProUGUI tmp = starObj.AddComponent<TextMeshProUGUI>();
        tmp.text = "★";
        tmp.fontSize = 50 * scale;
        tmp.alignment = TextAlignmentOptions.Center;
        RectTransform rt = starObj.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(60, 60);
        return starObj;
    }

    private static GameObject CreateVictoryStarUI(Transform parent, Vector2 pos, float scale = 1f)
    {
        GameObject starObj = new GameObject("Star");
        starObj.transform.SetParent(parent, false);
        Image image = starObj.AddComponent<Image>();
        image.sprite = LoadSprite("Assets/Sprites/UI/UI_Star_Empty.png");
        image.preserveAspect = true;
        image.raycastTarget = false;
        RectTransform rt = starObj.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(60, 60) * scale;
        return starObj;
    }

    private static (Button btnPlay, TextMeshProUGUI txtCoins, TextMeshProUGUI txtScore, GameObject[] stars, GameObject lockOverlay) CreateLevelCard(Transform parent, string name, string levelTag, string diffTag, string title, string desc, Vector2 pos, Color bgColor, bool isLocked)
    {
        GameObject cardObj = new GameObject(name);
        cardObj.transform.SetParent(parent, false);
        Image bg = cardObj.AddComponent<Image>();
        bg.color = bgColor;
        RectTransform rect = cardObj.GetComponent<RectTransform>();
        rect.anchoredPosition = pos;
        rect.sizeDelta = new Vector2(480, 680);

        // Tags en haut
        CreateTMPText(cardObj.transform, "Tag_Num", levelTag, 20, FontStyles.Bold, Color.white, new Vector2(-190, 300), new Vector2(60, 40));
        CreateTMPText(cardObj.transform, "Tag_Diff", diffTag, 16, FontStyles.Bold, new Color32(80, 220, 120, 255), new Vector2(175, 300), new Vector2(100, 35));

        // Titre & Description
        CreateTMPText(cardObj.transform, "Title", title, 28, FontStyles.Bold, Color.white, new Vector2(0, 60), new Vector2(430, 45));
        CreateTMPText(cardObj.transform, "Desc", desc, 16, FontStyles.Normal, new Color32(180, 185, 210, 220), new Vector2(0, -10), new Vector2(430, 65));

        // Étoiles
        GameObject star1 = CreateStarUI(cardObj.transform, new Vector2(-170, -85), 0.7f);
        GameObject star2 = CreateStarUI(cardObj.transform, new Vector2(-130, -85), 0.7f);
        GameObject star3 = CreateStarUI(cardObj.transform, new Vector2(-90, -85), 0.7f);

        // Pièces et Meilleur Score
        TextMeshProUGUI txtCoins = CreateTMPText(cardObj.transform, "Coins", "0/30", 18, FontStyles.Bold, new Color32(255, 215, 0, 255), new Vector2(160, -85), new Vector2(120, 30));
        CreateTMPText(cardObj.transform, "ScoreLabel", "MEILLEUR SCORE", 14, FontStyles.Bold, new Color32(160, 165, 190, 200), new Vector2(-130, -135), new Vector2(160, 30));
        TextMeshProUGUI txtScore = CreateTMPText(cardObj.transform, "ScoreVal", "---", 18, FontStyles.Bold, Color.white, new Vector2(160, -135), new Vector2(120, 30));

        // Bouton Jouer
        Button btnPlay = CreateMenuButton(cardObj.transform, "Btn_PlayCard", "▶  JOUER", new Vector2(0, -240), new Vector2(400, 75), new Color32(255, 185, 0, 255), Color.white, 28);

        // Lock Overlay si verrouillé
        GameObject lockOverlay = new GameObject("LockOverlay");
        lockOverlay.transform.SetParent(cardObj.transform, false);
        Image lImg = lockOverlay.AddComponent<Image>();
        lImg.color = new Color32(15, 12, 28, 220);
        RectTransform lRect = lockOverlay.GetComponent<RectTransform>();
        lRect.sizeDelta = new Vector2(480, 680);

        CreateTMPText(lockOverlay.transform, "LockIcon", "VERROUILLÉ", 60, FontStyles.Bold, Color.white, new Vector2(0, 40), new Vector2(100, 100));
        CreateTMPText(lockOverlay.transform, "LockText", "Terminez le niveau précédent pour débloquer", 18, FontStyles.Bold, new Color32(180, 180, 210, 200), new Vector2(0, -60), new Vector2(380, 60));

        lockOverlay.SetActive(isLocked);

        return (btnPlay, txtCoins, txtScore, new GameObject[] { star1, star2, star3 }, lockOverlay);
    }

    #endregion

    #region 2D OBJECT & SPRITE HELPERS

    private static GameObject CreateCuteCharacter(Vector3 position, bool forMenu = false)
    {
        GameObject root = new GameObject(forMenu ? "MenuPlayerCharacter" : "Player");
        root.transform.position = position;

        Sprite idle = LoadSprite("Assets/Sprites/Characters/Player_Classic_Idle.png");
        Sprite run1 = LoadSprite("Assets/Sprites/Characters/Player_Classic_Run1.png");
        Sprite run2 = LoadSprite("Assets/Sprites/Characters/Player_Classic_Run2.png");
        Sprite jump = LoadSprite("Assets/Sprites/Characters/Player_Classic_Jump.png");
        Sprite hurt = LoadSprite("Assets/Sprites/Characters/Player_Classic_Hurt.png");

        Sprite skinAdv = LoadSprite("Assets/Sprites/Characters/Player_Adventurer_Idle.png");
        Sprite skinNin = LoadSprite("Assets/Sprites/Characters/Player_Ninja_Idle.png");
        Sprite skinMag = LoadSprite("Assets/Sprites/Characters/Player_Mage_Idle.png");
        Sprite skinKng = LoadSprite("Assets/Sprites/Characters/Player_King_Idle.png");

        GameObject visual = new GameObject("Visual_Sprite");
        visual.transform.SetParent(root.transform, false);
        visual.transform.localPosition = forMenu ? Vector3.zero : new Vector3(0, -0.08f, 0);
        visual.transform.localScale = new Vector3(0.38f, 0.38f, 1f);

        SpriteRenderer sr = visual.AddComponent<SpriteRenderer>();
        sr.sprite = idle;
        sr.sortingOrder = 10;

        CharacterSpriteAnimator anim = visual.AddComponent<CharacterSpriteAnimator>();
        anim.idleSprite = idle;
        anim.run1Sprite = run1;
        anim.run2Sprite = run2;
        anim.jumpSprite = jump;
        anim.hurtSprite = hurt;
        anim.skinAdventurer = skinAdv;
        anim.skinNinja = skinNin;
        anim.skinMage = skinMag;
        anim.skinKing = skinKng;

        return root;
    }

    private static void CreateCollectibleCoin(Transform parent, Vector3 position)
    {
        GameObject coin = new GameObject("GoldCoin");
        coin.transform.SetParent(parent);
        coin.transform.position = position;

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

        SphereCollider col = coin.AddComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = 0.45f;

        coin.AddComponent<Coin>();
    }

    private static void CreateSpinningCoinVisual(Transform parent, Vector3 position, float speed)
    {
        GameObject coin = new GameObject("MenuSpinningCoin");
        coin.transform.SetParent(parent);
        coin.transform.position = position;

        GameObject visual = new GameObject("Visual");
        visual.transform.SetParent(coin.transform, false);
        visual.transform.localScale = new Vector3(0.16f, 0.16f, 1f);

        SpriteRenderer sr = visual.AddComponent<SpriteRenderer>();
        Sprite s0 = LoadSprite("Assets/Sprites/Items/Coin_Frame0.png");
        Sprite s1 = LoadSprite("Assets/Sprites/Items/Coin_Frame1.png");
        Sprite s2 = LoadSprite("Assets/Sprites/Items/Coin_Frame2.png");
        Sprite s3 = LoadSprite("Assets/Sprites/Items/Coin_Frame3.png");
        sr.sprite = s0;
        sr.sortingOrder = 3;

        CoinSpriteAnimator anim = visual.AddComponent<CoinSpriteAnimator>();
        anim.spinFrames = new Sprite[] { s0, s1, s2, s3 };
        anim.frameRate = 8f;
        anim.bobAmplitude = 0.12f;
        anim.bobSpeed = speed * 0.02f;
    }

    private static void CreateSimpleEnemy(Vector3 pos, float patrolDist, string enemyType = "slime_green", float moveSpeed = 2.4f)
    {
        GameObject enemy = new GameObject("Enemy_" + enemyType);
        enemy.transform.position = pos;

        GameObject visual = new GameObject("Visual");
        visual.transform.SetParent(enemy.transform, false);
        visual.transform.localScale = new Vector3(0.24f, 0.24f, 1f);

        SpriteRenderer sr = visual.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 6;

        string idlePath = enemyType switch
        {
            "slime_purple" => "Assets/Sprites/Enemies/Slime_Purple_Idle.png",
            "slime_fire" => "Assets/Sprites/Enemies/Slime_Fire_Idle.png",
            _ => "Assets/Sprites/Enemies/Slime_Green_Idle.png"
        };
        Sprite idle = LoadSprite(idlePath);
        Sprite squash = LoadSprite("Assets/Sprites/Enemies/Slime_Green_Squash.png");
        sr.sprite = idle;

        SlimeSpriteAnimator anim = visual.AddComponent<SlimeSpriteAnimator>();
        anim.idleSprite = idle;
        anim.squashSprite = squash;

        BoxCollider col = enemy.AddComponent<BoxCollider>();
        col.isTrigger = true;
        col.size = new Vector3(1.2f, 0.85f, 1.5f);
        col.center = new Vector3(0, 0, 0);

        SimpleEnemy se = enemy.AddComponent<SimpleEnemy>();
        se.patrolDistance = patrolDist;
        se.moveSpeed = moveSpeed;
        se.is2DSprite = true;
    }

    private static void CreateBatEnemy(Vector3 pos, float patrolDist, float moveSpeed = 3f)
    {
        GameObject bat = new GameObject("Enemy_Bat");
        bat.transform.position = pos;

        GameObject visual = new GameObject("Visual");
        visual.transform.SetParent(bat.transform, false);
        visual.transform.localScale = new Vector3(0.24f, 0.24f, 1f);

        SpriteRenderer sr = visual.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 6;

        Sprite f1 = LoadSprite("Assets/Sprites/Enemies/Bat_Fly1.png");
        Sprite f2 = LoadSprite("Assets/Sprites/Enemies/Bat_Fly2.png");
        sr.sprite = f1;

        BatSpriteAnimator anim = visual.AddComponent<BatSpriteAnimator>();
        anim.fly1Sprite = f1;
        anim.fly2Sprite = f2;
        anim.flapRate = 6f;
        anim.waveAmplitude = 0.35f;

        BoxCollider col = bat.AddComponent<BoxCollider>();
        col.isTrigger = true;
        col.size = new Vector3(1.2f, 0.8f, 1.5f);
        col.center = new Vector3(0, 0, 0);

        SimpleEnemy se = bat.AddComponent<SimpleEnemy>();
        se.patrolDistance = patrolDist;
        se.moveSpeed = moveSpeed;
        se.is2DSprite = true;
    }

    private static void CreateMainGround(Vector3 center, float totalWidth, float height, string spritePath)
    {
        GameObject ground = new GameObject("Ground_Main");
        ground.transform.position = center;

        // Collider global pour le sol
        BoxCollider col = ground.AddComponent<BoxCollider>();
        col.size = new Vector3(totalWidth, height, 2.5f);

        // Répétition des tuiles de plateforme le long du sol
        float step = 5.12f;
        int tileCount = Mathf.CeilToInt(totalWidth / step);
        float startX = -totalWidth * 0.5f + step * 0.5f;

        for (int i = 0; i < tileCount; i++)
        {
            GameObject tile = new GameObject("Ground_Tile_" + i);
            tile.transform.SetParent(ground.transform, false);
            tile.transform.localPosition = new Vector3(startX + i * step, 0, 0);
            tile.transform.localScale = new Vector3(1f, height / 1.6f, 1f);

            SpriteRenderer sr = tile.AddComponent<SpriteRenderer>();
            sr.sprite = LoadSprite(spritePath);
            sr.sortingOrder = 2;
        }
    }

    private static void CreateGrassPlatform(Vector3 pos, Vector3 size)
    {
        GameObject p = new GameObject("GrassPlatform");
        p.transform.position = pos;

        BoxCollider col = p.AddComponent<BoxCollider>();
        col.size = new Vector3(size.x, size.y, 2.5f);

        GameObject visual = new GameObject("Visual");
        visual.transform.SetParent(p.transform, false);
        visual.transform.localScale = new Vector3(size.x / 5.12f, size.y / 1.6f, 1f);

        SpriteRenderer sr = visual.AddComponent<SpriteRenderer>();
        sr.sprite = LoadSprite("Assets/Sprites/Environment/Platform_Grass.png");
        sr.sortingOrder = 2;
    }

    private static void CreateCavePlatform(Vector3 pos, Vector3 size)
    {
        GameObject p = new GameObject("CavePlatform");
        p.transform.position = pos;

        BoxCollider col = p.AddComponent<BoxCollider>();
        col.size = new Vector3(size.x, size.y, 2.5f);

        GameObject visual = new GameObject("Visual");
        visual.transform.SetParent(p.transform, false);
        visual.transform.localScale = new Vector3(size.x / 5.12f, size.y / 1.6f, 1f);

        SpriteRenderer sr = visual.AddComponent<SpriteRenderer>();
        sr.sprite = LoadSprite("Assets/Sprites/Environment/Platform_Crystal.png");
        sr.sortingOrder = 2;
    }

    private static void CreateSkyIsland(Vector3 pos, Vector3 size)
    {
        GameObject p = new GameObject("SkyIsland");
        p.transform.position = pos;

        BoxCollider col = p.AddComponent<BoxCollider>();
        col.size = new Vector3(size.x, size.y, 2.5f);

        GameObject visual = new GameObject("Visual");
        visual.transform.SetParent(p.transform, false);
        float verticalScale = size.y / 1.2f;
        visual.transform.localPosition = new Vector3(0f, 0.12f * verticalScale, 0f);
        visual.transform.localScale = new Vector3(size.x / 5.12f, verticalScale, 1f);

        SpriteRenderer sr = visual.AddComponent<SpriteRenderer>();
        sr.sprite = LoadSprite("Assets/Sprites/Environment/Platform_Wood.png");
        sr.sortingOrder = 2;
    }

    private static void CreateSpikeTrap(Vector3 pos)
    {
        GameObject trap = new GameObject("SpikeTrap");
        trap.transform.position = pos;

        GameObject visual = new GameObject("Visual");
        visual.transform.SetParent(trap.transform, false);
        visual.transform.localScale = new Vector3(0.32f, 0.32f, 1f);

        SpriteRenderer sr = visual.AddComponent<SpriteRenderer>();
        sr.sprite = LoadSprite("Assets/Sprites/Environment/Spikes_Hazard.png");
        sr.sortingOrder = 4;

        BoxCollider col = trap.AddComponent<BoxCollider>();
        col.isTrigger = true;
        col.size = new Vector3(1.5f, 0.75f, 1.5f);
        col.center = new Vector3(0, 0.2f, 0);

        trap.AddComponent<SpikeHazard>();
    }

    private static void CreateCloud(Transform parent, Vector3 pos, float scale = 1f)
    {
        GameObject cloud = new GameObject("Cloud_Decor");
        cloud.transform.SetParent(parent);
        cloud.transform.position = pos;
        cloud.transform.localScale = new Vector3(scale * 0.45f, scale * 0.45f, 1f);

        SpriteRenderer sr = cloud.AddComponent<SpriteRenderer>();
        sr.sprite = LoadSprite("Assets/Sprites/Environment/Cloud_Fluffy.png");
        sr.sortingOrder = 1;
    }

    private static GameObject CreateFlagExit(Vector3 pos, int minimumCoins)
    {
        GameObject exitObj = new GameObject("Exit_Flag");
        exitObj.transform.position = pos;

        GameObject visual = new GameObject("Visual");
        visual.transform.SetParent(exitObj.transform, false);
        visual.transform.localScale = new Vector3(0.48f, 0.48f, 1f);

        SpriteRenderer sr = visual.AddComponent<SpriteRenderer>();
        sr.sprite = LoadSprite("Assets/Sprites/Environment/Exit_Flag.png");
        sr.sortingOrder = 5;

        BoxCollider col = exitObj.AddComponent<BoxCollider>();
        col.isTrigger = true;
        col.size = new Vector3(2f, 2.6f, 2f);
        col.center = new Vector3(0, 0.3f, 0);

        exitObj.AddComponent<LevelExit>().minimumCoinsToWin = minimumCoins;
        return exitObj;
    }

    private static GameObject CreatePortalExit(Vector3 pos, int minimumCoins)
    {
        GameObject exitObj = new GameObject("Exit_Portal");
        exitObj.transform.position = pos;

        GameObject visual = new GameObject("Visual");
        visual.transform.SetParent(exitObj.transform, false);
        visual.transform.localScale = new Vector3(0.55f, 0.55f, 1f);

        SpriteRenderer sr = visual.AddComponent<SpriteRenderer>();
        sr.sprite = LoadSprite("Assets/Sprites/Environment/Exit_Portal.png");
        sr.sortingOrder = 5;

        BoxCollider col = exitObj.AddComponent<BoxCollider>();
        col.isTrigger = true;
        col.size = new Vector3(2.5f, 2.8f, 2f);

        exitObj.AddComponent<LevelExit>().minimumCoinsToWin = minimumCoins;
        return exitObj;
    }

    private static GameObject CreateGoldenGateExit(Vector3 pos, int minimumCoins)
    {
        GameObject exitObj = new GameObject("Exit_GoldenGate");
        exitObj.transform.position = pos;

        GameObject visual = new GameObject("Visual");
        visual.transform.SetParent(exitObj.transform, false);
        visual.transform.localScale = new Vector3(0.6f, 0.6f, 1f);

        SpriteRenderer sr = visual.AddComponent<SpriteRenderer>();
        sr.sprite = LoadSprite("Assets/Sprites/Environment/Exit_GoldenGate.png");
        sr.sortingOrder = 5;

        BoxCollider col = exitObj.AddComponent<BoxCollider>();
        col.isTrigger = true;
        col.size = new Vector3(3f, 3.2f, 2f);

        exitObj.AddComponent<LevelExit>().minimumCoinsToWin = minimumCoins;
        return exitObj;
    }

    private static void CreateDirectionalLight(Vector3 rot, Color color, float intensity)
    {
        GameObject lightObj = new GameObject("Directional Light");
        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Directional;
        light.color = color;
        light.intensity = intensity;
        light.transform.rotation = Quaternion.Euler(rot);
    }

    private static void CreateManagersInScene()
    {
        if (Object.FindFirstObjectByType<GameManager>() == null)
        {
            GameObject gm = new GameObject("GameManager");
            gm.AddComponent<GameManager>();
        }
        if (Object.FindFirstObjectByType<SoundManager>() == null)
        {
            GameObject sm = new GameObject("SoundManager");
            sm.AddComponent<SoundManager>();
        }
    }

    #endregion

    #region GENERAL HELPERS

    private static Canvas CreateCanvas(string name)
    {
        GameObject canvasObj = new GameObject(name);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0f;
        canvasObj.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    private static void CreateEventSystem()
    {
        if (Object.FindFirstObjectByType<EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }
    }

    private static TextMeshProUGUI CreateTMPText(Transform parent, string name, string text, float fontSize, FontStyles style, Color color, Vector2 pos, Vector2 size, bool bottomAnchor = false)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.fontStyle = style;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;

        RectTransform rect = obj.GetComponent<RectTransform>();
        if (bottomAnchor)
        {
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
        }
        else
        {
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
        }
        rect.anchoredPosition = pos;
        rect.sizeDelta = size;
        return tmp;
    }

    private static Button CreateMenuButton(Transform parent, string name, string label, Vector2 pos, Vector2 size, Color bgColor, Color textColor, float fontSize)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        Image img = obj.AddComponent<Image>();
        img.color = bgColor;

        Button btn = obj.AddComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.highlightedColor = Color.Lerp(bgColor, Color.white, 0.3f);
        cb.pressedColor = Color.Lerp(bgColor, Color.black, 0.25f);
        btn.colors = cb;

        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = pos;
        rect.sizeDelta = size;

        GameObject txtObj = new GameObject("Text");
        txtObj.transform.SetParent(obj.transform, false);
        TextMeshProUGUI tmp = txtObj.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = fontSize;
        tmp.fontStyle = FontStyles.Bold;
        tmp.color = textColor;
        tmp.alignment = TextAlignmentOptions.Center;

        RectTransform tRect = txtObj.GetComponent<RectTransform>();
        tRect.anchorMin = Vector2.zero;
        tRect.anchorMax = Vector2.one;
        tRect.offsetMin = Vector2.zero;
        tRect.offsetMax = Vector2.zero;

        return btn;
    }

    private static void AddEventTrigger(GameObject obj, EventTriggerType type, UnityEngine.Events.UnityAction action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>() ?? obj.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener((data) => action());
        trigger.triggers.Add(entry);
    }

    private static void SetupAllScenesInBuildSettings()
    {
        string[] scenePaths = new string[]
        {
            "Assets/Scenes/MainMenu.unity",
            "Assets/Scenes/LevelSelect.unity",
            "Assets/Scenes/Level_01_GreenValley.unity",
            "Assets/Scenes/Level_02_CrystalCave.unity",
            "Assets/Scenes/Level_03_SkyTemple.unity"
        };

        EditorBuildSettingsScene[] scenes = new EditorBuildSettingsScene[scenePaths.Length];
        for (int i = 0; i < scenePaths.Length; i++)
        {
            scenes[i] = new EditorBuildSettingsScene(scenePaths[i], true);
        }
        EditorBuildSettings.scenes = scenes;
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    #endregion
}
