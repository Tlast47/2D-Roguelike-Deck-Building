using System.Collections.Generic;
using UnityEngine;

public class MapUIManager : MonoBehaviour
{
    [SerializeField] private RunManager runManager;
    [SerializeField] private GameObject mapNodePrefab;

    [SerializeField] private Transform mapNodeContainer;
    [SerializeField] private GameObject mapLayerPrefab;

    private bool hasLoggedInitialMapUI;

    private void Start()
    {
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

                if (nodeView == null)
                {
                    Debug.LogError(
                        "Map Node Prefab does not have MapNodeView"
                    );

                    continue;
                }

                nodeView.Setup(
                    nodesInLayer[i],
                    runManager
                );
            }
        }

        if (!hasLoggedInitialMapUI)
        {
            Debug.Log(
                "Created All Map UI Node Count : " +
                GetDisplayedNodeCount()
            );

            hasLoggedInitialMapUI = true;
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
        if (mapNodeContainer == null)
        {
            return;
        }

        for (int i = mapNodeContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(
                mapNodeContainer.GetChild(i).gameObject
            );
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
            Transform layerContainer =
                mapNodeContainer.GetChild(i);

            totalCount += layerContainer.childCount;
        }

        return totalCount;
    }

    public void HideMapUI()
    {
        ClearMapUI();
    }
}