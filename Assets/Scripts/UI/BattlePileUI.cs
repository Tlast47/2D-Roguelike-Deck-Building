using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattlePileUI : MonoBehaviour
{
    private DeckManager deck;
    private RunManager run;
    private RectTransform canvas;
    private RectTransform browser;
    private Button deckButton;
    private Button drawButton;
    private Button discardButton;
    private int lastDeck = -1, lastDraw = -1, lastDiscard = -1;
    public bool IsOpen => browser != null && browser.gameObject.activeInHierarchy;

    public void Setup(DeckManager deckManager, RectTransform rootCanvas)
    {
        deck = deckManager;
        canvas = rootCanvas;
        run = FindFirstObjectByType<RunManager>();
        RectTransform root = EconomyUI.Rect("Battle Card Piles", canvas, 0f, 0f, 1f, 1f);
        deckButton = EconomyUI.Button("View Deck", root, "Deck", 0.30f, 0.065f, 0.47f, 0.12f, () => Open("Your deck", run?.CurrentRun?.Deck.Cards, false));
        drawButton = EconomyUI.Button("View Draw Pile", root, "Draw", 0.50f, 0.065f, 0.67f, 0.12f, () => Open("Draw pile", deck.DrawPile, true));
        discardButton = EconomyUI.Button("View Discard Pile", root, "Discard", 0.70f, 0.065f, 0.87f, 0.12f, () => Open("Discard pile", deck.DiscardPile, false));
    }

    private void Update()
    {
        if (deck == null || deckButton == null) return;
        int owned = run?.CurrentRun?.Deck.Count ?? 0;
        int draw = deck.DrawPile.Count;
        int discard = deck.DiscardPile.Count;
        if (owned != lastDeck) { lastDeck = owned; EconomyUI.ButtonText(deckButton, "Deck (" + owned + ")"); }
        if (draw != lastDraw) { lastDraw = draw; EconomyUI.ButtonText(drawButton, "Draw (" + draw + ")"); }
        if (discard != lastDiscard) { lastDiscard = discard; EconomyUI.ButtonText(discardButton, "Discard (" + discard + ")"); }
    }

    private void Open(string title, IReadOnlyList<CardData> cards, bool hideOrder)
    {
        if (IsOpen) return;
        List<CardData> snapshot = new List<CardData>();
        if (cards != null)
            foreach (CardData card in cards) if (card != null) snapshot.Add(card);
        // Browsing never changes the live pile or consumes random numbers.
        snapshot.Sort((a, b) => string.CompareOrdinal(a.CardName, b.CardName));
        browser = CardCollectionUI.Window(canvas, title + " (" + snapshot.Count + ")", out RectTransform panel);
        TMP_Text subtitle = EconomyUI.Label("Subtitle", panel,
            hideOrder ? "Sorted by name. This does not reveal the draw order." : "Each card copy is shown separately.",
            0.05f, 0.81f, 0.95f, 0.89f, 22f);
        subtitle.alignment = TextAlignmentOptions.Center;
        RectTransform content = CardCollectionUI.Grid(panel);
        foreach (CardData card in snapshot) CardCollectionUI.Card(content, card);
        if (snapshot.Count == 0)
        {
            TMP_Text empty = EconomyUI.Label("Empty", panel, "No cards in this pile.", 0.15f, 0.42f, 0.85f, 0.58f, 28f);
            empty.alignment = TextAlignmentOptions.Center;
        }
        EconomyUI.Button("Close", panel, "Back to battle", 0.35f, 0.04f, 0.65f, 0.12f, Close);
    }

    private void Close() { EconomyUI.Close(browser); browser = null; }
    private void OnDestroy() { Close(); }
}
