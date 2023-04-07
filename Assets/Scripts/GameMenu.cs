using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    //Replay the game by reloading the current scene
    public void NewGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //Quit the game
    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
