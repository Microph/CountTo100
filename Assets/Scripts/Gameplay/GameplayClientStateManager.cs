﻿using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using CountTo100.Utilities;
using Unity.Services.Lobbies.Models;

public class GameplayClientStateManager : StateManager
{
    public class GameplayClientContext
    {
        public GameplaySceneManager GameplaySceneManager;
        public NetworkManager NetworkManager;
        public UnityTransport Transport;
        public PlayerObject PlayerObject;

        public GameplayClientContext(GameplaySceneManager gameplaySceneManager, NetworkManager networkManager, UnityTransport transport, PlayerObject playerObject)
        {
            GameplaySceneManager = gameplaySceneManager;
            NetworkManager = networkManager;
            Transport = transport;
            PlayerObject = playerObject;
        }
    }

    private NetworkManager _networkManager;
    private UnityTransport _transport;

    public async Task InitializeAndStart()
    {
        if (!GlobalClientConfigManager.IsClient)
        {
            Debug.LogWarning("Did not config as a client");
            return;
        }

        string playerName = "Player";
        string serverIP = "127.0.0.1";
        ushort serverPort = 7777;
        if (LobbyManager.Instance != null)
        {
            //in case a player leaves gameplay and go back to lobby, they should not be in ready state right away
            await LobbyManager.Instance.UpdatePlayerReadyStatus(false);
            playerName = LobbyManager.Instance.PlayerName;
            LobbyManager.Instance.JoinedLobby.Data.TryGetValue(LobbyManager.KEY_GAMEPLAY_SERVER_IP, out DataObject serverIPDataObject);
            serverIP = serverIPDataObject?.Value;
            Debug.Log(serverIPDataObject?.Value);
            LobbyManager.Instance.JoinedLobby.Data.TryGetValue(LobbyManager.KEY_GAMEPLAY_SERVER_PORT, out DataObject serverPortDataObject);
            serverPort = ushort.Parse(serverPortDataObject?.Value);
            Debug.Log(ushort.Parse(serverPortDataObject?.Value));
        }

        _networkManager = NetworkManager.Singleton;
        Debug.Assert(_networkManager != null);
        _transport = _networkManager.GetComponent<UnityTransport>();
        Debug.Assert(_transport != null);
        _networkManager.OnClientDisconnectCallback += OnClientDisconnected;
        _transport.SetConnectionData(serverIP, serverPort);
        _networkManager.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes(playerName);
        _networkManager.StartClient();
        await TaskHelper.When(() => _networkManager.IsConnectedClient && IsPlayerObjectSpawned(_networkManager.LocalClient.PlayerObject));
        Debug.Log("Client is connected and player object is spawned!");
        SetState(new GameplayClientClientStartedState(
                stateManager: this,
                gameplayClientContext: new GameplayClientContext(
                    gameplaySceneManager: GameplaySceneManager.Instance,
                    networkManager: _networkManager,
                    transport: _transport,
                    playerObject: _networkManager.LocalClient.PlayerObject.GetComponent<PlayerObject>()
                )
            )
        );
    }

    private void OnDestroy()
    {
        if (_networkManager != null) 
        { 
            _networkManager.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }

    private bool IsPlayerObjectSpawned(NetworkObject playerObject)
    {
        return playerObject != null && playerObject.IsSpawned;
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"Disconnect reason: {_networkManager.DisconnectReason}");
        SceneManager.LoadScene("MainMenu");
    }
}