using System.Collections.Generic;
using UnityEngine;

namespace SpellCardDev.Core
{
    /// <summary>
    /// Pool genérico de GameObjects pre-instanciados.
    /// Elimina la necesidad de Instantiate/Destroy durante el juego,
    /// evitando GC spikes con cientos de balas en pantalla.
    ///
    /// Uso:
    ///   1. Asigná el prefab y poolSize en el Inspector.
    ///   2. Llamá GetBullet() para obtener una bala activa.
    ///   3. Llamá ReturnBullet(go) para devolvela al pool cuando ya no se necesite.
    /// </summary>
    public class BulletPool : MonoBehaviour
    {
        [Header("Configuración del Pool")]
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private int poolSize = 200;

        private readonly Queue<GameObject> _available = new Queue<GameObject>();
        private readonly List<GameObject> _active = new List<GameObject>();

        private void Awake()
        {
            if (bulletPrefab == null)
            {
                Debug.LogError($"[BulletPool] '{gameObject.name}': bulletPrefab no asignado.", this);
                return;
            }

            // Pre-instanciar todas las balas desactivadas
            for (int i = 0; i < poolSize; i++)
            {
                GameObject go = Instantiate(bulletPrefab, transform);
                go.SetActive(false);
                go.name = $"{bulletPrefab.name}_Pool_{i:000}";
                _available.Enqueue(go);
            }
        }

        /// <summary>
        /// Devuelve una bala del pool, activándola.
        /// Si el pool está vacío, instancia una extra y lanza un warning.
        /// </summary>
        public GameObject GetBullet()
        {
            GameObject go;

            if (_available.Count > 0)
            {
                go = _available.Dequeue();
            }
            else
            {
                // Pool agotado — expandir dinámicamente
                Debug.LogWarning($"[BulletPool] '{gameObject.name}': Pool agotado. Incrementá poolSize.", this);
                go = Instantiate(bulletPrefab, transform);
                go.name = $"{bulletPrefab.name}_Pool_Extra";
            }

            go.SetActive(true);
            _active.Add(go);
            return go;
        }

        /// <summary>
        /// Devuelve una bala al pool, desactivándola.
        /// </summary>
        public void ReturnBullet(GameObject go)
        {
            if (go == null) return;

            go.SetActive(false);
            _active.Remove(go);
            _available.Enqueue(go);
        }

        /// <summary>
        /// Desactiva y devuelve al pool TODAS las balas activas.
        /// Llamado por la bomba del jugador.
        /// </summary>
        public void ReturnAll()
        {
            // Iterar sobre copia porque modificamos la lista
            for (int i = _active.Count - 1; i >= 0; i--)
            {
                ReturnBullet(_active[i]);
            }
        }

        /// <summary>
        /// Cantidad de balas actualmente activas en pantalla.
        /// </summary>
        public int ActiveCount => _active.Count;
    }
}
