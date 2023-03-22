using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Card : MonoBehaviour
{
    public int suit;                //Club 0 , diamond 1, heart 2, spade 3
    public int value;               //Value ace 1, 2-10, jack 11, queen 12, king 13
    public Sprite spriteFace;       //Sprite to hold the card face
    public Sprite spriteBack;       //Sprite to hold the card back
    public GameObject card;         //GameObject to contain card and perform game logic on
    public SpriteRenderer r;
    public bool clicked;

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
        clicked = false;

        //Init the card, add a sprite renderer and set it to not active.
        card = new GameObject(spriteFace.name);
        card.AddComponent<ClickManager>();
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