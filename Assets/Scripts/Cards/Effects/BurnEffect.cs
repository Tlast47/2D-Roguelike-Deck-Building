using UnityEngine;

[CreateAssetMenu(fileName = "New Burn Effect", menuName = "Cards/Effects/Burn")]
public class BurnEffect : StatusCardEffect
{
    [Min(0)]
    [SerializeField] private int burnAmount;

    public int BurnAmount => burnAmount;
    protected override int StackAmount => burnAmount;

    protected override void ApplyStatus(CharacterData target, int amount)
    {
        target.AddBurn(amount);
        Debug.Log(target.name + " Burn: " + target.BurnStacks);
    }
}
