using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    //Start the game by loading the appropriate scene
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    //Quit the game
    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
