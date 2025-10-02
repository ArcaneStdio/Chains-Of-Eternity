using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public Vector2 MoveDirection { get; private set; }
    public bool DashPressed { get; private set; }
    public bool AttackPressed { get; private set; }
    public bool IsAttackHeld { get; private set; }
    public bool PausePressed { get; private set; }
    public bool StatsPressed { get; private set; } // Assuming Stats is the same as Pause for now
    public bool InventoryPressed { get; private set; }
    public bool InteractPressed { get; private set; }
    public bool Spell1Pressed { get; private set; }
    public bool Spell2Pressed { get; private set; }
    public bool Spell3Pressed { get; private set; }
    public bool Spell4Pressed { get; private set; }
    public bool Potion1Pressed { get; private set; }
    public bool Potion2Pressed { get; private set; }
    public bool Potion3Pressed { get; private set; }

    private PlayerInputActions inputActions;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        inputActions = new PlayerInputActions();
        inputActions.Enable();
    }
    public void DisableInput()
    {
        inputActions.Disable();
    }
    public void EnableInput()
    {
        inputActions.Enable();
    }

    private void Update()
    {
        MoveDirection = inputActions.Player.Move.ReadValue<Vector2>();
        DashPressed = inputActions.Player.Dash.triggered;
        AttackPressed = inputActions.Player.Attack.triggered;
        IsAttackHeld = inputActions.Player.Attack.ReadValue<float>() > 0.1f;
        PausePressed = inputActions.Player.Pause.triggered;
        StatsPressed = inputActions.Player.Stats.triggered;
        InventoryPressed = inputActions.Player.Inventory.triggered;
        InteractPressed = inputActions.Player.Interact.triggered;
        Spell1Pressed = inputActions.Player.Spell1.triggered;
        Spell2Pressed = inputActions.Player.Spell2.triggered;
        Spell3Pressed = inputActions.Player.Spell3.triggered;
        Spell4Pressed = inputActions.Player.Spell4.triggered;
        Potion1Pressed = inputActions.Player.potion1.triggered;
        Potion2Pressed = inputActions.Player.potion2.triggered;
        Potion3Pressed = inputActions.Player.potion3.triggered;


        if (InteractPressed)
        {
            Debug.Log("InputManager: InteractPressed is TRUE this frame!");
        }
    }
}
