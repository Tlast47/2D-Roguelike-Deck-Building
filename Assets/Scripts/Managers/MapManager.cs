using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapManager : MonoBehaviour
{
    public MapNode CurrentNode => currentNode;

    private MapNode currentNode;

    public IReadOnlyList<MapNode> AvailableNodes
    {
        get
        {
            if (currentNode == null)
            {
                return new List<MapNode>();
            }

            return currentNode.Connections;
        }
    }

    public void SetCurrentNode(MapNode node)
    {
        currentNode = node;
    }

    public bool MoveToNode(MapNode node)
    {
        if (currentNode == null)
        {
            Debug.LogError("Cannot move because Current Node is null");
            return false;
        }

        if (node == null)
        {
            Debug.LogError("Cannot move because target node is null");
            return false;
        }

        if (!AvailableNodes.Contains(node))
        {
            Debug.LogWarning("Target node is not connected to Current Node");
            return false;
        }

        currentNode.MarkVisited();
        currentNode = node;

        Debug.Log("MapManager Moved To Node");
        Debug.Log("Map Current Node Type : " + currentNode.NodeType);

        return true;
    }

    public bool CanMoveToNode(MapNode node)
    {
        if (currentNode == null)
        {
            return false;
        }

        if (node == null)
        {
            return false;
        }

        return AvailableNodes.Contains(node);
    }

    public void SetStartNode(MapNode node)
    {
        if (node == null)
        {
            Debug.LogError("Cannot set start node because node is null");
            return;
        }

        currentNode = node;

        Debug.Log("MapManager Start Node Set");
        Debug.Log("Map Current Node Type : " + currentNode.NodeType);
    }

    public void LogAvailableNodes()
    {
        if (currentNode == null)
        {
            Debug.LogError("Cannot log available nodes because Current Node is null");
            return;
        }
    
        Debug.Log("Available Node Count : " + AvailableNodes.Count);
    
        for (int i = 0; i < AvailableNodes.Count; i++)
        {
            Debug.Log(
                "Available Node " + i +
                " : " + AvailableNodes[i].NodeType
            );
        }
    }
    
}