using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCard : MonoBehaviour
{
    Transform tr;
    bool move = false;
    bool setParent = false;
    Pemain pemain;
    // Start is called before the first frame update
    public void Move(Transform target)
    {
        //Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        // tr is target
        Vector2 sizeCard = new Vector2(84.375f, 118.125f);
        
        tr = target;
        transform.SetParent(GameObject.Find("Canvas").transform);
        transform.GetComponent<RectTransform> ().sizeDelta = sizeCard;
        
        setParent = true;
        move = true;
    }

    public void JustMove(Transform target)
    {
        //Vector2 sizeCard = new Vector2(84.375f, 118.125f);
        
        tr = target;
        transform.SetParent(GameObject.Find("Canvas").transform);
        //transform.GetComponent<RectTransform> ().sizeDelta = sizeCard;
        
        setParent = false;
        move = true;
    }
    public void Move(Transform target, Pemain pemainP)
    {
        //Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        //
        // tr is target
        Vector2 sizeCard = new Vector2(84.375f, 118.125f);
        
        tr = target;
        pemain = pemainP;
        transform.SetParent(GameObject.Find("Canvas").transform);
        transform.SetAsFirstSibling();
        transform.GetComponent<RectTransform> ().sizeDelta = sizeCard;
        
        setParent = true;
        move = true;
    }


    // Update is called once per frame
    private void FixedUpdate() 
    {
        if (move)
        {
            float step = GameObject.Find("Canvas").GetComponent<RectTransform>().sizeDelta.y * 2f * Time.fixedDeltaTime;

            transform.position = Vector2.MoveTowards (transform.position, tr.position, step);
        }
            if (Vector2.Distance(tr.position, transform.position) < 0.01f)
            {
                move = false;
                
                if (setParent)
                {
                    transform.SetParent(tr, false);
                }
                //Debug.Log (" +++++++++++++++++++++++++++++++++++++++ ");
                if (pemain != null) {pemain.GetComponent<Player41> ().NextMove();}
                Destroy(GetComponent<MoveCard>());
            }
    }
}
