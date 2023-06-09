using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    /*
     * GameManager is a special name reserved for Scripts.
     * It allows certain functions such as using a UI for a GameObject while the normal script doesn't
     */
    public static bool GameIsOver = false;
    public static bool GameIsPaused = false;
    public GameObject gameOverUI;
    public GameObject VictoryUI;
    public GameObject PlayerUI;
    public GameObject ShopUI;
    public GameObject pauseUI;
    private BuildManager _buildManager;
    public static GameManager _gameManager;
    private AudioManager _audioManager;
    private int levelToUnlock;

    private void Awake()
    {
        if (_gameManager != null)
        {
            return;
        }

        _gameManager = this;
        _buildManager = BuildManager.Builder;
        _audioManager = AudioManager.instance;
    }

    private void Start()
    {
        GameIsOver = false;
        GameIsPaused = false;
        Time.timeScale = 1;
        levelToUnlock = 2;
    }

    public void EndGame(bool isVictorious)
    {
        GameIsOver = true;
        
        if (isVictorious)
        {
            VictoryUI.SetActive(true);
            _audioManager.PlayMusic("Victory");

            //PlayerPrefs.SetInt("LevelReached", levelToUnlock);
            levelToUnlock++;
        }
        else
        {
            // clear the screen
            var enemiesToRemove = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var enemy in enemiesToRemove)
            {
                Destroy(enemy);
            }

            // Deselect any Towers
            _buildManager.SelectTurretToBuild(null);
            // Open the GameOverScreen
            gameOverUI.SetActive(true);

            _audioManager.PlayMusic("GameOver");
        }
    }


    public static void FastForward()
    {
        // bug: will fail if the player is still in FF state and changes FFmultiplier in the editor
        Time.timeScale = (1 / Time.timeScale) * PlayerPrefs.GetFloat(SettingsMenu.FfMultiplier);
    }
    

    void Update()
    {
        if (Input.GetKeyDown("p"))
        {
            if (!GameIsPaused)
            {
                pauseUI.SetActive(true);
                _audioManager.Pause();
            }
            else
            {
                pauseUI.SetActive(false);
                _audioManager.Unpause();
            }

            GameIsPaused = !GameIsPaused;
        }
        if (GameIsOver)
        {
            return;
        }

        if (PlayerStats.Lives <= 0)
        {
            EndGame(false);
        }
    }
}
