using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DeckBuilder
{
    public DeckBuilder()
    {
        InitializeDeck();
    }

    public class Card
    {
        public int suit;            //Club 0 , diamond 1, heart 2, spade 3
        public int value;           //Value ace 1, 2-10, jack 11, queen 12, king 13
        public Sprite sprite;       //Sprite to hold the card face

        //Assignment Constructor
        public Card(int s, int v, Sprite sp)
        {
            suit = s;
            value = v;
            sprite = sp;
        }
    }

    public List<Card> deck;         //An array of card objects that will act as the deck for this game

    public void InitializeDeck()
    {
        deck = new List<Card>();    //Init the deck of card objects

        string[] values = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13" };

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

        Shuffle();
    }

    //A method to shuffle the deck which is passed in by reference and shuffles in place.
    public void Shuffle()
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
}
