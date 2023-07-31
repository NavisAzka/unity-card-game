using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflineGame : MonoBehaviour
{
    GameHandler GM;
    SaveData saveData; 
    public Pemain[] PemainList;
    public int maxPlayer = 4;
    [SerializeField] GameObject[] layouts41;
    [SerializeField] GameObject[] layoutsJoker;


    private void Start() 
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        GM = GetComponent<GameHandler> ();

        CreatePemain();
    }

    void CreatePemain()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        
        PemainList = new Pemain[maxPlayer];
        
        for (int i = 0; i < maxPlayer; i++)
        {
            PemainList[i] = Instantiate(GM.playerPrefabs, GameObject.Find("Players").transform).GetComponent<Pemain> ();
        }

        SetUpPemain();
    }

    void SetUpPemain ()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        
        PemainList[0].index = 0;
        PemainList[0].nickname = "Player";
        
        // Fill every player to pemain
        for (int i = 1; i < maxPlayer; i++)
        {
            PemainList[i].index = i;

            if (GM.gameMode == GameHandler.GameMode.empatSatu) {
                PemainList[i].gameObject.AddComponent<Bot41>();
            } else if (GM.gameMode == GameHandler.GameMode.joker) {
                PemainList[i].gameObject.AddComponent<BotJoker>();
            }

            PemainList[i].nickname = "Computer " + i.ToString();
            PemainList[i].gameObject.name = PemainList[i].nickname + PemainList[i].index;
        }

        // Arrange the player depend on index
        Positioning();
    }

    // void Positioning()
    // {
    //     Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
    //     //
    //     // Layout Untuk pemain pertama pasti layout 1
    //     Instantiate(layouts[0],PemainList[0].transform).name = "layout " + PemainList[0].gameObject.name;

    //     if (maxPlayer == 4)
    //     {
    //         Instantiate(layouts[1],PemainList[1].transform).name = "layout " + PemainList[1].gameObject.name;
    //         Instantiate(layouts[3],PemainList[2].transform).name = "layout " + PemainList[2].gameObject.name;
    //         Instantiate(layouts[2],PemainList[3].transform).name = "layout " + PemainList[3].gameObject.name;
    //     }
        
    //     InitializeAllPemain();
    // }

    public void Positioning ()
    {
        if (GM.gameMode == GameHandler.GameMode.empatSatu)
        {
            Instantiate(layouts41[0],PemainList[0].transform).name = "layout " + PemainList[0].gameObject.name;

            if (maxPlayer == 4)
            {
                Instantiate(layouts41[1],PemainList[1].transform).name = "layout " + PemainList[1].gameObject.name;
                Instantiate(layouts41[3],PemainList[2].transform).name = "layout " + PemainList[2].gameObject.name;
                Instantiate(layouts41[2],PemainList[3].transform).name = "layout " + PemainList[3].gameObject.name;
            }
        }
        else if (GM.gameMode == GameHandler.GameMode.joker)
        {
            Instantiate(layoutsJoker[0],PemainList[0].transform).name = "layout " + PemainList[0].gameObject.name;

            if (maxPlayer == 4)
            {
                Instantiate(layoutsJoker[1],PemainList[1].transform).name = "layout " + PemainList[1].gameObject.name;
                Instantiate(layoutsJoker[3],PemainList[2].transform).name = "layout " + PemainList[2].gameObject.name;
                Instantiate(layoutsJoker[2],PemainList[3].transform).name = "layout " + PemainList[3].gameObject.name;
            }
        }
        
        InitializeAllPemain();
    }


    void InitializeAllPemain()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        PemainList[0].local = true;
        GM.PemainLocal = PemainList[0];
        
        Shuffle();

        foreach (Pemain i in PemainList)
        {
            i.Initialize();
        }

        
    }

        void Shuffle()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        
        GM.SetPemainList (PemainList, PemainList);
        GM.deckMinum.GenerateShuffleCode();
        GM.deckMinum.ShuffleCard(GM.deckMinum.shuffleCode);
        GM.ReadyToStart();
    }

}
