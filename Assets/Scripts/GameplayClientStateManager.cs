using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using UnityEngine;

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
        SetState(new GameplayClientStartClientState(
                networkManager: _networkManager,
                transport: _transport
            )
        );
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