using UnityEngine;

public enum RelicEffect
{
    BattleStartBlock, BattleStartDraw, BattleStartEnergy, BattleStartBurn,
    BattleStartPoison, TurnStartBlock, VictoryHeal, VictoryGold, MaxHP, BattleStartDamage
}

[CreateAssetMenu(fileName = "New Relic", menuName = "Relics/Relic")]
public class RelicData : ScriptableObject
{
    [SerializeField] private string relicId;
    [SerializeField] private string relicName;
    [TextArea(2, 5)] [SerializeField] private string description;
    [SerializeField] private RelicEffect effect;
    [Min(1)] [SerializeField] private int amount = 1;
    [Min(1)] [SerializeField] private int shopPrice = 100;
    public string Id => relicId;
    public string RelicName => relicName;
    public string Description => description;
    public RelicEffect Effect => effect;
    public int Amount => amount;
    public int ShopPrice => shopPrice;
}
