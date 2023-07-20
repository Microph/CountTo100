using System;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

//main class for NetworkManager.LocalClient.PlayerObject
public class PlayerObject : NetworkBehaviour
{
    public NetworkVariable<ulong> NVClientId = new NetworkVariable<ulong>();
    public NetworkVariable<FixedString64Bytes> NVPlayerName = new NetworkVariable<FixedString64Bytes>();
    public NetworkVariable<Color> NVPlayerColor = new NetworkVariable<Color>();
    
    [SerializeField] private SpriteRenderer _playerSpriteRenderer;
    [SerializeField] private GameObject _winnerSpriteGameObject;
    [SerializeField] private TMP_Text _playerNameText;
    [SerializeField] private ParticleSystem _plus1Particle;

    private GameplayServerStateManager _gameplayServerStateManager;

    [ServerRpc]
    public void PlayerReadyServerRpc()
    {
        _gameplayServerStateManager.PlayerReady(OwnerClientId);
    }

    [ServerRpc]
    public void PlayerCountServerRpc()
    {
        _gameplayServerStateManager.PlayerCount(OwnerClientId);
    }

    [ClientRpc]
    public void EmitPlus1ClientRpc()
    {
        _plus1Particle.Emit(1);
    }

    public void Setup(
        GameplayServerStateManager gameplayServerStateManager, 
        ulong clientId, 
        string playerName,
        Color playerColor
    )
    {
        _gameplayServerStateManager = gameplayServerStateManager;
        _gameplayServerStateManager.NVLatestClickerId.OnValueChanged += OnLatestClickerIdChangedClientRpc;
        NVClientId.Value = clientId;
        NVPlayerName.Value = playerName;
        NVPlayerColor.Value = playerColor;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        RefreshVisual();
        NVPlayerName.OnValueChanged += OnNVPlayerNameChanged;
        NVPlayerColor.OnValueChanged += OnNVPlayerColorChanged;
    }

    public override void OnDestroy()
    {
        NVPlayerName.OnValueChanged -= OnNVPlayerNameChanged;
        NVPlayerColor.OnValueChanged -= OnNVPlayerColorChanged;
        if( _gameplayServerStateManager != null )
        {
            _gameplayServerStateManager.NVLatestClickerId.OnValueChanged -= OnLatestClickerIdChangedClientRpc;
        }
        base.OnDestroy();
    }

    [ClientRpc]
    private void OnLatestClickerIdChangedClientRpc(ulong _, ulong clientId)
    {
        if (clientId == OwnerClientId)
        {
            _playerSpriteRenderer.transform.localScale = Vector3.one * 1.25f;
        }
        else
        {
            _playerSpriteRenderer.transform.localScale = Vector3.one;
        }
    }

    private void RefreshVisual()
    {
        _playerNameText.text = NVPlayerName.Value.ToString();
        _playerSpriteRenderer.color = NVPlayerColor.Value;
    }

    private void OnNVPlayerNameChanged(FixedString64Bytes _, FixedString64Bytes __)
    {
        RefreshVisual();
    }

    private void OnNVPlayerColorChanged(Color _, Color __)
    {
        RefreshVisual();
    }
}
