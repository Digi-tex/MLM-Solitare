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
    public GameObject card;                     //The card prefab
    public GameObject drawButton;               //The button used to draw cards from the deck 
    public GameObject endScreen;                //The end screen UI
    public GameObject winScreen;                //The win screen UI

    public AudioClip flipCard;
    public AudioClip matchSuccess;

    public List<GameObject> drawCards;          //The deck and subsequent draw pile for the game
    public List<GameObject> boardCards;         //An list of card objects that are in play on the game board
    public Stack<GameObject> discardCards;      //An stack of card objects that are discarded
    public Stack<GameObject> spentCards;        //A stack of card objects that are used and removed from play
    public Queue<GameObject> clickedCards;      //A queue to hold all the cards that have been clicked by the user

    public int cardTotal;                       //Used to sum the clicked card to determine if they add up to 13

    private List<Vector3> cardPositions;        //A list to hold the Vector2 locations of where the cards will be on the board
    private Vector3 deckPosition;               //Location of the deck
    private Vector3 discardPosition;            //Location of the discard pile
    private Vector3 spentPosition;              //Location of the spent pile

    private float horStep;                      //Horizontal step size for locating the cards in a pyramid pattern
    private float vertStep;                     //Vertical step size for locating the cards in a pyramid pattern
    private float offset;                       //Distance each row is offset for the next level of the pyramid
    private int numCards;                       //Number of cards for the base level of the pyramid
    private int numRows;                        //Number of rows in the pyramid
    private int totalCards;                     //Total cards in the pyramid

    private int currentState = 0;               //Keeps the current state in the update loop

    private int cardforAnimation;               //A counter used to deal out the deck to the board

    private int currentLayerOrder = 99;         //The starting layer used for the draw deck and board cards
    private int discardLayer = 100;             //The starting layer used for the discard pile
    private int spentLayer = 200;               //The starting layer used for the spent pile



    //Deal a single card from the draw pile to the discard pile
    public void dealCard()
    {
        clickedCards.Clear();

        if (drawCards.Count > 0)
        {
            //Flip the card and change its sorting order
            drawCards[0].GetComponent<Card>().FlipCard();
            drawCards[0].GetComponent<SpriteRenderer>().sortingOrder = discardLayer++;

            //Remove the card from the draw pile and add it to the discard
            addToDiscard(drawCards[0]);
            drawCards.RemoveAt(0);

            //Move the card physically to the discard location
            discardCards.Peek().GetComponent<Card>().MoveObject(discardPosition);
        }
    }



    //Build a 52 card deck inside the drawCards list with each card having a unique suit and value
    private void Build52CardDeck()
    {
        //Just in case we had to use symbols, this was an array to hold the card values
        string[] values = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13" };

        //Loop through and create the deck of cards. 
        //Depending on the suit denoted by the first for loop, we grab the card faces from different folders
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                //Instantiate the card game object, set the camera so the card can add itself to the lists of the game manager script
                //Set the suit and value
                GameObject newCard = Instantiate(card, deckPosition, Quaternion.identity);
                newCard.GetComponent<Card>().mainCamera = this.gameObject;
                newCard.GetComponent<Card>().suit = i;
                newCard.GetComponent<Card>().value = j + 1;

                //Depending on suit, choose the sprite from the correct folder
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

                //Set the sprite on display for the card as the back (face down),
                //set the name to the name of the face-up sprite, set the collision layer, add the card to the draw pile
                newCard.GetComponent<SpriteRenderer>().sprite = newCard.GetComponent<Card>().spriteBack;
                newCard.name = newCard.GetComponent<Card>().spriteFace.name;
                newCard.layer = 3;
                drawCards.Add(newCard);

            }
        }
    }



    //Shuffle a deck of cards in a random order
    private void ShuffleDeck(ref List<GameObject> deck)
    {
        //Create a new random variable
        var rng = new System.Random();
        int n = deck.Count;
        
        //While we progress through the list, starting at the end and incrementing towards the beginning
        //Select a random value between 0 and n and swap locations with the n element
        while (n > 1)
        {
            int k = rng.Next(n--);
            GameObject temp = deck[n];
            deck[n] = deck[k];
            deck[k] = temp;
        }

        //With the deck shuffled, set the correct layer order for each card and disable the collider
        foreach(GameObject card in deck)
        {
            card.GetComponent<SpriteRenderer>().sortingOrder = currentLayerOrder -= 1;
            card.GetComponent<BoxCollider2D>().enabled = false;
        }    
    }



    //Setup the board variables, locations for cards, and add these locations to an easy to use list
    private void initializeBoard()
    {
        
        deckPosition = new Vector3(-5.0f, 2.0f, 0);     //Specify location of deck
        discardPosition = new Vector3(-3.5f, 2.0f, 0);  //Specify location of discard pile
        spentPosition = new Vector3(5.0f, 2.0f, 0);     //Specify location of spent pile  
        drawButton.transform.position = deckPosition;   //Set the draw button to the location of the draw pile    

        cardPositions = new List<Vector3>();            //Create a list that holds all of the positions of where boardCards can be played
        horStep = 1.1f;                                 //The horizontal step when laying out cards
        vertStep = 1f;                                  //The vertical step when laying out cards
        offset = horStep / 2;                           //The offset for the triangle when we move to the next row
        numCards = 7;                                   //Number of cards at the bottom row
        numRows = 7;                                    //Number of rows of the pyramid
        totalCards = (numCards * (numCards + 1)) / 2;   //Calculating the total cards in the pyramid

        //Initialize the various card piles/lists
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



    //Deal the cards from the draw pile onto the board in the positions created for the pyramid in initializeBoard()
    private void dealBoard()
    {
        //Move the cards physically from the draw pile location to the specific location in the pyramid
        drawCards[cardforAnimation].GetComponent<Card>().MoveObject(cardPositions[cardforAnimation]);

        //If the card has made it to the position in the pyramid, flip the card if its of the bottom row, and then move to the next card
        if (!(Mathf.Abs(drawCards[cardforAnimation].transform.position.x - cardPositions[cardforAnimation].x) > 0.1 || Mathf.Abs(drawCards[cardforAnimation].transform.position.y - cardPositions[cardforAnimation].y) > 0.1))
        {
            if (cardforAnimation < 7)
            {
                drawCards[cardforAnimation].GetComponent<Card>().FlipCard();
            }
            cardforAnimation++;
            this.GetComponent<AudioSource>().PlayOneShot(flipCard);
        }

        //If we have moved all of the cards in the pyramid, change the game state
        //Transfer the cards into the board pile from the draw pile
        if (cardforAnimation >= totalCards)
        {
            cardforAnimation = 0;
            changeGameState(1);

            //Move the card out of the draw pile, turn on collision, and put it into the board pile
            for (int i = 0; i < totalCards; i++)
            {
                GameObject temp = drawCards[0];
                temp.GetComponent<Card>().inPyramid = true;
                temp.GetComponent<BoxCollider2D>().enabled = true;
                boardCards.Add(temp);
                drawCards.RemoveAt(0);
            }
        }
    }



    //Change the gamestate - this is silly and used to be from previous code that did more here. Leaving it in for ease of coding
    private void changeGameState(int i)
    {
        currentState = i;
    }



    //Adds a card from to the discard pile and sets the collision accordingly
    //Only the top card of the pile is to have collision to assist with click registering
    private void addToDiscard(GameObject card)
    {
        //If the discard pile isn't empty, set the previously top card to have no collision
        //Set the new top card to have collision instead
        if(discardCards.Count != 0)
        {
            discardCards.Peek().GetComponent<BoxCollider2D>().enabled = false;
            discardCards.Push(card);
            discardCards.Peek().GetComponent<BoxCollider2D>().enabled = true;
        }
        //If the pile is empty, just add the card and enable collision
        else
        {
            discardCards.Push(card);
            discardCards.Peek().GetComponent<BoxCollider2D>().enabled = true;
        }
    }



    //Removes the card from the discard and sets the collision accordingly.
    //Only the top card of the pile is to have collision to assist with click registering
    private GameObject removeFromDiscard()
    {
        //If the discard pile isn't empty, remove the card and modify collision
        if (discardCards.Count != 0)
        {
            GameObject tempCard = discardCards.Pop();
            tempCard.GetComponent<BoxCollider2D>().enabled = false;

            //If after removing the card, there are still cards in the discard, set the top card to have collision
            if(discardCards.Count != 0)
                discardCards.Peek().GetComponent<BoxCollider2D>().enabled = true;

            //Return the card removed from the discard pile
            return tempCard;
        }
        //If no cards are in the discard, return nothing
        else
        {
            return null;
        }
    }



    //Check to see if any of the cards currently displayed to the player can form a match to 13
    //AKA the player still has matches to make and has not won or lost yet
    private bool checkMatches()
    {
        //For each card in board cards, check against the other cards and return true if any sum to 13
        //This method technically sums cards with itself but thats okay as no card summed with itself will add to 13
        foreach(GameObject firstCard in boardCards)
        {
            //Only check cards if they are face-up
            if(firstCard.GetComponent<SpriteRenderer>().sprite != firstCard.GetComponent<Card>().spriteBack)
            {
                //Compare the selected card to each card in the pile 
                foreach(GameObject secondCard in boardCards)
                {
                    if (secondCard.GetComponent<SpriteRenderer>().sprite != secondCard.GetComponent<Card>().spriteBack)
                    {
                        //If a card in the boardCards pile is a king, return true
                        if (secondCard.GetComponent<Card>().value == 13)
                            return true;

                        //If the cards equal 13 and the second card is face-up, return true
                        if (firstCard.GetComponent<Card>().value + secondCard.GetComponent<Card>().value == 13)
                        {
                            return true;
                        }
                    }
                }
            }
        }

        //Compare the top card in the discard pile, if there are any, against the board pile to see if there are any matches
        if (discardCards.Count > 0)
        {
            //If the top discardCard is a king, return true
            if(discardCards.Peek().GetComponent<Card>().value == 13)
                return true;

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
        }

        //Return false as the default
        return false;
    }


    //Start the game
    void Start()
    {
        initializeBoard();
        Build52CardDeck();
        ShuffleDeck(ref drawCards);
    }



    // either deal the pyramid or play the game based on the state selected
    void Update()
    {
        switch (currentState)
        {
            //Deal the pyramid onto the board
            case 0:
                {
                    dealBoard();
                    break;
                }
            
            //Play the game
            case 1:
                {
                    //Set cardTotal to zero each time we start a new loop
                    cardTotal = 0;

                    //If there cards that have been clicked, sum them and check for a total of 13 or if one of the cards (a king) equals 13
                    if (clickedCards.Count != 0)
                    {
                        //For each card in clicked cards, add to the sum
                        foreach (GameObject card in clickedCards)
                        {
                            //Check to see if the card is a king, if so, turn off collision and move the card to the spent pile logically and physically
                            if (card.GetComponent<Card>().value == 13)
                            {
                                this.GetComponent<AudioSource>().PlayOneShot(matchSuccess);
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
                                
                                //clear the clicked cards list and reset the summing value
                                clickedCards.Clear();
                                cardTotal = 0;

                                break;
                            }
                            //If it doesn't equal 13, add it to the summed value
                            else
                                cardTotal += card.GetComponent<Card>().value;
                        }

                        //If the sum of the clicked cards equals 13, move both cards to the spent pile both logically and physically
                        if (cardTotal == 13)
                        {
                            this.GetComponent<AudioSource>().PlayOneShot(matchSuccess);

                            //For each card in clicked cards, turn off collision and move the card
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

                            //Clear the clicked cards list
                            clickedCards.Clear();
                            cardTotal = 0;
                        }
                    }

                    //Check to see if the game has been either lost or won
                    //To lose, there must be no matches and the draw pile must be empty but there must be cards in the pyramid
                    if(drawCards.Count == 0 && 
                       !checkMatches() && 
                       boardCards.Count != 0)
                    {
                        endScreen.SetActive(true);
                    }
                    //To win, the pyramid must be empty
                    else if(boardCards.Count == 0)
                    {
                        winScreen.SetActive(true);
                    }

                    break;
                }

        }
    }
}
