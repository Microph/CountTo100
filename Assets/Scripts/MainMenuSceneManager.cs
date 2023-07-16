using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuSceneManager : MonoBehaviour
{
    [SerializeField] private Button _startAsServerButton;
    [SerializeField] private Button _startAsClientButton;

    private void Awake()
    {
        _startAsServerButton.onClick.AddListener(OnStartAsServerButtonClicked);
        _startAsClientButton.onClick.AddListener(OnStartAsClientButtonClicked);
    }

    private void OnStartAsServerButtonClicked()
    {
        GlobalConfigManager.IsServer = true;
        SceneManager.LoadScene("Gameplay");
    }

    private void OnStartAsClientButtonClicked()
    {
        GlobalConfigManager.IsServer = false;
        //TODO: To player naming and quick joining flow first
        SceneManager.LoadScene("Gameplay");
    }
}
