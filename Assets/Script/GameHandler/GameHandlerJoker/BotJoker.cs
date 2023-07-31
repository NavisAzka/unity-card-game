using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotJoker : MonoBehaviour
{
    PlayerJoker playerJoker;
    public int count = 1;
    public bool afk = false;
    bool evaluate = false;

    private void Start() 
    {

    }

    public void Initialize()
    {
        
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
            StartCoroutine ( RoutineCheck (1f) );
            count -= 1;
        
    }

    void CheckStatus()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        playerJoker = gameObject.GetComponent<PlayerJoker> ();

        if (playerJoker.statusNow == PlayerJoker.Status.AMBIL)
        {
            Ambil();
            evaluate = false;
            StartCoroutine(RoutineCheck(0.5f));
        }

        else if (playerJoker.statusNow == PlayerJoker.Status.BUANG)
        {
            if (!evaluate)
            {
                // Buang
                StartCoroutine(Buang());
            }
        }

        else if (playerJoker.statusNow == PlayerJoker.Status.SELESAI)
        {
            if (!evaluate)
            {
                // Buang
                StartCoroutine(Buang());
            }
            else
            {
                // Next Move
                Debug.Log("NEXT MOVE BOT");
                playerJoker.NextMove();
            }
        }

        else if (playerJoker.statusNow == PlayerJoker.Status.HABIS)
        {
            
            Debug.Log("NEXT MOVE BOT");
            playerJoker.NextMove();
            
        }

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

    IEnumerator Buang()
    {
        yield return new WaitForSeconds(.3f);
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > Buang > " + gameObject.name);
        //
        Transform deck = playerJoker.pemain.layout.deck;
        bool found = false;

        if (deck.childCount < 1)
        {
            evaluate = true;
            StartCoroutine(RoutineCheck(.5f));
        }
        else
        {
            for (int i = 0; i < deck.childCount; i++ )
            {
                yield return new WaitForEndOfFrame();
                if (!found)
                {
                    for (int j = 0; j < deck.childCount; j++ )
                    {   
                        yield return new WaitForEndOfFrame();

                        if (deck.GetChild(j) != deck.GetChild(i))
                        {
                            if (deck.GetChild(j).GetComponent<CardSc> ().numID == deck.GetChild(i).GetComponent<CardSc> ().numID)
                            {
                                yield return new WaitForEndOfFrame();
                                // Buang dari player joker
                                playerJoker.Buang (deck.GetChild(j).gameObject.name, deck.GetChild(i).gameObject.name);
                                
                                found = true;
                                
                                break;
                            }
                        }
                        
                        yield return new WaitForEndOfFrame();
                        
                        if (j == deck.childCount - 1 && i == deck.childCount - 1) 
                        {
                            Debug.Log("EVALUATE TRUE <> " + j.ToString() + "-" + i.ToString());
                            // end of loop
                            // do next
                            evaluate = true;
                            StartCoroutine(RoutineCheck(.5f));
                        }
                    }
                }
                
                if (found)
                {
                    yield return new WaitForEndOfFrame();
                    
                    StartCoroutine(Buang());
                    break;
                }
            }
        }
    }

    void Ambil ()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > Buang > " + gameObject.name);
        //
        PlayerJoker[] pemains = playerJoker.handlerJoker.GetPemainMain();

        for (int i = 0; i < pemains.Length; i++)
        {
            if (pemains[i].statusNow == PlayerJoker.Status.TUNGGU)
            {
                playerJoker.Ambil(pemains[i].pemain.layout.deck.GetChild(0).gameObject.name);
            }
        }
    }
}
