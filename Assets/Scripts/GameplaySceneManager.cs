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
    public InputManager InputManager => _inputManager;

    [SerializeField] private GameplayServerStateManager _gameplayServerStateManager;
    [SerializeField] private GameplayClientStateManager _gameplayClientStateManager;
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private GameplayUIManager _gameplayUIManager;
    
    protected override void Awake()
    {
        base.Awake();
        _gameplayUIManager.Initialize(_gameplayServerStateManager);
    }

    private async void Start()
    {
        try
        {
            if (GlobalServerConfigManager.IsServer)
            {
                await _gameplayServerStateManager.InitializeAndStart();
            }
            else
            {
                await _gameplayClientStateManager.InitializeAndStart();
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }
}
