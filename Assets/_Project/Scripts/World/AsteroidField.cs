using UnityEngine;

namespace Lightbound
{
    /// <summary>
    /// Periodically spawns a wave of asteroids inside a box volume centered on
    /// this object, drifting in a configured general direction. Caps concurrent
    /// asteroids (counting fragments too, via a scene-wide count) so an
    /// unattended field doesn't run away with itself.
    /// </summary>
    public class AsteroidField : MonoBehaviour
    {
        [SerializeField] private AsteroidFieldConfig config;
        [SerializeField] private Asteroid asteroidPrefab;

        private float _nextWaveTime;

        private void Update()
        {
            if (Time.time < _nextWaveTime) return;
            _nextWaveTime = Time.time + config.waveInterval;

            SpawnWave();
        }

        private void SpawnWave()
        {
            int currentCount = FindObjectsByType<Asteroid>(FindObjectsSortMode.None).Length;

            for (int i = 0; i < config.asteroidsPerWave && currentCount < config.maxConcurrent; i++)
            {
                SpawnOne();
                currentCount++;
            }
        }

        private void SpawnOne()
        {
            Vector3 halfVolume = config.spawnVolume * 0.5f;
            Vector3 spawnPoint = transform.position + new Vector3(
                Random.Range(-halfVolume.x, halfVolume.x),
                Random.Range(-halfVolume.y, halfVolume.y),
                Random.Range(-halfVolume.z, halfVolume.z));

            AsteroidSize size = PickSize();
            float speed = Random.Range(config.speedMin, config.speedMax);
            Vector3 direction = (config.driftDirection.normalized
                + Random.insideUnitSphere * config.driftSpread).normalized;

            Asteroid asteroid = Instantiate(asteroidPrefab, spawnPoint, Random.rotation);
            asteroid.Initialize(size, direction * speed, config.maxDriftDistance);
        }

        private AsteroidSize PickSize()
        {
            float roll = Random.value;
            if (roll < config.largeWeight) return AsteroidSize.Large;
            if (roll < config.largeWeight + config.mediumWeight) return AsteroidSize.Medium;
            return AsteroidSize.Small;
        }

        private void OnDrawGizmosSelected()
        {
            if (config == null) return;
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.position, config.spawnVolume);
        }
    }
}
