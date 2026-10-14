using UnityEngine;
using UnityEngine.UI;

public class MapConnectionUI : MonoBehaviour
{
    [SerializeField] private Image connectionLine;
    private MapPathGraphic path;
    private RectTransform pathRect;
    private RectTransform fromRect;
    private RectTransform toRect;
    private Vector2 lastStart;
    private Vector2 lastEnd;
    private bool hasGeometry;
    private MapNode fromNode;
    private MapNode toNode;
    private RunManager runManager;

    private void OnEnable()
    {
        Canvas.willRenderCanvases += RefreshGeometry;
        hasGeometry = false;
    }

    private void OnDisable()
    {
        Canvas.willRenderCanvases -= RefreshGeometry;
        if (path != null) path.SetVisible(false);
        hasGeometry = false;
    }

    private void LateUpdate()
    {
        RefreshGeometry();
    }

    public void Setup(RectTransform from, RectTransform to,
        MapNode source = null, MapNode target = null, RunManager manager = null)
    {
        fromRect = from;
        toRect = to;
        fromNode = source;
        toNode = target;
        runManager = manager;
        hasGeometry = false;
        if (connectionLine != null) connectionLine.enabled = false;
        RectTransform rect = pathRect != null ? pathRect : transform as RectTransform;
        if (rect == null)
        {
            // The existing prefab root is a regular Transform, not a RectTransform.
            GameObject container = new GameObject("Map Path", typeof(RectTransform));
            container.layer = gameObject.layer;
            rect = container.GetComponent<RectTransform>();
            rect.SetParent(transform.parent, false);
            transform.SetParent(rect, false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.offsetMin = rect.offsetMax = Vector2.zero;
        pathRect = rect;
        path = rect.GetComponent<MapPathGraphic>();
        if (path == null) path = rect.gameObject.AddComponent<MapPathGraphic>();
        RefreshGeometry();
        RefreshAppearance();
    }

    private void RefreshGeometry()
    {
        if (path == null || pathRect == null) return;
        if (!isActiveAndEnabled || fromRect == null || toRect == null ||
            !fromRect.gameObject.activeInHierarchy || !toRect.gameObject.activeInHierarchy)
        {
            path.SetVisible(false);
            hasGeometry = false;
            return;
        }

        // Read the actual transforms again after canvas layout, including size and scale changes.
        Vector2 start = pathRect.InverseTransformPoint(fromRect.TransformPoint(fromRect.rect.center));
        Vector2 end = pathRect.InverseTransformPoint(toRect.TransformPoint(toRect.rect.center));
        Vector2 edgeStart = EdgePoint(pathRect, fromRect, end);
        Vector2 edgeEnd = EdgePoint(pathRect, toRect, start);
        if (hasGeometry && (edgeStart - lastStart).sqrMagnitude < 0.000001f &&
            (edgeEnd - lastEnd).sqrMagnitude < 0.000001f) return;

        path.SetPath(edgeStart, edgeEnd);
        lastStart = edgeStart;
        lastEnd = edgeEnd;
        hasGeometry = true;
    }

    private static Vector2 EdgePoint(RectTransform space, RectTransform room, Vector2 toward)
    {
        // Intersect in room space so a rotated or scaled room still uses its real border.
        Vector2 center = room.rect.center;
        Vector2 localTarget = room.InverseTransformPoint(space.TransformPoint(toward));
        Vector2 direction = localTarget - center;
        Vector2 half = room.rect.size * 0.5f;
        float factor = Mathf.Min(half.x / Mathf.Max(0.001f, Mathf.Abs(direction.x)),
            half.y / Mathf.Max(0.001f, Mathf.Abs(direction.y)));
        return space.InverseTransformPoint(room.TransformPoint(center + direction * factor));
    }

    public void RefreshAppearance()
    {
        if (path == null) return;
        bool available = runManager != null && runManager.CurrentRun != null &&
            runManager.CurrentRun.CurrentNode == fromNode && runManager.CanMoveToNode(toNode);
        bool visited = fromNode != null && toNode != null && fromNode.IsVisited &&
            (toNode.IsVisited || (runManager != null && runManager.CurrentRun != null &&
                runManager.CurrentRun.CurrentNode == toNode));
        path.color = available ? new Color(1f, 0.87f, 0.48f, 0.95f) :
            visited ? new Color(0.55f, 0.85f, 0.72f, 0.85f) :
            new Color(0.90f, 0.92f, 0.96f, 0.75f);
    }
}
