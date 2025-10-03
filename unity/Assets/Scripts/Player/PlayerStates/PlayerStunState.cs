using UnityEngine;

public class PlayerStunState : PlayerState
{
    private float stunDuration = 0.5f;
    private float startTime;

    public PlayerStunState(Player player, StateMachine<Player> stateMachine)
        : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        player.Animator.SetBool("walking", false); 
        player.Animator.SetBool("walkingUp", false);
        player.Animator.SetBool("walkingDown", false);
        player.Animator.SetBool("dashing", false);
        player.Enable_DisableInput(false); // Disable input
        player.attackStateMachine.ChangeState(player.noAttackState);
        startTime = Time.time;
        player.SetVelocity(Vector2.zero); // Optional freeze
    }
    public override void Exit()
    {
        base.Exit();
        player.Enable_DisableInput(true); // Re-enable input
                                          // Reset animation to idle after stun
         player.spriteRenderer.color = Color.white; // Reset color if it was changed
    }
    public override void LogicUpdate()
    {
        if (Time.time >= startTime + stunDuration)
        {
            player.PlayAnimation("Idle"); // Reset animation to idle after stun
            stateMachine.ChangeState(player.idleState); // or moveState if input exists
        }
    }
 // Disable input while stunned
}
