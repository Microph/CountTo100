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
    [SerializeField] private TMP_InputField _localServerBindingIPInputTextField;
    [SerializeField] private TMP_InputField _localServerPortInputTextField;

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
        string localServerBindingIP = _localServerBindingIPInputTextField.text;
        ushort localServerPort = ushort.Parse(_localServerPortInputTextField.text);
        GlobalServerConfigManager.LocalServerAllocationPayload = new LocalServerAllocationPayload(numberOfPlayersLocalServerConfig, localServerBindingIP, localServerPort);
        SceneManager.LoadScene("Gameplay");
    }

    private void OnStartAsClientButtonClicked()
    {
        GlobalServerConfigManager.IsServer = false;
        SceneManager.LoadScene("ClientJoinLobbyScene");
    }
}
