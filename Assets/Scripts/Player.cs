using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private SpriteRenderer _playerSpriteRenderer;
    [SerializeField] private GameObject _winnerSpriteGameObject;
    [SerializeField] private TMP_Text _playerNameText;
}
