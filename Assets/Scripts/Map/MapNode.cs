using System.Collections.Generic;
using UnityEngine;

public class MapNode
{
    //// Node Data ////

    public MapNodeType NodeType { get; private set; }

    public int LayerIndex { get; }

    public IReadOnlyList<MapNode> Connections =>
        connections;

    public bool IsVisited => visited;

    //// Internal Data ////

    private List<MapNode> connections;

    private bool visited;

    //// Constructor ////

    public MapNode(
        MapNodeType nodeType,
        int layerIndex)
    {
        NodeType = nodeType;
        LayerIndex = layerIndex;

        connections =
            new List<MapNode>();
    }

    //// Connection Management ////

    public void AddConnection(
        MapNode node)
    {
        if (node == null)
        {
            return;
        }

        if (connections.Contains(node))
        {
            return;
        }

        connections.Add(node);
    }

    // Generated once per run, so visiting scenes does not reshuffle room positions.
    public Vector2 LayoutPosition { get; private set; }
    public bool HasLayoutPosition { get; private set; }

    public void SetLayoutPosition(Vector2 position)
    {
        LayoutPosition = position;
        HasLayoutPosition = true;
    }

    internal void SetNodeType(MapNodeType type) { NodeType = type; }

    //// Node State ////

    public void MarkVisited()
    {
        visited = true;
    }
}