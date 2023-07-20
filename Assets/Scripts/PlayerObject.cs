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

    public void Setup(
        GameplayServerStateManager gameplayServerStateManager, 
        ulong clientId, 
        string playerName,
        Color playerColor
    )
    {
        _gameplayServerStateManager = gameplayServerStateManager;
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
