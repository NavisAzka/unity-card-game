using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot41 : MonoBehaviour
{
    Player41 player41;
    public int count = 1;
    public bool afk = false;

    private void Start() 
    {

    }

    public void Initialize()
    {
        if (count > 0)
        {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
            StartCoroutine ( RoutineCheck (1f) );
            count -= 1;
        }
    }

    void CheckStatus()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        player41 = gameObject.GetComponent<Player41> ();
        player41.GetScore();

        if (player41.statusNow == Player41.Status.AMBIL)
        {
            Ambil ();
            StartCoroutine(RoutineCheck(1f));
        }

        else if (player41.statusNow == Player41.Status.BUANG)
        {
            Buang();
            StartCoroutine(RoutineCheck(1f));
        }

    }

    public IEnumerator RoutineCheck(float time)
    {
        yield return new WaitForSeconds ( time );
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        CheckStatus ();    
    }

    private void Ambil ()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        Debug.Log ( "AmbilKartu " + gameObject.name );

        if ( player41.pemain.layout.buangan.childCount > 0)
        {
            Debug.Log ( "AmbilKartu if " + gameObject.name );

            if ( player41.pemain.layout.buangan.GetChild(player41.pemain.layout.buangan.childCount - 1).GetComponent<CardSc>().symbol == player41.selectedSymbol)
            {
                if (player41.pemain.layout.buangan.GetChild(player41.pemain.layout.buangan.childCount - 1).GetComponent<CardSc>().value > NilaiTerkecil())
                {
                    //player41.handler41.Call (player41.pemain.layout.buangan.GetChild(player41.pemain.layout.buangan.childCount - 1).gameObject.name, gameObject.name, player41.statusNow);
                    player41.Ambil(player41.pemain.layout.buangan.GetChild(player41.pemain.layout.buangan.childCount - 1).gameObject.name);
                }
                else
                {
                    Minum();
                }
            }
            else
            {
                Minum();
            }
        }
        else
        {
            Minum();
        }
    }

    void Minum()
    {
        Transform deckMinum = player41.handler41.GM.deckMinum.transform;
        //player41.handler41.Call (deckMinum.GetChild(deckMinum.childCount - 1).gameObject.name, gameObject.name, player41.statusNow);
        player41.Ambil(deckMinum.GetChild(deckMinum.childCount - 1).gameObject.name);
    }

    void Buang()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        if (afk)
        {
            // player41.handler41.CallNutup (player41.pemain.layout.deck.GetChild(IndexKartuBuang()).gameObject.name, gameObject.name);
            player41.Nutup(player41.pemain.layout.deck.GetChild(IndexKartuBuang()).gameObject.name);
        }
        else
        {
            if (player41.GetScore() >= 39)
            {
                // Tutup kartu
                //player41.handler41.CallNutup (player41.pemain.layout.deck.GetChild(IndexKartuBuang()).gameObject.name, gameObject.name);
                player41.Nutup(player41.pemain.layout.deck.GetChild(IndexKartuBuang()).gameObject.name);
                
            }
            else
            {
                // player41.handler41.Call (player41.pemain.layout.deck.GetChild(IndexKartuBuang()).gameObject.name, gameObject.name, player41.statusNow);
                player41.Buang(player41.pemain.layout.deck.GetChild(IndexKartuBuang()).gameObject.name);
                player41.UpdateStatus (Player41.Status.TUNGGU);
            }
        }
    }

    private int IndexKartuBuang() // index (childPosition) dari card yang akan dibuang
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        int indexCard = 0;

        int highestValue = 0;
        int lowerValue = 11;
        bool simbolLengkap = true;

        for (int i = 0; i < player41.pemain.layout.deck.childCount; i++)
        {
            CardSc card = player41.pemain.layout.deck.GetChild(i).GetComponent<CardSc>();

            // jika symbol yang paling besar tidak sama dgn card yang di layout.deck
            if (card.symbol != player41.selectedSymbol)
            {
                // jika card memiliki nilai yang tinngi 
                if (card.value > highestValue)
                {
                    indexCard = i;
                    highestValue = card.value;
                    simbolLengkap = false;
                }
            }
            else
            {
                if (card.value < lowerValue)
                {
                    if (simbolLengkap)
                    {
                        indexCard = i;
                        lowerValue = card.value;
                    }
                }
            }
        }

        return indexCard;
    }

    private int NilaiTerkecil() // mencari nilai terkecil dari kartu pada deck
    {
        int nilai = 11;

        for (int i = 0; i < player41.pemain.layout.deck.childCount; i++)
        {
            if (player41.pemain.layout.deck.GetChild(i).GetComponent<CardSc> ().value < nilai)
            {
                nilai = player41.pemain.layout.deck.GetChild(i).GetComponent<CardSc>().value;
            }
        }

        return nilai;
    }



}
