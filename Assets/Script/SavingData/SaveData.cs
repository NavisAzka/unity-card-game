using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SaveData : MonoBehaviour
{
    public int firstTime;
    public int point;
    public bool sound;
    public string nickname;


    public void Save()
    {
        SaveSystem.Save(this);
    }

    public void Load()
    {
        PlayerData data = SaveSystem.LoadPlayer();

        if (data == null)
        {
            point = 0;
            firstTime = 0;
            sound = true;
            nickname = "";

            Save ();
        }
        else
        {
            point = data.point;
            firstTime = data.firstTime;
            sound = data.sound;     
            nickname = data.nickname;       
            
        }
    }

    private void Awake() {
        Load();

        //GameObject.Find("Sound").GetComponent<AudioSource> ().mute = sound;
    }

    public void ToggleSound(bool state)
    {
        sound = state;
        GameObject.Find("Sound").GetComponent<AudioSource> ().mute = sound;
    }

    public void ChangeName (string newName)
    {
        nickname = newName;
        Save();
    }

    public void AddPoint (int getPoint)
    {
        point += getPoint + 9;
        Save();
    }

    public void Deposit(int getPoint)
    {
        point += getPoint;
        Save();
    }
}
