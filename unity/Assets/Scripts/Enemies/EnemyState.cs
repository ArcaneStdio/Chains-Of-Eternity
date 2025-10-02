using NavMeshPlus.Extensions;
using UnityEngine;

public abstract class BaseEnemyState : State<Enemy>
{
    protected BaseEnemyState(Enemy owner, StateMachine<Enemy> stateMachine)
        : base(owner, stateMachine) { }

    protected Transform Player => owner.PlayerTransform;
    protected Rigidbody2D Rb => owner.RB;
    protected float MoveSpeed => owner.MoveSpeed;

    protected void MoveTowards(Vector2 target, float moveSpeed)
    {
        if(owner.agent.isActiveAndEnabled)
        {
            owner.agent.speed = moveSpeed;
            owner.agent.SetDestination(target);
        }
    }

    protected void StopMoving()
    {
        owner.agent.speed = 0;
        if(owner.RB != null)
        {
            owner.RB.linearVelocity = Vector2.zero;
        }
        //owner.RB.linearVelocity = Vector2.zero;
    }
}
