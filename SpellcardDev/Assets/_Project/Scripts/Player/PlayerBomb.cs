using System.Collections;
using UnityEngine;
using SpellCardDev.Core;

namespace SpellCardDev.Player
{
    public class PlayerBomb : MonoBehaviour
    {
        [SerializeField] private Sprite bombSprite;
        [SerializeField] private float bombCooldown = 4f;
        [SerializeField] private float expandDuration = 1.2f;
        [SerializeField] private float startRadius = 0.5f;
        [SerializeField] private float maxRadius = 14f;
        [SerializeField] private float damageToBoss = 25f;
        [SerializeField] private bool followPlayer = true;

        private float _lastBombTime = -99f;
        private PlayerController _playerController;

        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
        }

        private void Update()
        {
            if (_playerController != null && _playerController.CurrentLives <= 0)
                return;

            if (Input.GetButtonDown("Bomb"))
            {
                TryUseBomb();
            }
        }

        private void TryUseBomb()
        {
            if (Time.time - _lastBombTime < bombCooldown)
                return;

            _lastBombTime = Time.time;
            StartCoroutine(ExpandBombRoutine());
        }

        private IEnumerator ExpandBombRoutine()
        {
            GameObject waveGo = new GameObject("BombWave");
            Vector3 center = transform.position;
            waveGo.transform.position = center;

            SpriteRenderer sr = waveGo.AddComponent<SpriteRenderer>();
            sr.sortingOrder = 200;

            Sprite spriteToUse = bombSprite;
            if (spriteToUse == null)
            {
                spriteToUse = SpriteGenerator.CreateCircle(32, new Color(1f, 0.9f, 0.3f, 0.6f), new Color(1f, 0.4f, 0.1f, 0.0f));
            }
            sr.sprite = spriteToUse;

            float ppu = spriteToUse.pixelsPerUnit > 0f ? spriteToUse.pixelsPerUnit : 32f;
            float baseRadius = spriteToUse.rect.width / (2f * ppu);
            if (baseRadius <= 0f) baseRadius = 1f;

            Color baseColor = sr.color;
            float damageRate = damageToBoss / Mathf.Max(0.01f, expandDuration);
            float elapsed = 0f;

            while (elapsed < expandDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / expandDuration);
                float currentRadius = Mathf.Lerp(startRadius, maxRadius, Mathf.Sin(t * Mathf.PI * 0.5f));

                if (followPlayer)
                {
                    center = transform.position;
                }
                waveGo.transform.position = center;

                float scale = currentRadius / baseRadius;
                waveGo.transform.localScale = Vector3.one * scale;

                float alpha = t > 0.7f ? Mathf.Lerp(baseColor.a, 0f, (t - 0.7f) / 0.3f) : baseColor.a;
                sr.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);

                Collider2D[] hits = Physics2D.OverlapCircleAll(center, currentRadius);
                for (int i = 0; i < hits.Length; i++)
                {
                    Bullets.EnemyBullet bullet = hits[i].GetComponent<Bullets.EnemyBullet>();
                    if (bullet != null)
                    {
                        bullet.ReturnToPool();
                        continue;
                    }

                    Enemies.BossController boss = hits[i].GetComponent<Enemies.BossController>();
                    if (boss != null)
                    {
                        boss.TakeDamage(damageRate * Time.deltaTime);
                        continue;
                    }

                    Enemies.FairyEnemy fairy = hits[i].GetComponent<Enemies.FairyEnemy>();
                    if (fairy != null)
                    {
                        fairy.TakeDamage(damageRate * Time.deltaTime);
                    }
                }

                yield return null;
            }

            Destroy(waveGo);
        }

        private void OnGUI()
        {
            float remaining = Mathf.Max(0f, bombCooldown - (Time.time - _lastBombTime));
            GUI.Label(new Rect(10, 10, 200, 30), $"Bomba CD: {remaining:F1}s");
        }
    }
}
