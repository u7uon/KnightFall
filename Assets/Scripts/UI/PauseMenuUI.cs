using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] Button quitBtn ; 
    [SerializeField] Button continueBtn ;
    public static Action OnPauseMenuClose ; 
    void Awake()
    {
        quitBtn.onClick.AddListener(  () => SceneManager.LoadScene("MainMenu") ) ; 
        continueBtn.onClick.AddListener( () =>
        {
            OnPauseMenuClose?.Invoke();
            this.gameObject.SetActive(false);
        } );
    }

    void OnDisable()
    {
        Time.timeScale = 1f ;
    }
}
