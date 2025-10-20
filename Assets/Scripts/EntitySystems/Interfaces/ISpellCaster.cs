using System.Collections.Generic;
using UnityEngine;

public interface ISpellCaster
{
    Dictionary<string, bool> SpellCooldowns { get; }
    bool IsBuffed { get; }
}
