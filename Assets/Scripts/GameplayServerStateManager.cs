using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using UnityEngine;

public class GameplayServerStateManager : MonoBehaviour
{
    private NetworkManager _networkManager;
    private UnityTransport _transport;

    public void InitializeAndStartServer()
    {
        _networkManager = NetworkManager.Singleton;
        Debug.Assert(_networkManager != null);
        _transport = _networkManager.GetComponent<UnityTransport>();
        Debug.Assert(_transport != null);
        if (GlobalServerConfigManager.IsServer)
        {
            _transport.SetConnectionData("127.0.0.1", 7777); //TODO: not hardcoded
            _networkManager.StartServer();
        }
    }
}
