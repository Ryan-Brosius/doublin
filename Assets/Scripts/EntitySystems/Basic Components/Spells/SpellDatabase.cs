using UnityEngine;
using System.Collections.Generic;
using static UnityEngine.Rendering.VolumeComponent;


public class SpellDatabase : SingletonMonobehavior<SpellDatabase>
{
    public Dictionary<string, SpellData> SpellList;
    public Dictionary<string, string> RuneList;

    public BoltSpellData fireBolt;
    public BoltSpellData iceBolt;
    public BlastSpellData fireBlast;
    public BlastSpellData iceBlast;
    public BarrageSpellData fireBarrage;
    public BarrageSpellData iceBarrage;
    public GameObject spoof;

    protected override void Awake()
    {
        base.Awake(); 
        SpellList =  new Dictionary<string, SpellData>()
        {
            {"Fire Bolt", fireBolt },
            {"Fire Barrage", fireBarrage },
            {"Fire Blast", fireBlast},
            {"Ice Bolt", iceBolt },
            {"Ice Barrage", iceBarrage },
            {"Ice Blast", iceBlast },
            //eventually spoof will be a spelldata
        };

        RuneList = new Dictionary<string, string>(){
            {"IL", "Fire"},
            {"TF", "Fire"},
            {"KJ", "Ice"},
            {"GH", "Ice"},
            {"JL", "Bolt"},
            {"KLI", "Barrage"},
            {"LIJK", "Blast"},
            {"HF", "Shield"},
            {"GFT", "Boon"}
        };
    }

    public string ConvertInput(string input){
        string element = input.Substring(0, 2).ToUpper();
        Debug.Log("Element " + element);
        string spellType = input.Substring(2).ToUpper();
        Debug.Log("Spell Type " + spellType);
        string spellKey = "Invalid";
        if (RuneList.ContainsKey(element) && RuneList.ContainsKey(spellType))
        {
            spellKey = RuneList[element] + " " + RuneList[spellType];
        }
        return spellKey;
    }

    public SpellData GetSpell(string spell)
    {
        SpellData spellObject = null;
        //eventually spoof
        SpellList.TryGetValue(spell, out spellObject);
        return spellObject;
    }
}
