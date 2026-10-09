using UnityEngine;
using UnityEngine.InputSystem;

namespace Lightbound
{
    // DEBUG ONLY. Lets you drain the ring and test Low Power Mode / the Lantern
    // Battery station before the pea shooter exists. Delete this component (and
    // this file) once step 3's pea shooter drains energy for real.
    public class DebugRingDrain : MonoBehaviour
    {
        [SerializeField] private PlayerRingEnergy playerRingEnergy;
        [SerializeField] private float drainPerSecond = 20f;

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.kKey.isPressed)
            {
                playerRingEnergy.Ring.Drain(drainPerSecond * Time.deltaTime);
            }
        }
    }
}
