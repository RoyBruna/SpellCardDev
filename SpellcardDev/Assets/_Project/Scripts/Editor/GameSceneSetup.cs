using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SpellCardDev.Core;
using SpellCardDev.Player;
using SpellCardDev.Enemies;
using SpellCardDev.Background;
using SpellCardDev.EditorUtils;
using SpellCardDev.UI;

namespace SpellCardDev.Editor
{
    public class GameSceneSetup : EditorWindow
    {
        [MenuItem("SpellCardDev/Setup Game Scene")]
        public static void CreateGameScene()
        {
            // Crear nueva escena vacía
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "Game";

            // 1. Cámara Principal (Ortográfica, negra)
            GameObject camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            Camera cam = camGo.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 8f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.05f, 0.06f, 0.1f, 1f);
            cam.allowHDR = true;
            camGo.AddComponent<AudioListener>();
            camGo.transform.position = new Vector3(0, 0, -10f);

            // 2. GameManager
            GameObject gmGo = new GameObject("GameManager");
            GameManager gm = gmGo.AddComponent<GameManager>();

            // 3. BulletPools
            GameObject poolPlayerGo = new GameObject("BulletPool_Player");
            BulletPool poolPlayer = poolPlayerGo.AddComponent<BulletPool>();
            SerializedObject soPoolPlayer = new SerializedObject(poolPlayer);
            soPoolPlayer.FindProperty("poolSize").intValue = 200;
            soPoolPlayer.ApplyModifiedProperties();

            GameObject poolEnemyGo = new GameObject("BulletPool_Enemy");
            BulletPool poolEnemy = poolEnemyGo.AddComponent<BulletPool>();
            SerializedObject soPoolEnemy = new SerializedObject(poolEnemy);
            soPoolEnemy.FindProperty("poolSize").intValue = 500;
            soPoolEnemy.ApplyModifiedProperties();

            // Enlazar pools al GameManager
            SerializedObject soGm = new SerializedObject(gm);
            soGm.FindProperty("playerBulletPool").objectReferenceValue = poolPlayer;
            soGm.FindProperty("enemyBulletPool").objectReferenceValue = poolEnemy;
            soGm.ApplyModifiedProperties();

            // 4. Player
            GameObject playerGo = new GameObject("Player");
            playerGo.tag = "Player";
            playerGo.transform.position = new Vector3(0, -5f, 0);
            playerGo.AddComponent<SpriteRenderer>();
            playerGo.AddComponent<PlayerController>();
            
            PlayerShooter shooter = playerGo.AddComponent<PlayerShooter>();
            SerializedObject soShooter = new SerializedObject(shooter);
            soShooter.FindProperty("playerBulletPool").objectReferenceValue = poolPlayer;
            soShooter.ApplyModifiedProperties();
            
            playerGo.AddComponent<PlayerBomb>();
            playerGo.AddComponent<HitboxGizmo>();

            // 5. Enemy Spawner / Boss
            GameObject spawnerGo = new GameObject("EnemySpawner");
            spawnerGo.tag = "Enemy";
            spawnerGo.transform.position = new Vector3(0, 8f, 0);
            spawnerGo.AddComponent<SpriteRenderer>();
            spawnerGo.AddComponent<CircleCollider2D>();
            spawnerGo.AddComponent<Rigidbody2D>();
            CirclePatternSpawner spawner = spawnerGo.AddComponent<CirclePatternSpawner>();
            SerializedObject soSpawner = new SerializedObject(spawner);
            soSpawner.FindProperty("enemyBulletPool").objectReferenceValue = poolEnemy;
            soSpawner.ApplyModifiedProperties();
            spawnerGo.AddComponent<BossController>();

            GameObject bgGo = new GameObject("Background");
            bgGo.AddComponent<BackgroundVideoPlayer>();

            GameObject uiCanvasGo = new GameObject("UICanvas");
            Canvas uiCanvas = uiCanvasGo.AddComponent<Canvas>();
            uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler uiScaler = uiCanvasGo.AddComponent<CanvasScaler>();
            uiScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            uiScaler.referenceResolution = new Vector2(1280f, 960f);
            uiScaler.matchWidthOrHeight = 0.5f;
            uiCanvasGo.AddComponent<GraphicRaycaster>();

            GameObject pausePanelGo = new GameObject("PausePanel");
            pausePanelGo.transform.SetParent(uiCanvasGo.transform, false);
            RectTransform pauseRect = pausePanelGo.AddComponent<RectTransform>();
            pauseRect.anchorMin = new Vector2(0.3f, 0.25f);
            pauseRect.anchorMax = new Vector2(0.7f, 0.75f);
            pauseRect.offsetMin = Vector2.zero;
            pauseRect.offsetMax = Vector2.zero;
            Image pauseBg = pausePanelGo.AddComponent<Image>();
            pauseBg.color = new Color(0.02f, 0.02f, 0.08f, 0.92f);
            pausePanelGo.SetActive(false);

            string[] pauseLabels = { "RESUME", "RETRY", "QUIT TO MENU" };
            for (int i = 0; i < pauseLabels.Length; i++)
            {
                GameObject btnGo = new GameObject(pauseLabels[i]);
                btnGo.transform.SetParent(pausePanelGo.transform, false);
                RectTransform btnRect = btnGo.AddComponent<RectTransform>();
                btnRect.anchorMin = new Vector2(0.1f, 0.7f - i * 0.25f);
                btnRect.anchorMax = new Vector2(0.9f, 0.88f - i * 0.25f);
                btnRect.offsetMin = Vector2.zero;
                btnRect.offsetMax = Vector2.zero;
                Text btnTxt = btnGo.AddComponent<Text>();
                btnTxt.text = pauseLabels[i];
                btnTxt.fontSize = 30;
                btnTxt.fontStyle = FontStyle.Bold;
                btnTxt.color = new Color(0.85f, 0.78f, 0.45f);
                btnTxt.alignment = TextAnchor.MiddleCenter;
                Shadow btnShadow = btnGo.AddComponent<Shadow>();
                btnShadow.effectColor = new Color(0f, 0f, 0f, 0.9f);
                btnShadow.effectDistance = new Vector2(2f, -2f);
                btnGo.AddComponent<Button>();
            }

            GameObject pauseControllerGo = new GameObject("PauseMenuController");
            SpellCardDev.UI.PauseMenuController pauseCtrl = pauseControllerGo.AddComponent<SpellCardDev.UI.PauseMenuController>();
            SerializedObject soPause = new SerializedObject(pauseCtrl);
            soPause.FindProperty("pausePanel").objectReferenceValue = pausePanelGo;
            soPause.ApplyModifiedProperties();

            GameObject eventSystemGo = new GameObject("EventSystem");
            eventSystemGo.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemGo.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

            string scenePath = "Assets/_Project/Scenes/Game.unity";
            EditorSceneManager.SaveScene(scene, scenePath);
            Debug.Log("[SpellCardDev] Game scene created at: " + scenePath);
        }
    }
}
