using TMPro;
using UnityEngine;

public  class GameplayUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _currentGameplayScoreText;

    private GameplayServerStateManager _gameplayServerStateManager;

    public void Initialize(GameplayServerStateManager gameplayServerStateManager)
    {
        _gameplayServerStateManager = gameplayServerStateManager;
        //TODO: listen to server state change event and set active UI accordingly
        _gameplayServerStateManager.NVCurrentScore.OnValueChanged += OnCurrentScoreValueChanged;
    }

    private void OnCurrentScoreValueChanged(int previousValue, int newValue)
    {
        _currentGameplayScoreText.text = newValue.ToString();
    }
}