using System.Collections.Generic;

public class MapData
{
    public IReadOnlyList<MapNode> Nodes => nodes;

    private List<MapNode> nodes;

    public MapNode StartNode { get; private set; }

    public MapData()
    {
        nodes = new List<MapNode>();
    }

    public void AddNode(MapNode node)
    {
        if (node == null)
        {
            return;
        }

        if (nodes.Contains(node))
        {
            return;
        }

        nodes.Add(node);
    }

    public void SetStartNode(MapNode node)
    {
        StartNode = node;
    }
}