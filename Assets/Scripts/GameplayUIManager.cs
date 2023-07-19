using CountTo100.Utilities;
using TMPro;
using UnityEngine;

public  class GameplayUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _waitingForPlayerText;
    [SerializeField] private TMP_Text _countDownStartGameplayText;
    [SerializeField] private TMP_Text _currentGameplayScoreText;
    [SerializeField] private TMP_Text _winnerText;

    private GameplayClientStateManager _gameplayClientStateManager;

    //reflect changes on client-side only
    public void Initialize(GameplayClientStateManager gameplayClientStateManager)
    {
        _gameplayClientStateManager = gameplayClientStateManager;
        _gameplayClientStateManager.OnGameplayClientStateChanged += OnGameplayClientStateChanged;
        //TODO: _gameplayClientStateManager.CurrentScore.OnValueChanged += OnCurrentScoreValueChanged;
    }

    private void OnGameplayClientStateChanged(Enums.State previousState, Enums.State newState)
    {
        switch (newState)
        {
            case Enums.State.GameplayClient_BeginGameplayCountDown:
                HideAll();
                _countDownStartGameplayText.gameObject.SetActive(true);
                //TODO show countdown UI
                break;
            case Enums.State.GameplayClient_AllowCounting:
                //if client still counting down -> 
                HideAll();
                //show start gameplay UI
                //change clientstate -> allow gameplay input
                break;
        }
    }

    private void OnCurrentScoreValueChanged(int previousValue, int newValue)
    {
        _currentGameplayScoreText.text = newValue.ToString();
    }

    private void HideAll()
    {
        _waitingForPlayerText.gameObject.SetActive(false);
        _countDownStartGameplayText.gameObject.SetActive(false);
        _currentGameplayScoreText.gameObject.SetActive(false);
        _winnerText.gameObject.SetActive(false);
    }
}