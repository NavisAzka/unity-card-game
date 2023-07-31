using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MainConnect : MonoBehaviourPunCallbacks
{
    string[] forlobby;
    public const string MODE = "rank";
    public const string BOT_COUNT = "botCount";
    public const string GAME_MODE = "gm";

    void Start()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        // Connect to master server photon
        forlobby = new string[2] {MODE, GAME_MODE};

        Reconnect();
    }

    public void Reconnect()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        GetComponent<MenuHandler> ().Disconnected();


        if (PhotonNetwork.InRoom)
        {
            Debug.Log("BENER 0000000000000");
            GetComponent<MenuHandler> ().Connected();
        }
        else
        {
            Connect();
        }
    }

    void Connect ()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        // 
        PhotonNetwork.ConnectUsingSettings();
    }


    public void LeaveRoom()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);

        PhotonNetwork.LeaveRoom();
    }

    public void StartGame() /// Hanya Dipanggil oleh 1 orang
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);

        // ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable {{BOT_COUNT, 0}};
        // PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
        PhotonNetwork.CurrentRoom.MaxPlayers = (byte)PhotonNetwork.PlayerList.Length;
        
        GetComponent<PhotonView>().RPC("RPC_StartGame", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void RPC_StartGame()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        PhotonNetwork.LoadLevel(1);
    }

    public void JoinRanked(int mode)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        
        selectedMode = mode;
        selectedRank = true;

        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable { { MODE , selectedRank } };

        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 4);
        GetComponent<MenuHandler> ().MiniLoadingActive(true);
    }

    public void JoinCustom(int mode)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        
        selectedMode = mode;
        selectedRank = false;

        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable { { MODE , selectedRank }, {GAME_MODE, selectedMode} };

        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 4);
        GetComponent<MenuHandler> ().MiniLoadingActive(true);
    }

    int selectedMode = 0;
    bool selectedRank = true;

    void CreateRoom ()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);

        // setting room option for ranked 
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.CustomRoomPropertiesForLobby =  forlobby ;
        
        if (selectedRank)
        {
            roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable { 
            { BOT_COUNT, 0 }, {MODE , selectedRank}, {GAME_MODE, Random.Range(0,3)}};
        } 
        else
        {
            roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable { 
            { BOT_COUNT, 0 }, {MODE , selectedRank}, {GAME_MODE, selectedMode}};  
        }

        roomOptions.MaxPlayers = 4;
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.PublishUserId = true;

        if (selectedRank)
        {
            PhotonNetwork.CreateRoom("RANKED " + Random.Range(0,999).ToString(), roomOptions);
        } else {
            PhotonNetwork.CreateRoom("CUSTOM" + Random.Range(0,999).ToString(), roomOptions);
        }

    }
    



    public override void OnConnectedToMaster()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        Debug.Log("OnConnectedToMaster() was called by PUN.");
        
        GetComponent<MenuHandler> ().Connected();
        GetComponent<MenuHandler> ().MiniLoadingActive(false);

        PhotonNetwork.NickName = GetComponent <SaveData> ().nickname;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        Debug.Log(returnCode);

        CreateRoom();
    }


    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        Debug.Log(returnCode);
        
        CreateRoom();
    }

  public override void OnDisconnected(DisconnectCause cause)
  {
        GetComponent<MenuHandler> ().LostConnection();
    
  }
}
