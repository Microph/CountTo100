using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using CountTo100.Utilities;

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

        _networkManager = NetworkManager.Singleton;
        Debug.Assert(_networkManager != null);
        _transport = _networkManager.GetComponent<UnityTransport>();
        Debug.Assert(_transport != null);
        _networkManager.OnClientDisconnectCallback += OnClientDisconnected;
        _transport.SetConnectionData("127.0.0.1", 7777); //TODO: not hardcoded
        _networkManager.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes("TestPlayerName"); //TODO: get from text input
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