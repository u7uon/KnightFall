using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUIManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject mainMenu;

    [SerializeField] private Button settingBtn;
    [SerializeField] private Button closeSettingBtn;

    [SerializeField]private Button playButton;

    

    void Start()
    {
        settingBtn.onClick.AddListener(OpenSettings);
        closeSettingBtn.onClick.AddListener(CloseSettings);
        playButton.onClick.AddListener(OnPlayButtonClicked);
    }

    public void OpenSettings()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void OnPlayButtonClicked()
    {
        FindAnyObjectByType<BackgroundMusic>().gameObject.SetActive(false);
        SceneManager.LoadScene("GameScene");
    }
}
