using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;
using System.Collections.Generic;

public class LobbyManager : MonoBehaviour
{
    public const string KEY_PLAYER_NAME = "PlayerName";

    public class LobbyEventArgs : EventArgs
    {
        public Lobby lobby;
    }
    public event EventHandler<LobbyEventArgs> OnJoinedLobby;

    private string _playerName;
    private Lobby _joinedLobby;

    public async Task AuthenticateAndQuickJoinLobby(string playerName)
    {
        _playerName = playerName;
        InitializationOptions initializationOptions = new InitializationOptions();
        initializationOptions.SetProfile(playerName);
        await UnityServices.InitializeAsync(initializationOptions);
        AuthenticationService.Instance.SignedIn += OnSignedIn;
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private async void OnSignedIn()
    {
        AuthenticationService.Instance.SignedIn -= OnSignedIn;
        bool getNoOpenedLobbyError = false;
        try
        {
            await QuickJoinLobby();
        }
        catch (LobbyServiceException ex)
        {
            getNoOpenedLobbyError = ex.ErrorCode == 16006;
            Debug.LogException(ex);
        }

        if(getNoOpenedLobbyError)
        {
            try
            {
                await CreateAndJoinLobby(3);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }

    private async Task QuickJoinLobby()
    {
        QuickJoinLobbyOptions options = new QuickJoinLobbyOptions();
        Lobby lobby = await LobbyService.Instance.QuickJoinLobbyAsync(options);
        _joinedLobby = lobby;
        OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
        Debug.Log("Quick joined a lobby");
    }

    private async Task CreateAndJoinLobby(int maxPlayers)
    {
        Player player = new Player(AuthenticationService.Instance.PlayerId, null, new Dictionary<string, PlayerDataObject> {
            { KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, _playerName) },
        });

        CreateLobbyOptions options = new CreateLobbyOptions
        {
            Player = player,
            IsPrivate = false
        };

        Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(_playerName, maxPlayers, options);
        _joinedLobby = lobby;
        OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
        Debug.Log("Created and joined a lobby");
    }
}