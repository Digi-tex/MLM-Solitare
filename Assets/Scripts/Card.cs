using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Card : MonoBehaviour
{
    public int suit;                //Club 0 , diamond 1, heart 2, spade 3
    public int value;               //Value ace 1, 2-10, jack 11, queen 12, king 13
    public Sprite spriteFace;       //Sprite to hold the card face
    public Sprite spriteBack;       //Sprite to hold the card back

    public ContactFilter2D colFilter;

    public Collider2D[] colResults = new Collider2D[100];

    public IEnumerator MoveObject(Vector3 end, Vector3 vel, float smoothTime, float speed)
    {
        while (this.transform.position != end)
        {
            this.transform.position = Vector3.SmoothDamp(this.transform.position, end, ref vel, smoothTime, speed);
            yield return 0;
        }
    }

    public void FlipCard()
    {
        if (this.GetComponent<SpriteRenderer>().sprite.name != this.GetComponent<Card>().spriteFace.name)
        {
            this.GetComponent<SpriteRenderer>().sprite = this.GetComponent<Card>().spriteFace;
        }
        else
        {
            this.GetComponent<SpriteRenderer>().sprite = this.GetComponent<Card>().spriteBack;
        }
    }

    void Start()
    {
        colFilter = new ContactFilter2D();
        colFilter.layerMask = 3;
    }

    // Update is called once per frame
    void Update()
    {

    }
}