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
    public GameObject card;
    public GameObject drawButton;
    public GameObject endScreen;
    public bool drawCard = false;

    public List<GameObject> drawCards;               //The deck and subsequent draw pile for the game
    public List<GameObject> boardCards;              //An list of card objects that are in play on the game board
    public Stack<GameObject> discardCards;            //An list of card objects that are discarded
    public Stack<GameObject> spentCards;              //A list of card objects that are used and removed from play
    public Queue<GameObject> clickedCards;

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

    private int currentLayerOrder = 99;
    private int discardLayer = 100;
    private int spentLayer = 200;

    public void dealCard()
    {
        clickedCards.Clear();

        if (drawCards.Count > 0)
        {
            drawCards[0].GetComponent<Card>().FlipCard();
            drawCards[0].GetComponent<SpriteRenderer>().sortingOrder = discardLayer++;

            addToDiscard(drawCards[0]);
            drawCards.RemoveAt(0);

            discardCards.Peek().GetComponent<Card>().MoveObject(discardPosition);
        }
    }

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

    private void ShuffleDeck(ref List<GameObject> deck)
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

        discardCards = new Stack<GameObject>();
        spentCards = new Stack<GameObject>();
        clickedCards = new Queue<GameObject>();

        //Create the list of card positions based on the values specified above
        for(int i = 0; i < numRows; i++)
        {
            for(int j = 0; j < numCards - i; j++) 
            {
                cardPositions.Add(new Vector3((horStep * j) + (offset*i)-3, (vertStep * i)-4, 0));
            }
        }
    }

    private void dealBoard()
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
            return;
        }
        drawCards[cardforAnimation].GetComponent<Card>().MoveObject(cardPositions[cardforAnimation]);

        if (!(Mathf.Abs(drawCards[cardforAnimation].transform.position.x - cardPositions[cardforAnimation].x) > 0.1 || Mathf.Abs(drawCards[cardforAnimation].transform.position.y - cardPositions[cardforAnimation].y) > 0.1))
        {
            if (cardforAnimation < 7)
            {
                drawCards[cardforAnimation].GetComponent<Card>().FlipCard();
            }
            cardforAnimation++;
        }
    }

    private void changeGameState(int i)
    {
        previousState = currentState;
        currentState = i;
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

    private bool checkMatches()
    {
        foreach(GameObject firstCard in boardCards)
        {
            if(firstCard.GetComponent<SpriteRenderer>().sprite != firstCard.GetComponent<Card>().spriteBack)
            {
                foreach(GameObject secondCard in boardCards)
                {
                    if(firstCard.GetComponent<Card>().value + secondCard.GetComponent<Card>().value == 13 &&
                       secondCard.GetComponent<SpriteRenderer>().sprite != secondCard.GetComponent<Card>().spriteBack)
                    {
                        return true;
                    }
                }
            }
        }
        foreach (GameObject firstCard in boardCards)
        {
            if (firstCard.GetComponent<SpriteRenderer>().sprite != firstCard.GetComponent<Card>().spriteBack)
            {
                if (firstCard.GetComponent<Card>().value + discardCards.Peek().GetComponent<Card>().value == 13)
                {
                    return true;
                }
            }
        }
        return false;
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

            case 1:
                {

                    cardTotal = 0;

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

                                spentCards.Peek().GetComponent<Card>().MoveObject(spentPosition);
                                
                                clickedCards.Clear();
                                cardTotal = 0;

                                break;
                            }
                            else
                                cardTotal += card.GetComponent<Card>().value;
                        }

                        if (cardTotal == 13)
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

                                spentCards.Peek().GetComponent<Card>().MoveObject(spentPosition);
                            }
                            clickedCards.Clear();
                        }
                    }

                    if(drawCards.Count == 0 && !checkMatches())
                    {
                        endScreen.SetActive(true);
                    }

                    break;
                }

        }
    }
}
