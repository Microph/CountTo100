using CountTo100.Utilities;
using System;
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
        IStateManageable stateManager,
        NetworkManager networkManager, 
        UnityTransport transport
    )
        : base(
            stateEnum: Enums.State.GameplayClient_StartClient,
            stateManager: stateManager,
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
    }
}
