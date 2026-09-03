using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using SpellCardDev.Core;

namespace SpellCardDev.Enemies
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(CircleCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class BossController : MonoBehaviour
    {
        [SerializeField] private Sprite bossSprite;
        [SerializeField] private string bossName = "Stage 1 Boss";

        [SerializeField] private float maxHealth = 150f;
        [SerializeField] private float hitboxRadius = 0.5f;

        [SerializeField] private float introDuration = 2.5f;
        [SerializeField] private Vector3 spawnPosition = new Vector3(0f, 8f, 0f);
        [SerializeField] private Vector3 battlePosition = new Vector3(0f, 4f, 0f);

        private float _currentHealth;
        private bool _isIntro = true;
        private bool _isDefeated = false;

        private SpriteRenderer _sr;
        private CircleCollider2D _col;
        private Rigidbody2D _rb;
        private CirclePatternSpawner _spawner;
        private Coroutine _flashCoroutine;

        public bool IsIntro => _isIntro;
        public bool IsDefeated => _isDefeated;
        public float CurrentHealth => _currentHealth;
        public float MaxHealth => maxHealth;

        private void Awake()
        {
            gameObject.tag = "Enemy";

            _sr = GetComponent<SpriteRenderer>();
            _col = GetComponent<CircleCollider2D>();
            _rb = GetComponent<Rigidbody2D>();
            _spawner = GetComponent<CirclePatternSpawner>();

            if (_col == null) _col = gameObject.AddComponent<CircleCollider2D>();
            _col.isTrigger = true;
            _col.radius = hitboxRadius;
            _col.offset = Vector2.zero;

            if (_rb == null) _rb = gameObject.AddComponent<Rigidbody2D>();
            _rb.bodyType = RigidbodyType2D.Kinematic;
            _rb.gravityScale = 0f;
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            if (bossSprite == null)
            {
                bossSprite = SpriteGenerator.CreateCircle(32, new Color(0.9f, 0.2f, 0.4f), new Color(1f, 0.8f, 0.9f));
            }
            _sr.sprite = bossSprite;

            transform.position = spawnPosition;
            _currentHealth = maxHealth;
        }

        private void Start()
        {
            if (_spawner != null)
                _spawner.StopSpawning();

            StartCoroutine(IntroRoutine());
        }

        private IEnumerator IntroRoutine()
        {
            _isIntro = true;

            if (GameManager.Instance != null)
                GameManager.Instance.ClearAllEnemyBullets();

            float elapsed = 0f;
            while (elapsed < introDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / introDuration);
                transform.position = Vector3.Lerp(spawnPosition, battlePosition, t);
                yield return null;
            }

            transform.position = battlePosition;
            _isIntro = false;

            if (GameManager.Instance != null)
                GameManager.Instance.ClearAllEnemyBullets();

            if (_spawner != null)
            {
                _spawner.StartSpawning();
            }
        }

        public void TakeDamage(float damage)
        {
            if (_isIntro || _isDefeated) return;

            _currentHealth -= damage;

            if (_flashCoroutine != null)
                StopCoroutine(_flashCoroutine);
            _flashCoroutine = StartCoroutine(HitFlashRoutine());

            if (_currentHealth <= 0f)
            {
                _currentHealth = 0f;
                _isDefeated = true;

                if (_spawner != null)
                    _spawner.StopSpawning();

                if (GameManager.Instance != null)
                    GameManager.Instance.ClearAllEnemyBullets();

                _col.enabled = false;
                _sr.enabled = false;
            }
        }

        private IEnumerator HitFlashRoutine()
        {
            _sr.color = Color.red;
            yield return new WaitForSeconds(0.06f);
            _sr.color = Color.white;
        }

        private void OnGUI()
        {
            if (_isIntro)
            {
                GUIStyle introStyle = new GUIStyle(GUI.skin.label);
                introStyle.alignment = TextAnchor.MiddleCenter;
                introStyle.fontSize = 20;
                introStyle.fontStyle = FontStyle.Bold;
                introStyle.normal.textColor = Color.yellow;
                GUI.Label(new Rect(0, 80, Screen.width, 30), $"WARNING: {bossName}", introStyle);
                return;
            }

            if (!_isDefeated)
            {
                float barWidth = 360f;
                float barHeight = 14f;
                float x = (Screen.width - barWidth) / 2f;
                float y = 16f;

                GUI.Box(new Rect(x - 2, y - 2, barWidth + 4, barHeight + 4), GUIContent.none);
                float pct = Mathf.Clamp01(_currentHealth / maxHealth);
                GUI.color = Color.red;
                GUI.DrawTexture(new Rect(x, y, barWidth * pct, barHeight), Texture2D.whiteTexture);
                GUI.color = Color.white;

                GUIStyle nameStyle = new GUIStyle(GUI.skin.label);
                nameStyle.alignment = TextAnchor.UpperCenter;
                nameStyle.fontSize = 12;
                nameStyle.fontStyle = FontStyle.Bold;
                GUI.Label(new Rect(x, y + barHeight + 2, barWidth, 20), $"{bossName}  {(int)_currentHealth} / {(int)maxHealth}", nameStyle);
            }
            else
            {
                GUIStyle clearStyle = new GUIStyle(GUI.skin.label);
                clearStyle.alignment = TextAnchor.MiddleCenter;
                clearStyle.fontSize = 24;
                clearStyle.fontStyle = FontStyle.Bold;
                clearStyle.normal.textColor = Color.green;
                GUI.Label(new Rect(0, Screen.height / 2 - 60, Screen.width, 40), "STAGE CLEAR!", clearStyle);

                if (GUI.Button(new Rect(Screen.width / 2 - 60, Screen.height / 2, 120, 40), "Retry"))
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            }
        }
    }
}
