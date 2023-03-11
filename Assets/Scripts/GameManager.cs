using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{

    public DeckBuilder drawPile;                //The deck and subsequent draw pile for the game

    public List<DeckBuilder.Card> boardCards;   //An list of card objects that are in play on the game board
    public List<DeckBuilder.Card> discardCards; //An list of card objects that are discarded
    public List<DeckBuilder.Card> spentCards;   //A list of card objects that are used and removed from play

    private List<Vector2> cardPositions;        //A list to hold the Vector2 locations of where the cards will be on the board
    private Vector2 deckPosition;               //Location of the deck
    private Vector2 discardPosition;            //Location of the discard pile
    private Vector2 spentPosition;              //Location of the spent pile
    private Transform boardHolder;              //A variable to store a reference to the transform of our Board object

    private float horStep;                      //Horizontal step size for locating the cards in a pyramid pattern
    private float vertStep;                     //Vertical step size for locating the cards in a pyramid pattern
    private float offset;
    private int numCards;                       //Number of cards for the base level of the pyramid
    private int numRows;                        //Number of rows in the pyramid
    private int totalCards;                     //Total cards in the pyramid


    private void initializeBoard()
    {
        drawPile = new DeckBuilder();                   //Build a new deck of cards.
        boardCards = new List<DeckBuilder.Card>();      //Create a list to hold the cards currently on the board in play
        discardCards = new List<DeckBuilder.Card>();    //Create a list to hold cards currently in the discard pile
        spentCards = new List<DeckBuilder.Card>();      //Create a list to hold cards that have been matched and removed from play

        boardHolder = new GameObject("Board").transform;//Create a transform to hold all of the card game objects currenly in the boardCards list
        
        deckPosition = new Vector2(-5, 2);              //Specify location of deck
        discardPosition = new Vector2(-4, 2);           //Specify location of discard pile
        spentPosition = new Vector2(5, 2);              //Specify location of spent pile
        
        cardPositions = new List<Vector2>();            //Create a list that holds all of the positions of where boardCards can be played
        horStep = 1.1f;                                 //The horizontal step when laying out cards
        vertStep = 1f;                                  //The vertical step when laying out cards
        offset = horStep / 2;                           //The offset for the triangle when we move to the next row
        numCards = 7;                                   //Number of cards at the bottom row
        numRows = 7;                                    //Number of rows of the pyramid
        totalCards = (numCards * (numCards + 1)) / 2;   //Calculating the total cards in the pyramid


        //Create the List of card positions based on the values specified above
        for(int i = 0; i < numRows; i++)
        {
            for(int j = 0; j < numCards - i; j++) 
            {
                cardPositions.Add(new Vector2((horStep * j) + (offset*i), vertStep * i));
            }
        }
    }
    
    private void dealDeck()
    {
        for(int i = 0; i < drawPile.deck.Count; i++)
        {

        }
    }
    private void dealBoard()
    {

        for(int i = 0; i < totalCards; i++)
        {
            boardCards.Add(drawPile.deck[0]);
            drawPile.deck.RemoveAt(0);

            GameObject g = new GameObject(boardCards[i].sprite.name);
            g.transform.position = cardPositions[i];
            g.transform.parent = boardHolder;
            SpriteRenderer r = g.AddComponent<SpriteRenderer>();
            if (i < 7)
            {
                r.sprite = boardCards[i].sprite;
            }
            else
            {
                r.sprite = Resources.Load<Sprite>("Sprites/Standard 52 Cards/solitaire/individuals/card back/card_back");
            }
                r.sortingOrder = totalCards - i;
            
        }
        boardHolder.position = new Vector2(-3.3f, -3.5f);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        initializeBoard();
        //dealBoard();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
