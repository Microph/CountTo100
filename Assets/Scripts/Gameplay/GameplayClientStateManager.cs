using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using UnityEngine;
using System.Threading.Tasks;
using CountTo100.Utilities;
using Unity.Services.Lobbies.Models;
using System;

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

    private const string k_defaultClientPlayerName = "Player";
    private const string k_defaultServerIP = "127.0.0.1";
    private const ushort k_defaultServerPort = 7777;

    private NetworkManager _networkManager;
    private UnityTransport _transport;

    public async Task InitializeAndStart()
    {
        if (!GlobalClientConfig.IsClient)
        {
            Debug.LogWarning("Did not config as a client");
            return;
        }

        string playerName = k_defaultClientPlayerName;
        string serverIP = k_defaultServerIP;
        ushort serverPort = k_defaultServerPort;
        if (LobbyManager.Instance != null)
        {
            //in case a player leaves gameplay and go back to lobby, they should not be in ready state right away
            await LobbyManager.Instance.UpdatePlayerReadyStatus(false);
            playerName = LobbyManager.Instance.PlayerName;
            LobbyManager.Instance.JoinedLobby.Data.TryGetValue(LobbyManager.KEY_GAMEPLAY_SERVER_IP, out DataObject serverIPDataObject);
            serverIP = serverIPDataObject?.Value;
            LobbyManager.Instance.JoinedLobby.Data.TryGetValue(LobbyManager.KEY_GAMEPLAY_SERVER_PORT, out DataObject serverPortDataObject);
            serverPort = ushort.Parse(serverPortDataObject?.Value);
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

    private async void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"Disconnect reason: {_networkManager.DisconnectReason}");
        if(LobbyManager.Instance != null )
        {
            try
            {
                //LobbyManager's OnApplicationQuit is not called sometimes, so I call here again
                await LobbyManager.Instance.LeaveCurrentLobby();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}