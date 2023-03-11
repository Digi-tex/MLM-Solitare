using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.XR;

public class GameManager : MonoBehaviour
{

    public DeckBuilder drawPile;                //The deck and subsequent draw pile for the game

    public List<DeckBuilder.Card> boardCards;   //An list of card objects that are in play on the game board
    public List<DeckBuilder.Card> discardCards; //An list of card objects that are discarded
    public List<DeckBuilder.Card> spentCards;   //A list of card objects that are used and removed from play

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

    private void initializeBoard()
    {
        drawPile = new DeckBuilder();                   //Build a new deck of cards.
        cardforAnimation = drawPile.deck.Count -1;
        boardCards = new List<DeckBuilder.Card>();      //Create a list to hold the cards currently on the board in play
        discardCards = new List<DeckBuilder.Card>();    //Create a list to hold cards currently in the discard pile
        spentCards = new List<DeckBuilder.Card>();      //Create a list to hold cards that have been matched and removed from play

        boardHolder = new GameObject("Board").transform;//Create a transform to hold all of the card game objects currenly in the boardCards list
        boardHolder.position = new Vector3(-3.3f, -3.5f, 0);

        deckPosition = new Vector3(-5, 2, 0);              //Specify location of deck
        discardPosition = new Vector3(-4, 2, 0);           //Specify location of discard pile
        spentPosition = new Vector3(5, 2, 0);              //Specify location of spent pile
        

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
                cardPositions.Add(new Vector3((horStep * j) + (offset*i), vertStep * i, 0));
            }
        }

        for(int j = 0; j < drawPile.deck.Count; j++)
        {
            drawPile.deck[j].r.sortingOrder = currentLayerOrder--;
        }
    }
    
    private IEnumerator MoveObject(GameObject obj, Vector3 end, Vector3 vel, float smoothTime, float speed)
    {
        while (obj.transform.position != end)
        {
            obj.transform.position = Vector3.SmoothDamp(obj.transform.position, end, ref vel,smoothTime, speed);
            yield return 0;
        }
    }

    private void changeGameState(int i)
    {
        previousState = currentState;
        currentState = i;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        initializeBoard();
    }
    // Update is called once per frame
    void Update()
    {
        switch(currentState)
        {
            case 0:
                {
                    if (cardforAnimation < 0)
                    {
                        cardforAnimation = 0;
                        changeGameState(1);
                        break;
                    }
                    StartCoroutine(MoveObject(drawPile.deck[cardforAnimation].card, deckPosition, velocity, 0.5f, 40));
                    if (drawPile.deck[cardforAnimation].card.transform.position == deckPosition)
                        cardforAnimation--;
                    break;
                }
        }
        
    }
}
