using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
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
        NVPlayerName.OnValueChanged += OnNVPlayerNameChanged;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        NVPlayerName.OnValueChanged -= OnNVPlayerNameChanged;
    }

    private void OnNVPlayerNameChanged(FixedString64Bytes previousValue, FixedString64Bytes newValue)
    {
        RefreshVisual();
    }

    private void RefreshVisual()
    {
        _playerNameText.text = NVPlayerName.Value.ToString();
    }
}
