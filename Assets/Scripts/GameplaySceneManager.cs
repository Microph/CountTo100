using CountTo100.Utilities;
using System;
using UnityEngine;

public class GameplaySceneManager : MonoSingleton<GameplaySceneManager>
{
    public GameplayServerStateManager GameplayServerStateManager => _gameplayServerStateManager;
    public GameplayClientStateManager GameplayClientStateManager => _gameplayClientStateManager;
    public InputManager InputManager => _inputManager;
    public GameplayUIManager GameplayUIManager => _gameplayUIManager;

    [SerializeField] private GameplayServerStateManager _gameplayServerStateManager;
    [SerializeField] private GameplayClientStateManager _gameplayClientStateManager;
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private GameplayUIManager _gameplayUIManager;
    
    protected override void Awake()
    {
        base.Awake();
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
                _gameplayUIManager.Initialize();
                await _gameplayClientStateManager.InitializeAndStart();
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }
}
