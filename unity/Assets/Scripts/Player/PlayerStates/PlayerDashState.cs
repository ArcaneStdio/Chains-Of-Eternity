using UnityEngine;

public class PlayerDashState : PlayerState
{
    private Vector2 moveInput;
    private float dashStartTime;

    public PlayerDashState(Player player, StateMachine<Player> stateMachine)
        : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        player.Invincibilty(true);// Make player invincible during dash
        // Not enough energy? Go back to previous state
        if (player.currentEnergy < player.dashEnergyCost)
        {
            Debug.Log("Not enough energy to dash.");
            stateMachine.ChangeState(player.idleState);
            return;
        }

        player.Animator.SetBool("dashing", true);
        player.Animator.SetBool("walking", false);
        // Consume energy
        player.UseEnergy(player.dashEnergyCost);
        moveInput = InputManager.Instance.MoveDirection.normalized;
        if (moveInput == Vector2.zero)
            moveInput = player.IsFacingRight ? Vector2.right : Vector2.left;

        player.SetVelocity(moveInput * player.dashSpeed);
        dashStartTime = Time.time;

        player.StartCoroutine(DashCooldown());
    }
    public override void Exit()
    {   
        base.Exit();
        player.Animator.SetBool("dashing", false);
        player.Invincibilty(false); // Disable invincibility after dash

    }
    public override void LogicUpdate()
    {
        if (Time.time >= dashStartTime + player.dashDuration)
        {
            player.SetVelocity(Vector2.zero);

            if (InputManager.Instance.MoveDirection != Vector2.zero)
                stateMachine.ChangeState(player.moveState);
            else
                stateMachine.ChangeState(player.idleState);
        }
    }

    public override void HandleInput()
    {
        // Ignore input during dash
    }

    private System.Collections.IEnumerator DashCooldown()
    {
        player.canDash = false;
        yield return new WaitForSeconds(player.dashCooldown);
        player.canDash = true;
    }
}
