using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Lightbound
{
    public class PeaShooter : NetworkBehaviour
    {
        [SerializeField] private WeaponConfig weaponConfig;
        [SerializeField] private RingConfig ringConfig;
        [SerializeField] private PeaProjectile projectilePrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private Transform aimOrigin;

        private PlayerRingEnergy _playerRingEnergy;
        private HandRecoil _handRecoil;
        private InputAction _fireAction;
        private LayerMask _ignoreMask;
        private float _nextFireTime;

        private void Awake()
        {
            _playerRingEnergy = GetComponent<PlayerRingEnergy>();
            _handRecoil = firePoint != null ? firePoint.GetComponent<HandRecoil>() : null;
            _ignoreMask = 1 << LayerMask.NameToLayer(GameLayers.Player);
            _fireAction = GetComponent<PlayerInputActions>().Actions.FlightAction("Fire");
        }

        private void OnEnable()
        {
            _fireAction.Enable();
            _fireAction.performed += OnFirePerformed;
        }

        private void OnDisable()
        {
            _fireAction.performed -= OnFirePerformed;
            _fireAction.Disable();
        }

        private void OnFirePerformed(InputAction.CallbackContext context)
        {
            if (Time.time < _nextFireTime) return;
            _nextFireTime = Time.time + weaponConfig.cooldown;

            Fire();
        }

        private void Fire()
        {
            int layerMask = ~_ignoreMask;
            Vector3 aimPoint = Physics.Raycast(aimOrigin.position, aimOrigin.forward, out var hit, weaponConfig.maxRange, layerMask, QueryTriggerInteraction.Ignore)
                ? hit.point
                : aimOrigin.position + aimOrigin.forward * weaponConfig.maxRange;

            Vector3 direction = (aimPoint - firePoint.position).normalized;

            RingEnergy ring = _playerRingEnergy.Ring;
            bool lowPower = ring.IsLowPower;

            if (!lowPower)
            {
                ring.Drain(ringConfig.peaShotEnergyCost);
            }

            // Spawning a NetworkObject requires server authority, so a client that
            // isn't also the host asks the server to do it -- the server then hands
            // ownership straight back to the shooter, matching Player's own
            // Owner-Authoritative NetworkTransform so the shooter's local hit
            // detection isn't delayed by a round trip.
            FireServerRpc(
                firePoint.position,
                Quaternion.LookRotation(direction),
                lowPower ? weaponConfig.lowPowerDamage : weaponConfig.normalDamage,
                weaponConfig.projectileSpeed,
                weaponConfig.maxRange,
                _ignoreMask.value,
                lowPower,
                ringConfig.lowPowerChargePerHit);

            _handRecoil?.PlayRecoil();
        }

        [ServerRpc]
        private void FireServerRpc(Vector3 position, Quaternion rotation, float damage, float speed,
            float maxRange, int ignoreMask, bool grantsLowPowerCharge, float lowPowerChargeAmount,
            ServerRpcParams rpcParams = default)
        {
            ulong shooterClientId = rpcParams.Receive.SenderClientId;
            PlayerRingEnergy shooter = NetworkManager.ConnectedClients[shooterClientId].PlayerObject.GetComponent<PlayerRingEnergy>();

            PeaProjectile projectile = Instantiate(projectilePrefab, position, rotation);
            projectile.Initialize(damage, speed, maxRange, ignoreMask, grantsLowPowerCharge, lowPowerChargeAmount, shooter);
            projectile.NetworkObject.SpawnWithOwnership(shooterClientId);
        }
    }
}
