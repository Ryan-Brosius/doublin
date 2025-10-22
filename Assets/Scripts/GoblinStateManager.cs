using UnityEngine;

/*  Owner: Ryan
 * 
 *  Very dirty(?) mockup so I can just have a centeralized state to pull from so all of my little managers can properly communicate with eachother
 *  In the future Im sure whoever takes this over will modify this and add a bunch of useful functionality
 */
public class GoblinStateManager : SingletonMonobehavior<GoblinStateManager>
{
    // Keep this observable as many things will rely on watching this value change
    [SerializeField] public ObservableProperty<GoblinState> CurrentGoblinState;

    [Header("Goblin GameObject References")]
    [SerializeField] private GameObject CombinedGoblin;
    [SerializeField] private GameObject GrimoireGoblin;
    [SerializeField] private GameObject StaffGoblin;

    [Header("Goblin Controller References")]
    [SerializeField] private CombinedGoblinController CombinedGoblinController;
    [SerializeField] private SplitGoblinController GrimoireGoblinController;
    [SerializeField] private SplitGoblinController StaffGoblinController;

    protected override void Awake()
    {
        base.Awake();
        CurrentGoblinState.Subscribe(ConfigureControllers);
    }

    public float DistanceBetweenGoblins()
    {
        return Vector3.Distance(CombinedGoblin.transform.position, GrimoireGoblin.transform.position);
    }

    public void CombineGoblins(GoblinState newGoblinState)
    {
        CurrentGoblinState.Value = newGoblinState;
    }

    public void SeperateGoblins()
    {
        CurrentGoblinState.Value = GoblinState.Separated;
    }

    public void ConfigureControllers(GoblinState goblinState)
    {
        CombinedGoblin.SetActive(false);
        GrimoireGoblin.SetActive(false);
        StaffGoblin.SetActive(false);

        switch (goblinState)
        {
            case GoblinState.Separated:

                GrimoireGoblin.SetActive(true);
                StaffGoblin.SetActive(true);
                break;
            case GoblinState.StaffTop:
            case GoblinState.BookTop:
                CombinedGoblin.SetActive(true);
                break;
        }
    }

    public GameObject GetBottomGoblin()
    {
        switch (CurrentGoblinState.Value)
        {
            case GoblinState.StaffTop:
                return GrimoireGoblin;
            case GoblinState.BookTop:
                return StaffGoblin;
        }
        return null;
    }

    public GameObject GetTopGoblin()
    {
        switch (CurrentGoblinState.Value)
        {
            case GoblinState.StaffTop:
                return StaffGoblin;
            case GoblinState.BookTop:
                return GrimoireGoblin;
        }
        return null;
    }

    public void ThrowGoblin(GameObject goblin, Vector3 direction, float force)
    {
        var goblinController = goblin.GetComponent<SplitGoblinController>();
        goblinController.ThrowGoblin(direction, force);
    }
}
