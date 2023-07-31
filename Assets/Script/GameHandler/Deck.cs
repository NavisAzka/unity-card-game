using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public Vector2 cardSize;
    public Sprite backSprite, spadeIco, clubIco, diamondIco, heartIco;
    public Color red,black;
    
    [SerializeField] GameObject cardsPrefab;
    public int[] shuffleCode;

    // Start is called before the first frame update
    private void FixedUpdate() 
    {
        
    }

    public void GenerateShuffleCode()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        for (int i = 0; i < shuffleCode.Length ; i ++)
        {
            int temp = shuffleCode[i];
            int indexRandom = Random.Range(0, shuffleCode.Length - 1);
            shuffleCode[i] = shuffleCode[indexRandom];
            shuffleCode[indexRandom] = temp;
        }
    }

    public void ShuffleCard(int[] shuffleCodes)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " > " + this + " > " + gameObject.name);
        for (int i = 0; i < shuffleCodes.Length; i++)
        {
            transform.GetChild(transform.childCount - 1).SetSiblingIndex(shuffleCodes[i]);
        }
    }

    public void CreateCards()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                CardSc.Symbol s = CardSc.Symbol.spade;
                if (i == 0) { s = CardSc.Symbol.spade; }
                else if (i == 1) { s = CardSc.Symbol.club; }
                else if (i == 2) { s = CardSc.Symbol.diamond; }
                else if (i == 3) { s = CardSc.Symbol.heart; }

                cardsPrefab.GetComponent<CardSc>().id = j;
                cardsPrefab.GetComponent<CardSc>().value = j;
                cardsPrefab.GetComponent<CardSc>().symbol = s;
                cardsPrefab.GetComponent<CardSc>().valueText.text = j.ToString();

                switch (cardsPrefab.GetComponent<CardSc>().symbol)
                {
                case CardSc.Symbol.spade:
                    cardsPrefab.GetComponent<CardSc>().symSprt.sprite = spadeIco;
                    cardsPrefab.GetComponent<CardSc>().symSprt.color = black;
                    cardsPrefab.GetComponent<CardSc>().valueText.color = black;
                    break;
                case CardSc.Symbol.club:
                    cardsPrefab.GetComponent<CardSc>().symSprt.sprite = clubIco;
                    cardsPrefab.GetComponent<CardSc>().symSprt.color = black;
                    cardsPrefab.GetComponent<CardSc>().valueText.color = black;
                    break;
                case CardSc.Symbol.diamond:
                    cardsPrefab.GetComponent<CardSc>().symSprt.sprite = diamondIco;
                    cardsPrefab.GetComponent<CardSc>().symSprt.color = red;
                    cardsPrefab.GetComponent<CardSc>().valueText.color = red;
                    break;
                case CardSc.Symbol.heart:
                    cardsPrefab.GetComponent<CardSc>().symSprt.sprite = heartIco;
                    cardsPrefab.GetComponent<CardSc>().symSprt.color = red;
                    cardsPrefab.GetComponent<CardSc>().valueText.color = red;
                    break;
                }
                

                cardsPrefab.gameObject.name = s.ToString() + "-" + j.ToString();
                Instantiate(cardsPrefab, transform).name = s.ToString() + "-" + j.ToString();
            }
        }
    }
}
