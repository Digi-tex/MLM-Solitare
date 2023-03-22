using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.XR;

public class GameManager : MonoBehaviour
{
    public GameObject card;

    public List<GameObject> drawCards;               //The deck and subsequent draw pile for the game
    public List<GameObject> boardCards;              //An list of card objects that are in play on the game board
    public List<GameObject> discardCards;            //An list of card objects that are discarded
    public List<GameObject> spentCards;              //A list of card objects that are used and removed from play


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

    public void Build52CardDeck()
    {
        string[] values = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13" }; //Just in case we had to use symbols, this was an array to hold the card values

        //Loop through and create the deck of cards. 
        //Depending on the suit denoted by the first for loop, we grab the card faces from different folders
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                GameObject newCard = Instantiate(card, new Vector3(0, -6.0f, 0), Quaternion.identity);
                newCard.GetComponent<Card>().suit = i;
                newCard.GetComponent<Card>().value = j;

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
    }

    private void initializeBoard()
    {

        cardforAnimation = drawCards.Count -1;

        boardHolder = new GameObject("Board").transform;//Create a transform to hold all of the card game objects currenly in the boardCards list
        boardHolder.position = new Vector3(-3.3f, -3.5f, 0);

        deckPosition = new Vector3(-5, 2, 0);           //Specify location of deck
        discardPosition = new Vector3(-4, 2, 0);        //Specify location of discard pile
        spentPosition = new Vector3(5, 2, 0);           //Specify location of spent pile  

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

    public void CheckCollision(ref List<GameObject> deck)
    {
        foreach (GameObject card in deck)
        {
            
            if (card.GetComponent<Collider2D>().OverlapCollider(card.GetComponent<Card>().colFilter, card.GetComponent<Card>().colResults) > 2)
            {
                if (card != deck[deck.Count - 1])
                    card.GetComponent<SpriteRenderer>().sprite = card.GetComponent<Card>().spriteBack;
                else if (card.GetComponent<Collider2D>().OverlapCollider(card.GetComponent<Card>().colFilter, card.GetComponent<Card>().colResults) == 2)
                    card.GetComponent<SpriteRenderer>().sprite = card.GetComponent<Card>().spriteFace;
            }
            else
            {
                card.GetComponent<SpriteRenderer>().sprite = card.GetComponent<Card>().spriteFace;
            }

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
        Build52CardDeck();
        ShuffleDeck(ref drawCards);
        initializeBoard();
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            //Deal the deck onto the board
            case 0:
                {
                    if (cardforAnimation < 0)
                    {
                        cardforAnimation = 0;
                        changeGameState(1);
                        break;
                    }
                    StartCoroutine(drawCards[cardforAnimation].GetComponent<Card>().MoveObject(deckPosition, velocity, 0.2f, 120));
                    if (drawCards[cardforAnimation].transform.position == deckPosition)
                        cardforAnimation--;
                    break;
                }
            //Deal the board
            case 1:
                {
                    if (cardforAnimation >= totalCards)
                    {
                        cardforAnimation = 0;
                        changeGameState(2);

                        for (int i = 0; i < totalCards; i++)
                        {
                            GameObject temp = drawCards[0];
                            boardCards.Add(temp);
                            drawCards.RemoveAt(0);
                        }
                        break;
                    }
                    StartCoroutine(drawCards[cardforAnimation].GetComponent<Card>().MoveObject(cardPositions[cardforAnimation], velocity, 0.5f, 20));
                    if (drawCards[cardforAnimation].transform.position == cardPositions[cardforAnimation])
                    {
                        if (cardforAnimation < 7)
                        {
                            drawCards[cardforAnimation].GetComponent<Card>().FlipCard();
                        }
                        cardforAnimation++;
                    }

                    break;
                }
            //Wait for 
            case 2:
                {
                    
                    CheckCollision(ref boardCards);

                    /*
                    boardCards.CheckClicked(ref clickedCards);
                    discardCards.CheckClicked(ref clickedCards);

                    int sumCards = 0;

                    foreach(Card card in clickedCards.deck)
                    {
                        sumCards += card.value;
                    }
                    if(sumCards == 15)
                    {
                        changeGameState(3);
                        cardforAnimation = 0;
                    }
                    */
                    break;
                }
            case 3:
                {
                    /*
                    if (cardforAnimation >= 2)
                    {
                        cardforAnimation = 0;
                        changeGameState(2);

                        for (int i = 0; i < 2; i++)
                        {
                            spentCards.AddCard(clickedCards.deck[0]);
                            boardCards.deck.Remove(clickedCards.deck[0]);
                            clickedCards.RemoveCard();
                        }
                        break;
                    }
                    Card myCard = boardCards.deck.Find(bySprite(clickedCards.deck[cardforAnimation].r.sprite));

                    StartCoroutine(MoveObject(myCard.card, spentPosition, velocity, 0.5f, 20));
                    if (myCard.card.transform.position == spentPosition)
                    {
                        cardforAnimation++;
                    }
                    */
                    break;
                }
        }
    }
}
