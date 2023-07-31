using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUmben : MonoBehaviour
{
    public Pemain pemain;
    public HandlerUmben handlerUmben;
    public enum Status { BUANG, TUNGGU, AWAL, AMBIL, HABIS }
    public Status statusNow;
    CardSc selectedCard;
    bool update = true;

    private void Start() 
    {
        pemain = GetComponent<Pemain> ();
        handlerUmben = GameObject.Find("Master").GetComponent<GameHandler> ().handlerUmben;
    }

    public void UpdateStatus(Status newStatus)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        statusNow = newStatus;
    }

    private void FixedUpdate() 
    {
        if (update)
        {        
            
            for (int i = 0; i < pemain.layout.deck.childCount; i++)
            {
                pemain.layout.deck.GetChild(i).GetComponent<CardSc> ().Buka(pemain.local || handlerUmben.GM.gameOver);
            }
            

            for (int i = 0; i < pemain.layout.buangan.childCount; i++)
            {
                pemain.layout.buangan.GetChild(i).GetComponent<CardSc> ().Buka(true);
            }
        }
    }

    public void ClickedCard(CardSc card)
    {
        //Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        Transform cardParent = card.transform.parent;

        if (statusNow != Status.TUNGGU)
        {
            switch (statusNow)
            {
                
                case Status.BUANG:

                    if (cardParent == pemain.layout.deck)
                    {
                        if (ActionConfirmationBuang(card)) 
                        {
                            handlerUmben.Call (card.gameObject.name, gameObject.name, PlayerUmben.Status.BUANG);
                        }
                    }

                    if (cardParent == handlerUmben.GM.deckMinum.transform)
                    {
                        if (ActionConfirmation(card)) 
                        {
                            handlerUmben.CallAmbil (card.gameObject.name, gameObject.name, PlayerUmben.Status.AMBIL);
                        }
                    } 

                    
                    break;
                
                case Status.AWAL:
                    if (cardParent == pemain.layout.deck)
                    {
                        if (ActionConfirmation(card)) 
                        {
                            handlerUmben.Call (card.gameObject.name, gameObject.name, PlayerUmben.Status.AWAL);
                            
                        }
                    }
                    break;
            }

        } 
        else
        {
            if (selectedCard != null)
            {
                selectedCard.transform.localScale = Vector3.one * 1f;
                selectedCard = null;
            }
        }
    }

    // using button from ui
    public void ClickAmbilBuangan()
    {
        Stack<string> cards = new Stack<string>();
        if (statusNow == Status.BUANG)
        {
            for (int i = 0; i < handlerUmben.GM.PemainList.Length; i++)
        {
            if (handlerUmben.GM.PemainList[i].layout.buangan.childCount > 0)
            {
                cards.Push(handlerUmben.GM.PemainList[i].layout.buangan.GetChild(0).gameObject.name);
            }
        }
        handlerUmben.CallAmbilBuangan (cards.ToArray(), gameObject.name, PlayerUmben.Status.BUANG);
        }
    }

    public void AmbilBuangan()
    {
        Stack<string> cards = new Stack<string>();
        if (statusNow == Status.BUANG)
        {
            for (int i = 0; i < handlerUmben.GM.PemainList.Length; i++)
            {
                if (handlerUmben.GM.PemainList[i].layout.buangan.childCount > 0)
                {
                    cards.Push(handlerUmben.GM.PemainList[i].layout.buangan.GetChild(0).gameObject.name);
                }
            }
        handlerUmben.CallAmbilBuangan (cards.ToArray(), gameObject.name, PlayerUmben.Status.BUANG);
        AmbilBuangan(cards.ToArray());
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

    bool ActionConfirmationBuang(CardSc card)
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
                if (card.symbol == handlerUmben.selectedSymbol)
                {
                    // change scale of card after deselected
                    selectedCard.transform.localScale = Vector3.one * 1f;
                    selectedCard = null;
                    result = true;
                }
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

    public void NowMove()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //

        UpdateStatus(Status.BUANG);
        Initialize();
    }

    public void Initialize()
    {
        if (pemain.gameObject.TryGetComponent<BotUmben>(out BotUmben botSc))
        {
            botSc.Initialize();
        }
    }

    public void Awal (string originPath)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //

        handlerUmben.selectedSymbol = GameObject.Find(originPath).GetComponent<CardSc> ().symbol;
        handlerUmben.GM.MoveObject(GameObject.Find(originPath).transform, pemain.layout.buangan);
        handlerUmben.BuangQueue();
        
        if (pemain.layout.deck.childCount < 1)
        {
            handlerUmben.GetSisaPemainMain ();
        }
        
        UpdateStatus(Status.TUNGGU);
    }

    public void Ambil (string originPath)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //

        handlerUmben.GM.MoveObject(GameObject.Find(originPath).transform, pemain.layout.deck);
    }    

    public void Buang (string originPath)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //

        handlerUmben.GM.MoveObject(GameObject.Find(originPath).transform, pemain.layout.buangan);
        handlerUmben.BuangQueue();

        if (pemain.layout.deck.childCount < 1)
        {
            handlerUmben.GetSisaPemainMain ();
        }
        
        UpdateStatus(Status.TUNGGU);

    }

    public void AmbilBuangan(string[] originPath)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //

        handlerUmben.BuangAllQueue();

        UpdateStatus(Status.TUNGGU);
        StartCoroutine(TimerForAmbilBuangan(originPath));
    }

    IEnumerator TimerForAmbilBuangan(string[] originPath)
    {
        yield return new WaitForSeconds (.8f);

        
        for (int i = 0; i < originPath.Length; i++)
        {
            handlerUmben.GM.MoveObject(GameObject.Find(originPath[i]).transform, pemain.layout.deck);
        }

    }
}
