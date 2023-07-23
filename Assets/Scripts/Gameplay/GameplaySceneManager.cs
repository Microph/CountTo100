using CountTo100.Utilities;
using System;
using UnityEngine;

public class GameplaySceneManager : MonoBehaviour
{
    public GameplayServerStateManager GameplayServerStateManager => _gameplayServerStateManager;
    public GameplayClientStateManager GameplayClientStateManager => _gameplayClientStateManager;
    public InputManager InputManager => _inputManager;
    public GameplayUIManager GameplayUIManager => _gameplayUIManager;

    [SerializeField] private GameplayServerStateManager _gameplayServerStateManager;
    [SerializeField] private GameplayClientStateManager _gameplayClientStateManager;
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private GameplayUIManager _gameplayUIManager;
    
    private async void Start()
    {
        try
        {
            if (GlobalServerConfig.IsServer)
            {
                await _gameplayServerStateManager.InitializeAndStart(this);
            }
            else
            {
                _gameplayUIManager.Initialize();
                await _gameplayClientStateManager.InitializeAndStart(this);
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }
}
