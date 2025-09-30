using UnityEngine;

public class StateMachine<T>
{
    public State<T> CurrentState { get; private set; }

    public void Initialize(State<T> startingState)
    {
        CurrentState = startingState;
        CurrentState.Enter();
    }

    public void ChangeState(State<T> newState)
    {
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    public void HandleInput()
    {
        CurrentState?.HandleInput();
    }

    public void LogicUpdate()
    {
        CurrentState?.LogicUpdate();
    }

    public void PhysicsUpdate()
    {
        CurrentState?.PhysicsUpdate();
    }
}
