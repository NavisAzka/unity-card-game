using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuHandler : MonoBehaviour
{
    MainConnect mainConnect;
    SaveData saveData;
    [SerializeField]Text inputnickname,nicknameText, pointText;
    [SerializeField] GameObject enterNickPanel, loadingPanel, roomPanel, mainPanel;
    [SerializeField] GameObject miniLoadingPanel;
    [SerializeField] GameObject lostConnectionPanel;

    void Start()
    {
        mainConnect = GetComponent<MainConnect> ();
        saveData = GetComponent<SaveData>();
        
        // SetUp UI
        CheckingNickname();
        pointText.text = saveData.point.ToString();
        nicknameText.text = saveData.nickname;

    }

    void SetRoom()
    {
        //mainPanel.SetActive(true);
        //roomPanel.SetActive(false);
    }

    void CheckingNickname()
    {
        enterNickPanel.SetActive (saveData.nickname == "");
    }

    public void SaveNickname ()
    {
        saveData.ChangeName(inputnickname.text);
        pointText.text = saveData.point.ToString();
        nicknameText.text = saveData.nickname;
    }

    public void Disconnected ()
    {
        loadingPanel.SetActive(true);
    }

    public void LostConnection()
    {
        lostConnectionPanel.SetActive(true);
    }

    public void Connected()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        loadingPanel.SetActive(false);
    }

    public void MiniLoadingActive(bool state)
    {
        miniLoadingPanel.SetActive(state);
    }

    public void StartGame()
    {
        mainConnect.StartGame();
    }

    public void JoinRanked(int mode)
    {
        mainConnect.JoinRanked(0);
    }

    public void JoinCustom(int mode)
    {
        mainConnect.JoinCustom(mode);
    }

    public void LeaveRoom()
    {
        mainConnect.LeaveRoom();
    }

}
