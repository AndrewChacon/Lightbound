using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Lightbound
{
    // DEBUG/TEMPORARY. No menus yet per CLAUDE.md, so this is the minimum way to
    // start a session for testing: H = Host, J = Join (connects to
    // targetAddress:port). Replace with a real SessionManager once
    // lobbies/menus are in scope.
    public class NetworkBootstrap : MonoBehaviour
    {
        [SerializeField] private string targetAddress = "127.0.0.1";
        [SerializeField] private ushort port = 7777;

        private void Update()
        {
            if (Keyboard.current == null) return;

            if (Keyboard.current.hKey.wasPressedThisFrame)
            {
                NetworkManager.Singleton.StartHost();
            }
            else if (Keyboard.current.jKey.wasPressedThisFrame)
            {
                var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                transport.ConnectionData.Address = targetAddress;
                transport.ConnectionData.Port = port;
                NetworkManager.Singleton.StartClient();
            }
        }
    }
}
