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
    private string currentSpell = "Spoof";

    [Tooltip("Optional cast point where the spell will be cast from")]
    [SerializeField] private GameObject castPoint;

    LazyDependency<InputManager> _InputManager;
    LazyDependency<SpellDatabase> _SpellDatabase;
    LazyDependency<AimingManager> _AimingManager;


    public bool DmgBuffed => dmgBuffed;
    public Dictionary<string, bool> SpellCooldowns => cooldowns;


    void Start()
    {
        _InputManager.Value.OnGrimoireIncant += HandleInput;
        _InputManager.Value.OnStaffCast += HandleCastConfirm;
        _InputManager.Value.OnStaffCancelIncant += HandleCastCancel;
    }

    private void HandleInput(string c)
    {
        incant += c;
        
        if (incant.Length > 6)
        {
            HandleCastSpell("Invalid");
            incant = "";
        }
        else if (incant.Length > 3){
            currentSpell = _SpellDatabase.Value.ConvertInput(incant);
        }
    }

    private void HandleCastConfirm()
    {
        HandleCastSpell(currentSpell);
        incant = "";
        currentSpell = "";
    }

    private void HandleCastCancel() 
    {
        HandleCastSpell("Cancel");
        incant = "";
    }

    private void HandleCastSpell(string spell)
    {
        if (!cooldowns.ContainsKey(spell))
            cooldowns.Add(spell, false);
        if (!cooldowns[spell]){
            var spellData = _SpellDatabase.Value.GetSpell(spell);
            if (spellData != null)
            {
                spellData.Cast(
                    gameObject,
                    castPoint != null ? castPoint.transform.position : gameObject.transform.position,
                    positionTarget: _AimingManager.Value.CurrentTarget != null ? _AimingManager.Value.CurrentTarget.transform.position : _AimingManager.Value.CrosshairPosition
                );
                cooldowns[spell] = true;
                StartCoroutine(CooldownCounter(spell, _SpellDatabase.Value.GetSpell(spell).GetCooldown()));
            }
        } else {
            _SpellDatabase.Value.GetSpell("Cooldown").Cast(
                gameObject,
                castPoint != null ? castPoint.transform.position : gameObject.transform.position
            );
        }
        incant = "";
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
