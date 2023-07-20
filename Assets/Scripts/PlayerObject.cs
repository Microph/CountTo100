using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

//main class for NetworkManager.LocalClient.PlayerObject
public class PlayerObject : NetworkBehaviour
{
    public NetworkVariable<ulong> NVClientId = new NetworkVariable<ulong>();
    public NetworkVariable<FixedString64Bytes> NVPlayerName = new NetworkVariable<FixedString64Bytes>();
    //TODO: color

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

    public void Setup(GameplayServerStateManager gameplayServerStateManager, ulong clientId, string playerName)
    {
        _gameplayServerStateManager = gameplayServerStateManager;
        NVClientId.Value = clientId;
        NVPlayerName.Value = playerName;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        RefreshVisual();
        NVPlayerName.OnValueChanged += OnNVPlayerNameChanged;
    }

    public override void OnDestroy()
    {
        NVPlayerName.OnValueChanged -= OnNVPlayerNameChanged;
        base.OnDestroy();
    }

    private void RefreshVisual()
    {
        _playerNameText.text = NVPlayerName.Value.ToString();
        //TODO: color
    }

    private void OnNVPlayerNameChanged(FixedString64Bytes _, FixedString64Bytes __)
    {
        RefreshVisual();
    }
}
