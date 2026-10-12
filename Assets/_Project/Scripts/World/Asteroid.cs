using UnityEngine;

namespace Lightbound
{
    /// <summary>
    /// Drifts at a constant velocity (zero-g coast, no gravity, same pattern as
    /// everything else in this project). Breaks into smaller self-clones when
    /// destroyed, unless already the smallest size. Self-destroys if it drifts
    /// too far from where it spawned, so an unattended field doesn't accumulate
    /// forever. Physical collisions (with the player or anything else) are plain
    /// Rigidbody physics -- no damage system, just a real bump.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class Asteroid : MonoBehaviour, IDamageable
    {
        [SerializeField] private AsteroidConfig config;

        private Rigidbody _rigidbody;
        private Health _health;
        private AsteroidSize _size;
        private Vector3 _spawnPosition;
        private float _maxDriftDistance;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.useGravity = false;
        }

        public void Initialize(AsteroidSize size, Vector3 velocity, float maxDriftDistance)
        {
            _size = size;
            _maxDriftDistance = maxDriftDistance;
            _spawnPosition = transform.position;

            float scale = config.GetScale(size);
            transform.localScale = Vector3.one * scale;
            _rigidbody.mass = scale;
            _rigidbody.linearVelocity = velocity;

            _health = new Health(config.GetMaxHealth(size));
            _health.OnDepleted += HandleDepleted;
        }

        public void TakeDamage(float amount)
        {
            _health.TakeDamage(amount);
        }

        private void HandleDepleted()
        {
            AsteroidSize? nextSize = config.GetNextSmallerSize(_size);
            if (nextSize.HasValue)
            {
                for (int i = 0; i < config.fragmentCount; i++)
                {
                    Vector3 direction = Random.onUnitSphere;
                    float speed = Random.Range(config.fragmentSpeedMin, config.fragmentSpeedMax);

                    Asteroid fragment = Instantiate(this, transform.position, Random.rotation);
                    fragment.Initialize(nextSize.Value, direction * speed, _maxDriftDistance);
                }
            }

            Destroy(gameObject);
        }

        private void Update()
        {
            if (Vector3.Distance(transform.position, _spawnPosition) > _maxDriftDistance)
            {
                Destroy(gameObject);
            }
        }
    }
}
