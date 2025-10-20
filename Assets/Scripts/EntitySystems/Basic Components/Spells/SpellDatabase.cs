using UnityEngine;

public class SpellDatabase : SingletonMonobehavior<SpellDatabase>
{
    public Dictionary<string, string> SpellList;
    void Awake()
    {
        SpellList =  new Dictionary<string, string>()
        {
            { "ILJL", "Fire Bolt" },
            { "TFJL", "Fire Bolt" },
            { "ILKLI", "Fire Barrage" },
            { "TFKLI", "Fire Barrage" },
            { "ILLIJK", "Fire Blast" },
            { "TFLIJA", "Fire Blast" },
            { "KJJL", "Ice Bolt" },
            { "GHJL", "Ice Bolt" },
            { "KJKLI", "Ice Barrage" },
            { "GHKLI", "Ice Barrage" },
            { "KJLIJK", "Ice Blast" },
            { "GHLIJK", "Ice Blast" },
        };
    }
}
