using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    //Replay the game by reloading the current scene
    public void NewGame()
    {
        this.GetComponent<AudioSource>().Play();

        Invoke("begin", 0.3f);
    }

    //Quit the game
    public void QuitGame()
    {
        this.GetComponent<AudioSource>().Play();
        Invoke("end", 0.3f);
    }

    private void begin()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void end()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
