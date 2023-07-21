using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class LobbyManager : MonoBehaviour 
{
    public async Task Authenticate(string playerName)
    {
        InitializationOptions initializationOptions = new InitializationOptions();
        initializationOptions.SetProfile(playerName);
        await UnityServices.InitializeAsync(initializationOptions);
        AuthenticationService.Instance.SignedIn += OnSignedIn;
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void OnSignedIn()
    {
        throw new NotImplementedException();
        //TODO: quick join lobby
    }
}