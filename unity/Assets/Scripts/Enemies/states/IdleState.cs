using System;
using UnityEngine;

public class IdleState : BaseEnemyState
{ 
    private float idleDuration = 2f;
    private float idleTimer;

    public IdleState(Enemy owner, StateMachine<Enemy> stateMachine, float idleDuration = 2f)
        : base(owner, stateMachine)
    {
        this.idleDuration = idleDuration;
    }

    public override void Enter()
    {
        StopMoving();
        idleTimer = 0f;
        Debug.Log("Entering idle state");
        owner.animator.SetBool("followPlayer", false);
        owner.animator.SetBool("freeRoam", false);
        owner.animator.SetBool("isAttacking", false);   
    }

    public override void LogicUpdate()
    {
        StopMoving();
        idleTimer += Time.deltaTime;

        if(owner.CanSeePlayer())
        {
            Debug.Log("Player detected, transitioning to follow state");
            stateMachine.ChangeState(owner.FollowState);
        }

        if (idleTimer >= idleDuration)
        {
            Debug.Log("Idle duration reached, transitioning to free roaming state");
            stateMachine.ChangeState(owner.FreeRoamingState);

        }
    }
}
