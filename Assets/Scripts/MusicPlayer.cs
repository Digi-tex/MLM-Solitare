using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioSource gameMusic;
    
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        gameMusic.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
