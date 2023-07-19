using System;
using TMPro;
using UnityEngine;

public class GameplayUIManager : MonoBehaviour
{
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

    public void ShowCurrentGameplayScoreText()
    {
        HideAll();
        _currentGameplayScoreText.gameObject.SetActive(true) ;
    }

    public void UpdateGameplayScoreText(int newValue)
    {
        _currentGameplayScoreText.text = newValue.ToString();
    }

    public void ShowWinnerText()
    {
        HideAll();
        _winnerText.gameObject.SetActive(true);
    }

    public void SetWinnerText(string playerName, ulong clientId)
    {
        _winnerText.text = $"{playerName} (ID: {clientId}) counted to\n100";
    }
}