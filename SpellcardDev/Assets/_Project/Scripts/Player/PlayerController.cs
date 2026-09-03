using System.Collections;
using UnityEngine;
using SpellCardDev.Core;

namespace SpellCardDev.Player
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(CircleCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Sprite playerSprite;
        [SerializeField] private Sprite hitboxSprite;

        [SerializeField] private float normalSpeed = 6f;
        [SerializeField] private float focusSpeed = 2.5f;

        [SerializeField] private float hitboxScale = 0.12f;
        [SerializeField] private float hitboxRadius = 0.06f;

        [SerializeField] private int maxLives = 3;
        [SerializeField] private float invulnerabilityDuration = 2f;

        private int _currentLives;
        private bool _isInvulnerable;

        private SpriteRenderer _sr;
        private CircleCollider2D _col;
        private Rigidbody2D _rb;
        private GameObject _hitboxGo;
        private SpriteRenderer _hitboxSr;
        private Camera _cam;

        private float _xMin, _xMax, _yMin, _yMax;

        public int CurrentLives => _currentLives;
        public int MaxLives => maxLives;
        public bool IsInvulnerable => _isInvulnerable;

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
            _cam = Camera.main;

            _col = GetComponent<CircleCollider2D>();
            if (_col == null) _col = gameObject.AddComponent<CircleCollider2D>();
            _col.isTrigger = true;
            _col.radius = hitboxRadius;
            _col.offset = Vector2.zero;

            _rb = GetComponent<Rigidbody2D>();
            if (_rb == null) _rb = gameObject.AddComponent<Rigidbody2D>();
            _rb.bodyType = RigidbodyType2D.Kinematic;
            _rb.gravityScale = 0f;
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            _currentLives = maxLives;

            SetupSprites();
            SetupHitbox();
            CalculateBounds();
        }

        private void Update()
        {
            HandleMovement();
        }

        private void SetupSprites()
        {
            if (playerSprite == null)
            {
                Color bodyColor = new Color(0.27f, 0.53f, 1f);
                Color accentColor = new Color(0.8f, 0.9f, 1f);
                playerSprite = SpriteGenerator.CreateShipShape(48, bodyColor, accentColor);
            }
            _sr.sprite = playerSprite;
        }

        private void SetupHitbox()
        {
            _hitboxGo = new GameObject("Hitbox");
            _hitboxGo.transform.SetParent(transform, false);
            _hitboxGo.transform.localPosition = Vector3.zero;

            _hitboxSr = _hitboxGo.AddComponent<SpriteRenderer>();
            _hitboxSr.sortingOrder = _sr.sortingOrder + 1;

            if (hitboxSprite == null)
            {
                hitboxSprite = SpriteGenerator.CreateCircle(4, new Color(1f, 0.1f, 0.1f));
            }
            _hitboxSr.sprite = hitboxSprite;

            _hitboxGo.transform.localScale = Vector3.one * hitboxScale;
            _hitboxGo.SetActive(false);
        }

        private void CalculateBounds()
        {
            if (_cam == null) return;

            float halfHeight = _cam.orthographicSize;
            float halfWidth = halfHeight * _cam.aspect;
            float margin = 0.3f;

            _xMin = -halfWidth + margin;
            _xMax = halfWidth - margin;
            _yMin = -halfHeight + margin;
            _yMax = halfHeight - margin;
        }

        private void HandleMovement()
        {
            if (_currentLives <= 0) return;

            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            Vector2 dir = new Vector2(h, v);
            if (dir.sqrMagnitude > 1f) dir.Normalize();

            bool isFocus = Input.GetButton("Focus");
            float speed = isFocus ? focusSpeed : normalSpeed;

            if (_hitboxGo != null)
            {
                _hitboxGo.SetActive(isFocus);
            }

            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x + dir.x * speed * Time.deltaTime, _xMin, _xMax);
            pos.y = Mathf.Clamp(pos.y + dir.y * speed * Time.deltaTime, _yMin, _yMax);
            transform.position = pos;
        }

        public void TakeDamage()
        {
            if (_isInvulnerable || _currentLives <= 0) return;

            _currentLives--;

            if (_currentLives > 0)
            {
                StartCoroutine(InvulnerabilityRoutine());
            }
            else
            {
                _isInvulnerable = true;
                _sr.enabled = false;
                if (_hitboxGo != null) _hitboxGo.SetActive(false);
            }
        }

        private IEnumerator InvulnerabilityRoutine()
        {
            _isInvulnerable = true;
            float elapsed = 0f;
            float interval = 0.1f;

            while (elapsed < invulnerabilityDuration)
            {
                _sr.enabled = !_sr.enabled;
                yield return new WaitForSeconds(interval);
                elapsed += interval;
            }

            _sr.enabled = true;
            _isInvulnerable = false;
        }

        private void OnGUI()
        {
            GUI.Label(new Rect(10, 35, 200, 30), $"Vidas: {_currentLives} / {maxLives}");

            if (_currentLives <= 0)
            {
                GUIStyle gameOverStyle = new GUIStyle(GUI.skin.label);
                gameOverStyle.alignment = TextAnchor.MiddleCenter;
                gameOverStyle.fontSize = 24;
                gameOverStyle.fontStyle = FontStyle.Bold;
                gameOverStyle.normal.textColor = Color.red;
                GUI.Label(new Rect(0, Screen.height / 2 - 60, Screen.width, 40), "GAME OVER", gameOverStyle);

                if (GUI.Button(new Rect(Screen.width / 2 - 60, Screen.height / 2, 120, 40), "Retry"))
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
                }
            }
        }

#if UNITY_EDITOR
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 0.1f, 0.1f, 0.85f);
            Gizmos.DrawWireSphere(transform.position, hitboxRadius);
        }
#endif
    }
}
