using System;
using System.Threading.Tasks;
using UnityEngine;

public class ClientJoinLobbySceneManager : MonoBehaviour
{
    [SerializeField] private ClientJoinLobbyUIManager _clientJoinLobbyUIManager;

    private void Start()
    {
        _clientJoinLobbyUIManager.Setup(LobbyManager.Instance);
    }
}
