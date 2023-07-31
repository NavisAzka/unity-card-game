using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimateLobby : MonoBehaviour
{
    [SerializeField]Transform cardDeck, selectedCard, initialPos;
    bool move;
    [SerializeField] Vector2 distance;

    private void Start() 
    {
        AmbilBawah();
    }

    public void AmbilBawah()
    {
        selectedCard = cardDeck.GetChild(Random.Range(0,40));
        StartCoroutine (GeserLuar());
        
    }

    public IEnumerator GeserLuar ()
    {  
        yield return new WaitForSeconds(0.2f);
        PindahAtas();
        yield return new WaitForFixedUpdate();
        StartCoroutine (GeserDalam());
        
    }

    public IEnumerator GeserDalam ()
    {
        yield return new WaitForFixedUpdate();
    
        
        AmbilBawah();
        
    }

    public void PindahAtas()
    {
        selectedCard.SetAsLastSibling();
    }

}
