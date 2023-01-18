using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    // Called when the singleplayer button is pressed
    public void OnClickSinglePlayer()
    {
        Debug.Log("Loading singleplayer game");
    }

    // Called when the multiplayer button is pressed
    public void OnClickMultiPlayer()
    {
        SceneManager.LoadScene("Multiplayer_Launcher");
        Debug.Log("Loading multiplayer game");
    }
}
