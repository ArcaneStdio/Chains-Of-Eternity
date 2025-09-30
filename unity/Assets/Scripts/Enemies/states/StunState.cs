using UnityEngine;

public class StunState : BaseEnemyState
{
    private float stunDuration = 0.5f;
    private float startTime;

    public StunState(Enemy owner, StateMachine<Enemy> stateMachine)
        : base(owner, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        owner.animator.SetBool("followPlayer", false);
        owner.animator.SetBool("freeRoam", false);
        owner.animator.SetBool("isAttacking", false);
        //owner.animator.SetBool("isStunned", true); // Optional: Set a stunned animation
        StopMoving(); // Stop movement
        startTime = Time.time;
    }
    public override void Exit()
    {
        base.Exit();
        owner.animator.SetBool("isStunned", false); // Reset stunned animation
        owner.spriteRenderer.color = Color.white; // Reset color if it was changed
    }
    public override void LogicUpdate()
    {
        if (Time.time >= startTime + stunDuration)
        {

            if (owner.CanSeePlayer())
            {
                stateMachine.ChangeState(owner.FollowState);
            }

            stateMachine.ChangeState(owner.FreeRoamingState);
        }

    }
    // Disable input while stunned
}
