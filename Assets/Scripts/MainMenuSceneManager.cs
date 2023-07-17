using CountTo100.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuSceneManager : MonoSingleton<MainMenuSceneManager>
{
    [SerializeField] private Button _startAsServerButton;
    [SerializeField] private Button _startAsClientButton;
    [SerializeField] private TMP_Dropdown _numberOfPlayersDropDown;

    protected override void Awake()
    {
        base.Awake();
        _startAsServerButton.onClick.AddListener(OnStartAsServerButtonClicked);
        _startAsClientButton.onClick.AddListener(OnStartAsClientButtonClicked);
    }

    private void OnStartAsServerButtonClicked()
    {
        GlobalServerConfigManager.IsServer = true;
        //in local testing, simulate getting allocation payloads to get number of players, etc.
        //https://docs.unity.com/game-server-hosting/manual/concepts/allocations-payload
        int numberOfPlayersLocalServerConfig = int.Parse(_numberOfPlayersDropDown.options[_numberOfPlayersDropDown.value].text);
        GlobalServerConfigManager.LocalServerAllocationPayload = new LocalServerAllocationPayload(numberOfPlayersLocalServerConfig);
        SceneManager.LoadScene("Gameplay");
    }

    private void OnStartAsClientButtonClicked()
    {
        GlobalServerConfigManager.IsServer = false;
        //TODO: To player naming and quick joining flow first
        SceneManager.LoadScene("Gameplay");
    }
}
