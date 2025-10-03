using System;
using UnityEngine;

public class PlayerMoveState : PlayerState
{
    private Vector2 moveInput;
    private bool spellcast;
    public PlayerMoveState(Player player, StateMachine<Player> stateMachine)
        : base(player, stateMachine) { }

    public override void Enter()
    {
        //player.PlayAnimation("Run");
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        moveInput = InputManager.Instance.MoveDirection;

        if (moveInput == Vector2.zero)
        {
            //Debug.Log("Player has stopped moving.");
            stateMachine.ChangeState(player.idleState);
            return;
        }
        if (InputManager.Instance.DashPressed && player.canDash && player.CurrentEnergy >= player.DashEnergyCost)
        {
            stateMachine.ChangeState(player.dashState);
            return;
        }


        if (moveInput.y > 0)
        {
            player.Animator.SetBool("walkingUp", true);
            player.Animator.SetBool("walking", false);
            player.Animator.SetBool("walkingDown", false);
        }
        else if (moveInput.y < 0)
        {
            player.Animator.SetBool("walkingUp", false);
            player.Animator.SetBool("walkingDown", true);
            player.Animator.SetBool("walking", false);
        }
        else
        {
            player.Animator.SetBool("walkingUp", false);
            player.Animator.SetBool("walkingDown", false);
            player.Animator.SetBool("walking", true);
        }

        player.FlipIfNeeded(moveInput.x);
        
        spellcast = InputManager.Instance.Spell1Pressed || InputManager.Instance.Spell2Pressed || 
                    InputManager.Instance.Spell3Pressed || InputManager.Instance.Spell4Pressed;
        if (spellcast && player.CurrentMana > 20)
        {
            stateMachine.ChangeState(player.spellState);
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        player.SetVelocity(moveInput * player.moveSpeed);
    }

    public override void Exit()
    {
        //Debug.Log("CHal raha hai kya");
        //player.Animator.SetBool("walkingUp", false);
        //player.Animator.SetBool("walkingDown", false);
        player.Animator.SetBool("walking", false);
        //player.Animator.SetBool("walkingUp", false);
        //player.Animator.SetBool("walkingDown", false);
    }
}
