using System.Collections.Generic;
using UnityEngine;

public class EnemyTargetDisplayManager : MonoBehaviour
{
    [SerializeField] private EnemyTargetUI enemyTargetPrefab;
    [SerializeField] private Transform targetArea;
    [SerializeField] private BattleTargetManager battleTargetManager;

    private List<EnemyTargetUI> targetButtons =
        new List<EnemyTargetUI>();

    public void RefreshTargets(
        IReadOnlyList<EnemyData> enemies)
    {
        ClearTargets();

        if (enemies == null)
        {
            Debug.LogWarning(
                "EnemyTargetDisplayManager : Enemy list is null"
            );

            return;
        }

        foreach (EnemyData enemy in enemies)
        {
            if (enemy == null)
            {
                continue;
            }

            if (enemy.IsDead())
            {
                continue;
            }

            if (enemyTargetPrefab == null)
            {
                Debug.LogError(
                    "EnemyTargetDisplayManager : Enemy Target Prefab is null"
                );

                return;
            }

            if (targetArea == null)
            {
                Debug.LogError(
                    "EnemyTargetDisplayManager : Target Area is null"
                );

                return;
            }

            EnemyTargetUI targetUI =
                Instantiate(
                    enemyTargetPrefab,
                    targetArea
                );

            targetUI.Setup(
                enemy,
                battleTargetManager
            );

            targetButtons.Add(targetUI);
        }

        SetTargetSelectionEnabled(false);
    }

    public void SetTargetSelectionEnabled(bool enabled)
    {
        foreach (EnemyTargetUI targetUI in targetButtons)
        {
            if (targetUI == null)
            {
                continue;
            }

            targetUI.SetInteractable(enabled);
        }
    }

    private void ClearTargets()
    {
        targetButtons.Clear();

        if (targetArea == null)
        {
            return;
        }

        foreach (Transform child in targetArea)
        {
            Destroy(child.gameObject);
        }
    }
}