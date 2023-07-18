using CountTo100.Utilities;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayClientStartClientState : State
{
    private NetworkManager _networkManager;
    private UnityTransport _transport;

    public GameplayClientStartClientState(
        NetworkManager networkManager, 
        UnityTransport transport
    )
        : base(
            stateEnum: Enums.State.GameplayClient_StartClient,
            availableStateTransitions: new StateTransition[]
            {
            }
        )
    {
        _networkManager = networkManager;
        _transport = transport;
    }

    public override void OnEnter()
    {
        _networkManager.OnClientDisconnectCallback += OnClientDisconnected;
        _transport.SetConnectionData("127.0.0.1", 7777); //TODO: not hardcoded
        _networkManager.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes("TestPlayerName"); //TODO: get from text input
        _networkManager.StartClient();
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"Disconnect reason: {_networkManager.DisconnectReason}");
        SceneManager.LoadScene("MainMenu");
    }
}
