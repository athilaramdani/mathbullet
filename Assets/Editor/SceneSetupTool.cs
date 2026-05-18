using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEditor.Events;
using UnityEngine.EventSystems;

public class SceneSetupTool : EditorWindow
{
    // ═══════════════════════════════════════════
    // SATU TOMBOL — hapus semua, buat dari awal
    // ═══════════════════════════════════════════
    [MenuItem("MathBullet/▶ SETUP GAME (One Click)")]
    public static void SetupEverything()
    {
        if (!EditorUtility.DisplayDialog("Math Bullet Setup",
            "Hapus semua object lama dan setup ulang dari awal?\n(Player/Gun/GunSprite tetap aman)", "Ya!", "Batal")) return;

        AssetDatabase.Refresh();

        NukeScene(); // Hapus lama di scene saat ini (SampleScene/scene aktif)
        CreateAllPrefabs();
        BuildGameScene();  // Buka GameScene, nuke lagi di dalamnya, lalu build
        BuildMainMenuScene();
        AutoAssignAudio();

        EditorSceneManager.MarkAllScenesDirty();
        AssetDatabase.SaveAssets();

        EditorUtility.DisplayDialog("✅ Selesai!",
            "Game siap!\n\n" +
            "1. File > Build Settings > tambah MainMenu(0) & GameScene(1)\n" +
            "2. Taruh .wav di Assets/Audio/ lalu tekan Setup lagi\n" +
            "3. ▶ Play!", "OK");
    }


    // Auto-assign audio dipanggil di akhir SetupEverything
    static void AutoAssignAudio()
    {
        string p = "Assets/Audio/";

        // Force import semua audio agar Unity kenal file-filenya
        string[] audioFiles = {
            p+"shoot.wav", p+"benar.wav", p+"freezesound.wav", p+"ost1.wav",
            p+"zombiedead1.wav", p+"zombiedead2.wav", p+"zombiedead3.wav", p+"zombiedead4.wav"
        };
        foreach (var f in audioFiles)
        {
            if (System.IO.File.Exists(f))
                AssetDatabase.ImportAsset(f, ImportAssetOptions.ForceUpdate);
        }
        AssetDatabase.Refresh();

        var sm = Object.FindObjectOfType<SoundManager>();
        if (sm == null) { Debug.LogWarning("[MB] SoundManager not found, skip audio."); return; }

        var shoot   = AssetDatabase.LoadAssetAtPath<AudioClip>(p + "shoot.wav");
        var correct = AssetDatabase.LoadAssetAtPath<AudioClip>(p + "benar.wav");
        var freeze  = AssetDatabase.LoadAssetAtPath<AudioClip>(p + "freezesound.wav");
        var ost     = AssetDatabase.LoadAssetAtPath<AudioClip>(p + "ost1.wav");

        if (shoot   != null) sm.shootSound   = shoot;
        if (correct != null) sm.correctSound = correct;
        if (freeze  != null) sm.freezeSound  = freeze;
        if (ost     != null) sm.ostMusic     = ost;

        // Zombie dead sounds
        var deadClips = new System.Collections.Generic.List<AudioClip>();
        for (int i = 1; i <= 4; i++)
        {
            string path = p + "zombiedead" + i + ".wav";
            var c = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            if (c != null)
            {
                deadClips.Add(c);
                Debug.Log("[MB] Loaded: " + path);
            }
            else
                Debug.LogWarning("[MB] TIDAK DITEMUKAN: " + path);
        }
        if (deadClips.Count > 0) sm.zombieDeadSounds = deadClips.ToArray();
        Debug.Log("[MB] zombieDeadSounds assigned: " + deadClips.Count + " clips");

        EditorUtility.SetDirty(sm);
        EditorSceneManager.MarkSceneDirty(sm.gameObject.scene);
    }


    // ─────────────────────────────────────────
    // STEP 1: Hapus SEMUA kecuali Player hierarchy
    // ─────────────────────────────────────────
    static void NukeScene()
    {
        // Hapus semua Canvas
        foreach (var c in Object.FindObjectsOfType<Canvas>())
            if (c) Object.DestroyImmediate(c.gameObject);

        // Hapus semua Camera (akan dibuat baru)
        foreach (var c in Object.FindObjectsOfType<Camera>())
            if (c) Object.DestroyImmediate(c.gameObject);

        // Hapus manager objects by name
        string[] kill = { "GameManager","EnemySpawner","ItemSpawner","SoundManager",
                          "EventSystem","MathUIRefs","Background","DirectionalLight","Directional Light" };
        var all = Object.FindObjectsOfType<GameObject>();
        foreach (var go in all)
            foreach (var k in kill)
                if (go && go.name == k) Object.DestroyImmediate(go);

        Debug.Log("[MB] Scene cleared.");
    }

    // ─────────────────────────────────────────
    // STEP 2: Buat prefab
    // ─────────────────────────────────────────
    static void CreateAllPrefabs()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
            AssetDatabase.CreateFolder("Assets", "Prefabs");

        MakeBullet();
        MakeEnemy();
        MakeMathAmmo();
        MakePowerUp(PowerUpType.Heal,          "PowerUp_Heal",   "Assets/Sprites/Powerup_Heal.png");
        MakePowerUp(PowerUpType.FreezeEnemies, "PowerUp_Freeze", "Assets/Sprites/Powerup_Freeze.png");
        MakePowerUp(PowerUpType.Shield,        "PowerUp_Shield", "Assets/Sprites/Powerup_Shield.png");
        MakePowerUp(PowerUpType.SpeedBoost,    "PowerUp_Speed",  "Assets/Sprites/Powerup_Speed.png");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    static Sprite ImportSprite(string path)
    {
        var ti = AssetImporter.GetAtPath(path) as TextureImporter;
        if (ti != null)
        {
            ti.textureType = TextureImporterType.Sprite;
            ti.spriteImportMode = SpriteImportMode.Single;
            ti.alphaIsTransparency = true;
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    static void SavePrefab(GameObject go, string name)
    {
        PrefabUtility.SaveAsPrefabAsset(go, "Assets/Prefabs/" + name + ".prefab");
        Object.DestroyImmediate(go);
    }

    static void MakeBullet()
    {
        var go = new GameObject("Bullet");
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = ImportSprite("Assets/Sprites/Bullet.png");
        sr.sortingOrder = 12;
        go.transform.localScale = new Vector3(0.25f, 0.12f, 1f);
        var rb = go.AddComponent<Rigidbody2D>(); rb.gravityScale = 0;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        var col = go.AddComponent<CapsuleCollider2D>();
        col.size = new Vector2(0.8f, 0.4f); col.isTrigger = true;
        go.AddComponent<Bullet>();
        SavePrefab(go, "Bullet");
    }

    static void MakeEnemy()
    {
        // Ensure tag exists
        EnsureTag("Enemy");
        var go = new GameObject("Enemy"); go.tag = "Enemy";
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = ImportSprite("Assets/Sprites/Enemy.png");
        sr.sortingOrder = 8;
        go.transform.localScale = Vector3.one * 0.55f;
        var rb = go.AddComponent<Rigidbody2D>(); rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        go.AddComponent<CircleCollider2D>().radius = 0.5f;
        go.AddComponent<Enemy>();
        SavePrefab(go, "Enemy");
    }

    static void MakeMathAmmo()
    {
        var go = new GameObject("MathAmmo");
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = ImportSprite("Assets/Sprites/MathAmmo.png");
        sr.sortingOrder = 6;
        go.transform.localScale = Vector3.one * 0.5f;
        go.AddComponent<CircleCollider2D>().isTrigger = true;
        go.AddComponent<MathAmmo>();
        go.AddComponent<BobAnimation>();
        SavePrefab(go, "MathAmmo");
    }

    static void MakePowerUp(PowerUpType type, string prefabName, string spritePath)
    {
        var go = new GameObject(prefabName);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = ImportSprite(spritePath);
        sr.sortingOrder = 6;
        go.transform.localScale = Vector3.one * 0.5f;
        go.AddComponent<CircleCollider2D>().isTrigger = true;
        var pu = go.AddComponent<PowerUp>(); pu.powerUpType = type;
        go.AddComponent<BobAnimation>();
        SavePrefab(go, prefabName);
    }

    static void EnsureTag(string tag)
    {
        var tm = new SerializedObject(
            AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        var tags = tm.FindProperty("tags");
        for (int i = 0; i < tags.arraySize; i++)
            if (tags.GetArrayElementAtIndex(i).stringValue == tag) return;
        tags.InsertArrayElementAtIndex(tags.arraySize);
        tags.GetArrayElementAtIndex(tags.arraySize - 1).stringValue = tag;
        tm.ApplyModifiedProperties();
    }

    // ─────────────────────────────────────────
    // STEP 3: Build GameScene
    // ─────────────────────────────────────────
    static void BuildGameScene()
    {
        string path = "Assets/Scenes/GameScene.unity";
        var scene = System.IO.File.Exists(path)
            ? EditorSceneManager.OpenScene(path)
            : EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Nuke LAGI setelah buka GameScene (hapus UI lama yang tersimpan)
        NukeScene();

        // Camera — SATU AudioListener saja
        var camGO = new GameObject("Main Camera"); camGO.tag = "MainCamera";
        var cam = camGO.AddComponent<Camera>();
        cam.orthographic = true; cam.orthographicSize = 6f;
        cam.backgroundColor = new Color(0.07f, 0.07f, 0.12f);
        cam.clearFlags = CameraClearFlags.SolidColor;
        camGO.AddComponent<AudioListener>(); // satu-satunya AudioListener
        camGO.transform.position = new Vector3(0, 0, -10);
        var follow = camGO.AddComponent<CameraFollow>();

        // Background
        var bg = new GameObject("Background");
        var bsr = bg.AddComponent<SpriteRenderer>();
        bsr.sprite = ImportSprite("Assets/Sprites/Background.png");
        bsr.sortingOrder = -10; bsr.drawMode = SpriteDrawMode.Tiled;
        bsr.size = new Vector2(50f, 50f);

        // Player
        var player = SetupPlayer();
        follow.target = player.transform;

        // Managers (TIDAK ada AudioListener di sini!)
        var gmGO = new GameObject("GameManager"); gmGO.AddComponent<GameManager>();

        var esGO = new GameObject("EnemySpawner");
        var es = esGO.AddComponent<EnemySpawner>();
        es.enemyPrefab = Load<GameObject>("Enemy");

        var isGO = new GameObject("ItemSpawner");
        var isp = isGO.AddComponent<ItemSpawner>();
        isp.mathAmmoPrefab = Load<GameObject>("MathAmmo");
        isp.powerUpPrefabs = new GameObject[] {
            Load<GameObject>("PowerUp_Heal"), Load<GameObject>("PowerUp_Freeze"),
            Load<GameObject>("PowerUp_Shield"), Load<GameObject>("PowerUp_Speed")
        };

        // SoundManager — AudioSource saja, BUKAN AudioListener
        var smGO = new GameObject("SoundManager");
        smGO.AddComponent<SoundManager>();
        smGO.AddComponent<AudioSource>(); // music
        smGO.AddComponent<AudioSource>(); // sfx

        // EventSystem
        var evGO = new GameObject("EventSystem");
        evGO.AddComponent<EventSystem>();
        evGO.AddComponent<StandaloneInputModule>();

        // UI
        BuildUI(gmGO.GetComponent<GameManager>());

        EditorSceneManager.SaveScene(scene, path);
        Debug.Log("[MB] GameScene saved.");
    }

    static T Load<T>(string prefabName) where T : Object
        => AssetDatabase.LoadAssetAtPath<T>("Assets/Prefabs/" + prefabName + ".prefab");

    static GameObject SetupPlayer()
    {
        EnsureTag("Player");

        var existing = GameObject.Find("Player");
        if (existing != null)
        {
            existing.tag = "Player";

            // Sort player on top
            var sr2 = existing.GetComponent<SpriteRenderer>();
            if (sr2) sr2.sortingOrder = 10;

            // Add missing components
            if (!existing.GetComponent<PlayerVisual>()) existing.AddComponent<PlayerVisual>();
            if (!existing.GetComponent<PlayerMovement>()) existing.AddComponent<PlayerMovement>();
            if (!existing.GetComponent<PlayerShooting>()) existing.AddComponent<PlayerShooting>();
            if (!existing.GetComponent<Rigidbody2D>())
            {
                var rb = existing.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0; rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
            if (!existing.GetComponent<CircleCollider2D>())
                existing.AddComponent<CircleCollider2D>().radius = 0.5f;

            var gun = existing.transform.Find("Gun");
            var fp  = gun != null ? gun.Find("FirePoint") : null;

            // Create FirePoint if missing
            if (gun != null && fp == null)
            {
                var fpGO = new GameObject("FirePoint");
                fpGO.transform.SetParent(gun);
                fpGO.transform.localPosition = new Vector3(0.6f, 0, 0);
                fp = fpGO.transform;
            }

            // Fix GunSprite sorting
            if (gun != null)
            {
                var gs = gun.Find("GunSprite");
                if (gs) { var gsr = gs.GetComponent<SpriteRenderer>(); if (gsr) gsr.sortingOrder = 11; }
            }

            var ps = existing.GetComponent<PlayerShooting>();
            if (gun) ps.gunPivot = gun;
            if (fp)  ps.firePoint = fp;
            ps.bulletPrefab = Load<GameObject>("Bullet");
            return existing;
        }

        // New player
        var p = new GameObject("Player"); p.tag = "Player";
        p.AddComponent<PlayerVisual>();
        var psr = p.AddComponent<SpriteRenderer>(); psr.sortingOrder = 10;
        var prb = p.AddComponent<Rigidbody2D>(); prb.gravityScale = 0;
        prb.constraints = RigidbodyConstraints2D.FreezeRotation;
        p.AddComponent<CircleCollider2D>().radius = 0.5f;
        p.AddComponent<PlayerMovement>();

        var gunGO = new GameObject("Gun"); gunGO.transform.SetParent(p.transform); gunGO.transform.localPosition = Vector3.zero;
        var fpNew = new GameObject("FirePoint"); fpNew.transform.SetParent(gunGO.transform); fpNew.transform.localPosition = new Vector3(0.6f, 0, 0);
        var gsGO = new GameObject("GunSprite"); gsGO.transform.SetParent(gunGO.transform);
        gsGO.transform.localPosition = new Vector3(0.35f, 0, 0); gsGO.transform.localScale = new Vector3(0.7f, 0.2f, 1f);
        var gsR = gsGO.AddComponent<SpriteRenderer>(); gsR.color = new Color(0.7f, 0.7f, 0.7f); gsR.sortingOrder = 11;

        var shooting = p.AddComponent<PlayerShooting>();
        shooting.gunPivot = gunGO.transform; shooting.firePoint = fpNew.transform;
        shooting.bulletPrefab = Load<GameObject>("Bullet");
        return p;
    }

    // ─────────────────────────────────────────
    // STEP 4: Build UI
    // ─────────────────────────────────────────
    static void BuildUI(GameManager gm)
    {
        // Canvas
        var cvGO = new GameObject("UICanvas");
        var cv = cvGO.AddComponent<Canvas>(); cv.renderMode = RenderMode.ScreenSpaceOverlay;
        var cs = cvGO.AddComponent<CanvasScaler>();
        cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(1920, 1080);
        cvGO.AddComponent<GraphicRaycaster>();

        // HUD — transparent background, TIDAK blokir klik!
        var hud = MakePanel(cvGO.transform, "HUD_Panel", Color.clear);
        var hudImg = hud.GetComponent<Image>();
        hudImg.raycastTarget = false; // PENTING: jangan blokir klik di bawah
        Stretch(hud);
        var hp  = MakeTMP(hud.transform, "HealthText",  "HP: 5/5",  30, new Vector2(25,-30),  TextAlignmentOptions.TopLeft);
        var ap  = MakeTMP(hud.transform, "AmmoText",    "Ammo: 30", 30, new Vector2(25,-70),  TextAlignmentOptions.TopLeft);
        var sp  = MakeTMP(hud.transform, "ScoreText",   "Score: 0", 30, new Vector2(-25,-30), TextAlignmentOptions.TopRight);
        var bsp = MakeTMP(hud.transform, "HighScore",   "Best: 0",  24, new Vector2(-25,-70), TextAlignmentOptions.TopRight);
        // Nonaktifkan raycast pada semua teks HUD
        hp.GetComponent<TextMeshProUGUI>().raycastTarget  = false;
        ap.GetComponent<TextMeshProUGUI>().raycastTarget  = false;
        sp.GetComponent<TextMeshProUGUI>().raycastTarget  = false;
        bsp.GetComponent<TextMeshProUGUI>().raycastTarget = false;
        SetAnchor(hp, 0, 1); SetAnchor(ap, 0, 1); SetAnchor(sp, 1, 1); SetAnchor(bsp, 1, 1);

        // MathPanel
        var mp  = MakePanel(cvGO.transform, "MathPanel", new Color(0,0,0,0.92f));
        var mrt = mp.GetComponent<RectTransform>(); mrt.sizeDelta = new Vector2(480,340); mrt.anchoredPosition = Vector2.zero;
        mp.SetActive(false);
        var qt  = MakeTMP(mp.transform, "QuestionText", "? + ? = ?", 44, new Vector2(0, 100), TextAlignmentOptions.Center);
        var fbt = MakeTMP(mp.transform, "FeedbackText", "",          22, new Vector2(0,-110), TextAlignmentOptions.Center);

        // InputField
        var inGO = new GameObject("AnswerInput"); inGO.transform.SetParent(mp.transform, false);
        var inRT = inGO.AddComponent<RectTransform>(); inRT.sizeDelta = new Vector2(220, 55); inRT.anchoredPosition = new Vector2(0, 15);
        inGO.AddComponent<Image>().color = new Color(1,1,1,0.18f);
        var tmpIF = inGO.AddComponent<TMPro.TMP_InputField>();
        var phGO = MakeTMP(inGO.transform, "Placeholder", "Ketik angka...", 20, Vector2.zero, TextAlignmentOptions.Center);
        phGO.GetComponent<TextMeshProUGUI>().color = new Color(1,1,1,0.4f);
        phGO.GetComponent<RectTransform>().sizeDelta = new Vector2(210, 50);
        var txGO = MakeTMP(inGO.transform, "Text", "", 26, Vector2.zero, TextAlignmentOptions.Center);
        txGO.GetComponent<RectTransform>().sizeDelta = new Vector2(210, 50);
        tmpIF.placeholder = phGO.GetComponent<TMPro.TMP_Text>();
        tmpIF.textComponent = txGO.GetComponent<TextMeshProUGUI>();
        tmpIF.contentType = TMPro.TMP_InputField.ContentType.IntegerNumber;

        // Submit Button
        var btnGO = new GameObject("SubmitButton"); btnGO.transform.SetParent(mp.transform, false);
        var btnRT = btnGO.AddComponent<RectTransform>(); btnRT.sizeDelta = new Vector2(180,55); btnRT.anchoredPosition = new Vector2(0,-50);
        btnGO.AddComponent<Image>().color = new Color(0.15f, 0.8f, 0.25f);
        var btn = btnGO.AddComponent<Button>();
        var btnTxt = MakeTMP(btnGO.transform, "L", "JAWAB", 26, Vector2.zero, TextAlignmentOptions.Center);
        btnTxt.GetComponent<RectTransform>().sizeDelta = new Vector2(170, 50);
        // Persistent listener
        var sbh = btnGO.AddComponent<SubmitButtonHandler>();
        UnityEventTools.AddPersistentListener(btn.onClick, sbh.Submit);

        // MathUIRefs
        var mur = new GameObject("MathUIRefs").AddComponent<MathUIRefs>();
        mur.mathPanel = mp; mur.questionText = qt.GetComponent<TextMeshProUGUI>();
        mur.answerInput = tmpIF; mur.feedbackText = fbt.GetComponent<TextMeshProUGUI>();

        // GameOver Panel — harus di atas HUD jadi bisa diklik
        var gop = MakePanel(cvGO.transform, "GameOverPanel", new Color(0,0,0,0.88f));
        Stretch(gop); gop.SetActive(false);
        // Pastikan GameOver Panel bisa menerima klik (raycastTarget default true sudah oke)
        var got = MakeTMP(gop.transform, "GOTitle",  "GAME OVER", 80, new Vector2(0,160), TextAlignmentOptions.Center);
        got.GetComponent<TextMeshProUGUI>().color = new Color(1f,0.3f,0.3f);
        var gos = MakeTMP(gop.transform, "GOScore", "Score: 0", 38, new Vector2(0, 60), TextAlignmentOptions.Center);
        var goh = MakeTMP(gop.transform, "GOHigh",  "Best: 0",  28, new Vector2(0, 10), TextAlignmentOptions.Center);
        var rbtn = MakeBtn(gop.transform, "RestartButton", "MAIN LAGI", new Vector2(0,-70),  new Color(0.2f,0.7f,1f));
        var qbtn = MakeBtn(gop.transform, "QuitButton",    "KELUAR",    new Vector2(0,-145), new Color(0.8f,0.2f,0.2f));

        // Persistent GameOver buttons
        var rp = rbtn.AddComponent<GameManagerProxy>(); UnityEventTools.AddPersistentListener(rbtn.GetComponent<Button>().onClick, rp.Restart);
        var qp = qbtn.AddComponent<GameManagerProxy>(); UnityEventTools.AddPersistentListener(qbtn.GetComponent<Button>().onClick, qp.Quit);

        // Wire GameManager
        if (gm)
        {
            gm.healthText = hp.GetComponent<TextMeshProUGUI>();
            gm.ammoText   = ap.GetComponent<TextMeshProUGUI>();
            gm.scoreText  = sp.GetComponent<TextMeshProUGUI>();
            gm.highScoreText = bsp.GetComponent<TextMeshProUGUI>();
            gm.gameOverPanel = gop;
            gm.gameOverScoreText = gos.GetComponent<TextMeshProUGUI>();
            gm.gameOverHighScoreText = goh.GetComponent<TextMeshProUGUI>();
        }
    }

    // ─────────────────────────────────────────
    // STEP 5: Build MainMenu scene
    // ─────────────────────────────────────────
    static void BuildMainMenuScene()
    {
        string path = "Assets/Scenes/MainMenu.unity";
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);

        var camGO = new GameObject("Main Camera"); camGO.tag = "MainCamera";
        var cam = camGO.AddComponent<Camera>(); cam.orthographic = true;
        cam.backgroundColor = new Color(0.05f, 0.05f, 0.1f);
        camGO.AddComponent<AudioListener>(); // SATU AudioListener
        camGO.transform.position = new Vector3(0,0,-10);

        // EventSystem untuk menu
        var evGO = new GameObject("EventSystem");
        evGO.AddComponent<EventSystem>(); evGO.AddComponent<StandaloneInputModule>();

        var bgGO = new GameObject("MenuBG");
        var bgsr = bgGO.AddComponent<SpriteRenderer>();
        bgsr.sprite = ImportSprite("Assets/Sprites/MenuBackground.png");
        bgsr.sortingOrder = -10; bgGO.transform.localScale = new Vector3(13f,7.5f,1f);

        var cvGO = new GameObject("MenuCanvas");
        var cv = cvGO.AddComponent<Canvas>(); cv.renderMode = RenderMode.ScreenSpaceOverlay;
        var cs = cvGO.AddComponent<CanvasScaler>();
        cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(1920,1080);
        cvGO.AddComponent<GraphicRaycaster>();

        var title = MakeTMP(cvGO.transform, "Title", "MATH BULLET", 100, new Vector2(0,210), TextAlignmentOptions.Center);
        title.GetComponent<RectTransform>().sizeDelta = new Vector2(900,130);
        title.GetComponent<TextMeshProUGUI>().color = new Color(0.2f,1f,0.4f);
        title.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        MakeTMP(cvGO.transform, "Sub", "Tembak zombie dengan matematika!", 32, new Vector2(0,120), TextAlignmentOptions.Center)
            .GetComponent<TextMeshProUGUI>().color = new Color(0.8f,0.9f,1f);

        var pBtn = MakeBtn(cvGO.transform, "PlayButton", "▶  MAIN SEKARANG", new Vector2(0,10), new Color(0.15f,0.75f,0.3f));
        pBtn.GetComponent<RectTransform>().sizeDelta = new Vector2(340,72);
        var qBtn = MakeBtn(cvGO.transform, "QuitButton", "KELUAR", new Vector2(0,-80), new Color(0.6f,0.15f,0.15f));

        MakeTMP(cvGO.transform, "Controls",
            "WASD: Gerak  |  Mouse: Arah  |  Klik Kiri: Tembak  |  Dekati ?: Soal Matematika",
            20, new Vector2(0,-260), TextAlignmentOptions.Center)
            .GetComponent<TextMeshProUGUI>().color = new Color(0.5f,0.65f,0.8f);

        var mm = cvGO.AddComponent<MainMenu>();
        mm.playButton = pBtn.GetComponent<Button>();
        mm.quitButton = qBtn.GetComponent<Button>();
        mm.titleTransform = title.transform;

        EditorSceneManager.SaveScene(scene, path);
        EditorSceneManager.OpenScene("Assets/Scenes/GameScene.unity");
    }




    // ─────────────────────────────────────────
    // UI Helpers
    // ─────────────────────────────────────────
    static GameObject MakePanel(Transform parent, string name, Color color)
    {
        var go = new GameObject(name); go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>(); rt.sizeDelta = new Vector2(400,300);
        go.AddComponent<Image>().color = color;
        return go;
    }

    static GameObject MakeTMP(Transform parent, string name, string text, int size, Vector2 pos, TextAlignmentOptions align)
    {
        var go = new GameObject(name); go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>(); rt.sizeDelta = new Vector2(500,55); rt.anchoredPosition = pos;
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text; tmp.fontSize = size; tmp.alignment = align; tmp.color = Color.white;
        return go;
    }

    static GameObject MakeBtn(Transform parent, string name, string label, Vector2 pos, Color color)
    {
        var go = new GameObject(name); go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>(); rt.sizeDelta = new Vector2(240,60); rt.anchoredPosition = pos;
        go.AddComponent<Image>().color = color;
        go.AddComponent<Button>();
        var t = MakeTMP(go.transform, "L", label, 26, Vector2.zero, TextAlignmentOptions.Center);
        t.GetComponent<RectTransform>().sizeDelta = new Vector2(230,55);
        return go;
    }

    static void Stretch(GameObject go)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }

    static void SetAnchor(GameObject go, float ax, float ay)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(ax, ay);
        rt.sizeDelta = new Vector2(380, 45);
    }
}
