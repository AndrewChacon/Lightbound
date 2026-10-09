using UnityEngine;

namespace Lightbound
{
    [CreateAssetMenu(fileName = "FlightConfig", menuName = "Lightbound/Flight Config")]
    public class FlightConfig : ScriptableObject
    {
        [Header("Movement")]
        public float thrustForce = 20f;
        public float linearDamping = 0.2f;
        public float angularDamping = 1f;

        [Header("Stopping (active brake once Move input releases)")]
        [Range(0f, 1f)] public float stopDragFactor = 0.1f;

        [Header("Look")]
        public float mouseSensitivity = 2f;
        public float minPitch = -89f;
        public float maxPitch = 89f;

        [Header("Launch impulse (double-tap Shift; also used for takeoff)")]
        public float launchImpulse = 8f;

        [Header("Walking (grounded)")]
        public float walkSpeed = 5f;
    }
}
