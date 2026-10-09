using UnityEngine;

namespace Lightbound
{
    [CreateAssetMenu(fileName = "WeaponConfig", menuName = "Lightbound/Weapon Config")]
    public class WeaponConfig : ScriptableObject
    {
        [Header("Fire rate")]
        public float cooldown = 0.12f;

        [Header("Projectile")]
        public float projectileSpeed = 50f;
        public float maxRange = 60f;

        [Header("Damage")]
        public float normalDamage = 10f;
        public float lowPowerDamage = 3f;
    }
}
