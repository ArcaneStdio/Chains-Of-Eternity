using UnityEngine;
using PurrNet;

[RequireComponent(typeof(PlayerStats))]
public class Player : NetworkBehaviour
{
    public PlayerStats Stats { get; private set; }
    public Animator Animator => Stats.Animator;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float dashSpeed = 15f;
    public float dashDuration = 1f;
    public float dashCooldown = 0.5f;
    public float knockbackForce = 20f;

    public bool canDash = true;
    public bool IsFacingRight = true;

    // State machines
    public StateMachine<Player> stateMachine { get; private set; }
    public StateMachine<Player> attackStateMachine { get; private set; }

    // States
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerStunState stunState { get; private set; }
    public PlayerAttackState attackState { get; private set; }
    public PlayerNoAttackState noAttackState { get; private set; }
    public PlayerSpellState spellState { get; private set; }

    // Components
    public Rigidbody2D rb { get; private set; }
    public Animator animator { get; private set; }
    public SpriteRenderer spriteRenderer;
    private attackHandler attackHandler;

    private BoxCollider2D attackHitbox;
    private BoxCollider2D upHitbox;
    private BoxCollider2D downHitbox;
    [Header("Stats")]
    [SerializeField] public int DashEnergyCost = 25;
    public float CurrentHealth => Stats.currentHealth.value;
    public float CurrentMana => Stats.currentMana.value;
    public float CurrentEnergy => Stats.currentEnergy.value;
    public void UseEnergy(int amount) => Stats.UseEnergy(amount);
    [SerializeField] private LayerMask enemyLayer;

    private PlayerStats stats;

    private void Awake()
    {
        Stats = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        attackHandler = GetComponentInChildren<attackHandler>();

        stateMachine = new StateMachine<Player>();
        attackStateMachine = new StateMachine<Player>();
        idleState = new PlayerIdleState(this, stateMachine);
        moveState = new PlayerMoveState(this, stateMachine);
        dashState = new PlayerDashState(this, stateMachine);
        stunState = new PlayerStunState(this, stateMachine);
        attackState = new PlayerAttackState(this, attackStateMachine);
        noAttackState = new PlayerNoAttackState(this, attackStateMachine);
        spellState = new PlayerSpellState(this, stateMachine);

        attackHitbox = attackHandler.GetComponent<BoxCollider2D>();
        upHitbox = attackHandler.upHitbox;
        downHitbox = attackHandler.downHitbox;
    }

    private void Start()
    {
        attackStateMachine.Initialize(noAttackState);
        stateMachine.Initialize(idleState);
    }

    private void Update()
    {
        stateMachine.HandleInput();
        stateMachine.LogicUpdate();
        attackStateMachine.HandleInput();
        attackStateMachine.LogicUpdate();
    }

    private void FixedUpdate()
    {
        stateMachine.PhysicsUpdate();
        attackStateMachine.PhysicsUpdate();
    }

    #region Movement + Animation
    public Vector2 CurrentVelocity => rb.linearVelocity;

    public void SetVelocity(Vector2 velocity) => rb.linearVelocity = velocity;

    public void SetXVelocity(float x) => rb.linearVelocity = new Vector2(x, rb.linearVelocity.y);

    public void FlipIfNeeded(float xInput)
    {
        if (xInput != 0)
        {
            bool shouldFlip = (xInput > 0 && !IsFacingRight) || (xInput < 0 && IsFacingRight);
            if (shouldFlip)
            {
                IsFacingRight = !IsFacingRight;
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }
    }

    public void PlayAnimation(string animName)
    {
        if (animator != null)
            animator.Play(animName);
    }
    #endregion

    #region Hitboxes
    public void EnableHitboxDef(bool value)
    {
        attackHitbox.enabled = value;
        upHitbox.enabled = value;
        downHitbox.enabled = value;
        if (value) attackHandler.ClearHitEnemies();
    }

    public void EnableHitbox() => EnableHitboxDef(true);
    public void DisableHitbox() => EnableHitboxDef(false);
    public void EnableUpHitbox() { upHitbox.enabled = true; attackHandler.ClearHitEnemies(); }
    public void DisableUpHitbox() => upHitbox.enabled = false;
    public void EnableDownHitbox() { downHitbox.enabled = true; attackHandler.ClearHitEnemies(); }
    public void DisableDownHitBox() => downHitbox.enabled = false;
    public void ClearHitEnemies()
    {
        attackHandler.ClearHitEnemies();
    }

    public void Enable_DisableInput(bool check) => Stats.Enable_DisableInput(check);

    public void AttackDone() => (attackStateMachine.CurrentState as PlayerAttackState)?.OnAttackAnimationComplete();
    #endregion
}
