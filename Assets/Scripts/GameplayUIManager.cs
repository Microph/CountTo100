using CountTo100.Utilities;
using System;
using TMPro;
using UnityEngine;

public  class GameplayUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _waitingForPlayerText;
    [SerializeField] private TMP_Text _countDownStartGameplayText;
    [SerializeField] private TMP_Text _currentGameplayScoreText;
    [SerializeField] private TMP_Text _winnerText;

    private GameplayServerStateManager _gameplayServerStateManager;

    public void Initialize(GameplayServerStateManager gameplayServerStateManager)
    {
        _gameplayServerStateManager = gameplayServerStateManager;
        _gameplayServerStateManager.NVCurrentStateEnum.OnValueChanged += OnGameplayServerStateChanged;
        _gameplayServerStateManager.NVCurrentScore.OnValueChanged += OnCurrentScoreValueChanged;
    }

    private void OnGameplayServerStateChanged(Enums.State previousState, Enums.State newState)
    {
        switch (newState)
        {
            case Enums.State.GameplayServer_BeginGameplayCountDown:
                HideAll();
                _countDownStartGameplayText.gameObject.SetActive(true);
                //TODO countdown number update in real time
                break;
            case Enums.State.GameplayServer_AllowCounting:
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