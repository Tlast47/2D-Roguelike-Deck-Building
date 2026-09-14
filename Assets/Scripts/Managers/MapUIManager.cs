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

    private bool hasLoggedInitialMapUI;
    private bool hasBuiltMapUI;

    private Dictionary<MapNode, RectTransform> nodeUIMap;

    private void Start()
    {
        nodeUIMap = new Dictionary<MapNode, RectTransform>();

        if (runManager == null)
        {
            Debug.LogError("MapUIManager RunManager is not assigned");
            return;
        }

        if (mapNodePrefab == null)
        {
            Debug.LogError("MapUIManager Map Node Prefab is not assigned");
            return;
        }

        if (mapNodeContainer == null)
        {
            Debug.LogError(
                "MapUIManager Map Node Container is not assigned"
            );

            return;
        }

        if (mapLayerPrefab == null)
        {
            Debug.LogError(
                "MapUIManager Map Layer Prefab is not assigned"
            );

            return;
        }

        Debug.Log("MapUIManager Ready");
    }

    public void RefreshMapUI()
    {
        if (!hasBuiltMapUI)
        {
            BuildMapUI();
            hasBuiltMapUI = true;
            return;
        }

        UpdateNodeStates();
    }

    private void BuildMapUI()
    {
        ClearMapUI();
    
        IReadOnlyList<MapNode> allNodes =
            runManager.GetAllMapNodes();
    
        if (allNodes == null)
        {
            Debug.LogWarning("All Map Nodes is null");
            return;
        }
    
        int highestLayerIndex =
            GetHighestLayerIndex(allNodes);
    
        for (
            int layerIndex = 1;
            layerIndex <= highestLayerIndex;
            layerIndex++
        )
        {
            List<MapNode> nodesInLayer =
                GetNodesInLayer(allNodes, layerIndex);
    
            if (!hasLoggedInitialMapUI)
            {
                Debug.Log(
                    "Layer " + layerIndex +
                    " Node Count : " + nodesInLayer.Count
                );
            }
    
            if (nodesInLayer.Count == 0)
            {
                continue;
            }
    
            GameObject layerObject =
                Instantiate(
                    mapLayerPrefab,
                    mapNodeContainer
                );
    
            Transform targetContainer =
                layerObject.transform;
    
            for (int i = 0; i < nodesInLayer.Count; i++)
            {
                GameObject nodeObject =
                    Instantiate(
                        mapNodePrefab,
                        targetContainer
                    );
    
                MapNodeView nodeView =
                    nodeObject.GetComponent<MapNodeView>();
    
                RectTransform nodeRectTransform =
                    nodeObject.GetComponent<RectTransform>();
    
                if (nodeRectTransform == null)
                {
                    Debug.LogError(
                        "Map Node Prefab does not have RectTransform"
                    );
    
                    continue;
                }
    
                if (nodeView == null)
                {
                    Debug.LogError(
                        "Map Node Prefab does not have MapNodeView"
                    );
    
                    continue;
                }
    
                nodeUIMap[nodesInLayer[i]] =
                    nodeRectTransform;
    
                nodeView.Setup(
                    nodesInLayer[i],
                    runManager
                );
            }
        }
    
        Canvas.ForceUpdateCanvases();

        LayoutRebuilder.ForceRebuildLayoutImmediate(
            mapNodeContainer.GetComponent<RectTransform>()
        );
    
        CreateMapConnections(allNodes);
    
        if (!hasLoggedInitialMapUI)
        {
            Debug.Log(
                "Created All Map UI Node Count : " +
                GetDisplayedNodeCount()
            );
    
            hasLoggedInitialMapUI = true;
        }
    }

    private void UpdateNodeStates()
    {
        foreach (MapNode node in nodeUIMap.Keys)
        {
            MapNodeView nodeView =
                nodeUIMap[node].GetComponent<MapNodeView>();
    
            if (nodeView == null)
            {
                continue;
            }
    
            nodeView.RefreshInteractable();
        }
    }

    private void CreateMapConnections(
    IReadOnlyList<MapNode> allNodes)
    {
        if (connectionLayer == null)
        {
            Debug.LogError("Connection Layer is not assigned");
            return;
        }

        if (mapConnectionPrefab == null)
        {
            Debug.LogError("Map Connection Prefab is not assigned");
            return;
        }

        for (int i = 0; i < allNodes.Count; i++)
        {
            MapNode startNode = allNodes[i];

            if (!nodeUIMap.ContainsKey(startNode))
            {
                continue;
            }

            IReadOnlyList<MapNode> connections =
                startNode.Connections;

            for (int j = 0; j < connections.Count; j++)
            {
                MapNode endNode = connections[j];

                if (!nodeUIMap.ContainsKey(endNode))
                {
                    continue;
                }

                GameObject connectionObject =
                    Instantiate(
                        mapConnectionPrefab,
                        connectionLayer
                    );

                MapConnectionUI connectionUI =
                    connectionObject.GetComponent<MapConnectionUI>();

                if (connectionUI == null)
                {
                    Debug.LogError(
                        "Map Connection Prefab does not have MapConnectionUI"
                    );

                    continue;
                }

                connectionUI.Setup(
                    nodeUIMap[startNode],
                    nodeUIMap[endNode]
                    
                );
            }
        }
    }

    private int GetHighestLayerIndex(
        IReadOnlyList<MapNode> allNodes
    )
    {
        int highestLayerIndex = 0;

        if (allNodes == null)
        {
            return highestLayerIndex;
        }

        for (int i = 0; i < allNodes.Count; i++)
        {
            if (allNodes[i].LayerIndex > highestLayerIndex)
            {
                highestLayerIndex =
                    allNodes[i].LayerIndex;
            }
        }

        return highestLayerIndex;
    }

    private void ClearMapUI()
    {
        if (connectionLayer != null)
        {
            for (int i = connectionLayer.childCount - 1; i >= 0; i--)
            {
                Destroy(
                    connectionLayer.GetChild(i).gameObject
                );
            }
        }

        if (mapNodeContainer != null)
        {
            for (int i = mapNodeContainer.childCount - 1; i >= 0; i--)
            {
                Transform child =
                    mapNodeContainer.GetChild(i);

                if (child == connectionLayer)
                {
                    continue;
                }

                Destroy(child.gameObject);
            }
        }

        if (nodeUIMap != null)
        {
            nodeUIMap.Clear();
        }
    }

    private List<MapNode> GetNodesInLayer(
        IReadOnlyList<MapNode> allNodes,
        int layerIndex
    )
    {
        List<MapNode> nodesInLayer =
            new List<MapNode>();

        if (allNodes == null)
        {
            return nodesInLayer;
        }

        for (int i = 0; i < allNodes.Count; i++)
        {
            if (allNodes[i].LayerIndex == layerIndex)
            {
                nodesInLayer.Add(allNodes[i]);
            }
        }

        return nodesInLayer;
    }

    public int GetDisplayedNodeCount()
    {
        int totalCount = 0;

        if (mapNodeContainer == null)
        {
            return 0;
        }

        for (int i = 0; i < mapNodeContainer.childCount; i++)
        {
            Transform child =
                mapNodeContainer.GetChild(i);

            if (child == connectionLayer)
            {
                continue;
            }

            totalCount += child.childCount;
        }

        return totalCount;
    }

    public void HideMapUI()
    {
        ClearMapUI();
    }
}