using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [Header("Shop Cards")]
    [SerializeField] private CardData[] shopCards;
    [Min(1)]
    [SerializeField] private int shopCardCount = 3;

    private readonly List<CardData> currentStock = new List<CardData>();
    private readonly HashSet<CardData> purchasedCards = new HashSet<CardData>();

    [Header("Card Prices")]
    [SerializeField] private int cardPrice = 50;

    [Header("Shop UI")]
    [SerializeField] private GameObject shopCardPrefab;
    [SerializeField] private Transform shopCardContainer;

    private RunManager runManager;

    private void Start()
    {
        runManager = FindFirstObjectByType<RunManager>();

        if (runManager == null)
        {
            Debug.LogError("ShopManager : RunManager not found");
            return;
        }

        if (runManager.CurrentRun == null)
        {
            Debug.LogError("ShopManager : CurrentRun is null");
            return;
        }

        Debug.Log("ShopManager Ready");
        Debug.Log("Player Gold : " + runManager.CurrentRun.Gold);

        BuildShop();
        //TestBuyCard();
    }


    public bool CanBuyCard(CardData card)
    {
        if (card == null)
            return false;

        if (runManager == null)
            return false;

        if (runManager.CurrentRun == null)
            return false;

        return currentStock.Contains(card) &&
               !purchasedCards.Contains(card) &&
               runManager.CurrentRun.Gold >= cardPrice;
    }


    public bool BuyCard(CardData card)
    {
        if (!CanBuyCard(card))
        {
            Debug.Log("Cannot Buy Card : " + 
                (card != null ? card.CardName : "NULL"));

            return false;
        }

        if (!runManager.CurrentRun.SpendGold(cardPrice))
            return false;

        purchasedCards.Add(card);

        runManager.CurrentRun.Deck.AddCard(card);

        foreach (ShopCardUI cardUI in shopCardContainer.GetComponentsInChildren<ShopCardUI>())
        {
            cardUI.RefreshBuyButton();
        }

        Debug.Log(
            "Card Purchased : " +
            card.CardName +
            " | Price : " +
            cardPrice
        );

        Debug.Log(
            "Player Deck Count : " +
            runManager.CurrentRun.Deck.Count
        );

        Debug.Log(
            "Player Gold : " +
            runManager.CurrentRun.Gold
        );

        return true;
    }

    private void BuildShop()
    {
        if (shopCardPrefab == null)
        {
            Debug.LogError("ShopManager : Shop Card Prefab is missing");
            return;
        }

        if (shopCardContainer == null)
        {
            Debug.LogError("ShopManager : Shop Card Container is missing");
            return;
        }

        if (shopCards == null || shopCards.Length == 0)
        {
            Debug.LogWarning("ShopManager : No shop cards assigned");
            return;
        }

        currentStock.Clear();
        purchasedCards.Clear();
        List<CardData> availableCards = new List<CardData>();
        foreach (CardData card in shopCards)
        {
            if (card != null && card.AvailableInCardPools && !availableCards.Contains(card))
            {
                availableCards.Add(card);
            }
        }

        int stockCount = Mathf.Min(Mathf.Max(1, shopCardCount), availableCards.Count);
        for (int i = 0; i < stockCount; i++)
        {
            int index = Random.Range(0, availableCards.Count);
            currentStock.Add(availableCards[index]);
            availableCards.RemoveAt(index);
        }

        foreach (CardData card in currentStock)
        {
            if (card == null)
                continue;

            GameObject cardObject =
                Instantiate(shopCardPrefab, shopCardContainer);

            ShopCardUI cardUI =
                cardObject.GetComponent<ShopCardUI>();

            if (cardUI == null)
            {
                Debug.LogError(
                    "ShopManager : Shop Card Prefab does not have ShopCardUI"
                );

                continue;
            }

            cardUI.Setup(card, cardPrice, this);
        }

        Debug.Log("Shop Built : " + currentStock.Count + " cards");
    }

    public void LeaveShop()
    {
        if (runManager == null)
            return;
    
        runManager.ReturnToMap();
    }

    private void TestBuyCard()
    {
        if (shopCards == null || shopCards.Length == 0)
        {
            Debug.LogWarning("ShopManager : No shop cards assigned");
            return;
        }

        BuyCard(shopCards[0]);
    }

}