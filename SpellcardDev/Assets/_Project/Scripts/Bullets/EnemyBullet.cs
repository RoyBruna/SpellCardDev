using UnityEngine;
using SpellCardDev.Core;

namespace SpellCardDev.Bullets
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class EnemyBullet : MonoBehaviour
    {
        [SerializeField] private Sprite bulletSprite;
        private static Sprite _cachedFallbackSprite;

        [SerializeField] private float speed = 4f;
        [SerializeField] private float hitboxRatio = 0.75f;

        private Vector2 _direction;
        private float _activeSpeed;
        private BulletPool _pool;
        private float _screenLimit;

        private SpriteRenderer _sr;
        private CircleCollider2D _col;

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
            _col = GetComponent<CircleCollider2D>();
            _col.isTrigger = true;

            ApplySprite();

            Camera cam = Camera.main;
            if (cam != null)
            {
                float halfH = cam.orthographicSize + 2f;
                float halfW = halfH * cam.aspect + 2f;
                _screenLimit = Mathf.Max(halfH, halfW);
            }
            else
            {
                _screenLimit = 12f;
            }
        }

        private void OnEnable()
        {
            ApplySprite();
        }

        private void Update()
        {
            transform.Translate(_direction * _activeSpeed * Time.deltaTime, Space.World);

            if (IsOutOfBounds())
                ReturnToPool();
        }

        public void Launch(Vector2 direction, float overrideSpeed, BulletPool pool)
        {
            _direction = direction.normalized;
            _activeSpeed = overrideSpeed > 0f ? overrideSpeed : speed;
            _pool = pool;

            if (_direction != Vector2.zero)
            {
                float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg - 90f;
                transform.rotation = Quaternion.Euler(0f, 0f, angle);
            }
        }

        public void OverrideSprite(Sprite newSprite)
        {
            if (newSprite != null)
            {
                bulletSprite = newSprite;
                if (_sr == null) _sr = GetComponent<SpriteRenderer>();
                if (_col == null) _col = GetComponent<CircleCollider2D>();
                if (_sr != null) _sr.sprite = newSprite;
                UpdateHitbox();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Player.PlayerController player = other.GetComponent<Player.PlayerController>();
                if (player != null)
                {
                    player.TakeDamage();
                    ReturnToPool();
                }
            }
        }

        private bool IsOutOfBounds()
        {
            Vector3 p = transform.position;
            return Mathf.Abs(p.x) > _screenLimit || Mathf.Abs(p.y) > _screenLimit;
        }

        private void ApplySprite()
        {
            if (bulletSprite == null)
            {
                if (_cachedFallbackSprite == null)
                    _cachedFallbackSprite = SpriteGenerator.CreateCircle(24, new Color(1f, 0.4f, 0.4f), Color.red);
                bulletSprite = _cachedFallbackSprite;
            }
            if (_sr != null)
                _sr.sprite = bulletSprite;

            UpdateHitbox();
        }

        private void UpdateHitbox()
        {
            if (_sr != null && _sr.sprite != null && _col != null)
            {
                float ppu = _sr.sprite.pixelsPerUnit > 0f ? _sr.sprite.pixelsPerUnit : 32f;
                float minDim = Mathf.Min(_sr.sprite.rect.width, _sr.sprite.rect.height);
                _col.radius = (minDim / (2f * ppu)) * hitboxRatio;
                _col.offset = Vector2.zero;
            }
        }

        public void ReturnToPool()
        {
            if (_pool != null)
                _pool.ReturnBullet(gameObject);
            else
                gameObject.SetActive(false);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.6f);
            CircleCollider2D c = GetComponent<CircleCollider2D>();
            float r = c != null ? c.radius * transform.lossyScale.x : 0.1f;
            Gizmos.DrawWireSphere(transform.position, r);
        }
#endif
    }
}
