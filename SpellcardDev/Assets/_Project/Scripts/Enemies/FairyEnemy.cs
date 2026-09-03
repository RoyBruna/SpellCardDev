using System.Collections;
using UnityEngine;
using SpellCardDev.Core;

namespace SpellCardDev.Enemies
{
    public enum FairyAttackType
    {
        AimedSingle,
        AimedFan,
        Ring,
        FastStream
    }

    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(CircleCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class FairyEnemy : MonoBehaviour
    {
        [SerializeField] private Sprite fairySprite;
        [SerializeField] private float hitboxRadius = 0.4f;

        [SerializeField] private float maxHealth = 6f;

        [SerializeField] private Vector3 enterPoint = new Vector3(-3f, 7f, 0f);
        [SerializeField] private Vector3 hoverPoint = new Vector3(-3f, 3f, 0f);
        [SerializeField] private Vector3 exitPoint = new Vector3(-3f, -7f, 0f);

        [SerializeField] private float enterDuration = 1.5f;
        [SerializeField] private float hoverDuration = 3f;
        [SerializeField] private float exitDuration = 1.5f;

        [SerializeField] private FairyAttackType attackType = FairyAttackType.AimedFan;
        [SerializeField] private Sprite bulletSprite;
        [SerializeField] private float bulletSpeed = 5f;
        [SerializeField] private float fireInterval = 0.8f;
        [SerializeField] private int bulletsPerWave = 3;
        [SerializeField] private float fanSpreadAngle = 30f;

        [SerializeField] private BulletPool enemyBulletPool;

        private float _currentHealth;
        private SpriteRenderer _sr;
        private CircleCollider2D _col;
        private Rigidbody2D _rb;
        private Coroutine _flashCoroutine;
        private Transform _playerTransform;

        private void Awake()
        {
            gameObject.tag = "Enemy";

            _sr = GetComponent<SpriteRenderer>();
            _col = GetComponent<CircleCollider2D>();
            _rb = GetComponent<Rigidbody2D>();

            if (_col == null) _col = gameObject.AddComponent<CircleCollider2D>();
            _col.isTrigger = true;
            _col.radius = hitboxRadius;
            _col.offset = Vector2.zero;

            if (_rb == null) _rb = gameObject.AddComponent<Rigidbody2D>();
            _rb.bodyType = RigidbodyType2D.Kinematic;
            _rb.gravityScale = 0f;
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            if (fairySprite == null)
            {
                fairySprite = SpriteGenerator.CreateCircle(24, new Color(0.2f, 0.7f, 1f), new Color(1f, 1f, 1f));
            }
            _sr.sprite = fairySprite;

            transform.position = enterPoint;
            _currentHealth = maxHealth;
        }

        private void Start()
        {
            if (enemyBulletPool == null && GameManager.Instance != null)
                enemyBulletPool = GameManager.Instance.EnemyBulletPool;

            GameObject playerGo = GameObject.FindWithTag("Player");
            if (playerGo != null)
                _playerTransform = playerGo.transform;

            StartCoroutine(LifecycleRoutine());
        }

        public void Configure(Vector3 enter, Vector3 hover, Vector3 exit, FairyAttackType type, Sprite bSprite = null, float speed = 5f)
        {
            enterPoint = enter;
            hoverPoint = hover;
            exitPoint = exit;
            attackType = type;
            if (bSprite != null) bulletSprite = bSprite;
            bulletSpeed = speed;
            transform.position = enterPoint;
        }

        private IEnumerator LifecycleRoutine()
        {
            float elapsed = 0f;
            while (elapsed < enterDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / enterDuration);
                transform.position = Vector3.Lerp(enterPoint, hoverPoint, t);
                yield return null;
            }

            transform.position = hoverPoint;

            float hoverElapsed = 0f;
            float shootTimer = 0.2f;

            while (hoverElapsed < hoverDuration)
            {
                hoverElapsed += Time.deltaTime;
                shootTimer -= Time.deltaTime;

                if (shootTimer <= 0f)
                {
                    FireAttack();
                    shootTimer = fireInterval;
                }

                yield return null;
            }

            elapsed = 0f;
            while (elapsed < exitDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / exitDuration);
                transform.position = Vector3.Lerp(hoverPoint, exitPoint, t);
                yield return null;
            }

            Destroy(gameObject);
        }

        private void FireAttack()
        {
            if (enemyBulletPool == null) return;

            Vector2 toPlayer = Vector2.down;
            if (_playerTransform != null)
            {
                Vector2 diff = _playerTransform.position - transform.position;
                if (diff.sqrMagnitude > 0.001f)
                    toPlayer = diff.normalized;
            }

            switch (attackType)
            {
                case FairyAttackType.AimedSingle:
                    SpawnBullet(toPlayer, bulletSpeed);
                    break;

                case FairyAttackType.AimedFan:
                    int count = Mathf.Max(2, bulletsPerWave);
                    float halfSpread = fanSpreadAngle * 0.5f;
                    for (int i = 0; i < count; i++)
                    {
                        float angle = Mathf.Lerp(-halfSpread, halfSpread, (float)i / (count - 1));
                        Vector2 dir = Quaternion.Euler(0f, 0f, angle) * toPlayer;
                        SpawnBullet(dir, bulletSpeed);
                    }
                    break;

                case FairyAttackType.Ring:
                    int ringCount = Mathf.Max(4, bulletsPerWave);
                    float step = 360f / ringCount;
                    for (int i = 0; i < ringCount; i++)
                    {
                        float rad = (i * step) * Mathf.Deg2Rad;
                        Vector2 dir = new Vector2(Mathf.Sin(rad), Mathf.Cos(rad));
                        SpawnBullet(dir, bulletSpeed);
                    }
                    break;

                case FairyAttackType.FastStream:
                    StartCoroutine(StreamRoutine(toPlayer));
                    break;
            }
        }

        private IEnumerator StreamRoutine(Vector2 dir)
        {
            for (int i = 0; i < 3; i++)
            {
                SpawnBullet(dir, bulletSpeed * 1.4f);
                yield return new WaitForSeconds(0.1f);
            }
        }

        private void SpawnBullet(Vector2 direction, float speed)
        {
            GameObject go = enemyBulletPool.GetBullet();
            if (go == null) return;

            go.transform.position = transform.position;

            Bullets.EnemyBullet bullet = go.GetComponent<Bullets.EnemyBullet>();
            if (bullet != null)
            {
                bullet.Launch(direction, speed, enemyBulletPool);
                if (bulletSprite != null)
                {
                    bullet.OverrideSprite(bulletSprite);
                }
            }
        }

        public void TakeDamage(float damage)
        {
            _currentHealth -= damage;

            if (_flashCoroutine != null)
                StopCoroutine(_flashCoroutine);
            _flashCoroutine = StartCoroutine(HitFlashRoutine());

            if (_currentHealth <= 0f)
            {
                Destroy(gameObject);
            }
        }

        private IEnumerator HitFlashRoutine()
        {
            _sr.color = Color.red;
            yield return new WaitForSeconds(0.06f);
            _sr.color = Color.white;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, hitboxRadius);
        }
#endif
    }
}
