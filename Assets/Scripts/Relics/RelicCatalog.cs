using System.Collections.Generic;
using UnityEngine;

public static class RelicCatalog
{
    public static List<RelicData> Available(RelicCollection collection)
    {
        List<RelicData> result = new List<RelicData>();
        HashSet<string> ids = new HashSet<string>();
        foreach (RelicData relic in Resources.LoadAll<RelicData>("Relics"))
            if (relic != null && !string.IsNullOrEmpty(relic.Id) && ids.Add(relic.Id) &&
                (collection == null || !collection.Contains(relic))) result.Add(relic);
        result.Sort((a, b) => string.CompareOrdinal(a.Id, b.Id));
        return result;
    }
}
