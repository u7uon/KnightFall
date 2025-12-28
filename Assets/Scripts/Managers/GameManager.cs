using System;
using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    [SerializeField] private StageManager StageManager;
    [SerializeField] private MapManager mapManager ; 
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private GameObject hudUI;
    [SerializeField] private GameObject startUI;
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private CinemachineCamera _camera; 
    [SerializeField] private GameObject pauseGameUI ; 
    [SerializeField] private GameObject gameOverUI; 
    [SerializeField] private GameObject gameWinUI;

    private bool isPaused ; 
    private bool isGameStarted ;

    private PlayerStats character;
    
    private WeaponStats startingWeapon;

    void Update()
    {
        if(Input.GetKey(KeyCode.Escape) && !isPaused  )
        {
            hudUI.SetActive(false);
            pauseGameUI.SetActive(true); 
            isPaused = true;
            Time.timeScale = 0f ; 
        }

    }

    void Awake()
    {
        PauseMenuUI.OnPauseMenuClose += Resume;   
        UIEventManager.OnWinGame += Win ; 
    }

    private void Win()
    {
        isGameStarted = false ;
        spawner.gameObject.SetActive(false); 
        hudUI.gameObject.SetActive(false);  
        //StageManager.gameObject.SetActive(false);
        //mapManager.gameObject.SetActive(false) ; 
        gameWinUI.SetActive(true);
        Time.timeScale = 0 ;
    }

    private void Resume()
    {
        Time.timeScale = 1f ;
        isPaused = false;
        if(isGameStarted)
        {
            hudUI.SetActive(true);
        }
    }

    public void SetCharacter(PlayerStats chacterStats)
    {
        character = chacterStats;
    }

    public void SetStartingWeapon(WeaponStats weaponStats)
    {
        startingWeapon = weaponStats;
        StartGame();
    }



    public void StartGame()
    {
        isGameStarted = true ;
        Time.timeScale = 1f ;
        
        if(startingWeapon == null || character == null)
        {

            return;
        }       
        startUI.SetActive(false);
        PlayerStatsManager.OnPlayerDie += GameOver; 
        mapManager.gameObject.SetActive(true); 
        var player = Instantiate(character.characterPrefab, playerSpawnPoint.position, Quaternion.identity);
        player.GetComponent<PlayerInventory>().AddWeapon(startingWeapon);
        hudUI.SetActive(true);
        StageManager.gameObject.SetActive(true);
        spawner.gameObject.SetActive(true);
        StageManager.Play();
        _camera.Follow   = player.transform;

    }


    private void GameOver()
    {
        isGameStarted = false ;
        spawner.gameObject.SetActive(false); 
        hudUI.gameObject.SetActive(false);  
        //StageManager.gameObject.SetActive(false);
        //mapManager.gameObject.SetActive(false) ; 
        OpenGameOverPanel();
    }

    private void OpenGameOverPanel()
    {
        gameOverUI.SetActive(true);
        Time.timeScale = 0 ;
    }

void OnDestroy()
{
    PlayerStatsManager.OnPlayerDie -= GameOver;
    PauseMenuUI.OnPauseMenuClose -= Resume;
    // Nếu bạn muốn destroy singleton khi chuyển scene:
    if (Instance == this) Instance = null;
}
}
