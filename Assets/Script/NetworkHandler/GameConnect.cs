using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
public class GameConnect : MonoBehaviourPunCallbacks
{
    GameHandler GM;
    SaveData saveData; 
    Pemain[] unsortedPemainList;
    public Pemain[] PemainList;
    public int maxPlayer = 0;
    public const string BOT_COUNT = "botCount";
    public const string GAME_MODE = "gm";

    public bool ranked = false;

    public GameHandler.GameMode GetGameMode()
    {
        GameHandler.GameMode gameMode = GameHandler.GameMode.empatSatu;
        saveData = GetComponent<SaveData> ();

        if ((int)PhotonNetwork.CurrentRoom.CustomProperties[GAME_MODE] == 0)
        {
            gameMode = GameHandler.GameMode.empatSatu;
        }
        else if ((int)PhotonNetwork.CurrentRoom.CustomProperties[GAME_MODE] == 1)
        {
            gameMode = GameHandler.GameMode.joker;
        }
        else if ((int)PhotonNetwork.CurrentRoom.CustomProperties[GAME_MODE] == 2)
        {
            gameMode = GameHandler.GameMode.umben;
        }

        return gameMode;
    }

    public void StartGame() 
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        GM = GetComponent<GameHandler> ();
        PhotonNetwork.AutomaticallySyncScene = false;
        ranked = ((bool)PhotonNetwork.CurrentRoom.CustomProperties["rank"]);

        GetEveryPoint ();
        CreatePemain();
    }

    void GetEveryPoint ()
    {
        if (ranked)
        {
            saveData.Deposit(-9);
        }
    }

    void CreatePemain()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        int botCount = (int)PhotonNetwork.CurrentRoom.CustomProperties[BOT_COUNT];
        maxPlayer = PhotonNetwork.PlayerList.Length + botCount;

        PemainList = new Pemain[maxPlayer];
        unsortedPemainList = new Pemain[maxPlayer];
        
        for (int i = 0; i < maxPlayer; i++)
        {
            unsortedPemainList[i] = Instantiate(GM.playerPrefabs, GameObject.Find("Players").transform).GetComponent<Pemain> ();
        }

        SetUpPemain();
    }

    void SetUpPemain ()
    {

        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //

        int localId = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        
        // Fill every player to pemain
        for (int i = 0; i < maxPlayer; i++)
        {
            if (i < PhotonNetwork.PlayerList.Length)
            {
                unsortedPemainList[i].player = PhotonNetwork.PlayerList[i];
                unsortedPemainList[i].index = i;
                unsortedPemainList[i].nickname = PhotonNetwork.PlayerList[i].NickName;
            }
            else
            {
                unsortedPemainList[i].index = i;

                if (GM.gameMode == GameHandler.GameMode.empatSatu)
                {
                    unsortedPemainList[i].gameObject.AddComponent<Bot41>();
                }
                else if (GM.gameMode == GameHandler.GameMode.joker)
                {
                    unsortedPemainList[i].gameObject.AddComponent<BotJoker>();
                }
                else if (GM.gameMode == GameHandler.GameMode.umben)
                {
                    unsortedPemainList[i].gameObject.AddComponent<BotUmben>();
                }

                unsortedPemainList[i].nickname = ComputerName(i);
            }
        }

        // Arrange the player depend on index
        for (int i = 0; i < maxPlayer; i++)
        {
            int j = unsortedPemainList[i].index - localId;
            Debug.Log(j + " " + i);

            if (j < 0) { j += maxPlayer; }
            Debug.Log(j + " " + i);

            PemainList[j] = unsortedPemainList[i];
            PemainList[j].gameObject.name = PemainList[j].nickname + PemainList[j].index;
        }

        GM.SetPemainList (unsortedPemainList, PemainList);
        GM.Positioning();
    }

    string ComputerName(int index)
    {
        string result = "Computer " + index.ToString();
        
        string[] nama = {
            "Pakde", "Abdul", "User412412", "User6845", "User69", "Kakek999", "Pakek Nanya", "AGEN", "LPG 12kg"
        };
        
        if (ranked) { result = nama[Random.Range(0, nama.Length)]; }

        return result;
    }

    public void InitializeAllPemain()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        PemainList[0].local = true;
        GM.PemainLocal = PemainList[0];

        foreach (Pemain i in PemainList)
        {
            i.Initialize();
        }
        Shuffle();
        // tell the gm who is playing
        
    }

    void Shuffle()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        if (PhotonNetwork.IsMasterClient)
        {
            GM.deckMinum.GenerateShuffleCode();
            GetComponent<PhotonView>().RPC("RPC_ShuffleForAll", RpcTarget.AllBuffered, GM.deckMinum.shuffleCode);
        }

    }

    [PunRPC]
    void RPC_ShuffleForAll(int[] shuffleCodes)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        GM.deckMinum.ShuffleCard(shuffleCodes);
        GM.ReadyToStart();
    }

    public void LeaveRoom ()
    {
        PhotonNetwork.LeaveRoom ();
    }

    public void Continue()
    {
        if (ranked) { PhotonNetwork.LeaveRoom (); }
        else {PhotonNetwork.LoadLevel (0);}
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        for (int i = 0; i < GM.PemainList.Length; i++)
        {
            if (PemainList[i].player == otherPlayer)
            {
                PemainList[i].CallBot(); 
            }
        }
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel (0);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        PhotonNetwork.LoadLevel (0);
    } 
}
