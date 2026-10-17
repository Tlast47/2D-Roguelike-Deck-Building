using TMPro;
using UnityEngine;

public class RestUpgradeUI : MonoBehaviour
{
    private RestManager manager;
    private Transform canvas;
    private RectTransform browser;
    private RectTransform preview;

    public void Open(RestManager rest, PlayerDeck deck, Transform parent)
    {
        if (browser != null) return;
        manager = rest;
        canvas = parent;
        browser = CardCollectionUI.Window(canvas, "Upgrade a card", out RectTransform panel);
        RectTransform content = CardCollectionUI.Grid(panel);
        int eligible = 0;
        for (int i = 0; i < deck.Count; i++)
        {
            int index = i;
            CardData card = deck.GetCard(index);
            if (card == null || card.UpgradedCard == null) continue;
            eligible++;
            CardCollectionUI.Card(content, card, "Preview upgrade", () => Preview(index, card));
        }
        TMP_Text subtitle = EconomyUI.Label("Subtitle", panel,
            eligible + " upgradable cards / " + deck.Count + " in deck. Preview before confirming.",
            0.05f, 0.81f, 0.95f, 0.89f, 22f);
        subtitle.alignment = TextAlignmentOptions.Center;
        if (eligible == 0)
            EconomyUI.Label("Empty", panel, "All your cards are already upgraded.", 0.15f, 0.42f, 0.85f, 0.58f);
        EconomyUI.Button("Back", panel, "Back to rest", 0.35f, 0.04f, 0.65f, 0.12f, Close);
    }

    private void Preview(int index, CardData card)
    {
        if (preview != null || card == null || card.UpgradedCard == null) return;
        preview = CardCollectionUI.Window(canvas, "Confirm upgrade", out RectTransform panel);
        TMP_Text before = EconomyUI.Label("Before", panel, "CURRENT", 0.12f, 0.79f, 0.48f, 0.88f, 23f);
        before.alignment = TextAlignmentOptions.Center;
        TMP_Text after = EconomyUI.Label("After", panel, "UPGRADED", 0.52f, 0.79f, 0.88f, 0.88f, 23f);
        after.alignment = TextAlignmentOptions.Center;
        after.color = new Color(1f, 0.85f, 0.40f);
        RectTransform left = EconomyUI.Rect("Current Card", panel, 0.10f, 0.21f, 0.48f, 0.78f);
        RectTransform right = EconomyUI.Rect("Upgraded Card", panel, 0.52f, 0.21f, 0.90f, 0.78f);
        left.gameObject.AddComponent<ResponsiveCardRow>();
        right.gameObject.AddComponent<ResponsiveCardRow>();
        CardCollectionUI.Card(left, card);
        CardCollectionUI.Card(right, card.UpgradedCard);
        EconomyUI.Button("Back", panel, "Choose another", 0.10f, 0.05f, 0.43f, 0.13f, ClosePreview);
        EconomyUI.Button("Upgrade", panel, "Confirm upgrade", 0.57f, 0.05f, 0.90f, 0.13f, () =>
        {
            if (manager.TryUpgradeCard(index, card)) Close();
            else ClosePreview();
        });
    }

    private void ClosePreview() { EconomyUI.Close(preview); preview = null; }
    private void Close() { ClosePreview(); EconomyUI.Close(browser); browser = null; }
    private void OnDestroy() { Close(); }
}
