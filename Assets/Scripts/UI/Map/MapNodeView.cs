using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapNodeView : MonoBehaviour
{
    [SerializeField] private Button nodeButton;
    [SerializeField] private TMP_Text nodeText;

    private MapNode node;
    private RunManager runManager;

    public void Setup(MapNode targetNode, RunManager targetRunManager)
    {
        node = targetNode;
        runManager = targetRunManager;

        if (nodeButton == null)
        {
            Debug.LogError("MapNodeView Button is not assigned");
            return;
        }

        if (nodeText == null)
        {
            Debug.LogError("MapNodeView Text is not assigned");
            return;
        }

        nodeText.text = node.NodeType.ToString();

        nodeButton.onClick.RemoveAllListeners();
        nodeButton.onClick.AddListener(OnNodeClicked);

        RefreshInteractable();
    }

    public void RefreshInteractable()
    {
        if (nodeButton == null)
        {
            return;
        }

        if (runManager == null || node == null)
        {
            nodeButton.interactable = false;
            return;
        }

        nodeButton.interactable = runManager.CanMoveToNode(node);
    }

    private void OnNodeClicked()
    {
        if (runManager == null || node == null)
        {
            return;
        }
    
        runManager.MoveToNode(node);
    
        MapUIManager mapUIManager = FindFirstObjectByType<MapUIManager>();
    
        if (mapUIManager != null)
        {
            mapUIManager.RefreshMapUI();
        }
    }
}