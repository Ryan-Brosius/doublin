
/*  Owner: Ryan
 * 
 *  Needs to be changed in the future to include all of the spell information that we will want
 *  Making it right now just so I don't get any compiler errors when setting up entity framework
 */
using UnityEngine;

public struct Damage
{
    public float Amount;
    public Element Element;
    public GameObject Source;

    public Damage(float amount, Element element, GameObject source = null)
    {
        Amount = amount;
        Element = element;
        Source = source;
    }
}