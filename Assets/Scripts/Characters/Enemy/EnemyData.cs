using UnityEngine;

public class EnemyData : CharacterData
{
    [Header("Enemy Action")]
    //[SerializeField] private int attackDamage = 10;
    //[SerializeField] private int defendAmount = 5;

    [SerializeField] private EnemyActionPattern actionPattern;

    private EnemyAction currentAction;
    public EnemyAction CurrentAction => currentAction;

    private int currentActionIndex = 0;

    public void DecideAction()
    {
        currentAction = null;
        
        if (actionPattern == null)
        {
            Debug.LogWarning("Enemy Action Pattern is missing.");
            return;
        }

        if (actionPattern.Actions.Count == 0)
        {
            Debug.LogWarning("Enemy Action Pattern has no actions.");
            return;
        }

        currentAction = actionPattern.Actions[currentActionIndex];

        Debug.Log(
            "Enemy Decided: " +
            currentAction.ActionType +
            " Value: " +
            currentAction.Value
        );

        currentActionIndex++;

        if (currentActionIndex >= actionPattern.Actions.Count)
        {
            currentActionIndex = 0;
        }

        
        // int randomAction = Random.Range(0, 2);
    
        // if (randomAction == 0)
        // {
        //     currentAction = new EnemyAction(
        //         EnemyActionType.Attack,
        //         attackDamage
        //     );
    
        //     return;
        // }
    
        // currentAction = new EnemyAction(
        //     EnemyActionType.Defend,
        //     defendAmount
        // );
    }
}