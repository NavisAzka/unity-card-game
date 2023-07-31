using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Layout : MonoBehaviour
{
    public Transform deck, buangan;
    public Text nameText;
    public Transform initialPos;

    public bool isJoker;
    public bool center = false;
    [SerializeField]float childSizeX;
    [SerializeField]float marginDeck;
    [SerializeField]float spacingY;
    void Start()
    {
        
        StartCoroutine (UpdateDeck());

    }

    IEnumerator UpdateDeck ()
    {
        yield return new WaitForFixedUpdate();

        if (center)
        {
            deck.GetComponent<GridLayoutGroup>().spacing = new Vector2 (-15,spacingY);
        }
        else
        {
            if (childSizeX * deck.childCount > marginDeck)
            {
                deck.GetComponent<GridLayoutGroup>().spacing = new Vector2 (((childSizeX * deck.childCount) -  marginDeck) / deck.childCount * -1,spacingY);
            }
            else
            {
                deck.GetComponent<GridLayoutGroup>().spacing = new Vector2 ( -2,spacingY);
            }
        }

        StartCoroutine (UpdateDeck());
    }


}
