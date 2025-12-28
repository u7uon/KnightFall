using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField]private Button quitBtn ;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        quitBtn.onClick.AddListener(Quit);
    }

    void Quit()
    {
        Time.timeScale = 1f ;
        SceneManager.LoadScene("MainMenu") ;
    }

}
