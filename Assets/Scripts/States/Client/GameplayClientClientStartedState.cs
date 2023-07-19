using CountTo100.Utilities;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameplayClientStateManager;

public class GameplayClientClientStartedState : State
{
    GameplayClientContext _gameplayClientContext;

    public GameplayClientClientStartedState(
        IStateManageable stateManager,
        GameplayClientContext gameplayClientContext
    )
        : base(
            stateEnum: Enums.State.GameplayClient_ClientStarted,
            stateManager: stateManager,
            availableStateTransitions: new StateTransition[]
            {
            }
        )
    {
        _gameplayClientContext = gameplayClientContext;
    }

    public override void OnEnter()
    {
        _gameplayClientContext.GameplaySceneManager.GameplayServerStateManager.NVCurrentStateEnum.OnValueChanged += OnGameplayServerStateChanged;
        //TODO: Server RPC ready signal
    }

    public override void OnExit()
    {
        _gameplayClientContext.GameplaySceneManager.GameplayServerStateManager.NVCurrentStateEnum.OnValueChanged -= OnGameplayServerStateChanged;
    }

    private void OnGameplayServerStateChanged(Enums.State previousValue, Enums.State newValue)
    {
        if(newValue == Enums.State.GameplayServer_BeginGameplayCountDown)
        {
            //TODO _stateManager.TransitTo(GameplayServer_BeginGameplayCountDown)
        }
    }
}
