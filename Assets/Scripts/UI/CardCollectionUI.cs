using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class CardCollectionUI
{
    public static RectTransform Window(Transform parent, string title, out RectTransform panel)
    {
        RectTransform overlay = EconomyUI.Modal(parent, title, out panel);
        EconomyUI.SetRect(panel, 0.12f, 0.08f, 0.88f, 0.92f);
        return overlay;
    }

    public static RectTransform Grid(Transform parent, float x0 = 0.05f, float y0 = 0.18f,
        float x1 = 0.95f, float y1 = 0.80f)
    {
        RectTransform content = EconomyUI.Scroll(parent, x0, y0, x1, y1, true);
        GridLayoutGroup grid = content.gameObject.AddComponent<GridLayoutGroup>();
        grid.padding = new RectOffset(24, 24, 24, 24);
        grid.spacing = new Vector2(22f, 24f);
        grid.childAlignment = TextAnchor.UpperCenter;
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 4;
        grid.cellSize = new Vector2(220f, 330f);
        content.gameObject.AddComponent<ResponsiveCardGrid>();
        return content;
    }

    public static CardTileView Card(Transform parent, CardData card, string action = null, UnityAction onSelect = null)
    {
        RectTransform rect = EconomyUI.Rect(card.CardName, parent, 0.5f, 0.5f, 0.5f, 0.5f);
        rect.sizeDelta = new Vector2(220f, 330f);
        CardTileView tile = rect.gameObject.AddComponent<CardTileView>();
        tile.Initialize(card, action, onSelect);
        return tile;
    }
}

