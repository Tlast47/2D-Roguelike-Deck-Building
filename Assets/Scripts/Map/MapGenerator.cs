using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    private const int LastRoomLayer = 5;
    private const int BossLayer = 6;
    public MapData CurrentMap { get; private set; }

    public void GenerateMap()
    {
        for (int attempt = 0; attempt < 200; attempt++)
        {
            MapData candidate = CreateMap();
            if (candidate != null)
            {
                CurrentMap = candidate;
                return;
            }
        }
        CurrentMap = CreateFallbackMap();
        Debug.LogWarning("Map generation used the guaranteed fallback layout.");
    }

    private MapData CreateMap()
    {
        MapData map = new MapData();
        MapNode start = new MapNode(MapNodeType.NormalBattle, 0);
        start.SetLayoutPosition(new Vector2(0f, -1f));
        map.AddNode(start);
        map.SetStartNode(start);
        List<List<MapNode>> layers = new List<List<MapNode>>();

        for (int layer = 1; layer <= LastRoomLayer; layer++)
        {
            int count = layer == 1 ? 3 : Random.Range(2, layer == LastRoomLayer ? 4 : 5);

            float[] horizontalPositions = CreateHorizontalPositions(count);
            List<MapNode> row = new List<MapNode>();
            for (int i = 0; i < count; i++)
            {
                MapNode node = new MapNode(MapNodeType.NormalBattle, layer);
                float x = horizontalPositions[i];
                bool positioned = false;
                for (int attempt = 0; attempt < 40; attempt++)
                {
                    float y = layer - 1 + Random.Range(-0.38f, 0.38f);
                    if (!HasRoomSpace(map, x, y)) continue;
                    // Leave room for the fixed boss at the top.
                    if (layer == LastRoomLayer && Mathf.Abs(x) < 0.40f && 5f - y < 0.70f) continue;
                    node.SetLayoutPosition(new Vector2(x, y));
                    positioned = true;
                    break;
                }
                if (!positioned) return null;
                row.Add(node);
                map.AddNode(node);
            }
            layers.Add(row);
        }
        foreach (MapNode first in layers[0]) start.AddConnection(first);
        for (int i = 0; i < layers.Count - 1; i++)
        {
            if (!ConnectRows(layers[i], layers[i + 1])) return null;
        }
        MapNode boss = new MapNode(MapNodeType.Boss, BossLayer);
        boss.SetLayoutPosition(new Vector2(0f, BossLayer - 1));
        map.AddNode(boss);
        foreach (MapNode last in layers[layers.Count - 1]) last.AddConnection(boss);

        // Assign room types after topology, so adjacency rules use the real routes.
        if (!AssignRooms(map, MapNodeType.Rest, Random.Range(2, 4), 3) ||
            !AssignRooms(map, MapNodeType.Shop, Random.Range(2, 4), 2) ||
            !AssignRooms(map, MapNodeType.Elite, Random.Range(2, 4), 2)) return null;
        return map;
    }

    // Random gaps including empty space on both sides; rooms are not tied to columns.
    private static float[] CreateHorizontalPositions(int count)
    {
        const float minimumGap = 0.42f;
        float freeSpace = 1.90f - (count - 1) * minimumGap;
        float[] weights = new float[count + 1];
        float totalWeight = 0f;
        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] = Random.Range(0.15f, 1.8f);
            totalWeight += weights[i];
        }
        float[] positions = new float[count];
        float cursor = -0.95f + freeSpace * weights[0] / totalWeight;
        positions[0] = cursor;
        for (int i = 1; i < count; i++)
        {
            cursor += minimumGap + freeSpace * weights[i] / totalWeight;
            positions[i] = cursor;
        }
        return positions;
    }

    private static bool HasRoomSpace(MapData map, float x, float y)
    {
        foreach (MapNode other in map.Nodes)
        {
            if (other.LayerIndex == 0) continue;
            if (Mathf.Abs(other.LayoutPosition.x - x) < 0.40f &&
                Mathf.Abs(other.LayoutPosition.y - y) < 0.70f) return false;
        }
        return true;
    }

    private bool ConnectRows(List<MapNode> from, List<MapNode> to)
    {
        // A monotone staircase covers every room without crossed or disconnected routes.
        int source = 0;
        int target = 0;
        from[source].AddConnection(to[target]);
        while (source < from.Count - 1 || target < to.Count - 1)
        {
            bool canRight = source < from.Count - 1 && IncomingCount(from, to[target]) < 3;
            bool canUp = target < to.Count - 1 && from[source].Connections.Count < 3;
            bool canDiagonal = source < from.Count - 1 && target < to.Count - 1;
            if (canDiagonal && (Random.value < 0.68f || (!canRight && !canUp)))
            {
                source++;
                target++;
            }
            else if (canRight && (!canUp || Random.value < 0.5f)) source++;
            else if (canUp) target++;
            else if (canDiagonal) { source++; target++; }
            else return false;
            from[source].AddConnection(to[target]);
        }

        // Add a small number of readable choices instead of joining every nearby room.
        int branchBudget = Random.value < 0.25f ? 2 : 1;
        for (int attempt = 0; attempt < 16 && branchBudget > 0; attempt++)
        {
            int a = Random.Range(0, from.Count);
            int b = Random.Range(0, to.Count);
            if (from[a].Connections.Count >= 2 || IncomingCount(from, to[b]) >= 3 ||
                HasConnection(from[a], to[b]) ||
                Mathf.Abs(from[a].LayoutPosition.x - to[b].LayoutPosition.x) > 0.72f ||
                WouldCross(from, to, a, b)) continue;
            from[a].AddConnection(to[b]);
            branchBudget--;
        }
        return true;
    }

    private static bool WouldCross(List<MapNode> from, List<MapNode> to, int a, int b)
    {
        for (int i = 0; i < from.Count; i++)
        {
            foreach (MapNode next in from[i].Connections)
            {
                int j = to.IndexOf(next);
                if (j >= 0 && (i - a) * (j - b) < 0) return true;
            }
        }
        return false;
    }

    private static bool HasConnection(MapNode source, MapNode target)
    {
        foreach (MapNode next in source.Connections) if (next == target) return true;
        return false;
    }

    private static int IncomingCount(List<MapNode> from, MapNode target)
    {
        int count = 0;
        foreach (MapNode source in from) if (HasConnection(source, target)) count++;
        return count;
    }

    private bool AssignRooms(MapData map, MapNodeType type, int count, int firstLayer)
    {
        List<MapNode> candidates = new List<MapNode>();
        foreach (MapNode node in map.Nodes)
        {
            if (node.LayerIndex >= firstLayer && node.LayerIndex <= LastRoomLayer)
                candidates.Add(node);
        }
        for (int i = candidates.Count - 1; i > 0; i--)
        {
            int index = Random.Range(0, i + 1);
            MapNode temp = candidates[i];
            candidates[i] = candidates[index];
            candidates[index] = temp;
        }
        int assigned = 0;
        foreach (MapNode node in candidates)
        {
            if (node.NodeType != MapNodeType.NormalBattle || !CanAssign(map, node, type)) continue;
            node.SetNodeType(type);
            if (++assigned == count) return true;
        }
        return false;
    }

    private static bool CanAssign(MapData map, MapNode node, MapNodeType type)
    {
        foreach (MapNode other in map.Nodes)
        {
            if (other.NodeType != type) continue;
            // Different options in a row, and no consecutive shops or rests.
            if (other.LayerIndex == node.LayerIndex) return false;
            if ((type == MapNodeType.Shop || type == MapNodeType.Rest) &&
                (HasConnection(other, node) || HasConnection(node, other))) return false;
        }
        return true;
    }

    private MapData CreateFallbackMap()
    {
        MapData map = new MapData();
        MapNode start = new MapNode(MapNodeType.NormalBattle, 0);
        start.SetLayoutPosition(new Vector2(0f, -1f));
        map.AddNode(start);
        map.SetStartNode(start);
        MapNodeType[,] types = {
            { MapNodeType.NormalBattle, MapNodeType.NormalBattle, MapNodeType.NormalBattle },
            { MapNodeType.Shop, MapNodeType.Elite, MapNodeType.NormalBattle },
            { MapNodeType.NormalBattle, MapNodeType.Rest, MapNodeType.Shop },
            { MapNodeType.Rest, MapNodeType.NormalBattle, MapNodeType.Elite },
            { MapNodeType.NormalBattle, MapNodeType.Shop, MapNodeType.Rest }
        };
        List<MapNode> previous = new List<MapNode>();
        for (int layer = 1; layer <= LastRoomLayer; layer++)
        {
            List<MapNode> row = new List<MapNode>();
            for (int i = 0; i < 3; i++)
            {
                MapNode node = new MapNode(types[layer - 1, i], layer);
                node.SetLayoutPosition(new Vector2((i - 1) * 0.55f, layer - 1));
                map.AddNode(node);
                row.Add(node);
                if (layer == 1) start.AddConnection(node);
                else previous[i].AddConnection(node);
            }
            previous = row;
        }
        MapNode boss = new MapNode(MapNodeType.Boss, BossLayer);
        boss.SetLayoutPosition(new Vector2(0f, BossLayer - 1));
        map.AddNode(boss);
        foreach (MapNode last in previous) last.AddConnection(boss);
        return map;
    }

    public void SetCurrentMap(MapData map)
    {
        if (map != null) CurrentMap = map;
    }
}
