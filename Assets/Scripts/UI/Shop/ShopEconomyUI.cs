using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopEconomyUI : MonoBehaviour
{
    private ShopManager shop;
    private RectTransform canvas;
    private Button removeButton;
    private TMP_Text removalHint;
    private TMP_Text goldBalance;
    private int lastGold = -1;
    private readonly List<CardTileView> removalTiles = new List<CardTileView>();
    private readonly List<Button> relicButtons = new List<Button>();
    private RectTransform removalModal;
    private int selectedIndex = -1;
    private CardData selectedCard;
    private Button confirmRemoval;

    public void Setup(ShopManager manager, RectTransform cards)
    {
        shop = manager;
        canvas = cards.GetComponentInParent<Canvas>().rootCanvas.transform as RectTransform;
        EconomyUI.SetRect(cards, 0.10f, 0.43f, 0.90f, 0.83f);
        RectTransform root = EconomyUI.Rect("Shop Economy", canvas, 0f, 0f, 1f, 1f);
        TMP_Text title = EconomyUI.Label("Shop Title", root, "SHOP", 0.2f, 0.86f, 0.8f, 0.94f, 40f);
        title.alignment = TextAlignmentOptions.Center;
        goldBalance = EconomyUI.Label("Shop Gold Balance", root, "", 0.14f, 0.86f, 0.37f, 0.94f, 28f);
        goldBalance.color = new Color(1f, 0.85f, 0.40f);
        TMP_Text relicHeading = EconomyUI.Label("Relics Heading", root, "RELICS", 0.15f, 0.40f, 0.85f, 0.43f, 20f);
        relicHeading.alignment = TextAlignmentOptions.Center;
        for (int i = 0; i < shop.RelicOffers.Count; i++)
        {
            ShopRelicOffer offer = shop.RelicOffers[i];
            float width = 0.68f / Mathf.Max(1, shop.RelicOffers.Count);
            float left = 0.16f + i * width;
            RectTransform panel = EconomyUI.Panel(offer.Relic.RelicName, root,
                left + 0.008f, 0.18f, left + width - 0.008f, 0.39f, EconomyUI.PanelColor);
            TMP_Text name = EconomyUI.Label("Name", panel, offer.Relic.RelicName, 0.05f, 0.73f, 0.95f, 0.96f, 25f);
            name.color = new Color(1f, 0.85f, 0.40f);
            EconomyUI.Label("Description", panel, offer.Relic.Description, 0.05f, 0.31f, 0.95f, 0.73f, 22f);
            Button buy = EconomyUI.Button("Buy Relic", panel, "", 0.05f, 0.04f, 0.95f, 0.27f, () => shop.BuyRelic(offer));
            relicButtons.Add(buy);
        }
        if (shop.RelicOffers.Count == 0)
            EconomyUI.Label("No Relics", root, "All relics collected.", 0.25f, 0.23f, 0.75f, 0.35f);
        foreach (Button button in canvas.GetComponentsInChildren<Button>())
            if (button.name == "LeaveShopButton")
            {
                RectTransform rect = button.transform as RectTransform;
                EconomyUI.SetRect(rect, 0.22f, 0.07f, 0.43f, 0.13f);
            }
        removeButton = EconomyUI.Button("Remove Card", root, "", 0.52f, 0.07f, 0.82f, 0.13f, OpenRemoval);
        removalHint = EconomyUI.Label("Removal Hint", root, "", 0.48f, 0.025f, 0.90f, 0.065f, 18f);
        removalHint.alignment = TextAlignmentOptions.Center;
        Refresh();
    }

    private void Update()
    {
        if (shop != null && goldBalance != null && lastGold != shop.Gold)
        {
            lastGold = shop.Gold;
            goldBalance.text = "Gold: " + shop.Gold;
        }
    }

    public void Refresh()
    {
        if (shop == null || removeButton == null) return;
        if (goldBalance != null) goldBalance.text = "Gold: " + shop.Gold;
        removeButton.interactable = shop.CanRemoveCard;
        EconomyUI.ButtonText(removeButton, shop.RemovalUsed ? "Removal used" : "Remove card - " + shop.RemovalPrice + " Gold");
        removalHint.text = shop.RemovalUsed ? "One removal per shop." : shop.Deck.Count <= 1 ?
            "Keep at least one card in your deck." : shop.Gold < shop.RemovalPrice ? "Not enough Gold." : "Choose a card before paying.";
        for (int i = 0; i < relicButtons.Count; i++)
        {
            ShopRelicOffer offer = shop.RelicOffers[i];
            relicButtons[i].interactable = shop.CanBuyRelic(offer);
            EconomyUI.ButtonText(relicButtons[i], offer.Sold ? "SOLD" : "Buy - " + offer.Price + " Gold");
        }
    }

    private void OpenRemoval()
    {
        if (!shop.CanRemoveCard || removalModal != null) return;
        selectedIndex = -1;
        selectedCard = null;
        removalModal = CardCollectionUI.Window(canvas, "Remove a card from your deck", out RectTransform panel);
        EconomyUI.Label("Cost", panel, "Cost: " + shop.RemovalPrice + " Gold. Select a card, then confirm.", 0.05f, 0.81f, 0.95f, 0.89f);
        removalTiles.Clear();
        RectTransform content = CardCollectionUI.Grid(panel);
        for (int i = 0; i < shop.Deck.Count; i++)
        {
            int index = i;
            CardData card = shop.Deck.GetCard(index);
            if (card == null) continue;
            CardTileView tile = CardCollectionUI.Card(content, card, "Select", () => SelectRemoval(index, card));
            removalTiles.Add(tile);
        }
        EconomyUI.Button("Cancel", panel, "Cancel", 0.05f, 0.04f, 0.25f, 0.12f, CloseRemoval);
        confirmRemoval = EconomyUI.Button("Confirm Removal", panel, "Select a card first", 0.30f, 0.04f, 0.95f, 0.12f, ConfirmRemoval);
        confirmRemoval.interactable = false;
    }

    private void SelectRemoval(int index, CardData card)
    {
        selectedIndex = index;
        selectedCard = card;
        int tileIndex = 0;
        for (int i = 0; i < shop.Deck.Count; i++)
        {
            if (shop.Deck.GetCard(i) == null) continue;
            removalTiles[tileIndex++].SetSelected(i == index);
        }
        EconomyUI.ButtonText(confirmRemoval, "Remove " + card.CardName + " - " + shop.RemovalPrice + " Gold");
        confirmRemoval.interactable = shop.CanRemoveCard;
    }

    private void ConfirmRemoval()
    {
        if (shop.RemoveCard(selectedIndex, selectedCard)) CloseRemoval();
        else if (confirmRemoval != null) confirmRemoval.interactable = false;
    }

    private void CloseRemoval()
    {
        EconomyUI.Close(removalModal);
        removalModal = null;
        selectedCard = null;
        selectedIndex = -1;
    }

    private void OnDestroy() { if (removalModal != null) EconomyUI.Close(removalModal); }
}


