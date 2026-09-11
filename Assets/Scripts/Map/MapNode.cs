using System.Collections.Generic;

public class MapNode
{
    public MapNodeType NodeType { get; }

    public int LayerIndex { get; }

    public IReadOnlyList<MapNode> Connections => connections;

    public bool IsVisited => visited;

    private List<MapNode> connections;
    private bool visited;

    public MapNode(MapNodeType nodeType, int layerIndex)
    {
        NodeType = nodeType;
        LayerIndex = layerIndex;
        connections = new List<MapNode>();
    }

    public void AddConnection(MapNode node)
    {
        if (node == null) return;
        if (connections.Contains(node)) return;

        connections.Add(node);
    }

    public void MarkVisited()
    {
        visited = true;
    }
}