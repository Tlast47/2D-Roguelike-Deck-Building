using UnityEngine;
using UnityEngine.UI;

// Fits a consistent card grid to the viewport without stretching individual cards.
[RequireComponent(typeof(GridLayoutGroup))]
public class ResponsiveCardGrid : MonoBehaviour
{
    private GridLayoutGroup grid;
    private RectTransform viewport;
    private float lastWidth = -1f;

    private void OnEnable()
    {
        grid = GetComponent<GridLayoutGroup>();
        viewport = transform.parent as RectTransform;
        Canvas.willRenderCanvases += Refresh;
    }
    private void OnDisable() { Canvas.willRenderCanvases -= Refresh; }
    private void LateUpdate() { Refresh(); }
    private void Refresh()
    {
        if (grid == null || viewport == null) return;
        float width = viewport.rect.width;
        if (width <= 0f || Mathf.Approximately(width, lastWidth)) return;
        lastWidth = width;
        float usable = Mathf.Max(1f, width - grid.padding.horizontal);
        int columns = Mathf.Clamp(Mathf.FloorToInt((usable + grid.spacing.x) / (200f + grid.spacing.x)), 1, 4);
        float cardWidth = Mathf.Min(220f, (usable - (columns - 1) * grid.spacing.x) / columns);
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = columns;
        grid.cellSize = new Vector2(cardWidth, cardWidth * 1.5f);
    }
}
