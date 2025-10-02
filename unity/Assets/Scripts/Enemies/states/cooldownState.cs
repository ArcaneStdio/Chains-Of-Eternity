using System;
using UnityEngine;

public class CooldownState : BaseEnemyState
{
    private float idleDuration = 0;
    private float idleTimer;

    public CooldownState(Enemy owner, StateMachine<Enemy> stateMachine, float idleDuration = 2f)
        : base(owner, stateMachine)
    {
        this.idleDuration = owner.AttackCooldown;
    }

    public override void Enter()
    {
        StopMoving();
        idleTimer = 0f;
        Debug.Log("------------------------Entering cooldown state----------------------");
        owner.animator.SetBool("followPlayer", false);
        owner.animator.SetBool("freeRoam", false);
        owner.animator.SetBool("isAttacking", false);
    }

    public override void LogicUpdate()
    {
        //StopMoving();
        idleTimer += Time.deltaTime;


        if (idleTimer >= idleDuration)
        {
            if (owner.CanSeePlayer())
            {
                Debug.Log("Player detected, transitioning to follow state");
                stateMachine.ChangeState(owner.FollowState);
            }else
            Debug.Log("Idle duration reached, transitioning to free roaming state");
            stateMachine.ChangeState(owner.FreeRoamingState);

        }
    }
}
