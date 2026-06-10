#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

namespace MiaFantasyRun.Editor
{
    public static class SceneSetupMenu
    {
        private const string MiaRunnerSpritePath = "Assets/_MiaFantasyRun/Art/Characters/mia-modern-front.png";
        private const string MiaBackSpritePath = "Assets/_MiaFantasyRun/Art/Characters/mia-modern-back.png";
        private const string MiaRunSheetPath = "Assets/_MiaFantasyRun/Art/Characters/mia-run-cycle.png";
        private const string MagicShieldSpritePath = "Assets/_MiaFantasyRun/Art/Pickups/magic-shield.png";
        private const string CoinUiSpritePath = "Assets/_MiaFantasyRun/Art/UI/coin-ui.png";
        private const string GemUiSpritePath = "Assets/_MiaFantasyRun/Art/UI/gem-ui.png";
        private const string GameOverBackgroundPath = "Assets/_MiaFantasyRun/Art/UI/game-over-background-landscape.png";

        [MenuItem("Mia Fantasy Run/Create Starter Scene")]
        public static void CreateStarterScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
            var bootstrap = new GameObject("GameBootstrapper");
            bootstrap.AddComponent<Core.GameBootstrapper>();
            bootstrap.AddComponent<Services.SaveManager>();
            bootstrap.AddComponent<Services.AnalyticsManager>();
            bootstrap.AddComponent<Services.AdsManager>();
            bootstrap.AddComponent<Services.GooglePlayGamesManager>();

            var music = bootstrap.AddComponent<AudioSource>();
            music.playOnAwake = false;
            music.loop = true;

            var sfx = bootstrap.AddComponent<AudioSource>();
            sfx.playOnAwake = false;

            var audioManager = bootstrap.AddComponent<Services.AudioManager>();
            var serializedAudio = new SerializedObject(audioManager);
            serializedAudio.FindProperty("musicSource").objectReferenceValue = music;
            serializedAudio.FindProperty("sfxSource").objectReferenceValue = sfx;
            serializedAudio.ApplyModifiedPropertiesWithoutUndo();

            var swipeInput = new GameObject("SwipeInput").AddComponent<Gameplay.SwipeInput>();
            var runnerObject = new GameObject("RunnerGameManager");
            var runnerGameManager = runnerObject.AddComponent<Gameplay.RunnerGameManager>();
            var rewardedAdsRewardManager = runnerObject.AddComponent<Services.RewardedAdsRewardManager>();
            runnerObject.AddComponent<Services.InterstitialAdFrequencyController>();
            runnerObject.AddComponent<Gameplay.RewardedAdsDebugInput>();
            new GameObject("UIManager").AddComponent<UI.UIManager>();

            RenderSettings.ambientLight = new Color(0.78f, 0.9f, 1f);
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.72f, 0.9f, 1f);
            RenderSettings.fogDensity = 0.012f;

            CreatePlayablePrototype(swipeInput, runnerGameManager, rewardedAdsRewardManager);
            EditorSceneManager.SaveScene(scene, "Assets/_MiaFantasyRun/Scenes/Main.unity");
        }

        [MenuItem("Mia Fantasy Run/Create Splash Scene")]
        public static void CreateSplashScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
            scene.name = "Splash";

            var bootstrap = new GameObject("GameBootstrapper");
            bootstrap.AddComponent<Core.GameBootstrapper>();
            bootstrap.AddComponent<Services.SaveManager>();
            var analytics = bootstrap.AddComponent<Services.AnalyticsManager>();
            var ads = bootstrap.AddComponent<Services.AdsManager>();

            var music = bootstrap.AddComponent<AudioSource>();
            music.playOnAwake = false;
            music.loop = true;

            var sfx = bootstrap.AddComponent<AudioSource>();
            sfx.playOnAwake = false;

            var audioManager = bootstrap.AddComponent<Services.AudioManager>();
            var serializedAudio = new SerializedObject(audioManager);
            serializedAudio.FindProperty("musicSource").objectReferenceValue = music;
            serializedAudio.FindProperty("sfxSource").objectReferenceValue = sfx;
            serializedAudio.ApplyModifiedPropertiesWithoutUndo();

            var canvasObject = new GameObject("Splash Canvas");
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObject.AddComponent<UnityEngine.UI.CanvasScaler>();
            scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            canvasObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();

            var background = CreatePanel(canvasObject.transform, "Background", new Color(0.05f, 0.17f, 0.26f), Vector2.zero, new Vector2(0f, 0f), new Vector2(1f, 1f));
            background.transform.SetAsFirstSibling();

            var logo = CreateSplashText(canvasObject.transform, "Logo", "Mia's Fantasy Run", 58, new Vector2(0f, 190f), TextAnchor.MiddleCenter);
            logo.color = new Color(1f, 0.84f, 0.35f);

            var subtitle = CreateSplashText(canvasObject.transform, "Subtitle", "Magical worlds are waking up", 26, new Vector2(0f, 120f), TextAnchor.MiddleCenter);
            subtitle.color = new Color(0.84f, 0.96f, 1f);

            var sliderObject = new GameObject("Loading Slider");
            sliderObject.transform.SetParent(canvasObject.transform, false);
            var sliderRect = sliderObject.AddComponent<RectTransform>();
            sliderRect.anchorMin = new Vector2(0.5f, 0.5f);
            sliderRect.anchorMax = new Vector2(0.5f, 0.5f);
            sliderRect.pivot = new Vector2(0.5f, 0.5f);
            sliderRect.anchoredPosition = new Vector2(0f, -40f);
            sliderRect.sizeDelta = new Vector2(520f, 28f);
            var slider = sliderObject.AddComponent<UnityEngine.UI.Slider>();
            slider.minValue = 0f;
            slider.maxValue = 1f;

            var fill = CreatePanel(sliderObject.transform, "Fill", new Color(0.1f, 0.75f, 0.92f), Vector2.zero, new Vector2(0f, 0f), new Vector2(1f, 1f));
            slider.fillRect = fill.rectTransform;

            var status = CreateSplashText(canvasObject.transform, "Status", "Loading", 24, new Vector2(0f, -92f), TextAnchor.MiddleCenter);

            var controller = canvasObject.AddComponent<Runtime.SplashSceneController>();
            var serialized = new SerializedObject(controller);
            serialized.FindProperty("loadingSlider").objectReferenceValue = slider;
            serialized.FindProperty("statusText").objectReferenceValue = status;
            serialized.FindProperty("logoText").objectReferenceValue = logo;
            serialized.FindProperty("analyticsManager").objectReferenceValue = analytics;
            serialized.FindProperty("adsManager").objectReferenceValue = ads;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.SaveScene(scene, "Assets/_MiaFantasyRun/Scenes/Splash.unity");
            ConfigureBuildScenes();
        }

        [MenuItem("Mia Fantasy Run/Create Main Menu Scene")]
        public static void CreateMainMenuScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
            scene.name = "MainMenu";
            var miaSprite = EnsureSpriteAsset(MiaRunnerSpritePath, 480f);

            var canvasObject = new GameObject("Main Menu Canvas");
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObject.AddComponent<UnityEngine.UI.CanvasScaler>();
            scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            canvasObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();

            CreatePanel(canvasObject.transform, "Fantasy Backdrop", new Color(0.025f, 0.13f, 0.18f), Vector2.zero, new Vector2(0f, 0f), new Vector2(1f, 1f));
            CreatePanel(canvasObject.transform, "Sky Glow", new Color(0.07f, 0.36f, 0.48f, 0.45f), new Vector2(0f, 480f), new Vector2(0f, 0.5f), new Vector2(1f, 1f));
            CreateMenuDecoration(canvasObject.transform, new Vector2(-435f, 420f), new Color(0.1f, 0.75f, 0.9f));
            CreateMenuDecoration(canvasObject.transform, new Vector2(430f, 330f), new Color(1f, 0.44f, 0.2f));

            var title = CreateSplashText(canvasObject.transform, "Title", "Mia's Fantasy Run", 60, new Vector2(0f, 695f), TextAnchor.MiddleCenter);
            title.color = new Color(1f, 0.82f, 0.28f);
            AddTextShadow(title, new Color(0f, 0f, 0f, 0.42f), new Vector2(0f, -3f));

            var subtitle = CreateSplashText(canvasObject.transform, "Subtitle", "Fantasy City Adventure", 26, new Vector2(0f, 625f), TextAnchor.MiddleCenter);
            subtitle.color = new Color(0.83f, 0.96f, 1f);

            var controller = canvasObject.AddComponent<UI.MainMenuController>();
            canvasObject.AddComponent<Services.BannerAdController>();
            var serialized = new SerializedObject(controller);
            serialized.FindProperty("titleText").objectReferenceValue = title;

            var buttonParent = new GameObject("Menu Buttons");
            buttonParent.transform.SetParent(canvasObject.transform, false);
            CreateCharacterShowcase(canvasObject.transform, miaSprite);
            CreateMenuButton(buttonParent.transform, "Play", new Vector2(-260f, 430f), controller.Play);
            CreateMenuButton(buttonParent.transform, "Shop", new Vector2(-260f, 322f), controller.OpenShop);
            CreateMenuButton(buttonParent.transform, "Characters", new Vector2(-260f, 214f), controller.OpenCharacters);
            CreateMenuButton(buttonParent.transform, "Pets", new Vector2(-260f, 106f), controller.OpenPets);
            CreateMenuButton(buttonParent.transform, "Daily Reward", new Vector2(-260f, -2f), controller.OpenDailyReward);
            CreateMenuButton(buttonParent.transform, "Settings", new Vector2(-260f, -110f), controller.OpenSettings);
            CreateMenuButton(buttonParent.transform, "Leaderboard", new Vector2(-260f, -218f), controller.OpenLeaderboard);

            var panel = CreateInfoPanel(canvasObject.transform, controller, serialized);
            serialized.FindProperty("infoPanel").objectReferenceValue = panel;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.SaveScene(scene, "Assets/_MiaFantasyRun/Scenes/MainMenu.unity");
            ConfigureBuildScenes();
        }

        [MenuItem("Mia Fantasy Run/Create All Game Scenes")]
        public static void CreateAllGameScenes()
        {
            CharacterDataGenerator.Generate();
            EnsureSpriteAsset(MiaBackSpritePath, 480f);
            EnsureRuntimeTextureAsset(MiaRunSheetPath);
            EnsureSpriteAsset(MagicShieldSpritePath, 560f);
            EnsureSpriteAsset(CoinUiSpritePath, 128f);
            EnsureSpriteAsset(GemUiSpritePath, 128f);
            EnsureSpriteAsset(GameOverBackgroundPath, 720f);
            CreateStarterScene();
            CreateMainMenuScene();
            CreateSplashScene();
            ConfigureBuildScenes();
        }

        [MenuItem("Mia Fantasy Run/Play Main Scene")]
        public static void PlayMainScene()
        {
            EditorPrefs.SetBool("MiaFantasyRun.AutoPlayMainScene", true);
            EditorSceneManager.OpenScene("Assets/_MiaFantasyRun/Scenes/Main.unity");
            AutoPlayMainSceneWatcher.StartWatching();
        }

        private static UnityEngine.UI.Image CreatePanel(Transform parent, string name, Color color, Vector2 anchoredPosition, Vector2 anchorMin, Vector2 anchorMax)
        {
            var panel = new GameObject(name);
            panel.transform.SetParent(parent, false);
            var image = panel.AddComponent<UnityEngine.UI.Image>();
            image.color = color;
            var rect = image.rectTransform;
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.anchoredPosition = anchoredPosition;
            return image;
        }

        private static UnityEngine.UI.Text CreateSplashText(Transform parent, string name, string value, int size, Vector2 anchoredPosition, TextAnchor alignment)
        {
            var textObject = new GameObject(name);
            textObject.transform.SetParent(parent, false);
            var text = textObject.AddComponent<UnityEngine.UI.Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.text = value;
            text.fontSize = size;
            text.color = Color.white;
            text.alignment = alignment;
            text.raycastTarget = false;
            var rect = text.rectTransform;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(760f, 90f);
            return text;
        }

        private static void ConfigureBuildScenes()
        {
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene("Assets/_MiaFantasyRun/Scenes/Splash.unity", true),
                new EditorBuildSettingsScene("Assets/_MiaFantasyRun/Scenes/MainMenu.unity", true),
                new EditorBuildSettingsScene("Assets/_MiaFantasyRun/Scenes/Main.unity", true)
            };
        }

        private static void CreateMenuDecoration(Transform parent, Vector2 anchoredPosition, Color color)
        {
            var image = CreatePanel(parent, "Magic Crystal", color, anchoredPosition, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            image.rectTransform.sizeDelta = new Vector2(130f, 210f);
            image.rectTransform.rotation = Quaternion.Euler(0f, 0f, 28f);
            image.raycastTarget = false;
        }

        private static UnityEngine.UI.Button CreateMenuButton(Transform parent, string label, Vector2 anchoredPosition, UnityAction action)
        {
            var buttonObject = new GameObject($"{label} Button");
            buttonObject.transform.SetParent(parent, false);
            var image = buttonObject.AddComponent<UnityEngine.UI.Image>();
            image.color = label == "Play" ? new Color(1f, 0.62f, 0.16f) : new Color(0.06f, 0.47f, 0.62f);
            var outline = buttonObject.AddComponent<UnityEngine.UI.Outline>();
            outline.effectColor = new Color(1f, 1f, 1f, 0.18f);
            outline.effectDistance = new Vector2(2f, -2f);
            var shadow = buttonObject.AddComponent<UnityEngine.UI.Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.34f);
            shadow.effectDistance = new Vector2(0f, -5f);
            var button = buttonObject.AddComponent<UnityEngine.UI.Button>();
            var rect = buttonObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(label == "Play" ? 500f : 460f, label == "Play" ? 86f : 76f);

            var text = CreateSplashText(buttonObject.transform, "Label", label, label == "Play" ? 34 : 28, Vector2.zero, TextAnchor.MiddleCenter);
            text.color = Color.white;
            text.rectTransform.anchorMin = Vector2.zero;
            text.rectTransform.anchorMax = Vector2.one;
            text.rectTransform.offsetMin = Vector2.zero;
            text.rectTransform.offsetMax = Vector2.zero;
            AddTextShadow(text, new Color(0f, 0f, 0f, 0.38f), new Vector2(0f, -2f));

            button.onClick.AddListener(action);
            return button;
        }

        private static GameObject CreateInfoPanel(Transform parent, UI.MainMenuController controller, SerializedObject serialized)
        {
            var panelObject = new GameObject("Info Panel");
            panelObject.transform.SetParent(parent, false);
            var image = panelObject.AddComponent<UnityEngine.UI.Image>();
            image.color = new Color(0.02f, 0.08f, 0.13f, 0.86f);
            var rect = image.rectTransform;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(0f, -510f);
            rect.sizeDelta = new Vector2(850f, 220f);
            var outline = panelObject.AddComponent<UnityEngine.UI.Outline>();
            outline.effectColor = new Color(0.25f, 0.82f, 1f, 0.28f);
            outline.effectDistance = new Vector2(2f, -2f);

            var title = CreateSplashText(panelObject.transform, "Panel Title", "Welcome", 30, new Vector2(0f, 54f), TextAnchor.MiddleCenter);
            title.rectTransform.sizeDelta = new Vector2(680f, 46f);
            var body = CreateSplashText(panelObject.transform, "Panel Body", "", 22, new Vector2(0f, -12f), TextAnchor.MiddleCenter);
            body.rectTransform.sizeDelta = new Vector2(680f, 110f);

            serialized.FindProperty("panelTitleText").objectReferenceValue = title;
            serialized.FindProperty("panelBodyText").objectReferenceValue = body;
            return panelObject;
        }

        private static void CreatePlayablePrototype(Gameplay.SwipeInput swipeInput, Gameplay.RunnerGameManager runnerGameManager, Services.RewardedAdsRewardManager rewardedAdsRewardManager)
        {
            var player = new GameObject("Mia Prototype Runner");
            player.name = "Mia Prototype Runner";
            player.transform.position = new Vector3(0f, 1f, 0f);
            var characterController = player.AddComponent<CharacterController>();
            characterController.height = 2f;
            characterController.radius = 0.45f;
            characterController.center = new Vector3(0f, 1f, 0f);

            CreateMiaVisual(player.transform);

            var playerController = player.AddComponent<Gameplay.PlayerController>();
            var powerUpController = player.AddComponent<Gameplay.PowerUpController>();
            var shieldBubble = player.AddComponent<Gameplay.ShieldBubbleVisual>();
            player.AddComponent<Gameplay.RunnerDustTrail>();
            var serializedShieldBubble = new SerializedObject(shieldBubble);
            serializedShieldBubble.FindProperty("shieldIconSprite").objectReferenceValue = EnsureSpriteAsset(MagicShieldSpritePath, 560f);
            serializedShieldBubble.ApplyModifiedPropertiesWithoutUndo();
            var serializedPlayer = new SerializedObject(playerController);
            serializedPlayer.FindProperty("input").objectReferenceValue = swipeInput;
            serializedPlayer.ApplyModifiedPropertiesWithoutUndo();

            var serializedManager = new SerializedObject(runnerGameManager);
            serializedManager.FindProperty("player").objectReferenceValue = playerController;
            serializedManager.ApplyModifiedPropertiesWithoutUndo();

            var serializedRewarded = new SerializedObject(rewardedAdsRewardManager);
            serializedRewarded.FindProperty("runnerGameManager").objectReferenceValue = runnerGameManager;
            serializedRewarded.ApplyModifiedPropertiesWithoutUndo();

            var camera = Camera.main;
            if (camera != null)
            {
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = new Color(0.54f, 0.78f, 0.96f);
                camera.transform.position = new Vector3(0f, 4.15f, -7.2f);
                camera.transform.rotation = Quaternion.Euler(22f, 0f, 0f);
                camera.fieldOfView = 46f;
                var followCamera = camera.gameObject.AddComponent<Gameplay.FollowCamera>();
                var serializedCamera = new SerializedObject(followCamera);
                serializedCamera.FindProperty("target").objectReferenceValue = player.transform;
                serializedCamera.ApplyModifiedPropertiesWithoutUndo();
            }

            CreateWorldSpawner(player.transform);
            CreateHud(runnerGameManager, powerUpController, swipeInput);
        }

        private static void CreateWorldSpawner(Transform player)
        {
            var spawner = new GameObject("PrototypeWorldSpawner").AddComponent<Gameplay.PrototypeWorldSpawner>();
            var serializedSpawner = new SerializedObject(spawner);
            serializedSpawner.FindProperty("player").objectReferenceValue = player;
            serializedSpawner.FindProperty("roadMaterial").objectReferenceValue = CreateMaterial("Prototype Road Material", new Color(0.15f, 0.7f, 0.95f));
            serializedSpawner.FindProperty("coinMaterial").objectReferenceValue = CreateMaterial("Prototype Coin Material", new Color(1f, 0.82f, 0.12f));
            serializedSpawner.FindProperty("gemMaterial").objectReferenceValue = CreateMaterial("Prototype Gem Material", new Color(0.1f, 0.95f, 1f));
            serializedSpawner.FindProperty("shieldMaterial").objectReferenceValue = CreateMaterial("Prototype Shield Material", new Color(0.35f, 1f, 0.45f));
            serializedSpawner.FindProperty("obstacleMaterial").objectReferenceValue = CreateMaterial("Prototype Obstacle Material", new Color(0.65f, 0.25f, 0.9f));
            serializedSpawner.FindProperty("decorationMaterial").objectReferenceValue = CreateMaterial("Prototype Crystal Decoration Material", new Color(0.6f, 0.25f, 0.95f));
            serializedSpawner.FindProperty("laneMaterial").objectReferenceValue = CreateMaterial("Prototype Lane Material", new Color(0.92f, 0.98f, 1f));
            serializedSpawner.FindProperty("shieldPickupSprite").objectReferenceValue = EnsureSpriteAsset(MagicShieldSpritePath, 560f);
            serializedSpawner.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void CreateMiaVisual(Transform root)
        {
            var runSheet = EnsureRuntimeTextureAsset(MiaRunSheetPath);
            if (runSheet != null)
            {
                var spriteVisualRoot = new GameObject("Mia Back View Sprite Runner");
                spriteVisualRoot.transform.SetParent(root, false);
                spriteVisualRoot.transform.localPosition = new Vector3(0f, 0.05f, 0f);
                spriteVisualRoot.transform.localScale = Vector3.one * 1.75f;

                var spriteObject = new GameObject("Mia Running Sprite Sheet Player");
                spriteObject.transform.SetParent(spriteVisualRoot.transform, false);
                spriteObject.transform.localPosition = Vector3.zero;
                var spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sortingOrder = 10;

                var spriteAnimator = spriteVisualRoot.AddComponent<Gameplay.RunnerVisualAnimator>();
                var serializedSpriteAnimator = new SerializedObject(spriteAnimator);
                serializedSpriteAnimator.FindProperty("spriteRenderer").objectReferenceValue = spriteRenderer;
                serializedSpriteAnimator.FindProperty("runSheet").objectReferenceValue = runSheet;
                serializedSpriteAnimator.FindProperty("runSheetColumns").intValue = 4;
                serializedSpriteAnimator.FindProperty("runSheetRows").intValue = 2;
                serializedSpriteAnimator.FindProperty("spritePixelsPerUnit").floatValue = 430f;
                serializedSpriteAnimator.FindProperty("frameRate").floatValue = 12f;
                serializedSpriteAnimator.ApplyModifiedPropertiesWithoutUndo();

                var spriteShadow = CreatePart(root, "Runner Ground Shadow", PrimitiveType.Cylinder, new Vector3(0f, 0.02f, -0.08f), new Vector3(0.78f, 0.04f, 0.52f), CreateMaterial("Runner Shadow Material", new Color(0f, 0f, 0f, 0.24f)), new Vector3(90f, 0f, 0f));
                spriteShadow.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                return;
            }

            var visualRoot = new GameObject("Mia Back View Animated Model");
            visualRoot.transform.SetParent(root, false);
            visualRoot.transform.localPosition = Vector3.zero;

            var skin = CreateMaterial("Mia Skin Material", new Color(1f, 0.68f, 0.52f));
            var hair = CreateMaterial("Mia Black Hair Material", new Color(0.02f, 0.018f, 0.035f));
            var hoodie = CreateMaterial("Mia Pink Hoodie Material", new Color(1f, 0.28f, 0.58f));
            var jeans = CreateMaterial("Mia Blue Jeans Material", new Color(0.08f, 0.32f, 0.66f));
            var whiteShoe = CreateMaterial("Mia White Sneaker Material", new Color(0.96f, 0.96f, 0.92f));
            var trim = CreateMaterial("Mia Hoodie Trim Material", new Color(1f, 0.78f, 0.9f));
            var shadow = CreatePart(root, "Runner Ground Shadow", PrimitiveType.Cylinder, new Vector3(0f, 0.02f, -0.08f), new Vector3(0.78f, 0.04f, 0.52f), CreateMaterial("Runner Shadow Material", new Color(0f, 0f, 0f, 0.24f)), new Vector3(90f, 0f, 0f));
            shadow.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            var body = CreatePart(visualRoot.transform, "Back Pink Hoodie", PrimitiveType.Capsule, new Vector3(0f, 1.14f, 0f), new Vector3(0.66f, 0.78f, 0.42f), hoodie);
            var hood = CreatePart(visualRoot.transform, "Hoodie Hood", PrimitiveType.Sphere, new Vector3(0f, 1.46f, -0.18f), new Vector3(0.54f, 0.34f, 0.2f), hoodie);
            var head = CreatePart(visualRoot.transform, "Back Head", PrimitiveType.Sphere, new Vector3(0f, 1.78f, 0.02f), new Vector3(0.5f, 0.5f, 0.5f), skin);
            var hairCap = CreatePart(visualRoot.transform, "Back Black Hair", PrimitiveType.Sphere, new Vector3(0f, 1.88f, -0.09f), new Vector3(0.58f, 0.5f, 0.54f), hair);
            var ponytail = CreatePart(visualRoot.transform, "Swinging Black Ponytail", PrimitiveType.Sphere, new Vector3(0.3f, 1.62f, -0.32f), new Vector3(0.26f, 0.56f, 0.25f), hair, new Vector3(14f, 0f, -16f));
            var waist = CreatePart(visualRoot.transform, "Jeans Waist", PrimitiveType.Cube, new Vector3(0f, 0.75f, -0.02f), new Vector3(0.62f, 0.2f, 0.36f), jeans);
            var hoodieTrim = CreatePart(visualRoot.transform, "Hoodie Hem", PrimitiveType.Cube, new Vector3(0f, 0.88f, -0.22f), new Vector3(0.7f, 0.08f, 0.08f), trim);

            var leftArm = CreateLimb(visualRoot.transform, "Left Running Arm", new Vector3(-0.43f, 1.28f, -0.02f), new Vector3(0.16f, 0.56f, 0.16f), hoodie, new Vector3(-18f, 0f, -8f));
            var rightArm = CreateLimb(visualRoot.transform, "Right Running Arm", new Vector3(0.43f, 1.28f, -0.02f), new Vector3(0.16f, 0.56f, 0.16f), hoodie, new Vector3(18f, 0f, 8f));
            var leftLeg = CreateLimb(visualRoot.transform, "Left Running Leg", new Vector3(-0.2f, 0.58f, 0.01f), new Vector3(0.17f, 0.62f, 0.17f), jeans, new Vector3(18f, 0f, 0f));
            var rightLeg = CreateLimb(visualRoot.transform, "Right Running Leg", new Vector3(0.2f, 0.58f, 0.01f), new Vector3(0.17f, 0.62f, 0.17f), jeans, new Vector3(-18f, 0f, 0f));
            CreatePart(leftLeg.transform, "Left White Sneaker", PrimitiveType.Cube, new Vector3(0f, -0.62f, 0.08f), new Vector3(0.22f, 0.14f, 0.36f), whiteShoe);
            CreatePart(rightLeg.transform, "Right White Sneaker", PrimitiveType.Cube, new Vector3(0f, -0.62f, 0.08f), new Vector3(0.22f, 0.14f, 0.36f), whiteShoe);

            var animator = visualRoot.AddComponent<Gameplay.RunnerVisualAnimator>();
            var serializedAnimator = new SerializedObject(animator);
            serializedAnimator.FindProperty("leftArm").objectReferenceValue = leftArm.transform;
            serializedAnimator.FindProperty("rightArm").objectReferenceValue = rightArm.transform;
            serializedAnimator.FindProperty("leftLeg").objectReferenceValue = leftLeg.transform;
            serializedAnimator.FindProperty("rightLeg").objectReferenceValue = rightLeg.transform;
            serializedAnimator.FindProperty("cape").objectReferenceValue = hood.transform;
            serializedAnimator.FindProperty("ponytail").objectReferenceValue = ponytail.transform;
            serializedAnimator.ApplyModifiedPropertiesWithoutUndo();

            body.name = "Back Pink Hoodie";
            hood.name = "Hoodie Hood";
            head.name = "Back Head";
            hairCap.name = "Back Black Hair";
            ponytail.name = "Swinging Black Ponytail";
            waist.name = "Jeans Waist";
            hoodieTrim.name = "Hoodie Hem";
        }

        private static GameObject CreateLimb(Transform parent, string name, Vector3 pivotPosition, Vector3 visualScale, Material material, Vector3 startRotation)
        {
            var pivot = new GameObject(name);
            pivot.transform.SetParent(parent, false);
            pivot.transform.localPosition = pivotPosition;
            pivot.transform.localRotation = Quaternion.Euler(startRotation);

            var limb = CreatePart(pivot.transform, $"{name} Visual", PrimitiveType.Capsule, new Vector3(0f, -0.26f, 0f), visualScale, material);
            limb.transform.localRotation = Quaternion.identity;
            return pivot;
        }

        private static GameObject CreatePart(Transform parent, string name, PrimitiveType primitive, Vector3 localPosition, Vector3 localScale, Material material, Vector3? rotation = null)
        {
            var part = GameObject.CreatePrimitive(primitive);
            part.name = name;
            part.transform.SetParent(parent, false);
            part.transform.localPosition = localPosition;
            part.transform.localScale = localScale;
            if (rotation.HasValue)
            {
                part.transform.localRotation = Quaternion.Euler(rotation.Value);
            }

            part.GetComponent<Renderer>().sharedMaterial = material;
            var collider = part.GetComponent<Collider>();
            if (collider != null)
            {
                Object.DestroyImmediate(collider);
            }

            return part;
        }

        private static void CreateHud(Gameplay.RunnerGameManager runnerGameManager, Gameplay.PowerUpController powerUpController, Gameplay.SwipeInput swipeInput)
        {
            var canvasObject = new GameObject("Mobile Runner HUD");
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObject.AddComponent<UnityEngine.UI.CanvasScaler>();
            scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            canvasObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();

            CreateHudFrame(canvasObject.transform, "Currency Glass", new Vector2(28f, -28f), new Vector2(186f, 112f), TextAnchor.UpperLeft, new Color(1f, 1f, 1f, 0.12f));
            CreateHudFrame(canvasObject.transform, "Score Glass", new Vector2(0f, -24f), new Vector2(240f, 104f), TextAnchor.UpperCenter, new Color(1f, 1f, 1f, 0.12f));
            CreateHudFrame(canvasObject.transform, "Distance Glass", new Vector2(-28f, -28f), new Vector2(154f, 62f), TextAnchor.UpperRight, new Color(1f, 1f, 1f, 0.12f));

            var coinRow = CreateCurrencyRow(canvasObject.transform, "Coin Row", new Vector2(48f, -57f));
            var gemRow = CreateCurrencyRow(canvasObject.transform, "Gem Row", new Vector2(48f, -103f));
            CreateCoinIcon(coinRow.transform, new Vector2(0f, 0f), EnsureSpriteAsset(CoinUiSpritePath, 128f));
            CreateGemIcon(gemRow.transform, new Vector2(0f, 0f), EnsureSpriteAsset(GemUiSpritePath, 128f));

            var score = CreateHudText(canvasObject.transform, "Score", new Vector2(0f, -36f), TextAnchor.UpperCenter);
            var coins = CreateHudText(coinRow.transform, "Coins", new Vector2(42f, 0f), TextAnchor.MiddleLeft);
            var gems = CreateHudText(gemRow.transform, "Gems", new Vector2(42f, 0f), TextAnchor.MiddleLeft);
            var distance = CreateHudText(canvasObject.transform, "Distance", new Vector2(-52f, -43f), TextAnchor.UpperRight);
            var status = CreateHudText(canvasObject.transform, "Status", new Vector2(0f, 104f), TextAnchor.LowerCenter);
            var gameOverPanel = CreateGameOverOverlay(canvasObject.transform, runnerGameManager);

            var hud = canvasObject.AddComponent<UI.PrototypeHud>();
            var serializedHud = new SerializedObject(hud);
            serializedHud.FindProperty("manager").objectReferenceValue = runnerGameManager;
            serializedHud.FindProperty("powerUp").objectReferenceValue = powerUpController;
            serializedHud.FindProperty("scoreText").objectReferenceValue = score;
            serializedHud.FindProperty("coinsText").objectReferenceValue = coins;
            serializedHud.FindProperty("gemsText").objectReferenceValue = gems;
            serializedHud.FindProperty("distanceText").objectReferenceValue = distance;
            serializedHud.FindProperty("statusText").objectReferenceValue = status;
            serializedHud.FindProperty("gameOverPanel").objectReferenceValue = gameOverPanel;
            serializedHud.ApplyModifiedPropertiesWithoutUndo();
        }

        private static UnityEngine.UI.Text CreateHudText(Transform parent, string name, Vector2 anchoredPosition, TextAnchor alignment)
        {
            var textObject = new GameObject(name);
            textObject.transform.SetParent(parent, false);
            var text = textObject.AddComponent<UnityEngine.UI.Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = name switch
            {
                "Status" => 28,
                "Score" => 32,
                "Distance" => 28,
                _ => 32
            };
            text.fontStyle = FontStyle.Bold;
            text.color = Color.white;
            text.alignment = alignment;
            text.raycastTarget = false;
            AddTextShadow(text, new Color(0f, 0f, 0f, 0.65f), new Vector2(0f, -3f));

            var rect = text.rectTransform;
            rect.anchorMin = alignment switch
            {
                TextAnchor.UpperCenter => new Vector2(0.5f, 1f),
                TextAnchor.UpperRight => new Vector2(1f, 1f),
                TextAnchor.LowerCenter => new Vector2(0.5f, 0f),
                TextAnchor.MiddleLeft => new Vector2(0f, 0.5f),
                _ => new Vector2(0f, 1f)
            };
            rect.anchorMax = rect.anchorMin;
            rect.pivot = alignment switch
            {
                TextAnchor.UpperCenter => new Vector2(0.5f, 1f),
                TextAnchor.UpperRight => new Vector2(1f, 1f),
                TextAnchor.LowerCenter => new Vector2(0.5f, 0f),
                TextAnchor.MiddleLeft => new Vector2(0f, 0.5f),
                _ => new Vector2(0f, 1f)
            };
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = name switch
            {
                "Status" => new Vector2(360f, 46f),
                "Score" => new Vector2(210f, 76f),
                "Distance" => new Vector2(130f, 42f),
                _ => new Vector2(98f, 40f)
            };
            return text;
        }

        private static RectTransform CreateCurrencyRow(Transform parent, string name, Vector2 anchoredPosition)
        {
            var row = new GameObject(name);
            row.transform.SetParent(parent, false);
            var rect = row.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = rect.anchorMin;
            rect.pivot = new Vector2(0f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(132f, 40f);
            return rect;
        }

        private static void CreateHudFrame(Transform parent, string name, Vector2 anchoredPosition, Vector2 size, TextAnchor alignment, Color color)
        {
            var frame = CreatePanel(parent, name, color, anchoredPosition, Vector2.zero, Vector2.zero);
            var rect = frame.rectTransform;
            rect.anchorMin = alignment switch
            {
                TextAnchor.UpperCenter => new Vector2(0.5f, 1f),
                TextAnchor.UpperRight => new Vector2(1f, 1f),
                TextAnchor.LowerCenter => new Vector2(0.5f, 0f),
                _ => new Vector2(0f, 1f)
            };
            rect.anchorMax = rect.anchorMin;
            rect.pivot = alignment switch
            {
                TextAnchor.UpperCenter => new Vector2(0.5f, 1f),
                TextAnchor.UpperRight => new Vector2(1f, 1f),
                TextAnchor.LowerCenter => new Vector2(0.5f, 0f),
                _ => new Vector2(0f, 1f)
            };
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;
            frame.raycastTarget = false;
            var outline = frame.gameObject.AddComponent<UnityEngine.UI.Outline>();
            outline.effectColor = new Color(1f, 1f, 1f, 0.24f);
            outline.effectDistance = new Vector2(2f, -2f);
            var shadow = frame.gameObject.AddComponent<UnityEngine.UI.Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.28f);
            shadow.effectDistance = new Vector2(0f, -5f);
        }

        private static GameObject CreateGameOverOverlay(Transform parent, Gameplay.RunnerGameManager runnerGameManager)
        {
            var panel = new GameObject("Game Over Overlay");
            panel.transform.SetParent(parent, false);
            var rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var backgroundObject = new GameObject("Game Over Artwork");
            backgroundObject.transform.SetParent(panel.transform, false);
            var background = backgroundObject.AddComponent<UnityEngine.UI.Image>();
            background.sprite = EnsureSpriteAsset(GameOverBackgroundPath, 720f);
            background.color = Color.white;
            background.preserveAspect = false;
            background.raycastTarget = true;
            backgroundObject.AddComponent<MiaFantasyRun.UI.AspectFillImage>();
            var backgroundRect = background.rectTransform;
            backgroundRect.anchorMin = new Vector2(0.5f, 0.5f);
            backgroundRect.anchorMax = new Vector2(0.5f, 0.5f);
            backgroundRect.pivot = new Vector2(0.5f, 0.5f);
            backgroundRect.anchoredPosition = Vector2.zero;

            var vignette = CreatePanel(panel.transform, "Game Over Vignette", new Color(0f, 0f, 0f, 0.18f), Vector2.zero, Vector2.zero, Vector2.one);
            vignette.raycastTarget = false;

            var title = CreateSplashText(panel.transform, "Game Over Title", "GAME OVER", 76, new Vector2(0f, 390f), TextAnchor.MiddleCenter);
            title.fontStyle = FontStyle.Bold;
            title.color = new Color(1f, 0.9f, 0.72f);
            title.rectTransform.sizeDelta = new Vector2(760f, 96f);
            AddTextShadow(title, new Color(0.22f, 0.08f, 0.02f, 0.88f), new Vector2(0f, -5f));
            var titleOutline = title.gameObject.AddComponent<UnityEngine.UI.Outline>();
            titleOutline.effectColor = new Color(0.32f, 0.12f, 0.04f, 0.9f);
            titleOutline.effectDistance = new Vector2(3f, -3f);

            var tryAgain = CreateGameOverButton(panel.transform, runnerGameManager);
            tryAgain.transform.SetAsLastSibling();
            panel.SetActive(false);
            panel.transform.SetAsFirstSibling();
            return panel;
        }

        private static UnityEngine.UI.Button CreateGameOverButton(Transform parent, Gameplay.RunnerGameManager runnerGameManager)
        {
            var buttonObject = new GameObject("Try Again Button");
            buttonObject.transform.SetParent(parent, false);
            var image = buttonObject.AddComponent<UnityEngine.UI.Image>();
            image.color = new Color(1f, 0.58f, 0.18f, 0.94f);
            var button = buttonObject.AddComponent<UnityEngine.UI.Button>();
            button.onClick.AddListener(runnerGameManager.Restart);

            var rect = image.rectTransform;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = rect.anchorMin;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(0f, 300f);
            rect.sizeDelta = new Vector2(320f, 72f);

            var outline = buttonObject.AddComponent<UnityEngine.UI.Outline>();
            outline.effectColor = new Color(1f, 1f, 1f, 0.28f);
            outline.effectDistance = new Vector2(2f, -2f);
            var shadow = buttonObject.AddComponent<UnityEngine.UI.Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.35f);
            shadow.effectDistance = new Vector2(0f, -5f);

            var label = CreateSplashText(buttonObject.transform, "Label", "TRY AGAIN?", 34, Vector2.zero, TextAnchor.MiddleCenter);
            label.fontStyle = FontStyle.Bold;
            label.color = Color.white;
            label.rectTransform.anchorMin = Vector2.zero;
            label.rectTransform.anchorMax = Vector2.one;
            label.rectTransform.offsetMin = Vector2.zero;
            label.rectTransform.offsetMax = Vector2.zero;
            AddTextShadow(label, new Color(0f, 0f, 0f, 0.45f), new Vector2(0f, -2f));

            return button;
        }

        private static void CreateCoinIcon(Transform parent, Vector2 anchoredPosition, Sprite coinSprite)
        {
            var icon = new GameObject("Coin Icon");
            icon.transform.SetParent(parent, false);
            var image = icon.AddComponent<UnityEngine.UI.Image>();
            image.sprite = coinSprite;
            image.preserveAspect = true;
            image.color = Color.white;
            var rect = image.rectTransform;
            rect.anchorMin = new Vector2(0f, 0.5f);
            rect.anchorMax = rect.anchorMin;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(38f, 38f);
            image.raycastTarget = false;

            var shineObject = new GameObject("Coin Shine Sweep");
            shineObject.transform.SetParent(icon.transform, false);
            var shine = shineObject.AddComponent<UnityEngine.UI.Image>();
            shine.color = new Color(1f, 1f, 1f, 0.38f);
            shine.raycastTarget = false;
            var shineRect = shine.rectTransform;
            shineRect.anchorMin = new Vector2(0.5f, 0.5f);
            shineRect.anchorMax = shineRect.anchorMin;
            shineRect.pivot = new Vector2(0.5f, 0.5f);
            shineRect.anchoredPosition = new Vector2(-42f, 0f);
            shineRect.sizeDelta = new Vector2(8f, 46f);
            shineRect.localRotation = Quaternion.Euler(0f, 0f, -22f);

            var animator = icon.AddComponent<UI.HudIconAnimator>();
            var serializedAnimator = new SerializedObject(animator);
            serializedAnimator.FindProperty("icon").objectReferenceValue = rect;
            serializedAnimator.FindProperty("shine").objectReferenceValue = shine;
            serializedAnimator.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void CreateGemIcon(Transform parent, Vector2 anchoredPosition, Sprite gemSprite)
        {
            var icon = new GameObject("Gem Icon");
            icon.transform.SetParent(parent, false);
            var image = icon.AddComponent<UnityEngine.UI.Image>();
            image.sprite = gemSprite;
            image.preserveAspect = true;
            image.color = Color.white;
            var rect = image.rectTransform;
            rect.anchorMin = new Vector2(0f, 0.5f);
            rect.anchorMax = rect.anchorMin;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(36f, 36f);
            image.raycastTarget = false;

            var animator = icon.AddComponent<UI.HudIconAnimator>();
            var serializedAnimator = new SerializedObject(animator);
            serializedAnimator.FindProperty("icon").objectReferenceValue = rect;
            serializedAnimator.FindProperty("rotationDegrees").floatValue = 3f;
            serializedAnimator.FindProperty("pulseAmount").floatValue = 0.045f;
            serializedAnimator.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void CreateMobileTouchControls(Transform parent, Gameplay.SwipeInput input)
        {
            var controls = new GameObject("Mobile Touch Controls");
            controls.transform.SetParent(parent, false);
            CreateTouchButton(controls.transform, "Left Button", "<", new Vector2(72f, 74f), TextAnchor.LowerLeft, input.TriggerLeft);
            CreateTouchButton(controls.transform, "Right Button", ">", new Vector2(216f, 74f), TextAnchor.LowerLeft, input.TriggerRight);
            CreateTouchButton(controls.transform, "Jump Button", "^", new Vector2(-216f, 74f), TextAnchor.LowerRight, input.TriggerJump);
            CreateTouchButton(controls.transform, "Slide Button", "v", new Vector2(-72f, 74f), TextAnchor.LowerRight, input.TriggerSlide);
        }

        private static void CreateTouchButton(Transform parent, string name, string label, Vector2 anchoredPosition, TextAnchor alignment, UnityAction action)
        {
            var buttonObject = new GameObject(name);
            buttonObject.transform.SetParent(parent, false);
            var image = buttonObject.AddComponent<UnityEngine.UI.Image>();
            image.color = new Color(0.04f, 0.15f, 0.2f, 0.58f);
            var button = buttonObject.AddComponent<UnityEngine.UI.Button>();
            button.onClick.AddListener(action);

            var rect = buttonObject.GetComponent<RectTransform>();
            rect.anchorMin = alignment == TextAnchor.LowerRight ? new Vector2(1f, 0f) : new Vector2(0f, 0f);
            rect.anchorMax = rect.anchorMin;
            rect.pivot = alignment == TextAnchor.LowerRight ? new Vector2(1f, 0f) : new Vector2(0f, 0f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(112f, 112f);

            var outline = buttonObject.AddComponent<UnityEngine.UI.Outline>();
            outline.effectColor = new Color(1f, 1f, 1f, 0.28f);
            outline.effectDistance = new Vector2(2f, -2f);

            var text = CreateSplashText(buttonObject.transform, "Icon", label, 48, Vector2.zero, TextAnchor.MiddleCenter);
            text.fontStyle = FontStyle.Bold;
            text.rectTransform.anchorMin = Vector2.zero;
            text.rectTransform.anchorMax = Vector2.one;
            text.rectTransform.offsetMin = Vector2.zero;
            text.rectTransform.offsetMax = Vector2.zero;
            text.color = Color.white;
            AddTextShadow(text, new Color(0f, 0f, 0f, 0.45f), new Vector2(0f, -2f));
        }

        private static void CreateCharacterShowcase(Transform parent, Sprite miaSprite)
        {
            var glow = CreatePanel(parent, "Character Glow", new Color(0.08f, 0.55f, 0.7f, 0.24f), new Vector2(250f, 90f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            glow.rectTransform.sizeDelta = new Vector2(470f, 720f);
            glow.raycastTarget = false;

            if (miaSprite == null)
            {
                return;
            }

            var character = new GameObject("Mia Menu Artwork");
            character.transform.SetParent(parent, false);
            var image = character.AddComponent<UnityEngine.UI.Image>();
            image.sprite = miaSprite;
            image.preserveAspect = true;
            image.raycastTarget = false;
            image.color = Color.white;
            var rect = image.rectTransform;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(255f, 130f);
            rect.sizeDelta = new Vector2(520f, 800f);

            var name = CreateSplashText(parent, "Selected Character Label", "Mia", 34, new Vector2(250f, -320f), TextAnchor.MiddleCenter);
            name.color = new Color(1f, 0.84f, 0.35f);
            name.rectTransform.sizeDelta = new Vector2(300f, 54f);
            AddTextShadow(name, new Color(0f, 0f, 0f, 0.45f), new Vector2(0f, -2f));
        }

        private static void AddTextShadow(UnityEngine.UI.Text text, Color color, Vector2 distance)
        {
            var shadow = text.gameObject.AddComponent<UnityEngine.UI.Shadow>();
            shadow.effectColor = color;
            shadow.effectDistance = distance;
        }

        private static Sprite EnsureSpriteAsset(string assetPath, float pixelsPerUnit)
        {
            if (!File.Exists(assetPath))
            {
                return null;
            }

            AssetDatabase.ImportAsset(assetPath);
            var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritePixelsPerUnit = pixelsPerUnit;
                importer.alphaIsTransparency = true;
                importer.mipmapEnabled = false;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
        }

        private static Texture2D EnsureRuntimeTextureAsset(string assetPath)
        {
            if (!File.Exists(assetPath))
            {
                return null;
            }

            AssetDatabase.ImportAsset(assetPath);
            var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.alphaIsTransparency = true;
                importer.mipmapEnabled = false;
                importer.isReadable = false;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
        }

        private static Material CreateMaterial(string name, Color color)
        {
            var shader = Shader.Find("Standard")
                         ?? Shader.Find("Diffuse")
                         ?? Shader.Find("Unlit/Color")
                         ?? Shader.Find("Sprites/Default");

            var material = new Material(shader)
            {
                name = name
            };

            if (material.HasProperty("_Color"))
            {
                material.SetColor("_Color", color);
            }

            material.color = color;
            if (color.a < 0.99f)
            {
                material.SetFloat("_Mode", 3f);
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
            }
            return material;
        }

        [InitializeOnLoad]
        private static class AutoPlayMainSceneWatcher
        {
            static AutoPlayMainSceneWatcher()
            {
                if (EditorPrefs.GetBool("MiaFantasyRun.AutoPlayMainScene", false))
                {
                    StartWatching();
                }
            }

            public static void StartWatching()
            {
                EditorApplication.update -= TryStartPlayMode;
                EditorApplication.update += TryStartPlayMode;
            }

            private static void TryStartPlayMode()
            {
                if (!EditorPrefs.GetBool("MiaFantasyRun.AutoPlayMainScene", false))
                {
                    EditorApplication.update -= TryStartPlayMode;
                    return;
                }

                if (EditorApplication.isCompiling || EditorApplication.isUpdating || EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    return;
                }

                EditorPrefs.SetBool("MiaFantasyRun.AutoPlayMainScene", false);
                EditorApplication.update -= TryStartPlayMode;
                Debug.Log("Mia Fantasy Run: entering Play Mode from Main scene.");
                EditorApplication.isPlaying = true;
            }
        }
    }
}
#endif
