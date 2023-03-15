using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Networking.UnityWebRequest;


public class DeckBuilder
{
    public class Card
    {
        public int suit;                //Club 0 , diamond 1, heart 2, spade 3
        public int value;               //Value ace 1, 2-10, jack 11, queen 12, king 13
        public Sprite spriteFace;       //Sprite to hold the card face
        public Sprite spriteBack;       //Sprite to hold the card back
        public GameObject card;         //GameObject to contain card and perform game logic on
        public SpriteRenderer r;
        
        public BoxCollider2D col;
        public List<Collider2D> colResults;
        public ContactFilter2D colFilter;


        //Assignment Constructor
        public Card(int s, int v, Sprite sp)
        {
            suit = s;
            value = v;
            spriteFace = sp;
            spriteBack = Resources.Load<Sprite>("Sprites/Standard 52 Cards/solitaire/individuals/card back/card_back");

            //Init the card, add a sprite renderer and set it to not active.
            card = new GameObject(spriteFace.name);
            r = card.AddComponent<SpriteRenderer>();
            r.sprite = spriteBack;
            card.transform.position = new Vector2(0, -6.0f);

            col = card.AddComponent<BoxCollider2D>();
            col.gameObject.layer = 3;

            colResults = new List<Collider2D>();
            colFilter = new ContactFilter2D();
            colFilter.layerMask = 3;

            //card.SetActive(false);
        }
    }


    public List<Card> deck;             //An array of card objects that will act as the deck for this game
    public DeckBuilder()
    {
        deck = new List<Card>();        //Init the deck of card objects
    }


    public void Build52CardDeck()
    {
        string[] values = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13" }; //Just in case we had to use symbols, this was an array to hold the card values

        //Loop through and create the deck of cards. 
        //Depending on the suit denoted by the first for loop, we grab the card faces from different folders
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                switch (i)
                {
                    case 0:
                        Console.WriteLine("Sprites/Standard 52 Cards/solitaire/individuals/club/" + values[j] + "_club");
                        deck.Add(new Card(i, j, Resources.Load<Sprite>("Sprites/Standard 52 Cards/solitaire/individuals/club/" + values[j] + "_club")));
                        break;
                    case 1:
                        deck.Add(new Card(i, j, Resources.Load<Sprite>("Sprites/Standard 52 Cards/solitaire/individuals/diamond/" + values[j] + "_diamond")));
                        break;
                    case 2:
                        deck.Add(new Card(i, j, Resources.Load<Sprite>("Sprites/Standard 52 Cards/solitaire/individuals/heart/" + values[j] + "_heart")));
                        break;
                    case 3:
                        deck.Add(new Card(i, j, Resources.Load<Sprite>("Sprites/Standard 52 Cards/solitaire/individuals/spade/" + values[j] + "_spade")));
                        break;
                }
            }
        }

        ShuffleDeck();  //Shuffle the deck once it has been created.
    }

    //A method to shuffle the deck which is passed in by reference and shuffles in place.
    public void ShuffleDeck()
    {
        var rng = new System.Random();
        int n = deck.Count;
        while (n > 1)
        {
            int k = rng.Next(n--);
            Card temp = deck[n];
            deck[n] = deck[k];
            deck[k] = temp;
        }
    }

    public void AddCard(Card card)
    {
        deck.Add(card);
    }

    public void RemoveCard()
    {
        deck.RemoveAt(0);
    }

    public void FlipCard(Card card)
    {
        if (card.r.sprite.name != card.spriteFace.name)
        {
            card.r.sprite = card.spriteFace;
        }
        else
        {
            card.r.sprite = card.spriteBack;
        }
    }

    public void CheckCollision()
    {
        foreach(Card card in deck)
        {
            if (card.col.OverlapCollider(card.colFilter, card.colResults) > 2)
            {
                if (card != deck[deck.Count - 1])
                    card.r.sprite = card.spriteBack;
                else if(card.col.OverlapCollider(card.colFilter, card.colResults) == 2)
                    card.r.sprite = card.spriteFace;           
            }
            else
            {
                card.r.sprite = card.spriteFace;
            }

        }

    }
}
