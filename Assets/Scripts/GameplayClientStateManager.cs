using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayClientStateManager : NetworkBehaviour
{
    private NetworkManager _networkManager;
    private UnityTransport _transport;

    public void InitializeAndStartClient()
    {
        if(!GlobalClientConfigManager.IsClient)
        {
            Debug.LogWarning("Did not config as a client");
            return;
        }

        _networkManager = NetworkManager.Singleton;
        Debug.Assert(_networkManager != null);
        _transport = _networkManager.GetComponent<UnityTransport>();
        Debug.Assert(_transport != null);
        _networkManager.OnClientDisconnectCallback += OnClientDisconnected;
        _transport.SetConnectionData("127.0.0.1", 7777); //TODO: not hardcoded
        _networkManager.StartClient();
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"Disconnect reason: {_networkManager.DisconnectReason}");
        SceneManager.LoadScene("MainMenu");
    }
}