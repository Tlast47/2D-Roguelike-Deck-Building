using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [Header("Starter Deck")]
    [SerializeField] private CardData strikeCard;
    [SerializeField] private CardData defendCard;

    [Header("Draw Settings")]
    [SerializeField] private int startingHandSize = 5;

    [Header("Card Piles")]
    private List<CardData> drawPile = new();
    private List<CardData> hand = new();
    private List<CardData> discardPile = new();
    private List<CardData> exhaustPile = new();

    public void StartBattleSetup()
    {
        //TODO : clear card ก่อน run ใหม่
        CreateStarterDeck();
        ShuffleDeck();  
        SetupCards();
    }

    private void SetupCards()
    {
        BeforeDraw();
        DrawCards(GetStartingHandSize());
        AfterDraw();
    }

    public void StartPlayerTurnDraw()
    {
        //กันค่าติดลบ
        int cardsToDraw = GetStartingHandSize() - hand.Count;

        if (cardsToDraw <= 0)
        {
            return;
        }

        BeforeDraw();
        DrawCards(cardsToDraw);
        AfterDraw();
    }

    private int GetStartingHandSize()
    {
        return startingHandSize;
    }


    private void CreateStarterDeck()
    {
        for (int i = 0; i < 5; i++)
        {
            drawPile.Add(strikeCard);
        }
    
        for (int i = 0; i < 5; i++)
        {
            drawPile.Add(defendCard);
        }

        //cardDisplayManager.RefreshHand();

        //test
        Debug.Log("Starter Deck Created : " + drawPile.Count + " cards");
        //
    }

    private void ShuffleDeck()
    {
        for (int i = drawPile.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            CardData temp = drawPile[i];
            drawPile[i] = drawPile[randomIndex];
            drawPile[randomIndex] = temp;
        }
    }

    private void ReshuffleDiscardIntoDrawPile()
    {
        drawPile.AddRange(discardPile);
        discardPile.Clear();

        ShuffleDeck();

        Debug.Log("Discard reshuffled into Draw Pile");
    }

    private void BeforeDraw()
    {

    }

    private void DrawCards(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (drawPile.Count == 0)
            {
                //กรณีการสะลายหมด
                if (discardPile.Count == 0)
                {
                    return;
                }

                ReshuffleDiscardIntoDrawPile();
            }

            int lastIndex = drawPile.Count - 1;

            CardData drawnCard = drawPile[lastIndex];

            hand.Add(drawnCard);

            drawPile.RemoveAt(lastIndex);
        }

        //test
        Debug.Log("Deck Count: " + drawPile.Count);
        Debug.Log("Hand Count: " + hand.Count);
        foreach (CardData cardInHand in hand)
        {
            Debug.Log("Hand : " + cardInHand.CardName);
        }
        Debug.Log("Discard Count: " + discardPile.Count);
        //
    }

    private void AfterDraw()
    {
    
    }

    public bool CanPlayCard(CardData card)
    {
        return hand.Contains(card);
    }

    public void PlayCard(CardData card)
    {
        if (!hand.Contains(card))
        {
            return;
        }

        DiscardCard(card);

        //test
        Debug.Log("Card Played: " + card.CardName);
        Debug.Log("Hand Count: " + hand.Count);
        Debug.Log("Discard Count: " + discardPile.Count);
        //
    }

    private void DiscardCard(CardData card)
    {
        hand.Remove(card);
        discardPile.Add(card);
    }

    public IReadOnlyList<CardData> Hand => hand;
}