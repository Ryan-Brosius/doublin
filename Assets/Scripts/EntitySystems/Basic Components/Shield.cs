using System;
using UnityEngine;

/*  Owner: Ryan Brosius
 * 
 *  Monobehavior script to slap on prefabs to make them into shields that can be loaded into entities
 *  Can be created on-demand with the ShieldSpellData cast, and info is passed through initialize
 */
public class Shield : MonoBehaviour, IShield
{
    [SerializeField] private float shieldHealth = 10f;
    [SerializeField] private Element element;
    [SerializeField] private ElementalResolver resolver;

    [Header("Spell VFX Settings")]
    [Tooltip("The GameObject that contains the shield VFX")]
    [SerializeField] private GameObject ShieldVFX;
    [Tooltip("The scale multiplier the shield will have around the enemy. A value of 1 would encapsulate directly to the most outer point but potentially will clip into the entity.")]
    [SerializeField, Range(1f, 3f)] private float shieldScaleMultiplier = 1.7f;

    public float Health => shieldHealth;
    public Element Element => element;
    public bool IsActive => shieldHealth > 0f;

    private void Awake()
    {
        ScaleShieldVFX();
    }

    // Called from a ShieldSpellData scriptable object to dynamically create a shield
    public void Initialize(float hp, Element elem, ElementalResolver res = null, GameObject shieldVFX = null)
    {
        shieldHealth = hp;
        element = elem;
        resolver = res ?? null;
    }

    public bool AbsorbDamage(Damage damage, out float leftover)
    {
        leftover = 0f;
        // Grab element multiplier if we can, otherwise just use 1
        float multiplier = resolver?.GetMultiplier(damage.Element, element) ?? 1f;
        float adjustedDamage = damage.Amount * multiplier;

        if (adjustedDamage >= shieldHealth)
        {
            leftover = adjustedDamage - shieldHealth;
            Break();
            return true;
        }

        shieldHealth -= adjustedDamage;
        return false;
    }

    public void Break()
    {
        shieldHealth = 0f;
        Destroy(gameObject);
    }

    private void ScaleShieldVFX()
    {
        if (ShieldVFX == null)
            return;

        // Grab the parent that we are attatched to
        Transform parent = transform.parent;
        if (parent == null)
            // If parent not found just bail, was not spawned/attatched properly
            return;

        ShieldVFX.transform.SetParent(parent);

        // Grab all renderers in the parent
        Renderer[] renderers = parent.GetComponentsInChildren<Renderer>();

        // If no renderers are found then just bail
        if (renderers.Length == 0)
            return;

        // Grow a bounds to the size of the largest from the center
        Bounds totalBounds = renderers[0].bounds;
        foreach (var renderer in renderers)
            totalBounds.Encapsulate(renderer.bounds);

        // Scale up the sphere VFX as needed to be largest bounds size * multiplier
        float maxSize = Mathf.Max(totalBounds.size.x, totalBounds.size.y, totalBounds.size.z);
        ShieldVFX.transform.localScale = ShieldVFX.transform.localScale * maxSize * shieldScaleMultiplier;
    }
}
