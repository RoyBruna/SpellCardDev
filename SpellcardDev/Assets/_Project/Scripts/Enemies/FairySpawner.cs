using System.Collections;
using UnityEngine;

namespace SpellCardDev.Enemies
{
    public class FairySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject fairyPrefab;
        [SerializeField] private Sprite defaultFairySprite;
        [SerializeField] private Sprite defaultBulletSprite;
        [SerializeField] private float initialDelay = 1f;
        [SerializeField] private float waveInterval = 4.5f;

        private int _waveIndex = 0;

        private void Start()
        {
            StartCoroutine(SpawnWavesRoutine());
        }

        private IEnumerator SpawnWavesRoutine()
        {
            yield return new WaitForSeconds(initialDelay);

            while (true)
            {
                SpawnWave(_waveIndex);
                _waveIndex++;
                yield return new WaitForSeconds(waveInterval);
            }
        }

        private void SpawnWave(int index)
        {
            int pattern = index % 3;

            if (pattern == 0)
            {
                SpawnFairy(new Vector3(-4f, 7f, 0f), new Vector3(-3f, 3f, 0f), new Vector3(-5f, -7f, 0f), FairyAttackType.AimedFan, 5f);
                SpawnFairy(new Vector3(4f, 7f, 0f), new Vector3(3f, 3f, 0f), new Vector3(5f, -7f, 0f), FairyAttackType.AimedFan, 5f);
            }
            else if (pattern == 1)
            {
                SpawnFairy(new Vector3(-2f, 7f, 0f), new Vector3(-1.5f, 4f, 0f), new Vector3(6f, 2f, 0f), FairyAttackType.FastStream, 7f);
                SpawnFairy(new Vector3(2f, 7f, 0f), new Vector3(1.5f, 4f, 0f), new Vector3(-6f, 2f, 0f), FairyAttackType.FastStream, 7f);
            }
            else
            {
                SpawnFairy(new Vector3(0f, 7f, 0f), new Vector3(0f, 3.5f, 0f), new Vector3(0f, 7f, 0f), FairyAttackType.Ring, 4f);
            }
        }

        private void SpawnFairy(Vector3 enter, Vector3 hover, Vector3 exit, FairyAttackType type, float bSpeed)
        {
            GameObject go;
            if (fairyPrefab != null)
            {
                go = Instantiate(fairyPrefab);
            }
            else
            {
                go = new GameObject("Fairy");
            }

            FairyEnemy fairy = go.GetComponent<FairyEnemy>();
            if (fairy == null)
                fairy = go.AddComponent<FairyEnemy>();

            fairy.Configure(enter, hover, exit, type, defaultBulletSprite, bSpeed);
        }
    }
}
