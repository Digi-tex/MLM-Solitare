using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Networking.UnityWebRequest;


public class DeckBuilder
{

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

    public void CheckClicked(ref DeckBuilder clickedCards)
    {
        foreach (Card card in clickedCards.deck)
        {
            if (card.clicked)
            {
                clickedCards.deck.Add(card);
            }
            if (clickedCards.deck.Count > 2)
            {
                clickedCards.deck.RemoveAt(0);
            }
        }

    }
}
