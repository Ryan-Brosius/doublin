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
}
