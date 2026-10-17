using TMPro;
using UnityEngine;
using UnityEngine.UI;

// One layout for shop, hand, rewards, removal, upgrades and pile inspection.
public static class CardVisualStyle
{
    public static readonly Color Background = new Color(0.56f, 0.58f, 0.63f, 1f);
    public static readonly Color LightBackground = new Color(1f, 1f, 1f, 0.392f);
    public static readonly Color UpgradedName = new Color(1f, 0.86f, 0.42f, 1f);

    public static void Apply(RectTransform root, CardData card, TMP_Text title, TMP_Text description,
        TMP_Text footer = null, string footerValue = null, Button action = null, Color? background = null)
    {
        Image image = root.GetComponent<Image>();
        if (image != null) image.color = background ?? Background;
        Label(title, 0.08f, 0.75f, 0.92f, 0.96f, 24f, TextAlignmentOptions.Center);
        if (title != null)
        {
            title.text = card.CardName;
            title.color = card.IsUpgraded ? UpgradedName : Color.white;
        }
        Label(description, 0.08f, 0.31f, 0.92f, 0.71f, 22f, TextAlignmentOptions.TopLeft);
        if (description != null) description.text = card.Description;
        Label(footer, 0.08f, 0.18f, 0.92f, 0.28f, 24f, TextAlignmentOptions.Center);
        if (footer != null && footerValue != null) footer.text = footerValue;
        if (action == null) return;
        EconomyUI.SetRect((RectTransform)action.transform, 0.08f, 0.04f, 0.92f, 0.16f);
        Image buttonImage = action.GetComponent<Image>();
        if (buttonImage != null) buttonImage.color = Color.white;
        ColorBlock colors = ColorBlock.defaultColorBlock;
        action.colors = colors;
        TMP_Text buttonText = action.GetComponentInChildren<TMP_Text>(true);
        Label(buttonText, 0f, 0f, 1f, 1f, 24f, TextAlignmentOptions.Center);
        if (buttonText != null) buttonText.color = new Color(0.196f, 0.196f, 0.196f, 1f);
    }

    private static void Label(TMP_Text label, float x0, float y0, float x1, float y1,
        float maxSize, TextAlignmentOptions alignment)
    {
        if (label == null) return;
        EconomyUI.SetRect(label.rectTransform, x0, y0, x1, y1);
        label.color = Color.white;
        label.fontSize = maxSize;
        label.enableAutoSizing = true;
        label.fontSizeMax = maxSize;
        label.fontSizeMin = maxSize == 22f ? 14f : 16f;
        label.alignment = alignment;
        label.margin = Vector4.zero;
        label.overflowMode = TextOverflowModes.Truncate;
        label.raycastTarget = false;
    }
}

