using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class GameplaySceneManager : MonoBehaviour
{
    [SerializeField] private GameplayServerStateManager _gameplayServerStateManager;
    [SerializeField] private GameplayClientStateManager _gameplayClientStateManager;

    private void Start()
    {
        if(GlobalServerConfigManager.IsServer)
        {
            _gameplayServerStateManager.InitializeAndStartServer();
        }
        else
        {
            _gameplayClientStateManager.InitializeAndStartClient();
        }
    }
}
