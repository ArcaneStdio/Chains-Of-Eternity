using UnityEngine;

public class InventoryOpenState : State<GameManager>
{
    public InventoryOpenState(GameManager owner, StateMachine<GameManager> stateMachine)
        : base(owner, stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Entered Inventory State");
        Time.timeScale = 0f; // Optional: pause gameplay while inventory is open
        owner.ShowInventoryUI();
    }

    public override void Exit()
    {
        Debug.Log("Exiting Inventory State");
        owner.HideInventoryUI();
    }

    public override void HandleInput()
    {
        if (InputManager.Instance.InventoryPressed)
        {
            stateMachine.ChangeState(new InGameState(owner, stateMachine));
        }
    }
}
