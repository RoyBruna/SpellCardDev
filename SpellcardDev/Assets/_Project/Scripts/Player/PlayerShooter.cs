using UnityEngine;
using SpellCardDev.Core;

namespace SpellCardDev.Player
{
    public class PlayerShooter : MonoBehaviour
    {
        [SerializeField] private Sprite playerBulletSprite;

        [SerializeField] private float fireRate = 0.08f;
        [SerializeField] private float bulletSpeed = 12f;
        [SerializeField] private Vector2 shootOffset = new Vector2(0f, 0.4f);

        [SerializeField] private float maxChargeTime = 2f;
        [SerializeField] private int minBulletsCharged = 1;
        [SerializeField] private int maxBulletsCharged = 7;
        [SerializeField] private float chargedBulletSpeed = 18f;
        [SerializeField] private float chargedSpreadAngle = 30f;

        [SerializeField] private BulletPool playerBulletPool;

        private float _fireCooldown = 0f;
        private float _chargeTime = 0f;
        private bool _isCharging = false;

        private GameObject _chargeIndicator;
        private SpriteRenderer _chargeIndicatorSr;
        private PlayerController _playerController;

        private void Start()
        {
            _playerController = GetComponent<PlayerController>();

            if (playerBulletPool == null)
                playerBulletPool = GameManager.Instance?.PlayerBulletPool;

            SetupChargeIndicator();
        }

        private void Update()
        {
            if (_playerController != null && _playerController.CurrentLives <= 0)
            {
                if (_chargeIndicator != null && _chargeIndicator.activeSelf)
                    _chargeIndicator.SetActive(false);
                return;
            }

            HandleContinuousFire();
            HandleChargedShot();
        }

        private void SetupChargeIndicator()
        {
            _chargeIndicator = new GameObject("ChargeIndicator");
            _chargeIndicator.transform.SetParent(transform, false);
            _chargeIndicator.transform.localPosition = Vector3.zero;

            _chargeIndicatorSr = _chargeIndicator.AddComponent<SpriteRenderer>();
            _chargeIndicatorSr.sortingOrder = 10;

            Sprite indicatorSprite = SpriteGenerator.CreateCircle(32,
                new Color(0f, 0.85f, 1f, 0.4f),
                new Color(1f, 1f, 1f, 0.0f));
            _chargeIndicatorSr.sprite = indicatorSprite;
            _chargeIndicator.SetActive(false);
        }

        private void HandleContinuousFire()
        {
            _fireCooldown -= Time.deltaTime;

            if (Input.GetButton("Shoot") && _fireCooldown <= 0f)
            {
                FireSingleBullet(Vector2.up, bulletSpeed);
                _fireCooldown = fireRate;
            }
        }

        private void FireSingleBullet(Vector2 direction, float speed)
        {
            if (playerBulletPool == null) return;

            GameObject go = playerBulletPool.GetBullet();
            go.transform.position = (Vector2)transform.position + shootOffset;
            go.transform.up = direction;

            Bullets.PlayerBullet bullet = go.GetComponent<Bullets.PlayerBullet>();
            if (bullet != null)
            {
                bullet.Launch(direction, speed, playerBulletPool);
                AssignBulletSprite(bullet);
            }
        }

        private void HandleChargedShot()
        {
            bool chargeKeyHeld = Input.GetButton("ChargedShot");

            if (chargeKeyHeld)
            {
                _isCharging = true;
                _chargeTime = Mathf.Min(_chargeTime + Time.deltaTime, maxChargeTime);

                _chargeIndicator.SetActive(true);
                float t = _chargeTime / maxChargeTime;
                float scale = Mathf.Lerp(0.2f, 2f, t);
                _chargeIndicator.transform.localScale = Vector3.one * scale;
                _chargeIndicatorSr.color = new Color(0f, 0.85f, 1f, Mathf.Lerp(0.15f, 0.6f, t));
            }
            else if (_isCharging)
            {
                _isCharging = false;
                _chargeIndicator.SetActive(false);
                FireChargedBurst();
                _chargeTime = 0f;
            }
        }

        private void FireChargedBurst()
        {
            float t = _chargeTime / maxChargeTime;
            int count = Mathf.RoundToInt(Mathf.Lerp(minBulletsCharged, maxBulletsCharged, t));

            if (count == 1)
            {
                FireSingleBullet(Vector2.up, chargedBulletSpeed);
                return;
            }

            float halfSpread = chargedSpreadAngle * 0.5f;
            for (int i = 0; i < count; i++)
            {
                float angle = Mathf.Lerp(-halfSpread, halfSpread, (float)i / (count - 1));
                Vector2 dir = Quaternion.Euler(0f, 0f, angle) * Vector2.up;
                FireSingleBullet(dir, chargedBulletSpeed);
            }
        }

        private void AssignBulletSprite(Bullets.PlayerBullet bullet)
        {
            bullet.OverrideSprite(playerBulletSprite);
        }
    }
}
