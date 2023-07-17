using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuSceneManager : MonoBehaviour
{
    [SerializeField] private Button _startAsServerButton;
    [SerializeField] private Button _startAsClientButton;
    [SerializeField] private TMP_Dropdown _numberOfPlayersDropDown;

    private void Awake()
    {
        _startAsServerButton.onClick.AddListener(OnStartAsServerButtonClicked);
        _startAsClientButton.onClick.AddListener(OnStartAsClientButtonClicked);
    }

    private void OnStartAsServerButtonClicked()
    {
        GlobalConfigManager.IsServer = true;
        //in local testing, simulate getting allocation payloads to get number of players, etc.
        int numberOfPlayersLocalServerConfig = int.Parse(_numberOfPlayersDropDown.options[_numberOfPlayersDropDown.value].text);
        GlobalConfigManager.LocalServerAllocationPayload = new LocalServerAllocationPayload(numberOfPlayersLocalServerConfig);
        SceneManager.LoadScene("Gameplay");
    }

    private void OnStartAsClientButtonClicked()
    {
        GlobalConfigManager.IsServer = false;
        //TODO: To player naming and quick joining flow first
        SceneManager.LoadScene("Gameplay");
    }
}
