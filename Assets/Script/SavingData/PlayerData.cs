using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int firstTime;
    public int point;
    public bool sound;
    public string nickname;



    public PlayerData (SaveData player)
    {

        point = player.point;
        firstTime = player.firstTime;
        sound = player.sound;
        nickname = player.nickname;
    }
}
