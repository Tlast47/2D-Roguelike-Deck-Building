using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapUIManager : MonoBehaviour
{
    [SerializeField] private RunManager runManager;
    [SerializeField] private GameObject mapNodePrefab;
    [SerializeField] private Transform mapNodeContainer;
    [SerializeField] private GameObject mapLayerPrefab;
    [SerializeField] private RectTransform connectionLayer;
    [SerializeField] private GameObject mapConnectionPrefab;
    [SerializeField] private ScrollRect mapScrollRect;

    private readonly Dictionary<MapNode, RectTransform> nodeUIMap = new Dictionary<MapNode, RectTransform>();
    private readonly List<MapConnectionUI> connections = new List<MapConnectionUI>();
    private MapData displayedMap;
    private float rowSpacing;
    private float bottomMargin;
    private float halfWidth;
    private Vector2 roomSize;
    private Vector2 lastContainerSize;

    private void Start()
    {
        if (mapScrollRect != null)
        {
            mapScrollRect.vertical = true;
            mapScrollRect.horizontal = false;
        }
        RefreshMapUI();
    }

    private void LateUpdate()
    {
        RectTransform container = mapNodeContainer as RectTransform;
        if (displayedMap == null || container == null || container.rect.size == lastContainerSize) return;
        int highestLayer = 1;
        foreach (MapNode node in displayedMap.Nodes)
            highestLayer = Mathf.Max(highestLayer, node.LayerIndex);
        ResizeMapContainer(container, highestLayer);
        // Reuse generated positions and existing connections when the viewport changes.
        foreach (KeyValuePair<MapNode, RectTransform> item in nodeUIMap)
        {
            MapNode node = item.Key;
            RectTransform room = item.Value;
            if (room == null) continue;
            RectTransform row = room.parent as RectTransform;
            if (row == null) continue;
            row.anchoredPosition = new Vector2(0f, bottomMargin + (node.LayerIndex - 1) * rowSpacing);
            row.sizeDelta = new Vector2(container.rect.width, roomSize.y);
            room.sizeDelta = roomSize;
            room.anchoredPosition = new Vector2(node.LayoutPosition.x * halfWidth,
                (node.LayoutPosition.y - (node.LayerIndex - 1)) * rowSpacing);
        }
    }

    public void SetRunManager(RunManager targetRunManager)
    {
        if (targetRunManager != null) runManager = targetRunManager;
    }

    public void RefreshMapUI()
    {
        if (runManager == null || runManager.CurrentRun == null ||
            runManager.CurrentRun.CurrentMap == null) return;
        if (displayedMap != runManager.CurrentRun.CurrentMap) BuildMapUI();
        else UpdateNodeStates();
    }

    private void BuildMapUI()
    {
        RectTransform container = mapNodeContainer as RectTransform;
        if (container == null || mapNodePrefab == null || mapLayerPrefab == null ||
            connectionLayer == null || mapConnectionPrefab == null)
        {
            Debug.LogError("MapUIManager: map UI references are missing.");
            return;
        }
        ClearMapUI();
        displayedMap = runManager.CurrentRun.CurrentMap;

        // Automatic layout groups would overwrite the map's persistent positions.
        foreach (LayoutGroup layout in container.GetComponents<LayoutGroup>()) layout.enabled = false;
        ContentSizeFitter fitter = container.GetComponent<ContentSizeFitter>();
        if (fitter != null) fitter.enabled = false;
        int highestLayer = 1;
        foreach (MapNode node in displayedMap.Nodes)
            highestLayer = Mathf.Max(highestLayer, node.LayerIndex);
        ResizeMapContainer(container, highestLayer);

        connectionLayer.anchorMin = Vector2.zero;
        connectionLayer.anchorMax = Vector2.one;
        connectionLayer.offsetMin = connectionLayer.offsetMax = Vector2.zero;
        connectionLayer.SetAsFirstSibling();

        for (int layer = 1; layer <= highestLayer; layer++)
        {
            List<MapNode> nodes = new List<MapNode>();
            foreach (MapNode node in displayedMap.Nodes)
                if (node.LayerIndex == layer) nodes.Add(node);
            if (nodes.Count == 0) continue;
            GameObject rowObject = Instantiate(mapLayerPrefab, container);
            rowObject.name = "Map Layer " + layer;
            foreach (LayoutGroup layout in rowObject.GetComponents<LayoutGroup>()) layout.enabled = false;
            RectTransform row = rowObject.GetComponent<RectTransform>();
            row.anchorMin = row.anchorMax = new Vector2(0.5f, 0f);
            row.pivot = new Vector2(0.5f, 0.5f);
            row.anchoredPosition = new Vector2(0f, bottomMargin + (layer - 1) * rowSpacing);
            row.sizeDelta = new Vector2(container.rect.width, roomSize.y);

            for (int i = 0; i < nodes.Count; i++)
            {
                MapNode node = nodes[i];
                if (!node.HasLayoutPosition)
                    node.SetLayoutPosition(new Vector2((i - (nodes.Count - 1) * 0.5f) * 0.54f, layer - 1));
                GameObject room = Instantiate(mapNodePrefab, row);
                room.name = node.NodeType + " " + layer + "-" + i;
                RectTransform rect = room.GetComponent<RectTransform>();
                MapNodeView view = room.GetComponent<MapNodeView>();
                if (rect == null || view == null) { Destroy(room); continue; }
                rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.sizeDelta = roomSize;
                rect.anchoredPosition = new Vector2(
                    node.LayoutPosition.x * halfWidth,
                    (node.LayoutPosition.y - (layer - 1)) * rowSpacing);
                nodeUIMap[node] = rect;
                view.Setup(node, runManager);
            }
        }
        Canvas.ForceUpdateCanvases();
        foreach (MapNode start in displayedMap.Nodes)
        {
            if (!nodeUIMap.ContainsKey(start)) continue;
            foreach (MapNode end in start.Connections)
            {
                if (!nodeUIMap.ContainsKey(end)) continue;
                GameObject line = Instantiate(mapConnectionPrefab, connectionLayer);
                MapConnectionUI view = line.GetComponent<MapConnectionUI>();
                if (view == null) { Destroy(line); continue; }
                view.Setup(nodeUIMap[start], nodeUIMap[end], start, end, runManager);
                connections.Add(view);
            }
        }
        if (mapScrollRect != null)
        {
            mapScrollRect.StopMovement();
            mapScrollRect.verticalNormalizedPosition = 0f;
        }
        Canvas.ForceUpdateCanvases();
        UpdateNodeStates();
        Debug.Log("Map UI ready: " + nodeUIMap.Count + " rooms, " +
            connections.Count + " curved routes.");
    }

    private void ResizeMapContainer(RectTransform container, int highestLayer)
    {
        if (mapScrollRect != null)
        {
            container.anchorMin = new Vector2(0f, 0f);
            container.anchorMax = new Vector2(1f, 0f);
            container.pivot = new Vector2(0.5f, 0f);
            container.sizeDelta = new Vector2(0f, 160f + (highestLayer - 1) * 180f);
        }
        else
        {
            container.anchorMin = Vector2.zero;
            container.anchorMax = Vector2.one;
            container.offsetMin = container.offsetMax = Vector2.zero;
        }
        Canvas.ForceUpdateCanvases();
        float width = Mathf.Max(100f, container.rect.width);
        float height = Mathf.Max(200f, container.rect.height);
        roomSize = new Vector2(Mathf.Min(190f, width * 0.14f), Mathf.Min(62f, height * 0.085f));
        // Reserve enough space for vertical scatter so the lowest room cannot be clipped.
        float intervals = Mathf.Max(1, highestLayer - 1);
        float scatter = 0.38f;
        float requiredMargin = (roomSize.y * 0.5f + 12f + scatter * height / intervals) /
            (1f + 2f * scatter / intervals);
        bottomMargin = Mathf.Max(height * 0.10f, requiredMargin);
        rowSpacing = (height - bottomMargin * 2f) / Mathf.Max(1, highestLayer - 1);
        halfWidth = (width - roomSize.x) * 0.5f - width * 0.05f;
        lastContainerSize = container.rect.size;
    }

    private void UpdateNodeStates()
    {
        foreach (RectTransform rect in nodeUIMap.Values)
        {
            if (rect != null) rect.GetComponent<MapNodeView>().RefreshInteractable();
        }
        foreach (MapConnectionUI line in connections)
            if (line != null) line.RefreshAppearance();
    }

    private void ClearMapUI()
    {
        if (connectionLayer != null)
        {
            foreach (Transform child in connectionLayer)
            {
                child.gameObject.SetActive(false);
                Destroy(child.gameObject);
            }
        }
        if (mapNodeContainer != null)
        {
            foreach (Transform child in mapNodeContainer)
            {
                if (child == connectionLayer) continue;
                child.gameObject.SetActive(false);
                Destroy(child.gameObject);
            }
        }
        nodeUIMap.Clear();
        connections.Clear();
        displayedMap = null;
    }

    public int GetDisplayedNodeCount() { return nodeUIMap.Count; }
    public void HideMapUI() { ClearMapUI(); }
}

