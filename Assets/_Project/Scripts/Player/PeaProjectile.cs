using Unity.Netcode;
using UnityEngine;

namespace Lightbound
{
    /// <summary>
    /// Moves via a swept raycast each frame instead of a Rigidbody — the standard
    /// approach for small fast projectiles, since it can't tunnel through a thin
    /// collider and avoids giving every shot its own physics body. Configured by
    /// PeaShooter immediately after being spawned, via Initialize, before the
    /// first Update runs.
    /// Owner-authoritative, matching the Player prefab's own NetworkTransform: only
    /// the shooting client actually simulates movement and hit detection, so their
    /// own local Turret/Asteroid interaction feels exactly as it did before
    /// networking. NetworkTransform just replicates that client's resulting
    /// position out to everyone else so the shot is visible on their screens too.
    /// </summary>
    public class PeaProjectile : NetworkBehaviour
    {
        private float _damage;
        private float _speed;
        private float _maxRange;
        private bool _grantsLowPowerCharge;
        private float _lowPowerChargeAmount;
        private PlayerRingEnergy _shooter;
        private LayerMask _ignoreMask;
        private float _distanceTraveled;

        public void Initialize(float damage, float speed, float maxRange, LayerMask ignoreMask,
            bool grantsLowPowerCharge, float lowPowerChargeAmount, PlayerRingEnergy shooter)
        {
            _damage = damage;
            _speed = speed;
            _maxRange = maxRange;
            _ignoreMask = ignoreMask;
            _grantsLowPowerCharge = grantsLowPowerCharge;
            _lowPowerChargeAmount = lowPowerChargeAmount;
            _shooter = shooter;
        }

        private void Update()
        {
            if (!IsOwner) return;

            float step = _speed * Time.deltaTime;
            Vector3 origin = transform.position;
            int layerMask = ~_ignoreMask;

            if (Physics.Raycast(origin, transform.forward, out var hit, step, layerMask, QueryTriggerInteraction.Ignore))
            {
                var damageable = hit.collider.GetComponentInParent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(_damage);
                    if (_grantsLowPowerCharge && _shooter != null)
                    {
                        _shooter.Ring.Recharge(_lowPowerChargeAmount);
                    }
                }

                DespawnServerRpc();
                return;
            }

            transform.position = origin + transform.forward * step;
            _distanceTraveled += step;

            if (_distanceTraveled >= _maxRange)
            {
                DespawnServerRpc();
            }
        }

        [ServerRpc]
        private void DespawnServerRpc()
        {
            // Only the server may despawn a NetworkObject, even when this client
            // owns it for NetworkTransform purposes -- ownership covers sync
            // authority, not lifecycle.
            if (NetworkObject.IsSpawned)
            {
                NetworkObject.Despawn();
            }
        }
    }
}
