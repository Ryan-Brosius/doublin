using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PlayerSpellCaster : MonoBehaviour, ISpellCaster
{

    [SerializeField] private bool isBuffed = false;
    Dictionary<string, bool> cooldowns;
    Dictionary<string, string> incantDict;
    private string incant = "";

    public BoltSpellData fireBolt;
    public BoltSpellData iceBolt;
    public BlastSpellData fireBlast;
    public BlastSpellData iceBlast;
    public GameObject spoof;

    LazyDependency<InputManager> _InputManager;

    public bool IsBuffed => isBuffed;
    public Dictionary<string, bool> SpellCooldowns => cooldowns;

    void Awake(){
        SetupIncantDict();
        SetupSpellCooldowns();
    }

    void Start()
    {
        _InputManager.Value.OnGrimoireIncant += HandleInput;
    }

    private void HandleInput(string c)
    {
        incant += c;
        Debug.Log(incant);
        if (incant.Length > 6)
        {
            HandleCastSpell("Spoof");
            incant = "";
        }
        else if (incant.Length > 3){
            string spell;
            if (incantDict.TryGetValue(incant.ToUpper(), out spell))
            {
                HandleCastSpell(spell);
                incant = "";
            }
        }
    }

    private void HandleCastSpell(string spell)
    {
        if (spell.Equals("Spoof")){
            Instantiate(spoof, gameObject.transform);
        }
        else {
            var spellTokens = spell.Split();
            Element element;
            System.Enum.TryParse(spellTokens[0], out element);
            switch(element){
                case(Element.Fire):
                    switch (spellTokens[1])
                    {
                        case ("Bolt"):
                            if(!cooldowns[spell]){
                                fireBolt.Cast(this.gameObject);
                                cooldowns[spell] = true;
                                StartCoroutine(CooldownCounter(spell, fireBolt.GetCooldown()));
                            }
                            else {
                                Instantiate(spoof, gameObject.transform);
                            }
                            break;
                        case ("Barrage"):
                            break;
                        case ("Blast"):
                            if(!cooldowns[spell]){
                                fireBlast.Cast(this.gameObject);
                                cooldowns[spell] = true;
                                StartCoroutine(CooldownCounter(spell, fireBlast.GetCooldown()));
                            }
                            else {
                                Instantiate(spoof, gameObject.transform);
                            }
                            break;
                    }
                    break;
                case(Element.Ice):
                    switch (spellTokens[1])
                    {
                        case ("Bolt"):
                            if(!cooldowns[spell]){
                                iceBolt.Cast(this.gameObject);
                                cooldowns[spell] = true;
                                StartCoroutine(CooldownCounter(spell, iceBolt.GetCooldown()));
                            }
                            else {
                                Instantiate(spoof, gameObject.transform);
                            }
                            break;
                        case ("Barrage"):
                            break;
                        case ("Blast"):
                            if(!cooldowns[spell]){
                                iceBlast.Cast(this.gameObject);
                                cooldowns[spell] = true;
                                StartCoroutine(CooldownCounter(spell, iceBlast.GetCooldown()));
                            }
                            else {
                                Instantiate(spoof, gameObject.transform);
                            }
                            break;
                    }
                    break;
                default:
                    break;
            }
           
        }
    }

    private IEnumerator CooldownCounter(string spell, float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        cooldowns[spell] = false;
    }

    void SetupIncantDict() {
        incantDict = new Dictionary<string, string>()
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

    void SetupSpellCooldowns(){
        cooldowns = new Dictionary<string, bool>()
        {
            { "Fire Bolt", false },
            { "Fire Barrage", false },
            { "Fire Blast", false },
            { "Ice Bolt", false },
            { "Ice Barrage", false },
            { "Ice Blast", false }
        };
    }
}
