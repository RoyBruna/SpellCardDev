using System.Collections;
using UnityEngine;
using SpellCardDev.Core;

namespace SpellCardDev.Enemies
{
    public class CirclePatternSpawner : MonoBehaviour
    {
        [SerializeField] private int bulletCount = 16;
        [SerializeField] private float spawnInterval = 1.5f;
        [SerializeField] private float bulletSpeed = 4f;
        [SerializeField] private float rotationPerWave = 0f;
        [SerializeField] private bool autoStart = true;

        [SerializeField] private BulletPool enemyBulletPool;

        private float _currentRotation = 0f;
        private Coroutine _spawnCoroutine;

        private void Start()
        {
            if (enemyBulletPool == null && GameManager.Instance != null)
                enemyBulletPool = GameManager.Instance.EnemyBulletPool;

            if (autoStart && GetComponent<BossController>() == null)
            {
                StartSpawning();
            }
        }

        public void StartSpawning()
        {
            if (enemyBulletPool == null && GameManager.Instance != null)
                enemyBulletPool = GameManager.Instance.EnemyBulletPool;

            if (_spawnCoroutine != null)
                StopCoroutine(_spawnCoroutine);

            _spawnCoroutine = StartCoroutine(SpawnLoop());
        }

        public void StopSpawning()
        {
            if (_spawnCoroutine != null)
            {
                StopCoroutine(_spawnCoroutine);
                _spawnCoroutine = null;
            }
        }

        private IEnumerator SpawnLoop()
        {
            yield return new WaitForSeconds(0.5f);

            while (true)
            {
                EmitRing();
                yield return new WaitForSeconds(spawnInterval);
            }
        }

        private void EmitRing()
        {
            if (bulletCount <= 0 || enemyBulletPool == null) return;

            float angleStep = 360f / bulletCount;

            for (int i = 0; i < bulletCount; i++)
            {
                float angleDeg = _currentRotation + i * angleStep;
                float angleRad = angleDeg * Mathf.Deg2Rad;

                Vector2 direction = new Vector2(
                    Mathf.Sin(angleRad),
                    Mathf.Cos(angleRad)
                );

                SpawnBullet(direction);
            }

            _currentRotation = (_currentRotation + rotationPerWave) % 360f;
        }

        private void SpawnBullet(Vector2 direction)
        {
            GameObject go = enemyBulletPool.GetBullet();
            go.transform.position = transform.position;

            Bullets.EnemyBullet bullet = go.GetComponent<Bullets.EnemyBullet>();
            if (bullet != null)
                bullet.Launch(direction, bulletSpeed, enemyBulletPool);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.4f);
            Gizmos.DrawWireSphere(transform.position, 0.3f);

            if (bulletCount <= 0) return;
            float step = 360f / bulletCount;
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.25f);
            for (int i = 0; i < bulletCount; i++)
            {
                float a = (i * step) * Mathf.Deg2Rad;
                Vector3 dir = new Vector3(Mathf.Sin(a), Mathf.Cos(a), 0f);
                Gizmos.DrawRay(transform.position, dir * 0.5f);
            }
        }
#endif
    }
}
