using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public GameObject mainCamera;   //A reference to the main camera that holds the game manager script so that each card can modify the game params

    public int suit;                //Club 0 , diamond 1, heart 2, spade 3
    public int value;               //Value ace 1, 2-10, jack 11, queen 12, king 13
    public Sprite spriteFace;       //Sprite to hold the card face
    public Sprite spriteBack;       //Sprite to hold the card back
    public bool inPyramid = false;  //A flag to determine if we should check collision

    public ContactFilter2D colFilter;                       //Used to handle collisions (filter not actually used, but needed for method)
    public Collider2D[] colResults = new Collider2D[100];   //Used to handle collisions (100 is far too many array spots, but meh)

    private Vector3 vel;            //Hold the returned speed of the movement



    //Move an card from its current location to a new location
    public void MoveObject(Vector3 location)
    {
        //Stop all existing coroutines on thsi card and start a new one
        StopAllCoroutines();
        StartCoroutine(SmoothMove(location));
    }



    //Move the card smoothly from one spot to another using smooth damp
    //Allow for some sloppiness in the movement so that a card isn't trying to be pixel perfect
    private IEnumerator SmoothMove(Vector3 end)
    {
        while (Mathf.Abs(this.transform.position.x - end.x) > 0.01 || Mathf.Abs(this.transform.position.y - end.y) > 0.01)
        {
            this.transform.position = Vector3.SmoothDamp(this.transform.position, end, ref vel, 0.1f, 50);
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }



    //Flip the card face from face-up to face-down or vice versa
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



    //If a card has been clicked, add it to the clicked cards pile of the game manager script
    //Only do so if the card is face-up, meaning that it's in play
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


    //Check the collision on the card to determine if it should be face-up or face-down in the pyramid
    //Each card will check its own collision and automatically update
    public void CheckCollision()
    {
        //If the card is in the pyramid (board pile)
        if(inPyramid) 
        { 
            //If the card is covered by at least two other cards (or is on the edge of the pyramid by colliding with an invisible gameobject)
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

    //Initialize the contact filter (could just be initialized above but w/e)
    void Start()
    {
        colFilter = new ContactFilter2D();
    }

    //check collision every frame (kind of wasteful but w/e)
    void Update()
    {
        CheckCollision();
    }
}