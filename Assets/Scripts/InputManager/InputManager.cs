using System;
using UnityEngine;
using static UnityEngine.Rendering.VolumeComponent;

/*  Owner: Ryan Brosius
 * 
 *  This is the wrapper for unity input actions into our own actions. Instead of grabbing the input actions from the
 *  input actions ourselves, the idea is we can give *some* of the gamestate to this input manager, and it can evolve
 *  the actions on whoever needs it.
 *  
 *  For example, if the goblin state is the book goblin on the bottom, then we can allow their move input to be the overall move input (for the combined goblin).
 *  If the bottom goblin is the caster, then we use their inputs.
 *  
 *  Cheeky but is only really made for keyboard at the moment, I can gloss over and try to make it work with two controllers in the future :')
 */
public class InputManager : SingletonMonobehavior<InputManager>
{
    // This is the unity input actions generated class
    // Need to re-generate this each time in the root whenever we add more actions
    private DoublinInputActions inputActions;

    // GRIMOIRE GOBLIN ACTIONS HERE
    public event Action<Vector2> OnGrimoireMove;
    public event Action<Vector2> OnGrimoireLook;
    public event Action OnGrimoireJump;
    public event Action<string> OnGrimoireIncant;  // enum in the future(?)
    public event Action OnGrimoireBookInteract;

    // STAFF GOBLIN ACTIONS HERE
    public event Action<Vector2> OnStaffMove;
    public event Action<Vector2> OnStaffLook;
    public event Action OnStaffJump;
    public event Action OnStaffCast;
    public event Action OnStaffCancelIncant;

    // ADD COMBINED GOBLIN ACTIONS HERE
    public event Action<Vector2> OnCombinedMove;
    public event Action<Vector2> OnCombinedLook;
    public event Action OnCombinedSeperate;

    // ADD ALL UI ACTIONS HERE


    // Internal enum to make referencing states easier
    // This has actually become pretty important for determining players btw
    // Consider moving this to its own area in the future?
    public enum PlayerID
    {
        GrimoireGoblin,
        StaffGoblin
    }

    private LazyDependency<GoblinStateManager> _GoblinStateManager;

    protected override void Awake()
    {
        base.Awake();

        inputActions = new DoublinInputActions();

        // Player 1 (Grimoire Goblin)
        inputActions.Player1.Move.performed += ctx => HandleMove(PlayerID.GrimoireGoblin, ctx.ReadValue<Vector2>());
        inputActions.Player1.Move.canceled += _ => HandleMove(PlayerID.GrimoireGoblin, Vector2.zero);
        inputActions.Player1.Jump.performed += _ => HandleJump(PlayerID.GrimoireGoblin);
        inputActions.Player1.Look.performed += ctx => HandleLook(PlayerID.GrimoireGoblin, ctx.ReadValue<Vector2>());
        inputActions.Player1.Look.canceled += ctx => HandleLook(PlayerID.GrimoireGoblin, Vector2.zero);
        inputActions.Player1.Incant.performed += ctx => HandleIncant(PlayerID.GrimoireGoblin, ctx.control.name);
        inputActions.Player1.Jump.performed += ctx => HandleBookInteract(PlayerID.GrimoireGoblin);

        // Player 2 (Staff Goblin)
        inputActions.Player2.Move.performed += ctx => HandleMove(PlayerID.StaffGoblin, ctx.ReadValue<Vector2>());
        inputActions.Player2.Move.canceled += _ => HandleMove(PlayerID.StaffGoblin, Vector2.zero);
        inputActions.Player2.Jump.performed += _ => HandleJump(PlayerID.StaffGoblin);
        inputActions.Player2.Look.performed += ctx => HandleLook(PlayerID.StaffGoblin, ctx.ReadValue<Vector2>());
        inputActions.Player2.Look.canceled += ctx => HandleLook(PlayerID.StaffGoblin, Vector2.zero);
        inputActions.Player2.Jump.performed += _ => HandleCast(PlayerID.StaffGoblin);
        inputActions.Player2.CancelIncant.performed += _ => HandleCancelIncantation(PlayerID.StaffGoblin);
    }

    private void OnEnable()
    {
        inputActions.Player1.Enable();
        inputActions.Player2.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player1.Disable();
        inputActions.Player2.Disable();
    }

    private void HandleMove(PlayerID player, Vector2 input)
    {
        // Handles movement if players are seperated
        if (player == PlayerID.GrimoireGoblin)
        {
            OnGrimoireMove?.Invoke(input);
        }
        else
        {
            OnStaffMove?.Invoke(input);
        }

        // Handles movement if players are combined
        switch (_GoblinStateManager.Value.CurrentGoblinState.Value)
        {
            case GoblinState.Separated:
                OnCombinedMove?.Invoke(Vector2.zero);
                break;
            case GoblinState.StaffTop:
                if (player == PlayerID.GrimoireGoblin) OnCombinedMove?.Invoke(input);
                break;
            case GoblinState.BookTop:
                if (player == PlayerID.StaffGoblin) OnCombinedMove?.Invoke(input);
                break;
        }
    }

    private void HandleJump(PlayerID player)
    {
        // Handles jump if players are seperated
        if (player == PlayerID.GrimoireGoblin)
        {
            OnGrimoireJump?.Invoke();
        }
        else
        {
            OnStaffJump?.Invoke();
        }

        // Handles jump if players are combined
        // only top goblin is allowed to jump (off)
        switch (_GoblinStateManager.Value.CurrentGoblinState.Value)
        {
            case GoblinState.StaffTop:
                if (player == PlayerID.GrimoireGoblin) OnCombinedSeperate?.Invoke();
                break;
            case GoblinState.BookTop:
                if (player == PlayerID.StaffGoblin) OnCombinedSeperate?.Invoke();
                break;
        }
    }

    private void HandleLook(PlayerID player, Vector2 input)
    {
        // Handles look if players are seperated
        if (player == PlayerID.GrimoireGoblin)
        {
            OnGrimoireLook?.Invoke(input);
        }
        else
        {
            OnStaffLook?.Invoke(input);
        }

        if (player == PlayerID.StaffGoblin)
        {
            OnCombinedLook?.Invoke(input);
        }
    }

    private void HandleIncant(PlayerID player, string incant)
    {
        // Only incant if goblin state allows
        if (player == PlayerID.GrimoireGoblin)
        {
            switch (_GoblinStateManager.Value.CurrentGoblinState.Value)
            {
                case GoblinState.StaffTop:
                case GoblinState.BookTop:
                    OnGrimoireIncant?.Invoke(incant);
                    break;
                case GoblinState.Separated:
                    break;
            }
        }
    }

    private void HandleCast(PlayerID player)
    {
        if (player == PlayerID.StaffGoblin)
        {
            switch (_GoblinStateManager.Value.CurrentGoblinState.Value)
            {
                case GoblinState.StaffTop:
                    OnStaffCast?.Invoke();
                    break;
                case GoblinState.BookTop:
                case GoblinState.Separated:
                    break;
            }
        }
    }

    private void HandleBookInteract(PlayerID player)
    {
        if (player == PlayerID.StaffGoblin)
        {
            switch (_GoblinStateManager.Value.CurrentGoblinState.Value)
            {
                case GoblinState.StaffTop:
                    OnGrimoireBookInteract?.Invoke();
                    break;
                case GoblinState.BookTop:
                case GoblinState.Separated:
                    break;
            }
        }
    }

    private void HandleCancelIncantation(PlayerID player)
    {
        if (player == PlayerID.StaffGoblin)
        {
            switch (_GoblinStateManager.Value.CurrentGoblinState.Value)
            {
                case GoblinState.StaffTop:
                    OnStaffCancelIncant?.Invoke();
                    break;
                case GoblinState.BookTop:
                case GoblinState.Separated:
                    break;
            }
        }
    }
}
