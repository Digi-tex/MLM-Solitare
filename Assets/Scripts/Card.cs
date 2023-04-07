using System.Collections;
using System.Collections.Generic;
//using UnityEditor.SearchService;
using UnityEngine;



public class Card : MonoBehaviour
{
    public GameObject mainCamera;

    public int suit;                //Club 0 , diamond 1, heart 2, spade 3
    public int value;               //Value ace 1, 2-10, jack 11, queen 12, king 13
    public Sprite spriteFace;       //Sprite to hold the card face
    public Sprite spriteBack;       //Sprite to hold the card back
    public bool clicked = false;
    public bool inPyramid = false;

    public ContactFilter2D colFilter;

    public Collider2D[] colResults = new Collider2D[100];

    private Vector3 vel;

    public void MoveObject(Vector3 location)
    {
        StopAllCoroutines();
        StartCoroutine(SmoothMove(location));
    }

    private IEnumerator SmoothMove(Vector3 end)
    {
        while (Mathf.Abs(this.transform.position.x - end.x) > 0.01 || Mathf.Abs(this.transform.position.y - end.y) > 0.01)
        {
            this.transform.position = Vector3.SmoothDamp(this.transform.position, end, ref vel, 0.1f, 50);
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    public void FlipCard()
    {
        if (this.GetComponent<SpriteRenderer>().sprite.name != this.GetComponent<Card>().spriteFace.name)
        {
            this.GetComponent<SpriteRenderer>().sprite = this.GetComponent<Card>().spriteFace;
        }
        else
        {
            this.GetComponent<SpriteRenderer>().sprite = this.GetComponent<Card>().spriteBack;
        }
    }

    private void OnMouseDown()
    {
        if (this.GetComponent<SpriteRenderer>().sprite == spriteFace)
        {
            mainCamera.GetComponent<GameManager>().clickedCards.Enqueue(this.gameObject);

            if(mainCamera.GetComponent<GameManager>().clickedCards.Count > 2)
            {
                mainCamera.GetComponent<GameManager>().clickedCards.Dequeue();
            }
        }
    }

    public void CheckCollision()
    {
        if(inPyramid) 
        { 
            if (this.GetComponent<Collider2D>().OverlapCollider(colFilter, colResults) > 2)
            {
                this.GetComponent<SpriteRenderer>().sprite = spriteBack;
            }
            else
            {
                this.GetComponent<SpriteRenderer>().sprite = spriteFace;
            }

        }

    }

    void Start()
    {
        colFilter = new ContactFilter2D();
    }

    // Update is called once per frame
    void Update()
    {
        CheckCollision();
    }
}