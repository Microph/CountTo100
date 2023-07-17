using CountTo100.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class GameplaySceneManager : MonoSingleton<GameplaySceneManager>
{
    public GameplayServerStateManager GameplayServerStateManager => _gameplayServerStateManager;
    public GameplayClientStateManager GameplayClientStateManager => _gameplayClientStateManager;

    [SerializeField] private GameplayServerStateManager _gameplayServerStateManager;
    [SerializeField] private GameplayClientStateManager _gameplayClientStateManager;
    [SerializeField] private TMP_Text _currentGameplayScoreText;

    protected override void Awake()
    {
        base.Awake();
        _currentGameplayScoreText.text = "0";
        _gameplayServerStateManager.CurrentScore.OnValueChanged += OnCurrentScoreValueChanged;
    }

    private void Start()
    {
        if (GlobalServerConfigManager.IsServer)
        {
            _gameplayServerStateManager.InitializeAndStartServer();
        }
        else
        {
            _gameplayClientStateManager.InitializeAndStartClient();
        }
    }

    private void OnCurrentScoreValueChanged(int previousValue, int newValue)
    {
        _currentGameplayScoreText.text = newValue.ToString();
    }
}
