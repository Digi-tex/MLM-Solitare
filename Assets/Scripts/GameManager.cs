using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.XR;

public class GameManager : MonoBehaviour
{
    public GameObject card;
    public GameObject drawButton;

    public List<GameObject> drawCards;               //The deck and subsequent draw pile for the game
    public List<GameObject> boardCards;              //An list of card objects that are in play on the game board
    public List<GameObject> discardCards;            //An list of card objects that are discarded
    public List<GameObject> spentCards;              //A list of card objects that are used and removed from play
    public List<GameObject> clickedCards;

    public int cardTotal;

    private List<Vector3> cardPositions;        //A list to hold the Vector2 locations of where the cards will be on the board
    private Vector3 deckPosition;               //Location of the deck
    private Vector3 discardPosition;            //Location of the discard pile
    private Vector3 spentPosition;              //Location of the spent pile
    private Transform boardHolder;              //A variable to store a reference to the transform of our Board object

    private float horStep;                      //Horizontal step size for locating the cards in a pyramid pattern
    private float vertStep;                     //Vertical step size for locating the cards in a pyramid pattern
    private float offset;
    private int numCards;                       //Number of cards for the base level of the pyramid
    private int numRows;                        //Number of rows in the pyramid
    private int totalCards;                     //Total cards in the pyramid

    private int currentState = 0;
    private int previousState;

    private int cardforAnimation;
    private Vector3 velocity;

    private int currentLayerOrder = 0;
    private int discardLayer = 100;
    private int spentLayer = 100;

    public void Build52CardDeck()
    {
        string[] values = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13" }; //Just in case we had to use symbols, this was an array to hold the card values

        //Loop through and create the deck of cards. 
        //Depending on the suit denoted by the first for loop, we grab the card faces from different folders
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                GameObject newCard = Instantiate(card, deckPosition, Quaternion.identity);
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

    public void ShuffleDeck(ref List<GameObject> deck)
    {
        var rng = new System.Random();
        int n = deck.Count;
        while (n > 1)
        {
            int k = rng.Next(n--);
            GameObject temp = deck[n];
            deck[n] = deck[k];
            deck[k] = temp;
        }

        foreach(GameObject card in deck)
        {
            card.GetComponent<SpriteRenderer>().sortingOrder = currentLayerOrder -= 1;
            card.GetComponent<BoxCollider2D>().enabled = false;
        }    
    }

    private void initializeBoard()
    {

        boardHolder = new GameObject("Board").transform;//Create a transform to hold all of the card game objects currenly in the boardCards list
        boardHolder.position = new Vector3(-3.3f, -3.5f, 0);

        
        deckPosition = new Vector3(-5.0f, 2.0f, 0);           //Specify location of deck
        discardPosition = new Vector3(-3.5f, 2.0f, 0);        //Specify location of discard pile
        spentPosition = new Vector3(5.0f, 2.0f, 0);           //Specify location of spent pile  
        
        drawButton.transform.position = deckPosition;
        
        
        cardforAnimation = 0;

        cardPositions = new List<Vector3>();            //Create a list that holds all of the positions of where boardCards can be played
        horStep = 1.1f;                                 //The horizontal step when laying out cards
        vertStep = 1f;                                  //The vertical step when laying out cards
        offset = horStep / 2;                           //The offset for the triangle when we move to the next row
        numCards = 7;                                   //Number of cards at the bottom row
        numRows = 7;                                    //Number of rows of the pyramid
        totalCards = (numCards * (numCards + 1)) / 2;   //Calculating the total cards in the pyramid

        

        //Create the list of card positions based on the values specified above
        for(int i = 0; i < numRows; i++)
        {
            for(int j = 0; j < numCards - i; j++) 
            {
                cardPositions.Add(new Vector3((horStep * j) + (offset*i)-3, (vertStep * i)-4, 0));
            }
        }

        for(int j = 0; j < drawCards.Count; j++)
        {
            drawCards[j].GetComponent<SpriteRenderer>().sortingOrder = currentLayerOrder--;
        }
    }



    private void changeGameState(int i)
    {
        previousState = currentState;
        currentState = i;
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
                    if (cardforAnimation >= totalCards)
                    {
                        cardforAnimation = 0;
                        changeGameState(1);

                        for (int i = 0; i < totalCards; i++)
                        {
                            GameObject temp = drawCards[0];
                            temp.GetComponent<Card>().inPyramid = true;
                            temp.GetComponent<BoxCollider2D>().enabled = true;
                            boardCards.Add(temp);
                            drawCards.RemoveAt(0);
                        }
                        break;
                    }
                    StartCoroutine(drawCards[cardforAnimation].GetComponent<Card>().MoveObject(cardPositions[cardforAnimation], velocity, 0.5f, 20));
                    
                    if(!(Mathf.Abs(drawCards[cardforAnimation].transform.position.x - cardPositions[cardforAnimation].x) > 0.1 || Mathf.Abs(drawCards[cardforAnimation].transform.position.y - cardPositions[cardforAnimation].y) > 0.1))
                    {
                        if (cardforAnimation < 7)
                        {
                            drawCards[cardforAnimation].GetComponent<Card>().FlipCard();
                        }
                        cardforAnimation++;
                    }

                    break;
                }

            //Check collision and check to see if the draw pile has been clicked
            case 1:
                {
                    if (drawButton.GetComponent<ClickManager>().clicked == true)
                    {
                        drawButton.GetComponent<ClickManager>().clicked = false;

                        drawCards[0].GetComponent<Card>().FlipCard();
                        drawCards[0].GetComponent<SpriteRenderer>().sortingOrder = discardLayer++;

                        boardCards.Insert(0, drawCards[0]);
                        boardCards[0].GetComponent<BoxCollider2D>().enabled = true;
                        boardCards[1].GetComponent<BoxCollider2D>().enabled = false;
                        drawCards.RemoveAt(0);

                        StartCoroutine(boardCards[0].GetComponent<Card>().MoveObject(discardPosition, velocity, 0.2f, 40));
                        
                    }
                    changeGameState(2);
                    break;
                }

            case 2:
                {
                    foreach(GameObject card in boardCards)
                    {
                        if(card.GetComponent<Card>().clicked && !clickedCards.Contains(card))
                        {
                            if (clickedCards.Count >= 2)
                            {
                                if (discardCards.Contains(clickedCards[0]))
                                {
                                    discardCards[discardCards.IndexOf(clickedCards[0])].GetComponent<Card>().clicked = false;
                                }
                                else
                                    boardCards[boardCards.IndexOf(clickedCards[0])].GetComponent<Card>().clicked = false;

                                clickedCards.RemoveAt(0);
                                clickedCards.Add(card);
                            }
                            else
                                clickedCards.Add(card);
                        }
                    }
                    foreach (GameObject card in discardCards)
                    {
                        if (card.GetComponent<Card>().clicked && !clickedCards.Contains(card))
                        {
                            if (clickedCards.Count >= 2)
                            {
                                if (discardCards.Contains(clickedCards[0]))
                                {
                                    discardCards[discardCards.IndexOf(clickedCards[0])].GetComponent<Card>().clicked = false;
                                }
                                else
                                    boardCards[boardCards.IndexOf(clickedCards[0])].GetComponent<Card>().clicked = false;

                                clickedCards.RemoveAt(1);
                                clickedCards.Add(card);
                            }
                            else
                                clickedCards.Add(card);
                        }
                    }

                    cardTotal = 0;

                    foreach(GameObject card in clickedCards)
                    {
                        cardTotal += card.GetComponent<Card>().value;

                        if(card.GetComponent<Card>().value == 13)
                        {
                            cardTotal = -1;
                            break;
                        }
                    }

                    if(cardTotal == 13)
                    {
                        foreach(GameObject card in clickedCards)
                        {
                            spentCards.Add(card);
                            card.GetComponent<Card>().clicked = false;
                            card.GetComponent<BoxCollider2D>().enabled = false;

                            if (discardCards.Contains(card))
                            {
                                discardCards.RemoveAt(discardCards.IndexOf(card));
                            }
                            else
                                boardCards.RemoveAt(boardCards.IndexOf(card));

                            spentCards[spentCards.Count - 1].GetComponent<SpriteRenderer>().sortingOrder = spentLayer++;
                            StartCoroutine(spentCards[spentCards.IndexOf(card)].GetComponent<Card>().MoveObject(spentPosition, velocity, 0.5f, 20));
                        }
                        clickedCards.Clear();
                    }
                    else if(cardTotal == -1)
                    {
                        foreach(GameObject card in clickedCards)
                        {
                            if (card.GetComponent<Card>().value == 13)
                            {
                                spentCards.Add(card);
                                card.GetComponent<Card>().clicked = false;
                                card.GetComponent<BoxCollider2D>().enabled = false;

                                if (discardCards.Contains(card))
                                {
                                    discardCards.RemoveAt(discardCards.IndexOf(card));
                                }
                                else
                                    boardCards.RemoveAt(boardCards.IndexOf(card));

                                spentCards[spentCards.Count - 1].GetComponent<SpriteRenderer>().sortingOrder = spentLayer++;
                                StartCoroutine(spentCards[spentCards.IndexOf(card)].GetComponent<Card>().MoveObject(spentPosition, velocity, 0.5f, 20));
                            }
                            else
                                card.GetComponent<Card>().clicked = false;
                        }

                        clickedCards.Clear();
                    }

                    changeGameState(1);
                    break;
                }

        }
    }
}
