using UnityEngine;

public class StatsOpenState : State<GameManager>
{
    public StatsOpenState(GameManager owner, StateMachine<GameManager> stateMachine)
        : base(owner, stateMachine) { }

    public override void Enter()
    {
        Time.timeScale = 0f;
        owner.ShowStatsCanvas();
    }

    public override void Exit()
    {
        //Debug.Log("Exiting stats ");
        owner.HideStatsCanvas();
    }

    public override void HandleInput()
    {
        if (InputManager.Instance.StatsPressed)
        {
            stateMachine.ChangeState(new InGameState(owner, stateMachine));
        }
    }
}
