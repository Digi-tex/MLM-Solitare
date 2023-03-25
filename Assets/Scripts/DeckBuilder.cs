using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Networking.UnityWebRequest;


public class DeckBuilder
{
    public GameObject cardObject;

    public List<GameObject> deck;             //An array of card objects that will act as the deck for this game
    public DeckBuilder()
    {
        deck = new List<GameObject>();        //Init the deck of card objects
    }

}
/*





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
*/