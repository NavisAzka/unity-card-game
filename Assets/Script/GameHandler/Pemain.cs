using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Pemain : MonoBehaviour
{
    public Player player;
    public int index = 55;
    public Layout layout;
    public string nickname;
    public bool local = false;
    public int point = 0;
    
    GameHandler GM;
    // Start is called before the first frame update

    public void Initialize() 
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        GM = GameObject.Find("Master").GetComponent<GameHandler> ();

        // Get Layout
        layout = GetComponentInChildren<Layout>(); 
        
        // Input Name Text
        layout.nameText.text = nickname;

        // input pemain selanjutnya
        //pemainSelanjutnya = GM.PemainList[nextIndex];
    }

    public Pemain pemainSelanjutnya()
    {
        //Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        int nextIndex = index + 1;
        if (nextIndex > GM.PemainList.Length - 1) { nextIndex = 0 ;}
        
        return GM.unsortedPemainList[nextIndex];
    }

    public Pemain pemainSebelumnya()
    {
        //Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        int nextIndex = index - 1;
        if (nextIndex < 0) { nextIndex = GM.PemainList.Length - 1 ;}
        
        return GM.unsortedPemainList[nextIndex];
    }

    public void CallBot()
    {
        if (GM.gameMode == GameHandler.GameMode.empatSatu)
        {
            gameObject.AddComponent<Bot41>();
            GetComponent<Bot41> ().afk = true;
            GetComponent<Bot41> ().Initialize();
        }
        else  if (GM.gameMode == GameHandler.GameMode.joker)
        {
            gameObject.AddComponent<BotJoker>();
            GetComponent<BotJoker> ().afk = true;
            GetComponent<BotJoker> ().Initialize();
        }
        else  if (GM.gameMode == GameHandler.GameMode.umben)
        {
            gameObject.AddComponent<BotUmben>();
            GetComponent<BotUmben> ().afk = true;
            GetComponent<BotUmben> ().Initialize();
        }
    }

    public void AddPoint (int getPoint)
    {
        if (local)
        {
            point = getPoint;
            GM.saveData.AddPoint(getPoint);
        }
    }
}
