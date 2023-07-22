using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUIManager : MonoBehaviour
{
    [SerializeField] private GameObject _startingGameplayUIOverlay;
    [SerializeField] private Button _exitButton;
    [SerializeField] private TMP_Text _waitingForPlayerText;
    [SerializeField] private TMP_Text _countDownStartGameplayText;
    [SerializeField] private TMP_Text _currentGameplayScoreText;
    [SerializeField] private TMP_Text _winnerText;

    private Action _onExitButtonClickedAction;
    
    private void Awake()
    {
        _exitButton.onClick.AddListener(OnExitButtonClicked);
    }

    public void Initialize()
    {
        _onExitButtonClickedAction = null;
        HideAll();
        _startingGameplayUIOverlay.gameObject.SetActive(true);
    }

    public void HideAll()
    {
        _startingGameplayUIOverlay.gameObject.SetActive(false);
        _exitButton.gameObject.SetActive(false);
        _waitingForPlayerText.gameObject.SetActive(false);
        _countDownStartGameplayText.gameObject.SetActive(false);
        _currentGameplayScoreText.gameObject.SetActive(false);
        _winnerText.gameObject.SetActive(false);
    }

    public void ShowWaitingForPlayerText()
    {
        HideAll();
        _waitingForPlayerText.gameObject.SetActive(true);
    }

    public void ShowCountDownStartGameplayText()
    {
        HideAll();
        _countDownStartGameplayText.gameObject.SetActive(true);
    }

    public void UpdateCountDownStartGameplayNumber(int currentCountDownTime)
    {
        _countDownStartGameplayText.text = $"Game starts in\n{currentCountDownTime}";
    }

    public void ShowCurrentGameplayScoreText()
    {
        HideAll();
        _currentGameplayScoreText.gameObject.SetActive(true) ;
    }

    public void UpdateGameplayScoreText(int newValue)
    {
        _currentGameplayScoreText.text = newValue.ToString();
    }

    public void ShowWinnerTextAndExitButton()
    {
        HideAll();
        _winnerText.gameObject.SetActive(true);
        _exitButton.gameObject.SetActive(true);
    }

    public void SetWinnerTextAndExitButtonAction(string playerName, ulong clientId, Action onExitButtonClickedAction)
    {
        _winnerText.text = $"{playerName} (ID: {clientId}) counted to\n100";
        _onExitButtonClickedAction = onExitButtonClickedAction;
    }

    private void OnExitButtonClicked()
    {
        _onExitButtonClickedAction?.Invoke();
    }
}