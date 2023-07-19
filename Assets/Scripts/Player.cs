using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private SpriteRenderer _playerSpriteRenderer;
    [SerializeField] private GameObject _winnerSpriteGameObject;
    [SerializeField] private TMP_Text _playerNameText;

    private ulong _clientId;

    public void Setup(ulong clientId, string playerName)
    {
        _clientId = clientId;
        _playerNameText.text = playerName;
        //TODO set color
    }

    //TODO: later joined client failed to sync this -> will use netvar instead
    [ClientRpc]
    public void SetupClientRpc(ulong clientId, string playerName)
    {
        Setup(clientId, playerName);
    }
}
