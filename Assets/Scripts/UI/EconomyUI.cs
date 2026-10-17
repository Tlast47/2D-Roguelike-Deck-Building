using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Shared small UI primitives use the same canvas units and anchors as the existing scenes.
public static class EconomyUI
{
    public static readonly Color PanelColor = new Color(0.12f, 0.19f, 0.30f, 0.98f);
    public static RectTransform Rect(string name, Transform parent, float x0, float y0, float x1, float y1)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform));
        obj.layer = parent.gameObject.layer;
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        SetRect(rect, x0, y0, x1, y1);
        return rect;
    }

    public static void SetRect(RectTransform rect, float x0, float y0, float x1, float y1)
    {
        rect.anchorMin = new Vector2(x0, y0);
        rect.anchorMax = new Vector2(x1, y1);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.offsetMin = rect.offsetMax = Vector2.zero;
    }

    public static RectTransform Panel(string name, Transform parent, float x0, float y0, float x1, float y1, Color color)
    {
        RectTransform rect = Rect(name, parent, x0, y0, x1, y1);
        Image image = rect.gameObject.AddComponent<Image>();
        image.color = color;
        return rect;
    }

    public static TMP_Text Label(string name, Transform parent, string text,
        float x0, float y0, float x1, float y1, float fontSize = 24f)
    {
        RectTransform rect = Rect(name, parent, x0, y0, x1, y1);
        TextMeshProUGUI label = rect.gameObject.AddComponent<TextMeshProUGUI>();
        label.font = TMP_Settings.defaultFontAsset;
        label.text = text;
        label.color = Color.white;
        label.fontSize = fontSize;
        label.enableAutoSizing = true;
        label.fontSizeMax = fontSize;
        label.fontSizeMin = Mathf.Min(16f, fontSize);
        label.alignment = TextAlignmentOptions.MidlineLeft;
        label.overflowMode = TextOverflowModes.Truncate;
        label.raycastTarget = false;
        return label;
    }

    public static Button Button(string name, Transform parent, string text,
        float x0, float y0, float x1, float y1, UnityAction action)
    {
        RectTransform rect = Panel(name, parent, x0, y0, x1, y1, new Color(0.30f, 0.42f, 0.58f));
        Button button = rect.gameObject.AddComponent<Button>();
        button.targetGraphic = rect.GetComponent<Image>();
        TMP_Text label = Label("Label", rect, text, 0.04f, 0.06f, 0.96f, 0.94f, 22f);
        label.alignment = TextAlignmentOptions.Center;
        button.onClick.AddListener(action);
        return button;
    }

    public static void ButtonText(Button button, string text)
    {
        TMP_Text label = button.GetComponentInChildren<TMP_Text>();
        if (label != null) label.text = text;
    }

    public static RectTransform Modal(Transform parent, string title, out RectTransform panel)
    {
        RectTransform overlay = Panel("Modal Overlay", parent, 0f, 0f, 1f, 1f, new Color(0f, 0f, 0f, 0.75f));
        overlay.SetAsLastSibling();
        panel = Panel("Modal", overlay, 0.08f, 0.08f, 0.92f, 0.92f, PanelColor);
        TMP_Text heading = Label("Title", panel, title, 0.05f, 0.89f, 0.95f, 0.98f, 32f);
        heading.alignment = TextAlignmentOptions.Center;
        return overlay;
    }

    public static RectTransform Scroll(Transform parent, float x0, float y0, float x1, float y1, bool gridLayout = false)
    {
        RectTransform root = Panel("Scroll View", parent, x0, y0, x1, y1, new Color(0.07f, 0.12f, 0.20f));
        ScrollRect scroll = root.gameObject.AddComponent<ScrollRect>();
        scroll.horizontal = false;
        scroll.vertical = true;
        scroll.movementType = ScrollRect.MovementType.Clamped;
        scroll.scrollSensitivity = 35f;
        RectTransform viewport = Rect("Viewport", root, 0f, 0f, 0.975f, 1f);
        viewport.gameObject.AddComponent<RectMask2D>();
        RectTransform content = Rect("Content", viewport, 0f, 1f, 1f, 1f);
        content.pivot = new Vector2(0.5f, 1f);
        if (!gridLayout)
        {
            VerticalLayoutGroup layout = content.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(12, 12, 12, 12);
            layout.spacing = 12f;
            layout.childControlWidth = layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
        }
        ContentSizeFitter fitter = content.gameObject.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        scroll.viewport = viewport;
        scroll.content = content;
        RectTransform barRect = Panel("Scrollbar", root, 0.98f, 0f, 1f, 1f, new Color(0.10f, 0.16f, 0.25f));
        RectTransform handle = Panel("Handle", barRect, 0f, 0f, 1f, 1f, new Color(0.46f, 0.60f, 0.76f));
        Scrollbar scrollbar = barRect.gameObject.AddComponent<Scrollbar>();
        scrollbar.direction = Scrollbar.Direction.BottomToTop;
        scrollbar.handleRect = handle;
        scrollbar.targetGraphic = handle.GetComponent<Image>();
        scroll.verticalScrollbar = scrollbar;
        scroll.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
        scroll.verticalNormalizedPosition = 1f;
        return content;
    }

    public static RectTransform ListRow(Transform content, string name, float height = 110f)
    {
        RectTransform row = Panel(name, content, 0f, 0f, 1f, 1f, PanelColor);
        row.gameObject.AddComponent<LayoutElement>().preferredHeight = height;
        return row;
    }

    public static void Close(RectTransform overlay)
    {
        if (overlay == null) return;
        overlay.gameObject.SetActive(false);
        Object.Destroy(overlay.gameObject);
    }
}

