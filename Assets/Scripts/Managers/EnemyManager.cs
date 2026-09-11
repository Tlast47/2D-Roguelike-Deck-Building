using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private EnemyData enemy;
    [SerializeField] private PlayerData player;

    public void PerformTurn()
    {
        EnemyAction action = enemy.CurrentAction;

        ExecuteAction(action);
    }

    private void ExecuteAction(EnemyAction action)
    {
        if (action == null)
        {
            return;
        }

        switch (action.ActionType)
        {
            case EnemyActionType.Attack:
                player.TakeDamage(action.Value);

                Debug.Log("Enemy Attack : " + action.Value);
                Debug.Log("Player HP : " + player.GetCurrentHP());
                break;

            case EnemyActionType.Defend:
                enemy.AddBlock(action.Value);

                Debug.Log("Enemy Defend : " + action.Value);
                Debug.Log("Enemy Block : " + enemy.GetBlock());
                break;
        }
    }
}