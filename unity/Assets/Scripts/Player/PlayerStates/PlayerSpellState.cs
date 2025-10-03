using UnityEngine;

public class PlayerSpellState : PlayerState
{
    private float castDuration = 0.5f;
    private float startTime;

    public PlayerSpellState(Player player, StateMachine<Player> stateMachine)
        : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        player.Animator.SetBool("walking", false); 
        player.Animator.SetBool("walkingUp", false);
        player.Animator.SetBool("walkingDown", false);
        player.Animator.SetBool("dashing", false);
        player.Animator.SetBool("attacking", false);
        player.Animator.SetBool("casting", true); // Set casting animation
        player.Enable_DisableInput(false); // Disable input
        player.attackStateMachine.ChangeState(player.noAttackState);
        startTime = Time.time;
        player.SetVelocity(Vector2.zero); // Optional freeze
    }
    public override void Exit()
    {
        base.Exit();
        player.Enable_DisableInput(true); // Re-enable input
        player.Animator.SetBool("casting", false); // Reset casting animation
         // Reset animation to idle after stun
    }
    public override void LogicUpdate()
    {
        if (Time.time >= startTime + castDuration)
        {
            player.PlayAnimation("Idle"); // Reset animation to idle after stun
            stateMachine.ChangeState(player.idleState); // or moveState if input exists
        }
    }
 // Disable input while stunned
}
