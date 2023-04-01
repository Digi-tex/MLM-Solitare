using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR;

public class GameManager : MonoBehaviour
{
    public List<GameObject> drawCards = new List<GameObject>();         //The deck and subsequent draw pile for the game
    public List<GameObject> boardCards = new List<GameObject>();        //An list of card objects that are in play on the game board
    public Stack<GameObject> discardCards = new Stack<GameObject>();    //An Stack of card objects that are discarded
    public Stack<GameObject> spentCards = new Stack<GameObject>();      //A Stack of card objects that are used and removed from play
    public Queue<GameObject> clickedCards = new Queue<GameObject>();    //A queue to hold the cards that have been clicked by the user

    public GameObject card;                     //The GameObject to hold the reference to the card prefab
    public GameObject drawButton;               //The GameObject to hold the reference to the drawButton


    private List<Vector3> cardPositions;        //A list to hold the Vector2 locations of where the cards will be on the board
    private Vector3 deckPosition;               //Location of the deck
    private Vector3 discardPosition;            //Location of the discard pile
    private Vector3 spentPosition;              //Location of the spent pile
    private Transform boardHolder;              //A variable to store a reference to the transform of our Board object (currently unused)

    private float horStep;                      //Horizontal step size for locating the cards in a pyramid pattern
    private float vertStep;                     //Vertical step size for locating the cards in a pyramid pattern
    private float offset;                       //The offset for the triangle when we move to the next row
    private int numCards;                       //Number of cards for the base level of the pyramid
    private int numRows;                        //Number of rows in the pyramid
    private int cardCount;                      //Total cards in the pyramid


    private int currentState = 0;               //State variable to control what happens in the update loop
    private int cardforAnimation = 0;           //A counter variable to manage which card is being dealt to the board              
    private Vector3 velocity;                   //A variable to hold the velocity return value from the moveObject function (used for nothing)

    private int currentLayerOrder = 99;         //Starting layer for the draw pile / board pile
    private int discardLayer = 100;             //Starting layer for the discard pile
    private int spentLayer = 200;               //Starting layer for the spent pile



    //Deals a card from the drawpile to the discard pile
    public void dealCard()
    {
        clickedCards.Clear();

        drawCards[0].GetComponent<Card>().FlipCard();                              
        drawCards[0].GetComponent<SpriteRenderer>().sortingOrder = discardLayer++; 

        addToDiscard(drawCards[0]);
        drawCards.RemoveAt(0);

        StartCoroutine(discardCards.Peek().GetComponent<Card>().MoveObject(discardPosition, velocity, 0.2f, 40));
    }



    //Builds a 52 card deck in order and places them into the draw pile List
    //-Asigns suit, value, sprite for face, gameobject name, position, and layer
    private void Build52CardDeck()
    {
        string[] values = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13" }; //Just in case we had to use symbols, this was an array to hold the card values

        //Loop through and create the deck of cards. 
        //Depending on the suit denoted by the first for loop, we grab the card faces from different folders
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                GameObject newCard = Instantiate(card, deckPosition, Quaternion.identity);
                newCard.GetComponent<Card>().mainCamera = this.gameObject;
                newCard.GetComponent<Card>().suit = i;
                newCard.GetComponent<Card>().value = j + 1;

                switch (i)
                {
                    case 0:
                        newCard.GetComponent<Card>().spriteFace = Resources.Load<Sprite>("Sprites/Standard 52 Cards/solitaire/individuals/club/" + values[j] + "_club");
                        break;
                    case 1:
                        newCard.GetComponent<Card>().spriteFace = Resources.Load<Sprite>("Sprites/Standard 52 Cards/solitaire/individuals/diamond/" + values[j] + "_diamond");
                        break;
                    case 2:
                        newCard.GetComponent<Card>().spriteFace = Resources.Load<Sprite>("Sprites/Standard 52 Cards/solitaire/individuals/heart/" + values[j] + "_heart");
                        break;
                    case 3:
                        newCard.GetComponent<Card>().spriteFace = Resources.Load<Sprite>("Sprites/Standard 52 Cards/solitaire/individuals/spade/" + values[j] + "_spade");
                        break;
                }

                newCard.GetComponent<SpriteRenderer>().sprite = newCard.GetComponent<Card>().spriteBack;
                newCard.name = newCard.GetComponent<Card>().spriteFace.name;
                newCard.layer = 3;
                drawCards.Add(newCard);

            }
        }
    }



    //Shuffles the List which is passed by reference
    //-Sets the sorting order of the deck so that it properly displays in the game area
    private void ShuffleDeck(ref List<GameObject> deck)
    {
        var rng = new System.Random();  //Create a random number
        int n = deck.Count;

        //While we are still parsing through the deck
        while (n > 1)                   
        {
            int k = rng.Next(n--);      //Select a random position in the deck of remaining unsorted cards          
            GameObject temp = deck[n];  //Grab the top card @ n and swap it for a card at the randomly selected location
            deck[n] = deck[k];
            deck[k] = temp;
        }

        //Setup the sorting layers so the deck displays correctly on the board
        foreach(GameObject card in deck)
        {
            card.GetComponent<SpriteRenderer>().sortingOrder = currentLayerOrder -= 1;
            card.GetComponent<BoxCollider2D>().enabled = false;
        }    
    }



    //Initializes the game board by setting up all the locations cards will be placed
    private void initializeBoard()
    {

        boardHolder = new GameObject("Board").transform;        //Create a transform to hold all of the card game objects currenly in the boardCards list
        boardHolder.position = new Vector3(-3.3f, -3.5f, 0);    //Specify the location of the board holder 

        
        deckPosition = new Vector3(-5.0f, 2.0f, 0);             //Specify location of deck
        discardPosition = new Vector3(-3.5f, 2.0f, 0);          //Specify location of discard pile
        spentPosition = new Vector3(5.0f, 2.0f, 0);             //Specify location of spent pile  
        

        drawButton.transform.position = deckPosition;           //Set the position of the draw button to the deck position (kind of unnecessary as this is set in the scene)


        //Create the list of card positions for the pyramid based on the values specified above
        for(int i = 0; i < numRows; i++)
        {
            for(int j = 0; j < numCards - i; j++) 
            {
                cardPositions.Add(new Vector3((horStep * j) + (offset*i)-3, (vertStep * i)-4, 0));
            }
        }
    }



    //Deals the cards from the draw pile onto the board as a pyramid of cards
    private void dealBoard()
    {
        //If we have animated all of the cards for the pyramid, 
        if (cardforAnimation >= cardCount)
        {
            cardforAnimation = 0;
            currentState = 1;

            for (int i = 0; i < cardCount; i++)
            {
                GameObject temp = drawCards[0];
                temp.GetComponent<Card>().inPyramid = true;
                temp.GetComponent<BoxCollider2D>().enabled = true;
                boardCards.Add(temp);
                drawCards.RemoveAt(0);
            }
            return;
        }
        StartCoroutine(drawCards[cardforAnimation].GetComponent<Card>().MoveObject(cardPositions[cardforAnimation], velocity, 0.5f, 20));

        if (!(Mathf.Abs(drawCards[cardforAnimation].transform.position.x - cardPositions[cardforAnimation].x) > 0.1 || Mathf.Abs(drawCards[cardforAnimation].transform.position.y - cardPositions[cardforAnimation].y) > 0.1))
        {
            if (cardforAnimation < 7)
            {
                drawCards[cardforAnimation].GetComponent<Card>().FlipCard();
            }
            cardforAnimation++;
        }
    }

    private void addToDiscard(GameObject card)
    {
        if(discardCards.Count != 0)
        {
            discardCards.Peek().GetComponent<BoxCollider2D>().enabled = false;
            discardCards.Push(card);
            discardCards.Peek().GetComponent<BoxCollider2D>().enabled = true;
        }
        else
        {
            discardCards.Push(card);
            discardCards.Peek().GetComponent<BoxCollider2D>().enabled = true;
        }
    }
    private GameObject removeFromDiscard()
    {
        if (discardCards.Count != 0)
        {
            GameObject tempCard = discardCards.Pop();
            tempCard.GetComponent<BoxCollider2D>().enabled = false;

            if(discardCards.Count != 0)
                discardCards.Peek().GetComponent<BoxCollider2D>().enabled = true;

            return tempCard;
        }
        else
        {
            return null;
        }
    }



    // Start is called before the first frame update
    private void Start()
    {
        initializeBoard();
        Build52CardDeck();
        ShuffleDeck(ref drawCards);
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            //Deal the board
            case 0:
                {
                    dealBoard();
                    break;
                }
            //Play the game
            case 1:
                {

                    int cardSum = 0;

                    if (clickedCards.Count != 0)
                    {
                        foreach (GameObject card in clickedCards)
                        {

                            if (card.GetComponent<Card>().value == 13)
                            {
                                spentCards.Push(card);

                                card.GetComponent<BoxCollider2D>().enabled = false;
                                card.GetComponent<SpriteRenderer>().sortingOrder = spentLayer++;

                                if (discardCards.Contains(card))
                                {
                                    removeFromDiscard();
                                }
                                else
                                    boardCards.RemoveAt(boardCards.IndexOf(card));

                                StartCoroutine(spentCards.Peek().GetComponent<Card>().MoveObject(spentPosition, velocity, 0.5f, 20));
                                
                                clickedCards.Clear();
                                cardSum = 0;

                                break;
                            }
                            else
                                cardSum += card.GetComponent<Card>().value;
                        }

                        if (cardSum == 13)
                        {
                            foreach (GameObject card in clickedCards)
                            {
                                spentCards.Push(card);

                                card.GetComponent<BoxCollider2D>().enabled = false;
                                card.GetComponent<SpriteRenderer>().sortingOrder = spentLayer++;

                                if (discardCards.Contains(card))
                                {
                                    removeFromDiscard();
                                }
                                else
                                    boardCards.RemoveAt(boardCards.IndexOf(card));

                                StartCoroutine(spentCards.Peek().GetComponent<Card>().MoveObject(spentPosition, velocity, 0.5f, 20));
                            }
                            clickedCards.Clear();
                        }
                    }
                    break;
                }

        }
    }
}
