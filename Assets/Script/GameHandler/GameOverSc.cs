using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverSc : MonoBehaviour
{
    public Text info;
    public Transform playerInfo;
    public GameObject pemainGameOver;
    public void Continue()
    {
        GameObject.Find ("Master").GetComponent<GameHandler> ().Continue();
    }
}
