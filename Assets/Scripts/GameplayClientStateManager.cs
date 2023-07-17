using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using UnityEngine;

public class GameplayClientStateManager : MonoBehaviour
{
    private NetworkManager _networkManager;
    private UnityTransport _transport;

    public void InitializeAndStartClient()
    {
        _networkManager = NetworkManager.Singleton;
        Debug.Assert(_networkManager != null);
        _transport = _networkManager.GetComponent<UnityTransport>();
        Debug.Assert(_transport != null);
        if (GlobalClientConfigManager.IsClient)
        {
            _transport.SetConnectionData("127.0.0.1", 7777); //TODO: not hardcoded
            _networkManager.StartClient();
        }
    }
}