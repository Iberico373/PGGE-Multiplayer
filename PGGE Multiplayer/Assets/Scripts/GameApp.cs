using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PGGE.Patterns;

public class GameApp : Singleton<GameApp>
{
    // Determines wether the game is paused or not
    private bool m_Pause = false;
    // Bool property that pauses the game based on m_Pause
    public bool GamePaused
    {
        get { return m_Pause; }

        set
        {
            m_Pause = value;
            if (m_Pause)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
    }

    private void Start()
    {
        // GamePaused defaulted to false
        GamePaused = false;
        // Immediately go to the main menu after intializing GameApp
        SceneManager.LoadScene("Menu");
    }

    private void Update()
    {
        // Pause/Resume the game when the player presses escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GamePaused = !GamePaused;
        }
    }    

    private void OnEnable()
    {
        Debug.Log("OnEnable called");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        Debug.Log("OnDisable");
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Outputs current scene build index and name to console
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded - Scene Index: " + scene.buildIndex + "Scene Name: " + scene.name);
        Debug.Log(mode);
    }
}
