using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    //Start the game by loading the appropriate scene
    public void PlayGame()
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void end()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
