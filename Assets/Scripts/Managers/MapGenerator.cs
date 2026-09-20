using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    private MapNode startNode;
    private MapNode bossNode;

    public MapData CurrentMap { get; private set; }

    private int restCount;
    private int eliteCount;

    private const int MaxRestCount = 2;
    private const int MaxEliteCount = 2;

    public void SetCurrentMap(MapData map)
    {
        if (map == null)
        {
            Debug.LogError("Cannot set Current Map because map is null");
            return;
        }
    
        CurrentMap = map;
    }
    
    public void GenerateMap()
    {
        restCount = 0;
        eliteCount = 0;

        CurrentMap = new MapData();

        CreateStartAndBoss();

        List<MapNode> firstLayer = CreateRandomLayer(3, 1);
        List<MapNode> secondLayer = CreateRandomLayer(2, 2);

        /*
        List<MapNode> secondLayer = new List<MapNode>();
        for (int i = 0; i < 2; i++)
        {
            MapNode node = new MapNode(
                MapNodeType.NormalBattle, 2
            );

            secondLayer.Add(node);
            CurrentMap.AddNode(node);
        }*/

        startNode.AddConnection(firstLayer[0]);
        startNode.AddConnection(firstLayer[1]);
        startNode.AddConnection(firstLayer[2]);

        firstLayer[0].AddConnection(secondLayer[0]);
        firstLayer[1].AddConnection(secondLayer[0]);
        firstLayer[2].AddConnection(secondLayer[1]);

        secondLayer[0].AddConnection(bossNode);
        secondLayer[1].AddConnection(bossNode);

        CurrentMap.SetStartNode(startNode);

        AddNodesToMapData(CurrentMap, firstLayer);
        AddNodesToMapData(CurrentMap, secondLayer);
        CurrentMap.AddNode(bossNode);

        /*
        Debug.Log("Start Available Nodes : " + startNode.Connections.Count);
        Debug.Log("First Layer Count : " + firstLayer.Count);
        Debug.Log("Second Layer Count : " + secondLayer.Count);
        Debug.Log("Map Node Count : " + CurrentMap.Nodes.Count);

        Debug.Log("Start Node Type : " + startNode.NodeType);
        */

        if (ValidateMap())
        {
            Debug.Log("Map Validation Passed");
        }
        else
        {
            Debug.LogError("Map Validation Failed");
        }

        /*
        Debug.Log("Start Layer : " + startNode.LayerIndex);

        for (int i = 0; i < firstLayer.Count; i++)
        {
            Debug.Log(
                "First Layer Node " + i +
                " Type : " + firstLayer[i].NodeType +
                " Layer : " + firstLayer[i].LayerIndex
            );
        }

        for (int i = 0; i < secondLayer.Count; i++)
        {
            Debug.Log(
                "Second Layer Node " + i +
                " Type : " + secondLayer[i].NodeType +
                " Layer : " + secondLayer[i].LayerIndex
            );
        }

        Debug.Log("Boss Layer : " + bossNode.LayerIndex);
        */
        
        TestMapConnections();
    }

    private void TestMapConnections()
    {
        if (CurrentMap == null)
        {
            Debug.LogWarning("Current Map is null");
            return;
        }
    
        for (int i = 0; i < CurrentMap.Nodes.Count; i++)
        {
            MapNode node = CurrentMap.Nodes[i];
    
            /*
            Debug.Log(
                "Node Layer " + node.LayerIndex +
                " Type " + node.NodeType +
                " Connections : " + node.Connections.Count
            );
            */
    
            for (int j = 0; j < node.Connections.Count; j++)
            {
                MapNode connectedNode =
                    node.Connections[j];

                /*
                Debug.Log(
                    "    -> Layer " +
                    connectedNode.LayerIndex +
                    " Type " +
                    connectedNode.NodeType
                );
                */
            }
        }
    }

    private void CreateStartAndBoss()
    {
        startNode = new MapNode(MapNodeType.NormalBattle, 0);
        bossNode = new MapNode(MapNodeType.Boss, 3);

        CurrentMap.AddNode(startNode);
    }

    private List<MapNode> CreateLayer(MapNodeType[] nodeTypes)
    {
        List<MapNode> layer = new List<MapNode>();

        foreach (MapNodeType nodeType in nodeTypes)
        {
            MapNode node = new MapNode(nodeType, 1);
            layer.Add(node);
        }

        return layer;
    }

    private void AddNodesToMapData(MapData CurrentMap, List<MapNode> layer)
    {
        foreach (MapNode node in layer)
        {
            CurrentMap.AddNode(node);
        }
    }

    private MapNodeType GetRandomMiddleNodeType()
    {
        int randomValue = Random.Range(0, 4);
    
        switch (randomValue)
        {
            case 0:
                return MapNodeType.NormalBattle;
    
            case 1:
                if (eliteCount < MaxEliteCount)
                {
                    eliteCount++;
                    return MapNodeType.Elite;
                }
    
                return MapNodeType.NormalBattle;
    
            case 2:
                if (restCount < MaxRestCount)
                {
                    restCount++;
                    return MapNodeType.Rest;
                }
    
                return MapNodeType.NormalBattle;
    
            case 3:
                return MapNodeType.Shop;
    
            default:
                return MapNodeType.NormalBattle;
        }
    }

    private List<MapNode> CreateRandomLayer(int count, int layerIndex)
    {
        List<MapNode> layer = new List<MapNode>();

        for (int i = 0; i < count; i++)
        {
            MapNodeType nodeType = GetRandomMiddleNodeType();

            MapNode node = new MapNode(nodeType, layerIndex);

            layer.Add(node);
            CurrentMap.AddNode(node);
        }

        return layer;
    }

    private bool HasPathToBoss(MapNode currentNode, HashSet<MapNode> visitedNodes)
    {
        if (currentNode == bossNode)
        {
            return true;
        }

        if (visitedNodes.Contains(currentNode))
        {
            return false;
        }

        visitedNodes.Add(currentNode);

        foreach (MapNode nextNode in currentNode.Connections)
        {
            if (HasPathToBoss(nextNode, visitedNodes))
            {
                return true;
            }
        }

        return false;
    }

    private bool ValidateMap()
    {
        if (startNode == null)
        {
            Debug.LogError("Map Validation Failed: Start Node is null");
            return false;
        }

        if (bossNode == null)
        {
            Debug.LogError("Map Validation Failed: Boss Node is null");
            return false;
        }

        HashSet<MapNode> pathVisitedNodes = new HashSet<MapNode>();

        if (!HasPathToBoss(startNode, pathVisitedNodes))
        {
            Debug.LogError("Map Validation Failed: No path from Start to Boss");
            return false;
        }

        HashSet<MapNode> consecutiveVisitedNodes = new HashSet<MapNode>();

        if (HasInvalidConsecutiveNodeTypes(startNode, consecutiveVisitedNodes))
        {
            Debug.LogError("Map Validation Failed: Invalid consecutive Rest or Elite nodes");
            return false;
        }

        Debug.Log("Map Validation Passed");

        if (!ValidateNodeLayers())
        {
            return false;
        }
        return true;
    }

    private bool HasInvalidConsecutiveNodeTypes(
    MapNode currentNode,
    HashSet<MapNode> visitedNodes)
    {
        if (currentNode == null)
        {
            return false;
        }

        if (visitedNodes.Contains(currentNode))
        {
            return false;
        }

        visitedNodes.Add(currentNode);

        foreach (MapNode nextNode in currentNode.Connections)
        {
            bool bothRest =
                currentNode.NodeType == MapNodeType.Rest &&
                nextNode.NodeType == MapNodeType.Rest;

            bool bothElite =
                currentNode.NodeType == MapNodeType.Elite &&
                nextNode.NodeType == MapNodeType.Elite;

            if (bothRest || bothElite)
            {
                return true;
            }

            if (HasInvalidConsecutiveNodeTypes(nextNode, visitedNodes))
            {
                return true;
            }
        }

        return false;
    }

    private bool ValidateNodeLayers()
    {
        if (startNode == null)
        {
            Debug.LogError("Start Node is null");
            return false;
        }

        if (bossNode == null)
        {
            Debug.LogError("Boss Node is null");
            return false;
        }

        if (startNode.LayerIndex != 0)
        {
            Debug.LogError("Start Node must be in Layer 0");
            return false;
        }

        if (bossNode.LayerIndex != 3)
        {
            Debug.LogError("Boss Node must be in Layer 3");
            return false;
        }

        for (int i = 0; i < CurrentMap.Nodes.Count; i++)
        {
            MapNode node = CurrentMap.Nodes[i];

            if (node.LayerIndex < 0 || node.LayerIndex > 3)
            {
                Debug.LogError(
                    "Node has invalid LayerIndex : " +
                    node.LayerIndex
                );

                return false;
            }
        }

        return true;
    }
}