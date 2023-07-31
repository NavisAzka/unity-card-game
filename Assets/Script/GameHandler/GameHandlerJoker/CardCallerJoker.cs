using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardCallerJoker : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        PlayerJoker playerJoker = GameObject.Find("Master").GetComponent<GameHandler> ().PemainList[0].GetComponent<PlayerJoker> ();
        playerJoker.ClickedCard(GetComponent<CardSc>());
    }
}
