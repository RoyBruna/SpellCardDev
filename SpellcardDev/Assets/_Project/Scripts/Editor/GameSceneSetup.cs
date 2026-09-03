using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using SpellCardDev.Core;
using SpellCardDev.Player;
using SpellCardDev.Enemies;
using SpellCardDev.Background;
using SpellCardDev.EditorUtils;

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

            // 6. Background
            GameObject bgGo = new GameObject("Background");
            bgGo.AddComponent<BackgroundVideoPlayer>();
            
            // Guardar escena
            string scenePath = "Assets/_Project/Scenes/Game.unity";
            EditorSceneManager.SaveScene(scene, scenePath);
            Debug.Log("[SpellCardDev] Escena Game creada con éxito en: " + scenePath);
        }
    }
}
