
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class HandlerJoker : MonoBehaviourPunCallbacks
{
    public GameHandler GM;
    [SerializeField] GameObject[] layouts;
    PlayerJoker[] pemainMain;

    public PlayerJoker[] GetPemainMain()
    {
        Stack<PlayerJoker> p = new Stack<PlayerJoker>();

        for (int i = 0; i < GM.PemainList.Length; i++)
        {
            if (!(GM.PemainList[i].GetComponent<PlayerJoker> ().statusNow == PlayerJoker.Status.HABIS))
            {
                p.Push(GM.PemainList[i].GetComponent<PlayerJoker>());
            }
        }

        pemainMain = p.ToArray();
        
        return p.ToArray(); 
    }

    public PlayerJoker[] GetSisaPemain()
    {
        Stack<PlayerJoker> p = new Stack<PlayerJoker>();

        for (int i = 0; i < GM.PemainList.Length; i++)
        {
            if (!(GM.PemainList[i].GetComponent<PlayerJoker> ().deckChild() < 1))
            {
                p.Push(GM.PemainList[i].GetComponent<PlayerJoker>());
            }
        }

        pemainMain = p.ToArray();
        
        return p.ToArray(); 
    }

    void Awake()
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
        layouts = GameObject.Find("LayoutsHandler").GetComponent<LayoutsHandler> ().layoutsJoker;
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
        int pos = 0;
        while (GM.deckMinum.transform.childCount > 0)
        {

            yield return new WaitForSeconds(0.15f);
            
            pos = pos > pemains.Length - 1 ? 0 : pos;
            GM.MoveObject(GM.deckMinum.transform.GetChild(GM.deckMinum.transform.childCount - 1), pemains[pos].layout.deck);
            pos ++;
        }
        
        foreach (Pemain i in pemains)
        {

            i.gameObject.AddComponent<PlayerJoker> ();
            i.GetComponent<PlayerJoker> ().UpdateStatus (PlayerJoker.Status.BUANG);

            if (i.gameObject.TryGetComponent<BotJoker>(out BotJoker botSc))
            {

                botSc.count = 1;
                botSc.Initialize();
            }
        }

        // change status for first player
        pemains[0].GetComponent<PlayerJoker> ().UpdateStatus(PlayerJoker.Status.SELESAI);
        GM.nextButton.SetActive(true);
    }

    public void DeckMinumCount()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        if (GM.deckMinum.transform.childCount >= 50)
        {
            GameOver();
            Debug.Log("GAME OVER");
        }
    }

    void GameOver()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        GetSisaPemain();
        GM.GameOver();

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
        for (int i = 0; i < GM.PemainList.Length; i++)
        {
            if (GM.PemainList[i] != pemainMain[0].pemain)
            {
                GM.PemainList[i].AddPoint (3);
            }
        }

        GM.PemainList[0].AddPoint (-9);
    }

    IEnumerator SetGameOverPanel()
    {
        yield return new WaitForSeconds (2.5f);
        GameObject gameOverPanel = Instantiate(GM.gameOverPanelJoker, GameObject.Find ("Canvas").transform);
        
        gameOverPanel.GetComponent<GameOverSc> ().info.text = "";

        if (GM.online)
        {
            if (GM.GetComponent <GameConnect> ().ranked)
            {
                // memberi point jika ranked game
                //gameOverPanel.GetComponent<GameOverSc> ().info.text = "Point " + GM.PemainLocal.point.ToString();
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
            if (GM.PemainList[i] != pemainMain[0].pemain)
            {
                GameObject pemainPanel = Instantiate (gameOverPanel.GetComponent <GameOverSc> ().pemainGameOver, gameOverPanel.GetComponent<GameOverSc> ().playerInfo);

                pemainPanel.transform.GetChild(0).GetComponent<Text> ().text = GM.PemainList[i].nickname;
                pemainPanel.transform.GetChild(1).GetComponent<Text> ().text = "WIN";
            }
        }

        
        GameObject pemainPanelLose = Instantiate (gameOverPanel.GetComponent <GameOverSc> ().pemainGameOver, gameOverPanel.GetComponent<GameOverSc> ().playerInfo);

        pemainPanelLose.transform.GetChild(0).GetComponent<Text> ().text = pemainMain[0].pemain.nickname;
        pemainPanelLose.transform.GetChild(1).GetComponent<Text> ().text = "LOSE";

    }
    public void Call (string originPath, string pemainName, PlayerJoker.Status status)
    {
        //Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        if (status == PlayerJoker.Status.AMBIL) 
        {
            if (GM.online) { photonView.RPC ("RPC_Ambil", RpcTarget.All, originPath, pemainName); }
            else { GameObject.Find (pemainName).GetComponent<PlayerJoker> ().Ambil (originPath);}
        }
        else if (status == PlayerJoker.Status.SELESAI)
        {
            if (GM.online) { photonView.RPC ("RPC_NextMove", RpcTarget.All, pemainName); }
            else { GameObject.Find (pemainName).GetComponent<PlayerJoker> ().NextMove(); }
        }
    }


    public void Call (string card1, string card2, string pemainName, PlayerJoker.Status status)
    {
        //Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        // 
        
        if (GM.online) { photonView.RPC ("RPC_Buang", RpcTarget.All, card1,card2, pemainName); }
        else { GameObject.Find (pemainName).GetComponent<PlayerJoker> ().Buang (card1, card2);}

    }


    [PunRPC]
    void RPC_Ambil(string originPath, string pemainName)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        GameObject.Find (pemainName).GetComponent<PlayerJoker> ().Ambil (originPath);
    }

    [PunRPC]
    void RPC_Buang(string card1, string card2, string pemainName)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        GameObject.Find (pemainName).GetComponent<PlayerJoker> ().Buang (card1, card2);
    }

    [PunRPC]
    void RPC_NextMove(string pemainName)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        GameObject.Find (pemainName).GetComponent<PlayerJoker> ().NextMove ();
    }

}
