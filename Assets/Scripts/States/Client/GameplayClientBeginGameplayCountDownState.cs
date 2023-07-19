using CountTo100.Utilities;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameplayClientStateManager;

public class GameplayClientBeginGameplayCountDownState : State
{
    GameplayClientContext _gameplayClientContext;

    public GameplayClientBeginGameplayCountDownState(
        IStateManageable stateManager,
        GameplayClientContext gameplayClientContext
    )
        : base(
            stateEnum: Enums.State.GameplayClient_BeginGameplayCountDown,
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
        //TODO start countdown and show UI
    }
}
