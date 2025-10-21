using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PlayerSpellCaster : MonoBehaviour, ISpellCaster
{

    [SerializeField] private bool dmgBuffed = false;
    public float dmgBuffMultiplier = 1.5f;
    Dictionary<string, bool> cooldowns = new Dictionary<string, bool>();
    private string incant = "";
    
    public GameObject spoof;

    LazyDependency<InputManager> _InputManager;
    LazyDependency<SpellDatabase> _SpellDatabase;


    public bool DmgBuffed => dmgBuffed;
    public Dictionary<string, bool> SpellCooldowns => cooldowns;


    void Start()
    {
        _InputManager.Value.OnGrimoireIncant += HandleInput;
    }

    private void HandleInput(string c)
    {
        incant += c;
        
        if (incant.Length > 6)
        {
            HandleCastSpell("Spoof");
            incant = "";
        }
        else if (incant.Length > 3){
            string spell = _SpellDatabase.Value.ConvertInput(incant);
            HandleCastSpell(spell);
        }
    }

    private void HandleCastSpell(string spell)
    {
        if (spell.Equals("Spoof")){
            Instantiate(spoof, gameObject.transform);
        }
        else if (!spell.Equals("Invalid")){
            if (!cooldowns.ContainsKey(spell))
                cooldowns.Add(spell, false);
            if (!cooldowns[spell]){
                _SpellDatabase.Value.GetSpell(spell).Cast(this.gameObject);
                cooldowns[spell] = true;
                StartCoroutine(CooldownCounter(spell, _SpellDatabase.Value.GetSpell(spell).GetCooldown()));
            } else {
                Instantiate(spoof, gameObject.transform);
            }
                
            incant = "";
        }
    }

    public bool IsBuffed()
    {
        return dmgBuffed;
    }

    public void SetDmgBuff(bool value)
    {
        dmgBuffed = value;
    }

   
    public float GetDmgMultiplier()
    {
        if(dmgBuffed)
        {
            return dmgBuffMultiplier;
        }
        return 1f;
    }

    private IEnumerator CooldownCounter(string spell, float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        cooldowns[spell] = false;
    }

}
