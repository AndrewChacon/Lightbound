using UnityEngine;

namespace Lightbound
{
    [CreateAssetMenu(fileName = "AsteroidFieldConfig", menuName = "Lightbound/Asteroid Field Config")]
    public class AsteroidFieldConfig : ScriptableObject
    {
        [Header("Waves")]
        public float waveInterval = 5f;
        public int asteroidsPerWave = 3;
        public int maxConcurrent = 20;

        [Header("Size mix (weights; large+medium should total <= 1, remainder is small)")]
        [Range(0f, 1f)] public float largeWeight = 0.15f;
        [Range(0f, 1f)] public float mediumWeight = 0.35f;

        [Header("Spawn volume (box size centered on the field's position)")]
        public Vector3 spawnVolume = new Vector3(20f, 10f, 20f);

        [Header("Drift")]
        public Vector3 driftDirection = Vector3.forward;
        [Range(0f, 1f)] public float driftSpread = 0.3f;
        public float speedMin = 1f;
        public float speedMax = 3f;

        [Header("Cleanup")]
        public float maxDriftDistance = 60f;
    }
}
