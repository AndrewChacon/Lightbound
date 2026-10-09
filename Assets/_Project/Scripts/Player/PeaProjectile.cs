using UnityEngine;

namespace Lightbound
{
    /// <summary>
    /// Moves via a swept raycast each frame instead of a Rigidbody — the standard
    /// approach for small fast projectiles, since it can't tunnel through a thin
    /// collider and avoids giving every shot its own physics body. Configured by
    /// PeaShooter immediately after being spawned, before the first Update runs.
    /// </summary>
    public class PeaProjectile : MonoBehaviour
    {
        public float Damage { get; set; }
        public float Speed { get; set; }
        public float MaxRange { get; set; }
        public bool GrantsLowPowerCharge { get; set; }
        public float LowPowerChargeAmount { get; set; }
        public PlayerRingEnergy Shooter { get; set; }
        public LayerMask IgnoreMask { get; set; }

        private float _distanceTraveled;

        private void Update()
        {
            float step = Speed * Time.deltaTime;
            Vector3 origin = transform.position;
            int layerMask = ~IgnoreMask;

            if (Physics.Raycast(origin, transform.forward, out var hit, step, layerMask, QueryTriggerInteraction.Ignore))
            {
                var damageable = hit.collider.GetComponentInParent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(Damage);
                    if (GrantsLowPowerCharge && Shooter != null)
                    {
                        Shooter.Ring.Recharge(LowPowerChargeAmount);
                    }
                }

                Destroy(gameObject);
                return;
            }

            transform.position = origin + transform.forward * step;
            _distanceTraveled += step;

            if (_distanceTraveled >= MaxRange)
            {
                Destroy(gameObject);
            }
        }
    }
}
