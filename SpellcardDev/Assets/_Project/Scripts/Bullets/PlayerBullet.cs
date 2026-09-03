using UnityEngine;
using SpellCardDev.Core;

namespace SpellCardDev.Bullets
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class PlayerBullet : MonoBehaviour
    {
        [SerializeField] private Sprite bulletSprite;
        private static Sprite _cachedFallbackSprite;

        [SerializeField] private float speed = 12f;
        [SerializeField] private float hitboxRatio = 0.75f;

        private Vector2 _direction;
        private float _activeSpeed;
        private BulletPool _pool;
        private float _screenTop;
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
                _screenTop = cam.orthographicSize + 1f;
        }

        private void OnEnable()
        {
            ApplySprite();
        }

        private void Update()
        {
            transform.Translate(_direction * _activeSpeed * Time.deltaTime, Space.World);

            if (transform.position.y > _screenTop)
                ReturnToPool();
        }

        public void Launch(Vector2 direction, float overrideSpeed, BulletPool pool)
        {
            _direction = direction.normalized;
            _activeSpeed = overrideSpeed > 0f ? overrideSpeed : speed;
            _pool = pool;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Enemies.BossController boss = other.GetComponent<Enemies.BossController>();
            if (boss != null)
            {
                boss.TakeDamage(1f);
                ReturnToPool();
                return;
            }

            Enemies.FairyEnemy fairy = other.GetComponent<Enemies.FairyEnemy>();
            if (fairy != null)
            {
                fairy.TakeDamage(1f);
                ReturnToPool();
                return;
            }
        }

        public void OverrideSprite(Sprite overrideSprite)
        {
            if (overrideSprite != null)
            {
                _sr.sprite = overrideSprite;
                UpdateHitbox();
            }
        }

        private void ApplySprite()
        {
            if (bulletSprite == null)
            {
                if (_cachedFallbackSprite == null)
                    _cachedFallbackSprite = SpriteGenerator.CreateCircle(32, Color.white);
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

        private void ReturnToPool()
        {
            if (_pool != null)
                _pool.ReturnBullet(gameObject);
            else
                gameObject.SetActive(false);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.3f, 0.8f, 1f, 0.5f);
            CircleCollider2D c = GetComponent<CircleCollider2D>();
            float r = c != null ? c.radius * transform.lossyScale.x : 0.05f;
            Gizmos.DrawWireSphere(transform.position, r);
        }
#endif
    }
}
