using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotUmben : MonoBehaviour
{
    PlayerUmben playerUmben;
    public bool afk = false;


    public void Initialize ()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //

        StartCoroutine (RoutineCheck(1f));
    }

    public IEnumerator RoutineCheck(float time)
    {
        yield return new WaitForSeconds ( time );
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //

        if (!GameObject.Find("Master").GetComponent<GameHandler> ().gameOver)
        {
            CheckStatus ();    
        }
    }

    void CheckStatus()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //

        playerUmben = gameObject.GetComponent<PlayerUmben> ();

        if (playerUmben.statusNow == PlayerUmben.Status.BUANG)
        {
            Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
            StartCoroutine(Buang ());
        }

        else if (playerUmben.statusNow == PlayerUmben.Status.AWAL)
        {
            Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
            //playerUmben.handlerUmben.selectedSymbol = playerUmben.pemain.layout.deck.GetChild(0).GetComponent<CardSc> ().symbol;
            playerUmben.Awal(playerUmben.pemain.layout.deck.GetChild(0).gameObject.name);
        }
    }

    IEnumerator Buang ()
    {
        yield return new WaitForSeconds(.3f);
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //

        Transform deck = playerUmben.pemain.layout.deck;
        bool found = false;

        for (int i = 0; i< deck.childCount; i++)
        {
            if (!found)
            {
                if (deck.GetChild(i).GetComponent<CardSc> ().symbol == playerUmben.handlerUmben.selectedSymbol)
                {
                    playerUmben.Buang(deck.GetChild(i).gameObject.name);

                    found = true;
                    break;
                }
            }
        }

        if (!found)
        {
            Minum();
            StartCoroutine(RoutineCheck(.8f));
        }
    }

    void Minum ()
    {
        if (playerUmben.handlerUmben.GM.deckMinum.transform.childCount > 0)
        {
            playerUmben.Ambil(playerUmben.handlerUmben.GM.deckMinum.transform.GetChild(0).gameObject.name);
        }
        else
        {
            playerUmben.ClickAmbilBuangan();
        }
        
    }
}
