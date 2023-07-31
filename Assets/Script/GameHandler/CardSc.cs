using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSc : MonoBehaviour
{
    Deck deck;
    public Image symSprt, backSprt;
    public Text valueText;

    public enum Symbol {spade, club, diamond, heart};
    public Symbol symbol;
    public int numID, value, id;
    
    void Start()
    {
        Buka(false);


        //id = transform.GetSiblingIndex();
        //deck = transform.parent.GetComponent<Deck> ();
        //GetComponent<SpriteRenderer> ().size = deck.cardSize;
    }

    public void Buka (bool state)
    {
        //
        transform.GetChild(0).gameObject.SetActive(state);
    }

    


}
