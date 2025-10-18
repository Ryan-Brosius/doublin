using System.Collections.Generic;
using UnityEngine;

/*  Owner: Ryan Brosius
 * 
 *  Scriptable Object that contains the information around elements interacting with eachother
 *  Ideally only one of these scriptable objects should even exist in the files
 *  Its basically just a simple-lookup container that we can reference
 *  Think of it as a pointer to a map, but not a map, and not a pointer, and nothing like a pointer map
 */
[CreateAssetMenu(menuName = "Combat/Elemental Resolver")]
public class ElementalResolver : ScriptableObject
{
    [System.Serializable]
    public struct Reaction
    {
        public Element Attacker;
        public Element Defender;
        public float Multiplier;
    }

    [SerializeField]
    [Tooltip("List of reaction multipliers. If reaction doesnt exists, defaults to 1x multiplier")]
    private List<Reaction> reactions = new();

    public float GetMultiplier(Element attacker, Element defender)
    {
        foreach (var r in reactions)
            if (r.Attacker == attacker && r.Defender == defender)
                return r.Multiplier;
        return 1f;
    }
}
