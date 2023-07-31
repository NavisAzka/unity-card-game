using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RoomSc : MonoBehaviourPunCallbacks
{
    [SerializeField] Text roomName;
    [SerializeField] Text gameMode, playerCount;
    [SerializeField] Text[] playerListNameText;
    [SerializeField] GameObject roomPanel,rankPanel,roomMasterPanel;
    public const string MODE = "rank";
    public const string GAME_MODE = "gm";
    public const string STATUS = "ready";
    public const string BOT_COUNT = "botCount";
    float timerForRank = 5f;

    private void Start() {
        StartCoroutine (OneSec());
        UpdateRoom();
    }

    public void UpdateRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
            //

            if ((int)PhotonNetwork.CurrentRoom.CustomProperties[GAME_MODE] == 0)
            {
                gameMode.text = "GAME MODE : 41";
            }
            else if ((int)PhotonNetwork.CurrentRoom.CustomProperties[GAME_MODE] == 1)
            {
                gameMode.text = "GAME MODE : JOKER";
            }
            else if ((int)PhotonNetwork.CurrentRoom.CustomProperties[GAME_MODE] == 2)
            {
                gameMode.text = "GAME MODE : CANGKULAN";
            }

            UpdatePlayerNameList();
            UpdateBot();
            UpdateRoomPanel();
            RefreshStatusPlayer();
            roomMasterPanel.SetActive (PhotonNetwork.IsMasterClient);


            rankPanel.SetActive ((bool)PhotonNetwork.CurrentRoom.CustomProperties[MODE]);
            
            if ((bool)PhotonNetwork.CurrentRoom.CustomProperties[MODE])
            {
                UpdateRank ();
                timerForRank = Random.Range (5,10);
            }
        }
    }

    IEnumerator OneSec ()
    {
        
        yield return new WaitForSeconds(1f);
        
        if (PhotonNetwork.InRoom)
        {   
            if ((bool)PhotonNetwork.CurrentRoom.CustomProperties[MODE] && PhotonNetwork.IsMasterClient)
            {
                timerForRank -= 1f;
            
                if (timerForRank < 0)
                {
                    AddBot();
                    timerForRank = Random.Range (2,4);
                }
            }
        }

        StartCoroutine (OneSec());
    }

    void UpdateRank ()
    {
        UpdatePlayer();
    }

    void UpdatePlayer()
    {
        int kosong = 4;

        for (int j = 0; j < playerListNameText.Length; j++)
        {
            if (playerListNameText[j].text != "")
            {
                kosong -= 1;
            }
        }

        playerCount.text = PhotonNetwork.PlayerList.Length.ToString() + " / 4";

        Debug.Log(kosong);

        if (kosong == 0)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;

            if ((bool)PhotonNetwork.CurrentRoom.CustomProperties[MODE])
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    GetComponent<MenuHandler> ().StartGame ();
                }
            }
        }
    }

    void UpdateBot()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        int botCount = (int)PhotonNetwork.CurrentRoom.CustomProperties[BOT_COUNT];
        
        for (int i = 0; i < botCount; i++)
        {
            for (int j = 0; j < playerListNameText.Length; j++)
            {
                if (playerListNameText[j].text == "")
                {
                    playerListNameText[j].text = "Computer";
                    playerListNameText[j].transform.GetChild(0).gameObject.SetActive (true);

                    break;
                }
            }
        }
    }
    

    void UpdateRoomPanel()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        roomPanel.SetActive(PhotonNetwork.InRoom);
        GetComponent<MenuHandler> ().MiniLoadingActive(false);

        //roomName.text = PhotonNetwork.CurrentRoom.Name;
    }

    void RefreshStatusPlayer()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if ((bool)PhotonNetwork.PlayerList[i].CustomProperties[STATUS] == true)
            {
                foreach (Text j in playerListNameText)
                {
                    j.transform.GetChild(0).gameObject.SetActive (true);
                }
            }
        }
    }

    void UpdatePlayerNameList()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        //clear name list
        foreach (Text i in playerListNameText)
        {
            i.text = "";
        }

        // fill name list
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            playerListNameText[i].text = PhotonNetwork.PlayerList[i].NickName;
        }
    }


    public void SetPlayerReady(bool state) 
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable {{STATUS, state}};

        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
    }

    public void SetPlayerReady() 
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        bool state = (bool)PhotonNetwork.LocalPlayer.CustomProperties[STATUS] ;
        ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable {{STATUS, !state}};

        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
    }

    public void AddBot ()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        int botCount = (int)PhotonNetwork.CurrentRoom.CustomProperties[BOT_COUNT];
        
        if (botCount + PhotonNetwork.PlayerList.Length < 4)
        {
            botCount += 1;

            ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable {{BOT_COUNT, botCount}};
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
            //Debug.Log(botCount);
        }
    }

    public void SubstactBot ()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        int botCount = (int)PhotonNetwork.CurrentRoom.CustomProperties[BOT_COUNT];
        
        if (botCount + PhotonNetwork.PlayerList.Length > 1)
        {
            botCount -= 1;

            ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable {{BOT_COUNT, botCount}};
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
            //Debug.Log(botCount);
        }
    }

    public void ChangeGameMode()
    {

        if ((int)PhotonNetwork.CurrentRoom.CustomProperties[GAME_MODE] == 0)
        {
            //gameMode.text = "JOKER";

            ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable {{GAME_MODE, 1}};
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
        } 
        else if ((int)PhotonNetwork.CurrentRoom.CustomProperties[GAME_MODE] == 1)
        {
            //gameMode.text = "EMPAT SATU";

            ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable {{GAME_MODE, 2}};
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
        }
        else if ((int)PhotonNetwork.CurrentRoom.CustomProperties[GAME_MODE] == 2)
        {
            //gameMode.text = "UMBEN";

            ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable {{GAME_MODE, 0}};
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
        }
    }

    public void ChangeGameMode(int codeGame)
    {
        ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable {{GAME_MODE, codeGame}};
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
    }



    public override void OnJoinedRoom()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        
        SetPlayerReady(true);

        //UpdateRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        UpdateRoom();
    }

    public override void OnLeftRoom()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        SetPlayerReady(false);
        GetComponent<MenuHandler> ().MiniLoadingActive(true);

        roomPanel.SetActive(false);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        UpdateRoom();
        //Debug.Log(changedProps + " : " + (bool)PhotonNetwork.LocalPlayer.CustomProperties[STATUS]);
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        UpdateRoom();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        UpdateRoom();
    }

}
