using UnityEngine;

public abstract class PlayerState : State<Player>
{   
    protected Player player;
    protected PlayerState(Player player, StateMachine<Player> stateMachine)
        : base(player, stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    // You can optionally override and extend these for all PlayerStates
    public override void HandleInput()
    {
        base.HandleInput();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
