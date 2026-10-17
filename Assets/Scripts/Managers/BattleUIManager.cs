using UnityEngine;
using UnityEngine.UI;

public class BattleUIManager : MonoBehaviour
{
    [Header("Battle UI")]
    [SerializeField] private Button endTurnButton;

    private BattleManager battleManager;

    private void Awake()
    {
        battleManager = FindFirstObjectByType<BattleManager>();

        if (battleManager == null)
        {
            Debug.LogError(
                "BattleUIManager : BattleManager not found"
            );

            return;
        }

        if (endTurnButton == null)
        {
            Debug.LogError(
                "BattleUIManager : End Turn Button is null"
            );

            return;
        }

        endTurnButton.onClick.AddListener(OnEndTurnClicked);
    }

    private void Start()
    {
        Canvas canvas = endTurnButton != null ? endTurnButton.GetComponentInParent<Canvas>() : null;
        DeckManager deck = FindFirstObjectByType<DeckManager>();
        if (canvas == null || deck == null) return;
        BattlePileUI piles = gameObject.AddComponent<BattlePileUI>();
        piles.Setup(deck, canvas.rootCanvas.transform as RectTransform);
    }

    private void OnDestroy()
    {
        if (endTurnButton != null)
        {
            endTurnButton.onClick.RemoveListener(OnEndTurnClicked);
        }
    }

    private void OnEndTurnClicked()
    {
        if (battleManager == null)
        {
            Debug.LogError(
                "BattleUIManager : BattleManager is null"
            );

            return;
        }

        battleManager.EndPlayerTurn();
    }
}
