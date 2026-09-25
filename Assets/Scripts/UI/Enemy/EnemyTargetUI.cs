using UnityEngine;
using UnityEngine.UI;

public class EnemyTargetUI : MonoBehaviour
{
    [SerializeField] private Button targetButton;

    private EnemyData enemy;
    private BattleTargetManager battleTargetManager;

    public void Setup(
        EnemyData targetEnemy,
        BattleTargetManager targetManager)
    {
        enemy = targetEnemy;
        battleTargetManager = targetManager;

        if (targetButton == null)
        {
            Debug.LogError(
                "EnemyTargetUI : Target Button is not assigned"
            );

            return;
        }

        targetButton.onClick.RemoveAllListeners();

        targetButton.onClick.AddListener(
            SelectTarget
        );
    }

    public void SetInteractable(bool interactable)
    {
        if (targetButton == null)
        {
            Debug.LogError(
                "EnemyTargetUI : Target Button is not assigned"
            );

            return;
        }

        targetButton.interactable = interactable;
    }

    private void SelectTarget()
    {
        if (enemy == null)
        {
            Debug.LogWarning(
                "EnemyTargetUI : Enemy is null"
            );

            return;
        }

        if (battleTargetManager == null)
        {
            Debug.LogError(
                "EnemyTargetUI : BattleTargetManager is null"
            );

            return;
        }

        battleTargetManager.SelectEnemy(enemy);
    }
}