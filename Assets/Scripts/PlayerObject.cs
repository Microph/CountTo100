using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerObject : NetworkBehaviour
{
    public NetworkVariable<ulong> NVClientId = new NetworkVariable<ulong>();
    public NetworkVariable<FixedString64Bytes> NVPlayerName = new NetworkVariable<FixedString64Bytes>();

    [SerializeField] private SpriteRenderer _playerSpriteRenderer;
    [SerializeField] private GameObject _winnerSpriteGameObject;
    [SerializeField] private TMP_Text _playerNameText;

    [ServerRpc]
    public void SetupServerRpc(ulong clientId, string playerName)
    {
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
    }

    private void OnNVPlayerNameChanged(FixedString64Bytes _, FixedString64Bytes __)
    {
        RefreshVisual();
    }
}
