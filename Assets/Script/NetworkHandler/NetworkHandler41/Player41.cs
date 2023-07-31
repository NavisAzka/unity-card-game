using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class Player41 : MonoBehaviourPunCallbacks
{
    public Pemain pemain;
    public Handler41 handler41;
    public enum Status { BUANG, AMBIL, TUNGGU }
    public Status statusNow;
    CardSc selectedCard;
    bool update = true;
    public CardSc.Symbol selectedSymbol;

    private void Start() 
    {
        pemain = GetComponent<Pemain> ();
        handler41 = GameObject.Find("Master").GetComponent<GameHandler> ().handler41;
    }

    public void UpdateStatus(Status newStatus)
    {
        statusNow = newStatus;
    }

    private void FixedUpdate() 
    {
        if (update)
        {        
            for (int i = 0; i < pemain.layout.buangan.childCount; i++)
            {
                pemain.layout.buangan.GetChild(i).GetComponent<CardSc> ().Buka(true);
            }

            for (int i = 0; i < pemain.layout.deck.childCount; i++)
            {
                pemain.layout.deck.GetChild(i).GetComponent<CardSc> ().Buka(pemain.local || handler41.GM.gameOver);
            }

            if (handler41.GM.gameOver)
            {
                handler41.GM.deckMinum.transform.GetChild(handler41.GM.deckMinum.transform.childCount - 1).GetComponent<CardSc> ().Buka (true);
                
                update = false;
            }   
        }
    }

    public void ChangeName ()
    {
        pemain.layout.nameText.text = pemain.nickname + " [ " + GetScore().ToString() + " ] ";
    }
    
    public void HoldedCard(CardSc card)
    {
        //Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        Transform cardParent = card.transform.parent;
        switch (statusNow)
        {
            case Status.TUNGGU:
                break;
            case Status.AMBIL:
                break;
            
            case Status.BUANG:

                if (cardParent == pemain.layout.deck)
                {
                    if (ActionConfirmation(card)) 
                    {
                        handler41.CallNutup (card.gameObject.name, gameObject.name);
                    }
                }
                break;
            
        }
    }

    public void SemiHold (CardSc card)
    {
        //Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        Transform cardParent = card.transform.parent;
        switch (statusNow)
        {
            case Status.TUNGGU:
                break;
            case Status.AMBIL:
                break;
            
            case Status.BUANG:

                if (cardParent == pemain.layout.deck)
                {
                    card.Buka(false);
                }
                break;
            
        }
    }
    public void ClickedCard(CardSc card)
    {
        //Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        Transform cardParent = card.transform.parent;
        switch (statusNow)
        {
            case Status.TUNGGU:
                break;
            case Status.AMBIL:
                
                if (card.transform.GetSiblingIndex() == cardParent.childCount - 1 && (cardParent == pemain.layout.buangan || cardParent == handler41.GM.deckMinum.transform))
                {
                    if (ActionConfirmation(card)) 
                    {
                        handler41.Call (card.gameObject.name, gameObject.name, statusNow);
                        UpdateStatus(Status.BUANG);
                    }
                }    
                break;
            
            case Status.BUANG:

                if (cardParent == pemain.layout.deck)
                {
                    if (ActionConfirmation(card)) 
                    {
                        handler41.Call (card.gameObject.name, gameObject.name, statusNow);
                        UpdateStatus(Status.TUNGGU);
                    }
                }
                break;
            
        }
    }

    bool ActionConfirmation(CardSc card)
    {
        bool result = false;

        if (selectedCard == null)
        {
            // change scale of card after selected
            card.transform.localScale = Vector3.one * 1.2f;
            selectedCard = card;
            result = false;
        }
        else
        {
            if (selectedCard == card)
            {
                // change scale of card after deselected
                selectedCard.transform.localScale = Vector3.one * 1f;
                selectedCard = null;
                result = true;
            }
            else
            {
                // change scale of card after deselected
                card.transform.localScale = Vector3.one * 1.2f;
                selectedCard.transform.localScale = Vector3.one * 1f;
                selectedCard = card;
                result = false;
            }
        }

        return result;
    }

    public void Ambil (string originPath)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        handler41.GM.MoveObject(GameObject.Find(originPath).transform, pemain.layout.deck);
        UpdateStatus(Status.BUANG);
    }

    public void Buang(string originPath)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        handler41.GM.MoveObject(GameObject.Find(originPath).transform, pemain.pemainSelanjutnya().layout.buangan, pemain);
        
    
        Debug.Log (GetScore());
    }

    public void Nutup(string originPath)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        
        handler41.GM.MoveObject(GameObject.Find(originPath).transform, pemain.pemainSelanjutnya().layout.buangan);
        UpdateStatus(Status.TUNGGU);
        
    
        Debug.Log (GetScore());
        handler41.GameOver();
    }

    public void NextMove()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        UpdateStatus(Status.TUNGGU);
        pemain.pemainSelanjutnya().GetComponent<Player41> ().UpdateStatus(Status.AMBIL);

        if (pemain.pemainSelanjutnya().gameObject.TryGetComponent<Bot41>(out Bot41 botSc))
        {
            botSc.count = 1;
            botSc.Initialize();
        }
    }


    [SerializeField] int[] cardsScore;

    public int GetScore()
    {
        //Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        cardsScore = new int[] {0,0,0,0};

        int indexPilihan = 0;
        int result = 0;

        Transform deck =  pemain.layout.deck.transform;
        
        // input all card velue to card score array
        for (int i = 0; i < deck.childCount; i++)
        {
            CardSc card = deck.GetChild(i).GetComponent<CardSc> ();
            
            if (card.symbol == CardSc.Symbol.spade)
            { cardsScore[0] += card.value;}

            if (card.symbol == CardSc.Symbol.club)
            { cardsScore[1] += card.value;}

            if (card.symbol == CardSc.Symbol.diamond)
            { cardsScore[2] += card.value;}

            if (card.symbol == CardSc.Symbol.heart)
            { cardsScore[3] += card.value;}
        }
        // find the highest value
        int highestValue = cardsScore[0];
        for (int i = 0; i < cardsScore.Length; i++)
        {
            if (cardsScore[i] > highestValue)
            {
                highestValue = cardsScore[i];
                indexPilihan = i;
            }
        }
        
        result = highestValue;

        // substact result with another card value
        for (int i = 0; i < cardsScore.Length; i++)
        {
            if (i != indexPilihan)
            {
                result -= cardsScore[i];
            }
        }

        if (indexPilihan == 0) { selectedSymbol = CardSc.Symbol.spade; }
        if (indexPilihan == 1) { selectedSymbol = CardSc.Symbol.club; }
        if (indexPilihan == 2) { selectedSymbol = CardSc.Symbol.diamond; }
        if (indexPilihan == 3) { selectedSymbol = CardSc.Symbol.heart; }

        return result;
    }
}
