using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Handler41 : MonoBehaviourPunCallbacks
{
    public GameHandler GM;
    [SerializeField] GameObject[] layouts;


    private void Start() 
    {
        
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        GM = GetComponent<GameHandler> ();
    }

    public void Positioning()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        
        GM = GetComponent<GameHandler> ();
        layouts = GameObject.Find("LayoutsHandler").GetComponent<LayoutsHandler> ().layouts41;
        // Layout Untuk pemain pertama pasti layout 1
        Instantiate(layouts[0],GM.PemainList[0].transform).name = "layout " + GM.PemainList[0].gameObject.name;

        if (GM.GetComponent<GameConnect>().maxPlayer == 2)
        {
            Instantiate(layouts[3],GM.PemainList[1].transform).name = "layout " + GM.PemainList[1].gameObject.name;
        }
        if (GM.GetComponent<GameConnect>().maxPlayer == 3)
        {
            Instantiate(layouts[1],GM.PemainList[1].transform).name = "layout " + GM.PemainList[1].gameObject.name;
            Instantiate(layouts[2],GM.PemainList[2].transform).name = "layout " + GM.PemainList[2].gameObject.name;
        }
        if (GM.GetComponent<GameConnect>().maxPlayer == 4)
        {
            Instantiate(layouts[1],GM.PemainList[1].transform).name = "layout " + GM.PemainList[1].gameObject.name;
            Instantiate(layouts[3],GM.PemainList[2].transform).name = "layout " + GM.PemainList[2].gameObject.name;
            Instantiate(layouts[2],GM.PemainList[3].transform).name = "layout " + GM.PemainList[3].gameObject.name;
        }

        GM.GetComponent<GameConnect>().InitializeAllPemain();
    }

    public void ShareCard(Pemain[] pemains )
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        GM = GameObject.Find("Master").GetComponent<GameHandler> ();
        StartCoroutine(ShareCardRoutine(pemains));
    }

    IEnumerator ShareCardRoutine(Pemain[] pemains )
    {
        //Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //

        // pemains is unsorted pemain;
    
        for (int j = 0; j < 4; j++)
        {
            for (int i = 0; i < pemains.Length; i++)
            {   
                yield return new WaitForSeconds(0.15f);

                GM.MoveObject(GM.deckMinum.transform.GetChild(GM.deckMinum.transform.childCount - 1), pemains[i].layout.deck);
            }
        }
        
        foreach (Pemain i in pemains)
        {
            i.gameObject.AddComponent<Player41> ();
        }

        // change status for first player
        pemains[0].GetComponent<Player41> ().UpdateStatus(Player41.Status.AMBIL);
    }

    public void Call (string originPath, string pemainName, Player41.Status status)
    {
        //Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        if (status == Player41.Status.AMBIL) 
        {
            if (GM.online) { photonView.RPC ("RPC_Ambil", RpcTarget.All, originPath, pemainName); }
            else { GameObject.Find (pemainName).GetComponent<Player41> ().Ambil (originPath);}
        }
        else if (status == Player41.Status.BUANG)
        {
            if (GM.online) { photonView.RPC ("RPC_Buang", RpcTarget.All, originPath, pemainName); }
            else {GameObject.Find (pemainName).GetComponent<Player41> ().Buang (originPath); }
        }
    }

    public void CallNutup (string originPath, string pemainName)
    {
        //Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        if (GM.online) { photonView.RPC ("RPC_Nutup", RpcTarget.All, originPath, pemainName); }
        else { GameObject.Find (pemainName).GetComponent<Player41> ().Nutup (originPath);}
    }

    public void GameOver ()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        GM.GameOver();

        // penscoran
        BubbleSortPemain(GM.PemainList);
        
        foreach (Pemain i in GM.PemainList)
        {
            i.GetComponent<Player41> ().ChangeName();
        }

        if (GM.online)
        {
            if (GM.GetComponent<GameConnect> ().ranked)
            {
                Scoring();
                //GM.playerSc.point += GM.PemainLocal.point;
                //GM.playerSc.Save();
            }
        }
        

        StartCoroutine (SetGameOverPanel ());
    }

    void Scoring()
    {
        // penentuan point untuk ranked
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        int[] point = new int[] {9,5,-5,-9};
        // A B
        if (ScorePemain(0) == ScorePemain(1))
        {
            point[0] = 7;
            point[1] = 7;
        }

        // B C
        if (ScorePemain(1) == ScorePemain(2))
        {
            point[1] = 0;
            point[2] = 0;
        }

        // C D
        if (ScorePemain(2) == ScorePemain(3))
        {
            point[2] = -7;
            point[3] = -7;
        }

        // A B C
        if (ScorePemain (0) == ScorePemain(1)) 
        {
            if (ScorePemain(1) == ScorePemain(2))
            {
                point[0] = 3;
                point[1] = 3;
                point[2] = 3;
            }
        }

        // B C D
        if (ScorePemain (1) == ScorePemain(2)) 
        {
            if (ScorePemain(2) == ScorePemain(3))
            {
                point[1] = -3;
                point[2] = -3;
                point[3] = -3;
            }
        }

        // A B C D
        if (ScorePemain(0) == ScorePemain(1))
        {
            if (ScorePemain(1) == ScorePemain(2))
            {
                if (ScorePemain(2) == ScorePemain(3))
                {
                    point[0] = 0;
                    point[1] = 0;
                    point[2] = 0;
                    point[3] = 0;
                }
            }
        }

        for (int i = 0; i < GM.PemainList.Length; i++)
        {
            //GM.PemainList[i].point = point[i];
            GM.PemainList[i].AddPoint (point[i]);
        }
    }

    IEnumerator SetGameOverPanel()
    {
        yield return new WaitForSeconds (2.5f);
        GameObject gameOverPanel = Instantiate(GM.gameOverPanel41, GameObject.Find ("Canvas").transform);
        
        gameOverPanel.GetComponent<GameOverSc> ().info.text = "";

        if (GM.online)
        {
            if (GM.GetComponent <GameConnect> ().ranked)
            {
                // memberi point jika ranked game
                if (GM.PemainLocal.point > 0)
                {
                    gameOverPanel.GetComponent<GameOverSc> ().info.text = "+" + GM.PemainLocal.point.ToString();
                }
                else
                {
                    gameOverPanel.GetComponent<GameOverSc> ().info.text = GM.PemainLocal.point.ToString();
                }
            }
        }
        
        for (int i = 0; i < GM.PemainList.Length; i++)
        {
            GameObject pemainPanel = Instantiate (gameOverPanel.GetComponent <GameOverSc> ().pemainGameOver, gameOverPanel.GetComponent<GameOverSc> ().playerInfo);

            pemainPanel.transform.GetChild(0).GetComponent<Text> ().text = GM.PemainList[i].nickname;
            pemainPanel.transform.GetChild(1).GetComponent<Text> ().text = "Score : " + GM.PemainList[i].GetComponent<Player41> ().GetScore().ToString();

            for (int j = 0; j < 4; j ++)
            {
                // GM.PemainList[i].layout.deck.GetChild(0).SetParent(gameOverPanel.GetComponent<GameOverSc> ().deck[i]);
                Destroy(GM.PemainList[i].layout.deck.GetChild(j).GetComponent<CardSc> ());
                Instantiate (GM.PemainList[i].layout.deck.GetChild(j), pemainPanel.transform.GetChild(2));
            }
        }
    }

    int ScorePemain(int index)
    {
        return GM.PemainList[index].GetComponent<Player41>().GetScore();
    }

    public static void BubbleSortPemain(Pemain[] input)
    {
        var itemMoved = false;
        
        do
        {
            itemMoved = false;
            for (int i = 0; i < input.Length - 1; i++)
            {
                if (input[i].GetComponent<Player41>().GetScore() < input[i + 1].GetComponent<Player41>().GetScore())
                {
                    var lowerValue = input[i + 1];
                    input[i + 1] = input[i];
                    input[i] = lowerValue;
                    itemMoved = true;
                }
            }
        } while (itemMoved);
    }

    [PunRPC]
    void RPC_Ambil(string originPath, string pemainName)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        GameObject.Find (pemainName).GetComponent<Player41> ().Ambil (originPath);
    }

    [PunRPC]
    void RPC_Buang(string originPath, string pemainName)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        GameObject.Find (pemainName).GetComponent<Player41> ().Buang (originPath);
    }

    [PunRPC]
    void RPC_Nutup(string originPath, string pemainName)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        GameObject.Find (pemainName).GetComponent<Player41> ().Nutup (originPath);
    }

    
}
