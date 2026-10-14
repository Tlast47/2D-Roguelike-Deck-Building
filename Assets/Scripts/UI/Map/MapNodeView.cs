using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapNodeView : MonoBehaviour
{
    [SerializeField] private Button nodeButton;
    [SerializeField] private TMP_Text nodeText;

    private MapNode node;
    private RunManager runManager;

    private void Awake()
    {
        if (nodeButton == null)
        {
            nodeButton = GetComponent<Button>();
        }
        if (nodeText == null)
        {
            nodeText = GetComponentInChildren<TMP_Text>(true);
        }

        if (nodeButton == null)
        {
            Debug.LogError(
                "MapNodeView : Button component not found"
            );
        }

        if (nodeText == null)
        {
            Debug.LogError(
                "MapNodeView : TMP_Text component not found"
            );
        }
    }

    public void Setup(
        MapNode targetNode,
        RunManager targetRunManager)
    {
        node = targetNode;
        runManager = targetRunManager;

        if (nodeButton == null)
        {
            Debug.LogError(
                "MapNodeView : Button is null"
            );
            return;
        }

        if (nodeText == null)
        {
            Debug.LogError(
                "MapNodeView : Text is null"
            );
            return;
        }

        nodeText.text =
            node.NodeType == MapNodeType.NormalBattle ? "Battle" : node.NodeType.ToString();

        nodeText.enableAutoSizing = true;
        nodeText.fontSizeMin = 12f;
        nodeText.fontSizeMax = 22f;

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

        bool canMove =
            runManager.CanMoveToNode(node);

        nodeButton.interactable = canMove;
    }

    private void OnNodeClicked()
    {
        if (runManager == null || node == null)
        {
            return;
        }

        runManager.MoveToNode(node);

        MapUIManager mapUIManager =
            FindFirstObjectByType<MapUIManager>();

        if (mapUIManager != null)
        {
            mapUIManager.RefreshMapUI();
        }
    }
}