using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;

public class ClickManager : MonoBehaviour
{
    public GameObject mainCamera;


    private void OnMouseDown()
    {
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
