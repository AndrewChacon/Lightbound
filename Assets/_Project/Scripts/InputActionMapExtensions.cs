using UnityEngine.InputSystem;

namespace Lightbound
{
    public static class InputActionMapExtensions
    {
        /// <summary>
        /// Looks up an action in the project's "Flight" map, the one action map
        /// every player-input script in this project reads from.
        /// </summary>
        public static InputAction FlightAction(this InputActionAsset asset, string actionName)
        {
            return asset.FindActionMap("Flight", throwIfNotFound: true).FindAction(actionName);
        }
    }
}
