using UnityEngine;

public class InGameState : State<GameManager>
{
    public InGameState(GameManager owner, StateMachine<GameManager> stateMachine)
        : base(owner, stateMachine) { }

    public override void Enter()
    {
        //Debug.Log("Entered InGame State");
        Time.timeScale = 1f;
        owner.ShowGameplayUI();
    }

    public override void Exit()
    {
        //Debug.Log("Exiting InGame State");
    }

    public override void HandleInput()
    {
        if (InputManager.Instance.InventoryPressed)
        {
            stateMachine.ChangeState(new InventoryOpenState(owner, stateMachine));
        }
        else if (InputManager.Instance.PausePressed)
        {
            stateMachine.ChangeState(new PausedState(owner, stateMachine));
        }
        else if (InputManager.Instance.StatsPressed)
        {
            stateMachine.ChangeState(new StatsOpenState(owner, stateMachine));
        }
    }
}
