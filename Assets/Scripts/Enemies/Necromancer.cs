using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityHFSM;

/* 
 * Owner: Cameron Romero
 *
 * Logic for the Necromancer Wizard
 */

public class Necromancer : BaseEnemy
{
    /*
     * Struct for holding the abilities for the Necromancer. 
     * Can add more abilities by adding to the enum Ability.
     * Each ability has a default cooldown and a current cooldown, only the default cooldown is set in the Inspector.
     */

    [System.Serializable]
    public class NecromancerAbilities
    {
        public enum Ability
        {
            summonSlime,
            summonSkeleton,
            summonGhouls,
            boneSpikes
        }

        public Ability ability;

        [Tooltip("Cooldown for this ability")]
        public float defaultCooldown;

        [HideInInspector]
        public float currentCooldown = 0f;
    }

    // Inspector fields for the Necromancer abilities
    [SerializeField] List<NecromancerAbilities> necromancerAbilities = new List<NecromancerAbilities>();
    [SerializeField] private GameObject slimePrefab;
    [SerializeField] private GameObject skeletonPrefab;
    [SerializeField] private GameObject ghoulPrefab;
    [SerializeField] private float numberOfSlimes = 5;
    [SerializeField] private float numberOfSkeletons = 2;
    [SerializeField] private float numberOfGhouls = 1;
    [SerializeField] private GameObject spikePrefab;
    [SerializeField] private float numberOfSpikes = 150f;
    [SerializeField] private float spikeRadius = 2f;
    [SerializeField] private GameObject warningCirclePrefab;

    // Fields for the spawn range around the player
    private float minimumSpawnRange = 3f;
    private float maximumSpawnRange = 10f;

    // Global cooldown/buffer for all spells
    [SerializeField] private float spellBuffer = 5f;
    private float spellTimer = 0f;

    // Spike Spell parameters
    private GameObject boneSpikeWarning;
    private float boneSpikeWarningTimer;
    private bool boneSpikeActive;
    private Vector3 boneSpikeTarget;
    private Quaternion boneSpikeTargetRotation;
    private Vector3 boneSpikeTargetOffset;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        fsm.AddState("Idle", new State(
              onLogic: state => {/* do nothing */}
        ));

        fsm.AddState("Attack", new State(
              onLogic: state => Attack()
        ));

        fsm.SetStartState("Idle");

        fsm.AddTwoWayTransition("Idle", "Attack",
            transition => target != null && distanceToPlayer < scanningRange);

        fsm.Init();
        FindRandomTarget();
    }

    // Will cycle through all abilities and reduce their cooldowns
    protected override void Update()
    {
        base.Update();
        fsm.OnLogic();
        UpdateSpellCooldowns();
        ReduceSpellBuffer();
    }

    private void ReduceSpellBuffer()
    {
        spellBuffer -= Time.deltaTime;
    }

    private void UpdateSpellCooldowns()
    {
        for (int i = 0; i < necromancerAbilities.Count; i++)
        {
            if (necromancerAbilities[i].currentCooldown > 0f)
            {
                necromancerAbilities[i].currentCooldown -= Time.deltaTime;
            }
        }

        if (spellTimer > 0f)
            spellTimer -= Time.deltaTime;

        if (boneSpikeActive)
        {
            boneSpikeWarningTimer -= Time.deltaTime;

            if (boneSpikeWarningTimer <= 0)
            {
                SpawnBoneSpikes();
                boneSpikeActive = false;
            }
        }
    }

    /*
     * Attack State
     *
     * When adding more abilities, add more cases in the switch statement and link the ability name and the function for that ability.
     * Will reset ability cooldown after cast.
     */

    private void Attack()
    {
        for (int i = 0; i < necromancerAbilities.Count; i++)
        {
            var ability = necromancerAbilities[i];

            if (ability.currentCooldown <= 0f && spellTimer <= 0f && spellBuffer <= 0)
            {
                FindRandomTarget();
                switch (ability.ability)
                {
                    case NecromancerAbilities.Ability.summonSlime:
                        SummonSlimes();
                        break;
                    case NecromancerAbilities.Ability.summonSkeleton:
                        SummonSkeletons();
                        break;
                    case NecromancerAbilities.Ability.summonGhouls:
                        SummonGhouls();
                        break;
                    case NecromancerAbilities.Ability.boneSpikes:
                        TriggerBoneSpikes();
                        break;
                }
                spellTimer = 2f;
                ability.currentCooldown = ability.defaultCooldown;
                break;
            }
        }
    }

    /*
     * Logic for calculating the spawn point for summons
     *
     * Calculates random x and z offsets from the player's position within a specific radius
     * From that offset position, casts rays from above to determine the surface below it (slope vs flat)
     * Returns flat surface by default if the rays hit nothing
     */
    private Vector3 calculateSummonPoint()
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);

        float distance = Random.Range(minimumSpawnRange, maximumSpawnRange);
        float xOffset = Mathf.Cos(angle) * distance;
        float zOffset = Mathf.Sin(angle) * distance;

        Vector3 spawnOrigin = target.position + new Vector3(xOffset, 20, zOffset);

        RaycastHit hit;

        if (Physics.Raycast(spawnOrigin, Vector3.down, out hit, 20f, groundLayerMask))
        {
            return hit.point + hit.normal * 0.05f;
        }
        return target.position + new Vector3(xOffset, 0f, zOffset);
    }

    // For the summon spells, each individual summoned entity has their own unique summon point.
    // To have summoned groups spawn at the same point, like the slime or skeleton, move calculateSummonPoint() outside the loop and set a small offset in the loop.
    private void SummonSlimes()
    {
        for (int i = 0; i < numberOfSlimes; i++)
        {
            Instantiate(slimePrefab, calculateSummonPoint(), Quaternion.identity);
        }
    }

    private void SummonSkeletons()
    {
        for (int i = 0; i < numberOfSkeletons; i++)
        {
            Instantiate(skeletonPrefab, calculateSummonPoint(), Quaternion.identity);
        }
    }

    private void SummonGhouls()
    {
        for (int i = 0; i < numberOfGhouls; i++)
        {
            Instantiate(ghoulPrefab, calculateSummonPoint(), Quaternion.identity);
        }
    }

    /*
     * Logic for setting the target area for the bone spike warning and bone spikes
     *
     * Raycasts from above the players head towards the ground
     * For each surface hit by the rays, it will spawn a warning circle on that surface, rotated so that it's flat to the surface
     * 
     * For example, if the player was between a sloped surface and flat surface, the ray would hit the slope and the flat
     * This would then instantiate a warning circle on each surface, but centered on the player
     * This gives the illusion of only one warning circle, as the other parts of the circle are cut off
     *
     * This isn't perfect, as it doesn't work well for cases where the player is on a ledge, but probably good enough for the jam
     */
    private void TriggerBoneSpikes()
    {
        Vector3 rayStart = target.position + Vector3.up * 1f;
        RaycastHit[] hits = Physics.RaycastAll(rayStart, Vector3.down, 20f, groundLayerMask);
        foreach (var hit in hits)
        {
            boneSpikeWarning = Instantiate(warningCirclePrefab, hit.point, Quaternion.identity);

            boneSpikeWarning.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

            boneSpikeWarning.transform.position += hit.normal * 0.01f;

            boneSpikeTarget = boneSpikeWarning.transform.position;
            boneSpikeTargetRotation = boneSpikeWarning.transform.rotation;
            boneSpikeTargetOffset = boneSpikeWarning.transform.up * 0.01f;

            Destroy(boneSpikeWarning, 1f);
        }
        boneSpikeWarningTimer = 1f;
        boneSpikeActive = true;
    }

    /*
     * Logic for spawning the bone spikes
     *
     * Uses the target set in the TriggerBoneSpikes function
     *
     * Will spawn a the number of spikes set in the Inspector, default should be 150
     * For each spike, the location of the spike will be within the given radius, but it will be randomized
     * Similar to the warning circle, a ray will be cast above the spawn point of the spike
     * The spike will be instantiated on whatever surface that the ray hits, so spikes on a slope will be spawned at increasing heights and won't be flat
     */
    private void SpawnBoneSpikes()
    {
        for (int i = 0; i < numberOfSpikes; i++)
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);

            float radius = Mathf.Sqrt(Random.Range(0f, 1f)) * spikeRadius;

            float xOffset = Mathf.Cos(angle) * radius;
            float zOffset = Mathf.Sin(angle) * radius;

            Vector3 spawnPoint = boneSpikeTarget + new Vector3(xOffset, 15f, zOffset);

            RaycastHit hit;
            if (Physics.Raycast(spawnPoint, Vector3.down, out hit, 20f, groundLayerMask))
            {
                GameObject spike = Instantiate(spikePrefab, hit.point + hit.normal * 0.01f, Quaternion.LookRotation(Vector3.forward, hit.normal));
                Destroy(spike, 1f);
            }
        }
    }
}
