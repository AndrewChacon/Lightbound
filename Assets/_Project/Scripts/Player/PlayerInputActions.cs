using UnityEngine;
using UnityEngine.InputSystem;

namespace Lightbound
{
    /// <summary>
    /// Gives each spawned Player instance its own independent runtime copy of the
    /// shared project InputActionAsset. Without this, every Player prefab instance
    /// reads/enables/disables the exact same underlying InputAction objects --
    /// harmless with a single player, but with two Player instances on screen
    /// (local + networked-remote), NetworkPlayerSetup disabling the remote
    /// instance's input scripts also disables those *shared* actions for the
    /// local player, since it's the same object underneath both.
    /// DefaultExecutionOrder guarantees this clone exists before any other script
    /// on this prefab reads it, regardless of Unity's otherwise unspecified Awake
    /// order between sibling components.
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public class PlayerInputActions : MonoBehaviour
    {
        [SerializeField] private InputActionAsset sourceActions;

        public InputActionAsset Actions { get; private set; }

        private void Awake()
        {
            Actions = sourceActions.Clone();
        }

        private void OnDestroy()
        {
            Actions.Disable();
        }
    }
}
