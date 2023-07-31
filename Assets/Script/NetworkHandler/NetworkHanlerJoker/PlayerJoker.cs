using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerJoker : MonoBehaviour
{
    public Pemain pemain;
    public HandlerJoker handlerJoker;
    public enum Status { BUANG, AMBIL, TUNGGU, SELESAI, HABIS }
    public Status statusNow;
    CardSc selectedCard;
    CardSc selectedCard2;
    bool update = true;
    Transform selectedDeck;
    Transform originSlctdDeck;

    void Start()
    {
        pemain = GetComponent<Pemain> ();
        handlerJoker = GameObject.Find("Master").GetComponent<GameHandler> ().handlerJoker;

        if (pemain.local)
        {
        originSlctdDeck = new GameObject("originPos").transform;
        }
    }

    public void UpdateStatus(Status newStatus)
    {
        if (!(statusNow == Status.HABIS))
        {
            statusNow = newStatus;

            if (handlerJoker != null)
            {
                if (pemain.local)
                {
                    handlerJoker.GM.nextButton.SetActive(statusNow == Status.SELESAI);
                }
            }

        }
    }

    private void FixedUpdate() 
    {
        if (update)
        {        
            for (int i = 0; i < pemain.layout.deck.childCount; i++)
            {
                pemain.layout.deck.GetChild(i).GetComponent<CardSc> ().Buka(pemain.local || handlerJoker.GM.gameOver);
            }
        }
    }

    public void ClickedCard(CardSc card) {
        //Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        Transform cardParent = card.transform.parent;
        
        switch (statusNow)
        {
            case Status.TUNGGU:
                break;
            case Status.AMBIL:

                if (cardParent.parent.GetComponentInParent<PlayerJoker> ().statusNow == Status.TUNGGU)
                {
                    if (ActionConfirmation(card)) 
                    {
                        handlerJoker.Call (card.gameObject.name, gameObject.name, statusNow);
                        UpdateStatus(Status.SELESAI);
                    }
                }
                break;
            
            case Status.BUANG:
            
                if (cardParent == pemain.layout.deck)
                {
                    if (ActionConfirmationBuang(card)) 
                    {
                        handlerJoker.Call (card.gameObject.name,selectedCard2.gameObject.name, gameObject.name, statusNow);
                    }
                }
                break;
            case Status.SELESAI:
            
                if (cardParent == pemain.layout.deck)
                {
                    if (ActionConfirmationBuang(card)) 
                    {
                        handlerJoker.Call (card.gameObject.name,selectedCard2.gameObject.name, gameObject.name, statusNow);
                        selectedCard2 = null;
                    }
                }
                break;
                
        }
    }

    public void OnClickNextMove()
    {
        if (statusNow == Status.SELESAI)
        {
            handlerJoker.Call ("fr", gameObject.name, statusNow);
        }
    }

    bool ActionConfirmationBuang(CardSc card) {
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
            if (selectedCard != card)
            {
                if (selectedCard.numID == card.numID)
                {
                    selectedCard.transform.localScale = Vector3.one * 1f;
                    card.transform.localScale = Vector3.one * 1f;
                    selectedCard2 = selectedCard;
                    selectedCard = null;
                    result = true;
                }
                else
                {
                    card.transform.localScale = Vector3.one * 1.2f;
                    selectedCard.transform.localScale = Vector3.one * 1f;
                    selectedCard = card;
                    result = false;
                }
            }
            else
            {
                
            }
        }

        return result;
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

    // public void AmbilTurn(bool state)
    // {
    //     Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
    //     //
    //     if (pemain.local)
    //     {
    //         if (!IsOver())
    //         {                
    //             Pemain pemainSebelumnya = PemainSebelumnyaMain ();
                
    //             if (pemainSebelumnya != pemain)
    //             {
    //                 if (state)
    //                 {
    //                     Debug.Log(pemainSebelumnya);

    //                     pemainSebelumnya.layout.deck.position = handlerJoker.GM.deckMinum.transform.position;
    //                     pemainSebelumnya.layout.center = true;
    //                 }
    //                 else
    //                 {
    //                     Debug.Log(pemainSebelumnya);
    //                     pemainSebelumnya.layout.deck.position = pemainSebelumnya.layout.initialPos.position;
    //                     pemainSebelumnya.GetComponent<PlayerJoker> ().UpdateStatus(Status.BUANG);
    //                     pemainSebelumnya.layout.center = false;
    //                 }
    //             }
    //         }
    //     }
    // }

    public void Ambil (string originPath)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        pemainSebelumnya().GetCard(GameObject.Find(originPath).transform, pemain.layout.deck);
        SetMidDeck(false);

        UpdateStatus(Status.SELESAI);
    }

    public void Buang (string card1, string card2)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        GameObject.Find(card1).GetComponent<CardSc> ().Buka (true);
        GameObject.Find(card2).GetComponent<CardSc> ().Buka (true);
        handlerJoker.GM.MoveObject(GameObject.Find(card1).transform, handlerJoker.GM.deckMinum.transform);
        handlerJoker.GM.MoveObject(GameObject.Find(card2).transform, handlerJoker.GM.deckMinum.transform);
    
        handlerJoker.DeckMinumCount();
    }
    

    public void NextMove()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //

        PlayerJoker p = pemainSelanjutnya();

        
        if (deckChild() < 1)
        {
            UpdateStatus (Status.HABIS); // this is fuckin shit
            p.NowMove(true);
        }
        else
        {
            UpdateStatus(Status.TUNGGU);
            p.NowMove(false);
        }
    }

    public void NowMove(bool isOver)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        if (isOver)
        {
            UpdateStatus(Status.SELESAI);
        }
        else
        {
            UpdateStatus(Status.AMBIL);
            SetMidDeck(true);
        }

        InitializeBot();
    }

    void SetMidDeck(bool mid)
    {
        if (pemain.local)
        {
            if (mid)
            {
                selectedDeck = pemainSebelumnya().pemain.layout.deck;
                originSlctdDeck.position = pemainSebelumnya().pemain.layout.deck.position;

                selectedDeck.transform.position = handlerJoker.GM.deckMinum.transform.position;
            }
            else
            {
                if (selectedDeck != null)
                {
                    selectedDeck.transform.position = originSlctdDeck.position;
                    
                    selectedDeck = null;
                }
            }
        }
        
    }

    public void GetCard(Transform card, Transform target)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        handlerJoker.GM.MoveObject(card, target);

        if (deckChild () < 1) {
            UpdateStatus(Status.HABIS);
        } else {
            UpdateStatus(Status.BUANG);
        }
    }

    void InitializeBot()
    {
        if (pemain.gameObject.TryGetComponent<BotJoker>(out BotJoker botSc))
        {
            botSc.count = 1;
            botSc.Initialize();
        }
    }


    public PlayerJoker pemainSebelumnya ()
    {
        PlayerJoker p = this;
        PlayerJoker[] pemainMain = handlerJoker.GetPemainMain();

        for (int i = 0 ; i < pemainMain.Length; i++)
        {
            if (pemainMain[i] == this)
            {
                int nextIndex = i + 1;
                if (nextIndex > pemainMain.Length - 1) {nextIndex = 0;}

                p = pemainMain[nextIndex];
            }
        }

        Debug.Log(p);

        return p;
    }

    public PlayerJoker pemainSelanjutnya ()
    {
        PlayerJoker p = this;
        PlayerJoker[] pemainMain = handlerJoker.GetPemainMain();

        for (int i = 0 ; i < pemainMain.Length; i++)
        {
            if (pemainMain[i] == this)
            {
                int nextIndex = i - 1;
                if (nextIndex < 0) {nextIndex = pemainMain.Length - 1;}

                p = pemainMain[nextIndex];
            }
        }

        Debug.Log(p);

        return p;
    }

    public int deckChild()
    {
        return pemain.layout.deck.childCount;
    }

}


