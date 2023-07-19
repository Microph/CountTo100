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
    private const float k_defaultCountDownTime = 3;

    private GameplayClientContext _gameplayClientContext;
    private float _currentCountDownTime = 0;

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
        _currentCountDownTime = k_defaultCountDownTime;
        _gameplayClientContext.GameplaySceneManager.GameplayUIManager.UpdateCountDownStartGameplayNumber(Mathf.CeilToInt(_currentCountDownTime));
        _gameplayClientContext.GameplaySceneManager.GameplayUIManager.ShowCountDownStartGameplayText();
    }

    public override void OnUpdate()
    {
        _currentCountDownTime -= Time.deltaTime;
        _gameplayClientContext.GameplaySceneManager.GameplayUIManager.UpdateCountDownStartGameplayNumber(Mathf.CeilToInt(_currentCountDownTime));
        if (_currentCountDownTime <= 0)
        {
            _currentCountDownTime = 0;
            _gameplayClientContext.GameplaySceneManager.GameplayUIManager.UpdateCountDownStartGameplayNumber(Mathf.CeilToInt(_currentCountDownTime));
            //TODO: _stateManager.TransitTo(new GameplayClientAllowCountingState(_stateManager, _gameplayClientContext));
        }
    }
}
