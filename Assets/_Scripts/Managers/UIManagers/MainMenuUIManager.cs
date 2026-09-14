using UnityEngine;
using HeartOfIron.Enums;

public class MainMenuUIManager : MonoBehaviour
{
    public static MainMenuUIManager Instance { get; private set; }
    [SerializeField] private GameObject _mainMenuPanel;
    [SerializeField] private GameObject _settingsPanel;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        GameManager.Instance.LoadGame();
        ShowMainMenu();
    }

    public void OnStartButtonClicked()
    {
        SceneLoader.Instance.LoadScene(SceneName.Garage);
    }

    public void OnBackButtonClicked()
    {
        ShowMainMenu();
    }

    public void OnSettingsButtonClicked()
    {
        ShowSettings();
    }

    public void OnExitButtonClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveGame(); // Save the game before exiting
        }
        if (Application.isEditor)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
        else
        {
            Application.Quit();
        }
    }

    private void ShowMainMenu()
    {
        _mainMenuPanel.SetActive(true);
        _settingsPanel.SetActive(false);
    }

    private void ShowSettings()
    {
        _mainMenuPanel.SetActive(false);
        _settingsPanel.SetActive(true);
    }
}
