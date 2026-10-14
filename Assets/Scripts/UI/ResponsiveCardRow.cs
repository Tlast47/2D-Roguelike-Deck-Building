using UnityEngine;
using UnityEngine.UI;

// Keeps card proportions identical while fitting the entire row inside its reserved area.
[RequireComponent(typeof(RectTransform))]
public class ResponsiveCardRow : MonoBehaviour
{
    [SerializeField] private Vector2 cardSize = new Vector2(220f, 330f);
    [SerializeField] private float spacing = 20f;
    [Min(0)]
    [SerializeField] private int fixedSizeReferenceCount;
    private RectTransform area;
    private bool dirty = true;
    private Vector2 previousSize;
    private int previousCount = -1;

    private void OnEnable()
    {
        area = (RectTransform)transform;
        foreach (LayoutGroup layout in GetComponents<LayoutGroup>()) layout.enabled = false;
        dirty = true;
        Canvas.willRenderCanvases += RefreshLayout;
    }

    private void OnDisable() { Canvas.willRenderCanvases -= RefreshLayout; }
    private void OnTransformChildrenChanged() { dirty = true; }
    private void OnRectTransformDimensionsChange() { dirty = true; }
    private void LateUpdate() { RefreshLayout(); }

    private void RefreshLayout()
    {
        if (area == null || !isActiveAndEnabled) return;
        Vector2 size = area.rect.size;
        int count = 0;
        foreach (Transform child in transform)
            if (child.gameObject.activeSelf && child is RectTransform) count++;
        if (!dirty && size == previousSize && count == previousCount) return;
        if (size.x <= 0f || size.y <= 0f) return;
        previousSize = size;
        previousCount = count;
        dirty = false;
        if (count == 0) return;

        // A fixed reference keeps hand cards the same size after playing a card.
        int sizingCount = fixedSizeReferenceCount > 0 ? fixedSizeReferenceCount : count;
        float totalWidth = sizingCount * cardSize.x + (sizingCount - 1) * spacing;
        float scale = Mathf.Min(1f, size.x / totalWidth, size.y / cardSize.y);
        float step = (cardSize.x + spacing) * scale;
        // Extra cards overlap instead of becoming smaller or extending outside the hand.
        if (fixedSizeReferenceCount > 0 && count > 1)
            step = Mathf.Min(step, Mathf.Max(0f, (size.x - cardSize.x * scale) / (count - 1)));
        int index = 0;
        foreach (Transform child in transform)
        {
            RectTransform card = child as RectTransform;
            if (card == null || !child.gameObject.activeSelf) continue;
            card.anchorMin = card.anchorMax = new Vector2(0.5f, 0.5f);
            card.pivot = new Vector2(0.5f, 0.5f);
            card.sizeDelta = cardSize;
            card.localScale = Vector3.one * scale;
            card.anchoredPosition = new Vector2((index - (count - 1) * 0.5f) * step, 0f);
            index++;
        }
    }
}

