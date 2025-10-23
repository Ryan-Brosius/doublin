using System;
using System.Collections;
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
        StartCoroutine(ScaleShieldVFX());
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

    // We want to wait a frame before we do this because we need to wait for the clean-up from a previous shield if it existed
    // Destroy plays on late-update so we should wait for a cleanup of the previous shield before we get dimensions of this one
    private IEnumerator ScaleShieldVFX()
    {
        yield return null;

        if (ShieldVFX == null)
            yield break;

        // Grab the parent that we are attatched to
        Transform parent = transform.parent;
        if (parent == null)
            // If parent not found just bail, was not spawned/attatched properly
            yield break;

        //ShieldVFX.transform.SetParent(parent);

        // Grab all renderers in the parent
        Renderer[] renderers = parent.GetComponentsInChildren<Renderer>();

        // If no renderers are found then just bail
        if (renderers.Length == 0)
            yield break;

        // Grow a bounds to the size of the largest from the center
        Bounds totalBounds = renderers[0].bounds;
        foreach (var renderer in renderers)
            totalBounds.Encapsulate(renderer.bounds);

        // Scale up the sphere VFX as needed to be largest bounds size * multiplier
        float maxSize = Mathf.Max(totalBounds.size.x, totalBounds.size.y, totalBounds.size.z);
        ShieldVFX.transform.localScale = ShieldVFX.transform.localScale * maxSize * shieldScaleMultiplier;
    }

    private void OnDestroy()
    {
        if (ShieldVFX != null)
        {
            Destroy(ShieldVFX);
        }
    }
}
