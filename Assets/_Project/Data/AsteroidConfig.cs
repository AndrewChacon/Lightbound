using UnityEngine;

namespace Lightbound
{
    [CreateAssetMenu(fileName = "AsteroidConfig", menuName = "Lightbound/Asteroid Config")]
    public class AsteroidConfig : ScriptableObject
    {
        [Header("Health by size")]
        public float smallHealth = 10f;
        public float mediumHealth = 30f;
        public float largeHealth = 60f;

        [Header("Scale by size (also drives mass, bigger = heavier)")]
        public float smallScale = 0.5f;
        public float mediumScale = 1f;
        public float largeScale = 2f;

        [Header("Breaking")]
        public int fragmentCount = 2;
        public float fragmentSpeedMin = 2f;
        public float fragmentSpeedMax = 5f;

        public float GetMaxHealth(AsteroidSize size) => size switch
        {
            AsteroidSize.Small => smallHealth,
            AsteroidSize.Medium => mediumHealth,
            AsteroidSize.Large => largeHealth,
            _ => smallHealth,
        };

        public float GetScale(AsteroidSize size) => size switch
        {
            AsteroidSize.Small => smallScale,
            AsteroidSize.Medium => mediumScale,
            AsteroidSize.Large => largeScale,
            _ => smallScale,
        };

        public AsteroidSize? GetNextSmallerSize(AsteroidSize size) => size switch
        {
            AsteroidSize.Large => AsteroidSize.Medium,
            AsteroidSize.Medium => AsteroidSize.Small,
            _ => null,
        };
    }
}
