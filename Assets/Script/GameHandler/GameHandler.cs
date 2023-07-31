using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{
    public GameObject playerPrefabs;
    public GameObject gameOverPanel41;
    public GameObject gameOverPanelJoker;
    public GameObject gameOverPanelUmben;
    public GameObject nextButton;
    [SerializeField] GameObject exitToLobbyPanel;
    [SerializeField] GameObject jokerCard;
    [SerializeField] GameObject gameModeHandler41;
    public Text[] gameOverText;
    public Deck deckMinum;
    public Handler41 handler41;
    public HandlerJoker handlerJoker;
    public HandlerUmben handlerUmben;
    public Pemain[] unsortedPemainList;
    public Pemain[] PemainList;
    public Pemain PemainLocal;
    public SaveData saveData;
    public bool gameOver;
    public bool online;

    public enum GameMode {empatSatu, joker, umben}
    public GameMode gameMode;

    private void Start()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        
        //playerSc = GameObject.Find ("SaveSystem").GetComponent<PlayerSc> ();
        //playerSc.Load ();

        saveData = GetComponent<SaveData> ();
        saveData.Load();

        if (online)
        {
            gameMode = GetComponent<GameConnect> ().GetGameMode();
        }
        
        // Add HANDLER
        if (gameMode == GameMode.empatSatu)
        {
            handler41 = gameObject.AddComponent<Handler41> ();
        }
        else if (gameMode == GameMode.joker)
        {
            handlerJoker = gameObject.AddComponent<HandlerJoker> ();
            Instantiate(jokerCard, deckMinum.transform);
        }
        else if (gameMode == GameMode.umben)
        {
            handlerUmben = gameObject.AddComponent<HandlerUmben> ();
        }

        // Add CARD CALLER
        for (int i = 0; i < deckMinum.transform.childCount; i++)
        {
            if (gameMode == GameMode.empatSatu)
            {
                deckMinum.transform.GetChild(i).gameObject.AddComponent<CardCaller41> ();
            }
            else if (gameMode == GameMode.joker)
            {
                deckMinum.transform.GetChild(i).gameObject.AddComponent<CardCallerJoker> ();
            }
            else if (gameMode == GameMode.umben)
            {
                deckMinum.transform.GetChild(i).gameObject.AddComponent<CardCallerUmben> ();
            }
        }

        if (online) { GetComponent<GameConnect> ().StartGame();}
        else {GetComponent<OfflineGame> ();}

    }
    public void MoveObject(Transform origin,Transform target)
    {
        //Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        origin.gameObject.AddComponent <MoveCard> ().Move(target);
    }

    public void MoveObject(Transform origin,Transform target, Pemain pemain)
    {
        //Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        origin.gameObject.AddComponent <MoveCard> ().Move(target, pemain);
    }

    public void JustMoveObject(Transform origin,Transform target)
    {
        //Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        origin.gameObject.AddComponent <MoveCard> ().JustMove(target);
    }

    public void Positioning ()
    {
        if (gameMode == GameMode.empatSatu)
        {
            handler41.Positioning();
        }
        else if (gameMode == GameMode.joker)
        {
            handlerJoker.Positioning();
        }
        else if (gameMode == GameMode.umben)
        {
            handlerUmben.Positioning();
        }
    }

    public void SetPemainList(Pemain[] upl, Pemain[] pl)
    {
        unsortedPemainList = upl;
        PemainList = pl;
    }

    public void ReadyToStart()
    {
        if (gameMode == GameMode.empatSatu)
        {
            handler41.ShareCard(unsortedPemainList);
        }
        else if (gameMode == GameMode.joker)
        {
            handlerJoker.ShareCard(unsortedPemainList);
        }
        else if (gameMode == GameMode.umben)
        {
            handlerUmben.ShareCard(unsortedPemainList);
        }
    }

    public void GameOver()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //

        gameOver = true;
        if (deckMinum.transform.childCount > 0)
        {
            deckMinum.transform.GetChild(deckMinum.transform.childCount-1).GetComponent<CardSc> ().Buka (true);
        }
        
    }

    public void NextMove()
    {
        PemainList[0].GetComponent<PlayerJoker> ().OnClickNextMove();
    }

    public void AmbilBuangan()
    {
        PemainList[0].GetComponent<PlayerUmben> ().ClickAmbilBuangan();
    }

    public void OnClickExit()
    {
        if (GetComponent<GameConnect> ().ranked)
        {
            exitToLobbyPanel.SetActive(true);
        } else {
            LeaveRoom();
        }
    }

    public void LeaveRoom ()
    {
        if (online) 
        {
            GetComponent <GameConnect> ().LeaveRoom();
        }
        else {
            SceneManager.LoadScene (0);
        }
    }

    public void Continue()
    {
        if (online) 
        {
            GetComponent <GameConnect> ().Continue();
        }
        else {
            SceneManager.LoadScene (0);
        }
    }

    public int GetMaxPlayer()
    {
        int max = 0;

        if (online) {max = GetComponent<GameConnect> ().maxPlayer;}
        else {max = GetComponent<OfflineGame> ().maxPlayer;}

        return max;
    }


    
}
