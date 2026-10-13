using System;
using Unity.Netcode;
using UnityEngine;

namespace Lightbound
{
    /// <summary>
    /// Disables local input/control components on Player instances that aren't
    /// owned by this client. A remote player's object is driven entirely by
    /// NetworkTransform syncing from their owning client -- nothing here should
    /// locally re-simulate input that was never actually pressed on this machine.
    /// Also announces the local player's PlayerRingEnergy once it spawns, since
    /// Player is no longer a fixed scene object that UI can reference directly.
    /// </summary>
    public class NetworkPlayerSetup : NetworkBehaviour
    {
        public static event Action<PlayerRingEnergy> LocalPlayerSpawned;

        [SerializeField] private Behaviour[] ownerOnlyComponents;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                foreach (Behaviour component in ownerOnlyComponents)
                {
                    if (component != null)
                    {
                        component.enabled = false;
                    }
                }
                return;
            }

            LocalPlayerSpawned?.Invoke(GetComponent<PlayerRingEnergy>());
        }
    }
}
