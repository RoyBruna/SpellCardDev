using UnityEngine;

namespace SpellCardDev.Core
{
    /// <summary>
    /// Singleton global del juego.
    /// Mantiene referencias a los dos pools de balas y coordina acciones globales
    /// (como limpiar pantalla con la bomba).
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        // ─── Singleton ───────────────────────────────────────────────────────────
        public static GameManager Instance { get; private set; }

        // ─── Referencias ─────────────────────────────────────────────────────────
        [Header("Pools de Balas")]
        [SerializeField] private BulletPool playerBulletPool;
        [SerializeField] private BulletPool enemyBulletPool;

        // ─── Propiedades públicas ─────────────────────────────────────────────────
        public BulletPool PlayerBulletPool => playerBulletPool;
        public BulletPool EnemyBulletPool  => enemyBulletPool;

        // ─── Lifecycle ────────────────────────────────────────────────────────────
        private void Awake()
        {
            // Patrón Singleton estricto
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("[GameManager] Instancia duplicada destruida.", this);
                Destroy(gameObject);
                return;
            }
            Instance = this;
            // No usar DontDestroyOnLoad por ahora — escena única de desarrollo
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        // ─── API Pública ──────────────────────────────────────────────────────────

        /// <summary>
        /// Limpia TODAS las balas enemigas de la pantalla.
        /// Llamado por PlayerBomb cuando el jugador usa la bomba (C).
        /// </summary>
        public void ClearAllEnemyBullets()
        {
            if (enemyBulletPool == null)
            {
                Debug.LogWarning("[GameManager] enemyBulletPool no asignado.", this);
                return;
            }
            enemyBulletPool.ReturnAll();
        }
    }
}
