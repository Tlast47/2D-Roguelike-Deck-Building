using UnityEngine;

[CreateAssetMenu(fileName = "New Poison Effect", menuName = "Cards/Effects/Poison")]
public class PoisonEffect : StatusCardEffect
{
    [Min(0)]
    [SerializeField] private int poisonAmount;

    public int PoisonAmount => poisonAmount;
    protected override int StackAmount => poisonAmount;

    protected override void ApplyStatus(CharacterData target, int amount)
    {
        target.AddPoison(amount);
        Debug.Log(target.name + " Poison: " + target.PoisonStacks);
    }
}
