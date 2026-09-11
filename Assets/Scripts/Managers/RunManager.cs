using System.Collections.Generic;
using UnityEngine;

public class RunManager : MonoBehaviour
{
    public RunData CurrentRun { get; private set; }

    [SerializeField] private MapGenerator mapGenerator;
    [SerializeField] private MapManager mapManager;
    [SerializeField] private MapUIManager mapUIManager;

    private void Start()
    {
        if (mapGenerator == null)
        {
            Debug.LogError("MapGenerator is not assigned");
            return;
        }

        if (mapManager == null)
        {
            Debug.LogError("MapManager is not assigned");
            return;
        }

        mapGenerator.GenerateMap();

        StartNewRun();
    }

    public void StartNewRun()
    {
        CurrentRun = new RunData();

        Debug.Log("New Run Started");
        Debug.Log("Starting Gold : " + CurrentRun.Gold);

        if (mapGenerator == null)
        {
            Debug.LogError("MapGenerator is not assigned");
            return;
        }

        if (mapGenerator.CurrentMap == null)
        {
            Debug.LogError("Current Map is null");
            return;
        }

        if (mapGenerator.CurrentMap.StartNode == null)
        {
            Debug.LogError("Start Node is null");
            return;
        }

        CurrentRun.SetCurrentNode(mapGenerator.CurrentMap.StartNode);
        mapManager.SetStartNode(mapGenerator.CurrentMap.StartNode);

        CurrentRun.StartRun();

        Debug.Log("Run Start Node Set");
        Debug.Log("Has Run Started : " + CurrentRun.HasStarted);
        Debug.Log("Current Node Type : " + CurrentRun.CurrentNode.NodeType);
        Debug.Log("Run State : " + CurrentRun.State);
        Debug.Log("Is Run Active : " + IsRunActive());
        Debug.Log("Map and Run Synchronized : " + IsMapAndRunNodeSynchronized());
        
        mapManager.LogAvailableNodes();

        if (mapUIManager != null)
        {
            mapUIManager.RefreshMapUI();
        }
        else
        {
            Debug.LogError("MapUIManager is not assigned");
        }
    }
    
    public void AddGold(int amount)
    {
        if (CurrentRun == null)
        {
            Debug.LogError("Cannot add Gold because CurrentRun is null");
            return;
        }

        CurrentRun.AddGold(amount);

        Debug.Log("Current Gold : " + CurrentRun.Gold);
    }

    public void SetCurrentNode(MapNode node)
    {
        CurrentRun.SetCurrentNode(node);
    }

    public MapNode GetCurrentNode()
    {
        if (CurrentRun == null)
        {
            return null;
        }

        return CurrentRun.CurrentNode;
    }

    public RunState GetRunState()
    {
        if (CurrentRun == null)
        {
            return RunState.NotStarted;
        }

        return CurrentRun.State;
    }

    public bool IsRunActive()
    {
        return CurrentRun != null && CurrentRun.State == RunState.Playing;
    }

    public void MoveToNode(MapNode node)
    {
        if (CurrentRun == null)
        {
            Debug.LogError("Cannot move because CurrentRun is null");
            return;
        }

        if (mapManager == null)
        {
            Debug.LogError("Cannot move because MapManager is null");
            return;
        }

        if (!IsRunActive())
        {
            Debug.LogWarning("Cannot move because Run is not active");
            return;
        }

        if (!CanMoveToNode(node))
        {
            Debug.LogWarning("Cannot move to this node");
            return;
        }

        bool moveSucceeded = mapManager.MoveToNode(node);

        if (!moveSucceeded)
        {
            Debug.LogWarning("RunManager Move Failed");
            return;
        }

        CurrentRun.SetCurrentNode(node);

        Debug.Log("RunManager Moved To Node");
        Debug.Log("Current Node Type : " + CurrentRun.CurrentNode.NodeType);

        if (IsCurrentNodeBoss())
        {
            CompleteRun();
        }
    }

    public bool CanMoveToNode(MapNode node)
    {
        if (CurrentRun == null)
        {
            return false;
        }

        if (mapManager == null)
        {
            return false;
        }

        if (!IsRunActive())
        {
            return false;
        }

        return mapManager.CanMoveToNode(node);
    }

    public IReadOnlyList<MapNode> GetAvailableNodes()
    {
        if (mapManager == null)
        {
            Debug.LogError("Cannot get available nodes because MapManager is null");
            return null;
        }

        return mapManager.AvailableNodes;
    }

    public IReadOnlyList<MapNode> GetAllMapNodes()
    {
        if (mapGenerator == null)
        {
            Debug.LogError("Cannot get all map nodes because MapGenerator is null");
            return null;
        }

        if (mapGenerator.CurrentMap == null)
        {
            Debug.LogError("Cannot get all map nodes because CurrentMap is null");
            return null;
        }

        return mapGenerator.CurrentMap.Nodes;
    }

    public bool IsMapAndRunNodeSynchronized()
    {
        if (CurrentRun == null)
        {
            return false;
        }

        if (mapManager == null)
        {
            return false;
        }

        return CurrentRun.CurrentNode == mapManager.CurrentNode;
    }

    public void LogCurrentRunNode()
    {
        if (CurrentRun == null)
        {
            Debug.LogError("CurrentRun is null");
            return;
        }
    
        if (CurrentRun.CurrentNode == null)
        {
            Debug.LogError("CurrentRun CurrentNode is null");
            return;
        }
    
        Debug.Log("Run Current Node Type : " + CurrentRun.CurrentNode.NodeType);
    }

    public bool IsCurrentNodeBoss()
    {
        if (CurrentRun == null)
        {
            return false;
        }

        if (CurrentRun.CurrentNode == null)
        {
            return false;
        }

        return CurrentRun.CurrentNode.NodeType == MapNodeType.Boss;
    }

    public void CompleteRun()
    {
        if (CurrentRun == null)
        {
            return;
        }
    
        CurrentRun.CompleteRun();
    
        Debug.Log("Run Completed");
    
        if (mapUIManager != null)
        {
            mapUIManager.HideMapUI();
        }
    }
}