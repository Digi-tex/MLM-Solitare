using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickManager : MonoBehaviour
{
    public GameObject mainCamera;

    //Deal a card from the game managers draw pile when this button is clicked. 
    private void OnMouseDown()
    {
        this.GetComponent<AudioSource>().Play();
        mainCamera.GetComponent<GameManager>().dealCard();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
