using UnityEngine;
public class FollowState : BaseEnemyState
{
    public FollowState(Enemy owner, StateMachine<Enemy> stateMachine)
        : base(owner, stateMachine) { }

    public override void Enter()
    {
        owner.animator.SetBool("followPlayer", true);
        owner.animator.SetBool("freeRoam", false);
        owner.animator.SetBool("isAttacking", false);
    }
    public override void LogicUpdate()
    {
        if (!owner.CanSeePlayer())
        {
            stateMachine.ChangeState(owner.FreeRoamingState);
            return;
        }

        float distanceToPlayer = Vector2.Distance(owner.transform.position, Player.position);
        if (distanceToPlayer <= owner.AttackRange)
        {
            Debug.Log("Switching to Attack State");
            stateMachine.ChangeState(owner.AttackState);
            return;
        }

        MoveTowards(Player.position, owner.MoveSpeed + owner.FollowOffset);
    }
}
