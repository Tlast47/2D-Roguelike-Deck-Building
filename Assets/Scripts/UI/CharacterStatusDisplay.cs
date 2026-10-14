using System.Text;
using TMPro;
using UnityEngine;

// Uses each character's existing UI labels, so all current prefabs work without new references.
public static class CharacterStatusDisplay
{
    public static string BuildText(CharacterData character)
    {
        if (character == null || character.IsDead()) return string.Empty;
        StringBuilder text = new StringBuilder();
        Append(text, "Block", character.GetBlock());
        Append(text, "Burn", character.BurnStacks);
        Append(text, "Poison", character.PoisonStacks);
        return text.ToString();
    }

    private static void Append(StringBuilder text, string label, int amount)
    {
        if (amount <= 0) return;
        if (text.Length > 0) text.Append('\n');
        text.Append(label).Append(": ").Append(amount);
    }

    public static void Refresh(CharacterData character, TMP_Text hpText,
        TMP_Text statusText, TMP_Text footerText, TMP_Text footerValue = null)
    {
        if (statusText == null) return;
        string text = BuildText(character);
        statusText.text = text;
        statusText.enabled = text.Length > 0;

        // HP, active statuses, then Energy/Intent. Empty statuses consume no rows.
        if (hpText == null) return;
        RectTransform panel = hpText.rectTransform.parent as RectTransform;
        if (panel == null || statusText.rectTransform.parent != panel) return;
        float padding = panel.rect.width * 0.05f;
        float rowHeight = Mathf.Min(hpText.fontSize * 1.35f,
            Mathf.Max(1f, panel.rect.height - padding * 2f) / 5f);
        float fontSize = Mathf.Min(hpText.fontSize, rowHeight / 1.35f);
        float width = Mathf.Max(1f, panel.rect.width - padding * 2f);
        int rows = text.Length == 0 ? 0 : text.Split('\n').Length;

        Place(hpText, panel, padding, padding, width, rowHeight, fontSize);
        Place(statusText, panel, padding, padding + rowHeight,
            width, Mathf.Max(1, rows) * rowHeight, fontSize);
        float footerY = padding + (1 + rows) * rowHeight;
        Place(footerText, panel, padding, footerY,
            footerValue == null ? width : width * 0.7f, rowHeight, fontSize);
        Place(footerValue, panel, padding + width * 0.7f, footerY,
            width * 0.3f, rowHeight, fontSize);
    }

    private static void Place(TMP_Text label, RectTransform panel,
        float x, float y, float width, float height, float fontSize)
    {
        if (label == null || label.rectTransform.parent != panel) return;
        RectTransform rect = label.rectTransform;
        rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(x, -y);
        rect.sizeDelta = new Vector2(width, height);
        label.alignment = TextAlignmentOptions.TopLeft;
        label.fontSize = fontSize;
        label.raycastTarget = false;
    }
}
