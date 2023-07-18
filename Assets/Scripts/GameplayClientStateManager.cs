using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameplayClientStateManager : NetworkStateManager
{
    private NetworkManager _networkManager;
    private UnityTransport _transport;

    public void InitializeAndStart()
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
        InitializeGameplayInputEvents();
        _transport.SetConnectionData("127.0.0.1", 7777); //TODO: not hardcoded
        _networkManager.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes("TestPlayerName"); //TODO: get from text input
        _networkManager.StartClient();
    }

    private void InitializeGameplayInputEvents()
    {
        GameplaySceneManager.Instance.InputManager.MainInputAction = MainInputAction;
    }

    private void MainInputAction()
    {
        GameplaySceneManager.Instance.GameplayServerStateManager.TestAddCurrentScoreServerRpc();
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"Disconnect reason: {_networkManager.DisconnectReason}");
        SceneManager.LoadScene("MainMenu");
    }
}