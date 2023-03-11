using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{

    public DeckBuilder drawPile;                    //The deck and subsequent draw pile for the game

    public List<DeckBuilder.Card> boardCards;   //An list of card objects that are in play on the game board
    public List<DeckBuilder.Card> discardCards; //An list of card objects that are discarded
    public List<DeckBuilder.Card> spentCards;   //A list of card objects that are used and removed from play

    private List<Vector2> cardPositions;        //A list to hold the Vector2 locations of where the cards will be on the board
    private Transform boardHolder;              //A variable to store a reference to the transform of our Board object

    private float horStep;                        //Horizontal step size for locating the cards in a pyramid pattern
    private float vertStep;                       //Vertical step size for locating the cards in a pyramid pattern
    private float offset;
    private int numCards;                       //Number of cards for the base level of the pyramid
    private int numRows;                        //Number of rows in the pyramid
    private int totalCards;                     //Total cards in the pyramid


    private void initializeBoard()
    {
        drawPile = new DeckBuilder();       //Build a new deck of cards.
        boardCards = new List<DeckBuilder.Card>();
        discardCards = new List<DeckBuilder.Card>();

        boardHolder = new GameObject("Board").transform;

        cardPositions = new List<Vector2>();
        //horStep = 500;
        //vertStep = 400;
        horStep = 1.1f;
        vertStep = 1f;
        offset = horStep / 2;
        numCards = 7;
        numRows = 7;
        totalCards = (numCards * (numCards + 1)) / 2;

        for(int i = 0; i < numRows; i++)
        {
            for(int j = 0; j < numCards - i; j++) 
            {
                cardPositions.Add(new Vector2((horStep * j) + (offset*i), vertStep * i));
            }
        }
    }
    /*
    //privateNoid DealDeck
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
    */
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
