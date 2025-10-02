using System;
using UnityEngine;

public class AttackState : BaseEnemyState
{
    private float attackCooldown;
    private bool attacked = false;
    private float attackTimer;
    private bool isAnimationTriggered = false;

    public AttackState(Enemy owner, StateMachine<Enemy> stateMachine)
        : base(owner, stateMachine) { }

    public override void Enter()
    {
        owner.performedAttack = false;
        StopMoving();


        isAnimationTriggered = false;
        owner.animator.SetBool("isAttacking", true);
        owner.animator.SetBool("followPlayer", false);
        owner.animator.SetBool("freeRoam", false);
    }

    public override void Exit()
    {
        owner.animator.SetBool("isAttacking", false);
    }

    public override void LogicUpdate()
    {
        
        if (!owner.CanSeePlayer())
        {
            Debug.Log("Shifting from attack to free roaming state");
            stateMachine.ChangeState(owner.FreeRoamingState);
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        StopMoving();
        base.PhysicsUpdate();

        attackTimer += Time.fixedDeltaTime;
         Vector2 directionToPlayer = (Player.position - owner.transform.position).normalized;
        if (owner.IsFacingRight && directionToPlayer.x < 0 || !owner.IsFacingRight && directionToPlayer.x > 0)
        {
            owner.Flip();
        }
        if (owner.performedAttack)
        {
            attackTimer = 0f;
            // Disable attack animation until next trigger
            // owner.animator.SetBool("isAttacking", true);
            // owner.animator.SetBool("freeRoam", false);
            // owner.animator.SetBool("followPlayer", false);
            // isAnimationTriggered = false;
            Debug.Log("Attack performed, shifting to cooldown state");
             stateMachine.ChangeState(owner.CooldownState);
        }
    }
}

