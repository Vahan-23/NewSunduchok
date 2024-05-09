using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static CardDeck;

public class CardClickHandler : MonoBehaviour
{

    [SerializeField]private Card parentCard;
    private List<Card> player1Hand;
    private List<Card> player2Hand;
    //public UnityEvent<int> OnClickedOnCard;
    private CardDeck cardDeck;
    public CardSound cardSound;

    public void Initialize(Card parentCard, List<Card> player1Hand, List<Card> player2Hand, CardDeck cardDeck, CardSound cardSound)
    {
        this.parentCard = parentCard;
        this.player1Hand = player1Hand;
        this.player2Hand = player2Hand;
        this.cardDeck = cardDeck;


    }


    public void OnMouseDown()
    {
        if (parentCard != null)
        {
            //this.cardSound = gameObject.GetComponent<CardSound>();

            cardDeck.PlayerMove(player1Hand, player2Hand, (int)parentCard.value);
            
            changeColor(Color.gray);

            Debug.Log("??????? ?????" + parentCard.value);
        }
        else
        {
            Debug.LogError("parentCard is null in OnMouseDown");
        }
    }

    public void OnMouseUp()
    {
        //cardDeck.changeColor((int)parentCard.posPointIndex, Color.white);
        changeColor(Color.white);
    }

    private void changeColor(Color color)
    {
        gameObject.GetComponent<SpriteRenderer>().DOColor(color, 0.5f);
    }

}
