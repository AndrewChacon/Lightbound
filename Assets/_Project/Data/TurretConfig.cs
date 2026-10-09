using UnityEngine;

namespace Lightbound
{
    [CreateAssetMenu(fileName = "TurretConfig", menuName = "Lightbound/Turret Config")]
    public class TurretConfig : ScriptableObject
    {
        public float maxHealth = 100f;
        public float respawnDelay = 8f;
    }
}
