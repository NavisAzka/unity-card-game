using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardCaller41 : MonoBehaviour, IPointerClickHandler,  IUpdateSelectedHandler, IPointerDownHandler, IPointerUpHandler
{
    float counter = .75f;
    bool pressed = false;

    RectTransform rectButton;
    
    private void Start() {
        rectButton = GetComponent <RectTransform> ();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        Player41 player41 = GameObject.Find("Master").GetComponent<GameHandler> ().PemainList[0].GetComponent<Player41> ();
        player41.ClickedCard(GetComponent<CardSc>());
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pressed = true;
    }

    public void OnPointerUp(PointerEventData data)
    {
        if (counter < 0)
        {
            if (pressed)
            {
                if ((data.position.x > rectButton.position.x - (rectButton.rect.width / 1)) &
                    (data.position.x < rectButton.position.x + (rectButton.rect.width / 1)))
                {
                    if ((data.position.y > rectButton.position.y - (rectButton.rect.height / 1)) &
                        (data.position.y < rectButton.position.y + (rectButton.rect.height / 1)))
                    {
                        Player41 player41 = GameObject.Find("Master").GetComponent<GameHandler> ().PemainList[0].GetComponent<Player41> ();

                        player41.HoldedCard(GetComponent<CardSc>());
                    }
                }
            }
        }
        
        pressed = false;
        counter = .75f;
    }

    public void OnUpdateSelected(BaseEventData eventData)
    {
        Player41 player41 = GameObject.Find("Master").GetComponent<GameHandler> ().PemainList[0].GetComponent<Player41> ();

        if (pressed) 
        { 
            counter -= Time.deltaTime;

            if (counter <= 0)
            {
                player41.SemiHold(GetComponent<CardSc>());
            }
        }
    }
}
