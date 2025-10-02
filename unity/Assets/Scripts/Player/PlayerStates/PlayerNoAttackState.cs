using UnityEngine;

public class PlayerNoAttackState : PlayerState
{
    
    private bool attackComplete = false;

    public PlayerNoAttackState(Player player, StateMachine<Player> stateMachine)
        : base(player, stateMachine) { }

    public override void Enter()
    {
        player.Enable_DisableInput(true);
        player.Animator.SetBool("attacking", false);
    }

    public override void LogicUpdate()
    {
        //base.LogicUpdate();
        if (InputManager.Instance.AttackPressed)
        {
            //Debug.Log("Attack buttn pressed");
            stateMachine.ChangeState(player.attackState);
            return;
        }

    }

    public void OnAttackAnimationComplete() => attackComplete = true;

    public override void Exit()
    {
        player.ClearHitEnemies();
    }
}
