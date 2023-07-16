using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class GameplaySceneManager : MonoBehaviour
{
    private NetworkManager _networkManager;
    private UnityTransport _transport;

    private void Awake()
    {
        _networkManager = NetworkManager.Singleton;
        Debug.Assert(_networkManager != null);
        _transport = _networkManager.GetComponent<UnityTransport>();
        Debug.Assert(_transport != null);
    }

    private void Start()
    {
        if(GlobalConfigManager.IsServer)
        {
            SetupConnectionData();
            _networkManager.StartServer();
        }
        else
        {
            SetupConnectionData();
            _networkManager.StartClient();
        }
    }

    private void SetupConnectionData()
    {
        _transport.SetConnectionData("127.0.0.1", 7777); //TODO: not hardcoded
    }
}
