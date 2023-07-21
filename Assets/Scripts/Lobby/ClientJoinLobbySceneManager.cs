using System;
using System.Threading.Tasks;
using UnityEngine;

public class ClientJoinLobbySceneManager : MonoBehaviour
{
    [SerializeField] private LobbyManager _lobbyManager;
    [SerializeField] private ClientJoinLobbyUIManager _clientJoinLobbyUIManager;

    private void Awake()
    {
        _clientJoinLobbyUIManager.Setup(_lobbyManager);
    }
}
