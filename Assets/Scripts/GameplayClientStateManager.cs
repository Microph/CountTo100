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
        public NetworkManager NetworkManager;
        public UnityTransport Transport;

        public GameplayClientContext(NetworkManager networkManager, UnityTransport transport)
        {
            NetworkManager = networkManager;
            Transport = transport;
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
        await TaskHelper.When(() => _networkManager.IsConnectedClient);
        Debug.Log("Client is connected!");
        SetState(new GameplayClientClientStartedState(
                stateManager: this,
                gameplayClientContext: new GameplayClientContext(
                    networkManager: _networkManager,
                    transport: _transport
                )
            )
        );
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"Disconnect reason: {_networkManager.DisconnectReason}");
        SceneManager.LoadScene("MainMenu");
    }

    //TODO Set after countdown
    private void SetMainGameplayInputEvents()
    {
        GameplaySceneManager.Instance.InputManager.MainInputAction = MainInputAction;
    }

    private void MainInputAction()
    {
        GameplaySceneManager.Instance.GameplayServerStateManager.TestAddCurrentScoreServerRpc();
    }
}