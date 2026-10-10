using UnityEngine;
using UnityEngine.InputSystem;

namespace Lightbound
{
    public class PeaShooter : MonoBehaviour
    {
        [SerializeField] private WeaponConfig weaponConfig;
        [SerializeField] private RingConfig ringConfig;
        [SerializeField] private InputActionAsset inputActions;
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
            _ignoreMask = 1 << LayerMask.NameToLayer("Player");

            var flightMap = inputActions.FindActionMap("Flight", throwIfNotFound: true);
            _fireAction = flightMap.FindAction("Fire");
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

            PeaProjectile projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(direction));
            projectile.Damage = lowPower ? weaponConfig.lowPowerDamage : weaponConfig.normalDamage;
            projectile.Speed = weaponConfig.projectileSpeed;
            projectile.MaxRange = weaponConfig.maxRange;
            projectile.IgnoreMask = _ignoreMask;
            projectile.GrantsLowPowerCharge = lowPower;
            projectile.LowPowerChargeAmount = ringConfig.lowPowerChargePerHit;
            projectile.Shooter = _playerRingEnergy;

            _handRecoil?.PlayRecoil();
        }
    }
}
