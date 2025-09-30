using UnityEngine;

public abstract class State<T>
{
    protected T owner;
    protected StateMachine<T> stateMachine;

    protected State(T owner, StateMachine<T> stateMachine)
    {
        this.owner = owner;
        this.stateMachine = stateMachine;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void HandleInput() { }
    public virtual void LogicUpdate() { }
    public virtual void PhysicsUpdate() { }
}
