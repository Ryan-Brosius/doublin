using System.Collections.Generic;
using UnityEngine;

public interface ISpellCaster
{
    Dictionary<string, bool> SpellCooldowns { get; }
    bool DmgBuffed { get; }
    bool IsBuffed();
    float GetDmgMultiplier();
}
