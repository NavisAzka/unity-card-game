using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardCallerUmben : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        PlayerUmben playerUmben = GameObject.Find("Master").GetComponent<GameHandler> ().PemainList[0].GetComponent<PlayerUmben> ();
        playerUmben.ClickedCard(GetComponent<CardSc>());
    }
}
