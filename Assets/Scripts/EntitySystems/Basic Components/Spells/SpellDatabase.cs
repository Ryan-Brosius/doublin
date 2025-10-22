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
    public BoonSpellData fireBoon;
    public ShieldSpellData fireShield;
    public ShieldSpellData iceShield;
    public SpoofData invalidSpoof;
    public SpoofData cancelSpoof;
    public SpoofData cooldownSpoof;

    protected override void Awake()
    {
        base.Awake(); 
        SpellList =  new Dictionary<string, SpellData>()
        {
            {"Fire Bolt", fireBolt },
            {"Fire Barrage", fireBarrage },
            {"Fire Blast", fireBlast},
            {"Fire Boon", fireBoon},
            {"Fire Shield", fireShield},
            {"Ice Bolt", iceBolt },
            {"Ice Barrage", iceBarrage },
            {"Ice Blast", iceBlast },
            {"Ice Shield", iceShield},

            {"Invalid", invalidSpoof},
            {"Cancel", cancelSpoof},
            {"Cooldown", cooldownSpoof}

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
        string spellType = input.Substring(2).ToUpper();
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
        SpellList.TryGetValue(spell, out spellObject);
        return spellObject;
    }
}
