using UnityEngine;

[CreateAssetMenu(fileName = "New Block Effect", menuName = "Cards/Effects/Block")]
public class BlockEffect : CardEffect
{
    [SerializeField] private int blockAmount;

    public int BlockAmount => blockAmount;

    public override void Execute(CardEffectContext context)
    {
        if (Target == TargetType.Player)
        {
            context.Player.AddBlock(blockAmount);
            //test
            Debug.Log("Player Block: " + context.Player.GetBlock());
        }

        if (Target == TargetType.Enemy)
        {
            context.Enemy.AddBlock(blockAmount);
        }
    }
}