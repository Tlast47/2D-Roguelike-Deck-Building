using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [Header("Shop Cards")]
    [SerializeField] private CardData[] shopCards;
    [Min(2)] [SerializeField] private int shopCardCount = 3;
    [Header("Shop UI")]
    [SerializeField] private GameObject shopCardPrefab;
    [SerializeField] private Transform shopCardContainer;
    private RunManager runManager;
    private ShopInventory inventory;
    private ShopEconomyUI economyUI;
    private RunData Run => runManager != null ? runManager.CurrentRun : null;
    public IReadOnlyList<ShopRelicOffer> RelicOffers => inventory.Relics.AsReadOnly();
    public int RemovalPrice => EconomyBalance.RemovalBasePrice + (Run?.CardsRemoved ?? 0) * EconomyBalance.RemovalPriceIncrease;
    public bool RemovalUsed => inventory != null && inventory.RemovalUsed;
    public PlayerDeck Deck => Run?.Deck;
    public int Gold => Run?.Gold ?? 0;

    private void Start()
    {
        runManager = FindFirstObjectByType<RunManager>();
        if (Run == null || shopCardPrefab == null || shopCardContainer == null)
        {
            Debug.LogError("ShopManager: run or shop UI references are missing.");
            return;
        }
        inventory = Run.GetShop(Run.CurrentNode);
        if (inventory == null)
        {
            inventory = CreateStock();
            Run.SetShop(Run.CurrentNode, inventory);
        }
        BuildCards();
        economyUI = gameObject.AddComponent<ShopEconomyUI>();
        economyUI.Setup(this, shopCardContainer as RectTransform);
    }

    private ShopInventory CreateStock()
    {
        ShopInventory stock = new ShopInventory();
        List<CardData> families = new List<CardData>();
        if (shopCards != null)
            foreach (CardData card in shopCards)
                if (card != null && card.AvailableInCardPools && !card.IsUpgraded && !families.Contains(card))
                    families.Add(card);
        int count = Mathf.Min(Mathf.Max(2, shopCardCount), families.Count);
        HashSet<int> prices = new HashSet<int>();
        for (int i = 0; i < count; i++)
        {
            // Reserve an upgraded family for slot two so every normal shop has both variants.
            bool upgraded = i == 1 || (i > 1 && Random.value < 0.25f);
            List<CardData> choices = families.FindAll(c => !upgraded ||
                (c.UpgradedCard != null && c.UpgradedCard.AvailableInCardPools));
            if (choices.Count == 0) { upgraded = false; choices.AddRange(families); }
            if (i == 0 && count > 1 && families.Exists(c => c.UpgradedCard != null && c.UpgradedCard.AvailableInCardPools))
            {
                List<CardData> safe = choices.FindAll(c => families.Exists(other => other != c &&
                    other.UpgradedCard != null && other.UpgradedCard.AvailableInCardPools));
                if (safe.Count > 0) choices = safe;
            }
            CardData family = choices[Random.Range(0, choices.Count)];
            families.Remove(family);
            CardData selected = upgraded ? family.UpgradedCard : family;
            int price = EconomyBalance.CardPrice(selected);
            while (!prices.Add(price)) price += 5;
            stock.Cards.Add(new ShopCardOffer { Card = selected, Price = price });
        }
        List<RelicData> relics = RelicCatalog.Available(Run.Relics);
        for (int i = 0; i < EconomyBalance.RelicsPerShop && relics.Count > 0; i++)
        {
            int index = Random.Range(0, relics.Count);
            RelicData relic = relics[index];
            stock.Relics.Add(new ShopRelicOffer { Relic = relic, Price = relic.ShopPrice });
            relics.RemoveAt(index);
        }
        return stock;
    }

    private void BuildCards()
    {
        foreach (Transform child in shopCardContainer)
        {
            child.gameObject.SetActive(false);
            Destroy(child.gameObject);
        }
        foreach (ShopCardOffer offer in inventory.Cards)
        {
            ShopCardUI view = Instantiate(shopCardPrefab, shopCardContainer).GetComponent<ShopCardUI>();
            if (view != null) view.Setup(offer.Card, offer.Price, this);
        }
    }

    private ShopCardOffer FindCard(CardData card) => inventory?.Cards.Find(offer => offer.Card == card);
    public bool IsCardSold(CardData card) => FindCard(card)?.Sold ?? false;
    public bool CanBuyCard(CardData card)
    {
        ShopCardOffer offer = FindCard(card);
        return Run != null && offer != null && !offer.Sold && Run.Gold >= offer.Price;
    }

    public bool BuyCard(CardData card)
    {
        if (!CanBuyCard(card)) return false;
        ShopCardOffer offer = FindCard(card);
        if (!Run.SpendGold(offer.Price)) return false;
        offer.Sold = true;
        Run.Deck.AddCard(card);
        RefreshShop();
        return true;
    }

    public bool CanBuyRelic(ShopRelicOffer offer)
    {
        return Run != null && offer != null && inventory.Relics.Contains(offer) && !offer.Sold &&
            offer.Relic != null && !Run.Relics.Contains(offer.Relic) && Run.Gold >= offer.Price;
    }

    public bool BuyRelic(ShopRelicOffer offer)
    {
        if (!CanBuyRelic(offer) || !Run.SpendGold(offer.Price)) return false;
        if (!Run.Relics.TryAdd(offer.Relic)) { Run.AddGold(offer.Price); return false; }
        offer.Sold = true;
        RefreshShop();
        return true;
    }

    public bool CanRemoveCard => Run != null && inventory != null && !inventory.RemovalUsed &&
        Run.Deck.Count > 1 && Run.Gold >= RemovalPrice;

    public bool RemoveCard(int index, CardData expectedCard)
    {
        if (!CanRemoveCard || expectedCard == null || Run.Deck.GetCard(index) != expectedCard) return false;
        int price = RemovalPrice;
        if (!Run.SpendGold(price)) return false;
        if (!Run.Deck.RemoveCardAt(index)) { Run.AddGold(price); return false; }
        inventory.RemovalUsed = true;
        Run.RecordCardRemoval();
        RefreshShop();
        return true;
    }

    private void RefreshShop()
    {
        foreach (ShopCardUI view in shopCardContainer.GetComponentsInChildren<ShopCardUI>()) view.RefreshBuyButton();
        if (economyUI != null) economyUI.Refresh();
    }

    public void LeaveShop() { if (Run != null) runManager.ReturnToMap(); }
}
