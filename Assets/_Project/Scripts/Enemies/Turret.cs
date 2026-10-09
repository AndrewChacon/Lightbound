using System.Collections;
using UnityEngine;

namespace Lightbound
{
    /// <summary>
    /// Stationary destructible target. Disables its own Collider/Renderer (not the
    /// whole GameObject, which would also stop its respawn coroutine) on death,
    /// then resets and reappears after StationConfig's respawnDelay.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class Turret : MonoBehaviour, IDamageable
    {
        [SerializeField] private TurretConfig config;

        private Health _health;
        private Collider _collider;
        private Renderer _renderer;

        private void Awake()
        {
            _health = new Health(config.maxHealth);
            _health.OnDepleted += HandleDepleted;

            _collider = GetComponent<Collider>();
            _renderer = GetComponent<Renderer>();
        }

        public void TakeDamage(float amount)
        {
            _health.TakeDamage(amount);
        }

        private void HandleDepleted()
        {
            _collider.enabled = false;
            _renderer.enabled = false;
            StartCoroutine(RespawnAfterDelay());
        }

        private IEnumerator RespawnAfterDelay()
        {
            yield return new WaitForSeconds(config.respawnDelay);

            _health.Reset();
            _collider.enabled = true;
            _renderer.enabled = true;
        }
    }
}
