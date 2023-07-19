using System;
using TMPro;
using UnityEngine;

public  class GameplayUIManager : MonoBehaviour
{
    public TMP_Text WaitingForPlayerText => _waitingForPlayerText;
    public TMP_Text CountDownStartGameplayText => _countDownStartGameplayText;
    public TMP_Text CurrentGameplayScoreText => _currentGameplayScoreText;
    public TMP_Text WinnerText => _winnerText;

    [SerializeField] private TMP_Text _waitingForPlayerText;
    [SerializeField] private TMP_Text _countDownStartGameplayText;
    [SerializeField] private TMP_Text _currentGameplayScoreText;
    [SerializeField] private TMP_Text _winnerText;

    public void Initialize()
    {
        HideAll();
        _waitingForPlayerText.gameObject.SetActive(true);
    }

    public void HideAll()
    {
        _waitingForPlayerText.gameObject.SetActive(false);
        _countDownStartGameplayText.gameObject.SetActive(false);
        _currentGameplayScoreText.gameObject.SetActive(false);
        _winnerText.gameObject.SetActive(false);
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
}