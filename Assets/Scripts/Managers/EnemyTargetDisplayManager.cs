using System.Collections.Generic;
using UnityEngine;

public class EnemyTargetDisplayManager : MonoBehaviour
{
    [Header("Enemy Target")]
    [SerializeField] private EnemyTargetUI enemyTargetPrefab;
    [SerializeField] private BattleTargetManager battleTargetManager;

    private readonly List<EnemyTargetUI> targetUIs =
        new List<EnemyTargetUI>();

    public void RefreshTargets(
        IReadOnlyList<EnemyData> enemies)
    {
        ClearTargets();

        if (enemies == null)
        {
            return;
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            EnemyData enemy = enemies[i];

            if (enemy == null)
            {
                continue;
            }

            EnemyTargetUI targetUI =
                Instantiate(
                    enemyTargetPrefab,
                    enemy.transform
                );

            targetUI.transform.localPosition =
                Vector3.zero;

            targetUI.transform.localRotation =
                Quaternion.identity;

            targetUI.transform.localScale =
                Vector3.one;

            // Overlay the real enemy so resizing never separates its clickable target.
            RectTransform targetRect = targetUI.transform as RectTransform;
            if (targetRect != null)
            {
                targetRect.anchorMin = Vector2.zero;
                targetRect.anchorMax = Vector2.one;
                targetRect.offsetMin = targetRect.offsetMax = Vector2.zero;
            }

            targetUI.Setup(
                enemy,
                battleTargetManager
            );

            targetUIs.Add(targetUI);
        }
    }

    public void RefreshEnemyUI()
    {
        foreach (EnemyTargetUI targetUI in targetUIs)
        {
            if (targetUI == null)
            {
                continue;
            }

            targetUI.RefreshUI();
        }
    }

    public void SetTargetSelectionEnabled(
        bool enabled)
    {
        foreach (EnemyTargetUI targetUI in targetUIs)
        {
            if (targetUI == null)
            {
                continue;
            }

            targetUI.SetInteractable(enabled);
        }
    }

    public void RemoveDeadEnemies()
    {
        foreach (EnemyTargetUI targetUI in targetUIs)
        {
            if (targetUI == null)
            {
                continue;
            }

            if (targetUI.Enemy == null)
            {
                continue;
            }

            if (targetUI.Enemy.IsDead())
            {
                targetUI.SetVisible(false);
            }
        }
    }

    private void ClearTargets()
    {
        foreach (EnemyTargetUI targetUI in targetUIs)
        {
            if (targetUI != null)
            {
                Destroy(targetUI.gameObject);
            }
        }

        targetUIs.Clear();
    }
}

