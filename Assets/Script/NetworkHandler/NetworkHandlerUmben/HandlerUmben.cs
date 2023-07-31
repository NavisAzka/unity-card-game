using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class HandlerUmben : MonoBehaviour
{
    public GameHandler GM;
    [SerializeField] GameObject[] layouts;
    PhotonView photonView;
    public CardSc.Symbol selectedSymbol;
    Queue<PlayerUmben> pemainQueue = new Queue<PlayerUmben> ();
    Pemain[] pemainMain;
    PlayerUmben[] playerUmbenMain;

    bool gameOver;


    void Awake()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        GM = GetComponent<GameHandler> ();
        photonView = GetComponent<PhotonView> ();
    }

    public Pemain[] GetPemainMain()
    {
        Queue<Pemain> p = new Queue<Pemain>();

        for (int i = 0; i < GM.unsortedPemainList.Length; i++)
        {
            if (GM.unsortedPemainList[i].layout.deck.childCount > 0)
            {
                p.Enqueue(GM.unsortedPemainList[i]);
            }
        }
        pemainMain = p.ToArray();

        
        return p.ToArray(); 
    }

    public Pemain[] GetSisaPemainMain()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        Queue<Pemain> p = new Queue<Pemain>();

        for (int i = 0; i < GM.unsortedPemainList.Length; i++)
        {
            if (GM.unsortedPemainList[i].layout.deck.childCount > 0)
            {
                p.Enqueue(GM.unsortedPemainList[i]);
            }
        }
        pemainMain = p.ToArray();

        Debug.Log (pemainMain.Length);
        
        if (pemainMain.Length == 1)
        {
            GameOver();
        }
        
        return p.ToArray(); 
    }

    public void Positioning()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        
        GM = GetComponent<GameHandler> ();
        layouts = GameObject.Find("LayoutsHandler").GetComponent<LayoutsHandler> ().layoutsUmben;
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
    
        for (int j = 0; j < 5; j++)
        {
            for (int i = 0; i < pemains.Length; i++)
            {   
                yield return new WaitForSeconds(0.15f);

                GM.MoveObject(GM.deckMinum.transform.GetChild(GM.deckMinum.transform.childCount - 1), pemains[i].layout.deck);
            }
        }
        
        foreach (Pemain i in pemains)
        {
            i.gameObject.AddComponent<PlayerUmben> ();
            i.GetComponent<PlayerUmben> ().UpdateStatus (PlayerUmben.Status.TUNGGU);
        }

        // change status for first player
        pemains[0].GetComponent<PlayerUmben> ().UpdateStatus(PlayerUmben.Status.BUANG);

        Awal ();
    }

    void Awal ()
    {
        CardSc kartuAwal = GM.deckMinum.transform.GetChild(0).GetComponent<CardSc> ();
        kartuAwal.Buka(true);
        selectedSymbol = kartuAwal.symbol;
        
        GM.JustMoveObject(GM.deckMinum.transform, GameObject.Find("TombolAmbilSemua").transform);
        GM.MoveObject(kartuAwal.transform, GameObject.Find ("Lempar").transform);
        
        StartCoroutine(Visible());
        BuatQueue();
    }
    
    IEnumerator Visible()
    {
        yield return new WaitForSeconds (5f);
        GameObject.Find("TombolAmbilSemua").GetComponent<Image> ().enabled = true;
    }

    IEnumerator EditQueue(bool buangAll, float timea)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        yield return new WaitForSeconds(timea);
        int bigestNum = 0;
        int indexPlayer = 0;

        Pemain[] pemainMain = GetPemainMain();

        // hitung kartu yang palong  besar 
        for (int i = 0; i < pemainMain.Length; i++ )
        {
            if (pemainMain[i].layout.buangan.childCount > 0)
            {
                if (pemainMain[i].layout.buangan.GetChild(0).GetComponent<CardSc>().numID > bigestNum)
                {
                    bigestNum = pemainMain[i].layout.buangan.transform.GetComponentInChildren<CardSc>().numID;
                    indexPlayer = i;
                }
            }
        }

        PlayerUmben terbesar = pemainMain[indexPlayer].GetComponent <PlayerUmben> ();
        terbesar.UpdateStatus(PlayerUmben.Status.AWAL);
        Debug.Log(terbesar);

        for (int i = 0; i < pemainQueue.Count; i++)
        {
            if (pemainQueue.Peek() != terbesar)
            {
                pemainQueue.Enqueue(pemainQueue.Dequeue());
            }
            else
            {
                break;
            }
        }

        playerUmbenMain = pemainQueue.ToArray();


        if (!buangAll)
        {
            BuangKartuBuangan();
        }

        terbesar.Initialize();
    }

    void BuangKartuBuangan()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //s
        foreach (Pemain i in GM.PemainList)
        {
            if (i.layout.buangan.childCount > 0)
            {
                GM.MoveObject(i.layout.buangan.GetChild(0), GameObject.Find("Lempar").transform);
            }
        }

        GM.JustMoveObject(GameObject.Find("Lempar").transform, GameObject.Find("Luar").transform);
    }

    void BuatQueue()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        if (!gameOver)
        {
            Pemain[] pemainMain = GetPemainMain();
            Debug.Log (pemainQueue);

            for (int i = 0; i < pemainMain.Length; i++)
            {
                pemainQueue.Enqueue(pemainMain[i].GetComponent<PlayerUmben>());
            }
        }
    }

    public void BuangQueue ()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        if (!gameOver)
        {
            pemainQueue.Dequeue();

            if (pemainQueue.Count == 0)
            {
                BuatQueue();
                StartCoroutine(EditQueue(false, .8f));
            } 
            else 
            {
                pemainQueue.Peek().NowMove();
            }
        }
    }

    public void BuangAllQueue ()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        pemainQueue.Clear();

        BuatQueue();
        StartCoroutine(EditQueue(true, 0f));
    }

    void GameOver()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        GM.GameOver();
        StartCoroutine (SetGameOverPanel ());

        if (GM.online)
        {
            if (GM.GetComponent<GameConnect> ().ranked)
            {
                Scoring();
                //GM.playerSc.point += GM.PemainLocal.point;
                //GM.playerSc.Save();
            }
        }
        

    }

    void Scoring()
    {
        for (int i = 0; i < pemainMain.Length - 1; i++)
        {
            pemainMain[i].AddPoint (3);
        }

        pemainMain[0].AddPoint (-9);

    }

    IEnumerator SetGameOverPanel()
    {
        yield return new WaitForSeconds (3.5f);
        GameObject gameOverPanel = Instantiate(GM.gameOverPanelUmben, GameObject.Find ("Canvas").transform);
        
        gameOverPanel.GetComponent<GameOverSc> ().info.text = "";

        if (GM.online)
        {
            if (GM.GetComponent <GameConnect> ().ranked)
            {
                // memberi point jika ranked game
                //gameOverPanel.GetComponent<GameOverSc> ().info.text = GM.PemainLocal.point.ToString();
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
            if (GM.PemainList[i] != pemainMain[0])
            {
                GameObject pemainPanel = Instantiate (gameOverPanel.GetComponent <GameOverSc> ().pemainGameOver, gameOverPanel.GetComponent<GameOverSc> ().playerInfo);

                pemainPanel.transform.GetChild(0).GetComponent<Text> ().text = GM.PemainList[i].nickname;
                pemainPanel.transform.GetChild(1).GetComponent<Text> ().text = "WIN";
            }
        }

        
        GameObject pemainPanelLose = Instantiate (gameOverPanel.GetComponent <GameOverSc> ().pemainGameOver, gameOverPanel.GetComponent<GameOverSc> ().playerInfo);

        pemainPanelLose.transform.GetChild(0).GetComponent<Text> ().text = pemainMain[0].nickname;
        pemainPanelLose.transform.GetChild(1).GetComponent<Text> ().text = "LOSE";
    }

    public void Call (string originPath, string pemainName, PlayerUmben.Status status)
    {
        //Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //

        if (status == PlayerUmben.Status.BUANG)
        {
            if (GM.online) { photonView.RPC ("RPC_Buang", RpcTarget.All, originPath, pemainName); }
            else {GameObject.Find (pemainName).GetComponent<PlayerUmben> ().Buang (originPath); }
        }
        
        if (status == PlayerUmben.Status.AWAL)
        {

            if (GM.online) { photonView.RPC ("RPC_Awal", RpcTarget.All, originPath, pemainName); }
            else {GameObject.Find (pemainName).GetComponent<PlayerUmben> ().Buang (originPath); }
        }
    }

    public void CallAmbil(string originPath, string pemainName, PlayerUmben.Status status)
    {
        if (GM.online) { photonView.RPC ("RPC_Ambil", RpcTarget.All, originPath, pemainName); }
        else { GameObject.Find (pemainName).GetComponent<PlayerUmben> ().Ambil (originPath);}
    }

    public void CallAmbilBuangan(string[] originPath, string pemainName, PlayerUmben.Status status)
    {
        if (GM.online) { photonView.RPC ("RPC_AmbilBuangan", RpcTarget.All, originPath, pemainName); }
        else { GameObject.Find (pemainName).GetComponent<PlayerUmben> ().AmbilBuangan (originPath);}
    }

    
    [PunRPC]
    void RPC_Ambil(string originPath, string pemainName)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        GameObject.Find (pemainName).GetComponent<PlayerUmben> ().Ambil (originPath);
    }

    [PunRPC]
    void RPC_Buang(string originPath, string pemainName)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        GameObject.Find (pemainName).GetComponent<PlayerUmben> ().Buang (originPath);
    }

    [PunRPC]
    void RPC_Awal(string originPath, string pemainName)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        GameObject.Find (pemainName).GetComponent<PlayerUmben> ().Awal (originPath);
    }

    [PunRPC]
    void RPC_AmbilBuangan (string[] originPath,string pemainName)
    {
        GameObject.Find (pemainName).GetComponent<PlayerUmben> ().AmbilBuangan (originPath);
    }
}