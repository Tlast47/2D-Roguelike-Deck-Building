using System.Collections.Generic;

public class RelicCollection
{
    private readonly List<RelicData> owned = new List<RelicData>();
    private readonly PlayerRunData player;
    public IReadOnlyList<RelicData> Owned => owned.AsReadOnly();
    public int Count => owned.Count;
    public RelicCollection(PlayerRunData playerData) { player = playerData; }

    public bool Contains(RelicData relic)
    {
        if (relic == null) return false;
        foreach (RelicData item in owned)
            if (item == relic || item.Id == relic.Id) return true;
        return false;
    }

    public bool TryAdd(RelicData relic)
    {
        if (relic == null || string.IsNullOrEmpty(relic.Id) || Contains(relic)) return false;
        owned.Add(relic);
        if (relic.Effect == RelicEffect.MaxHP) player.IncreaseMaxHP(relic.Amount);
        return true;
    }

    public int Total(RelicEffect effect)
    {
        int amount = 0;
        foreach (RelicData relic in owned)
            if (relic != null && relic.Effect == effect) amount += relic.Amount;
        return amount;
    }
}
