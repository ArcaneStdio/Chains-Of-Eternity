using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class Player : MonoBehaviour
{
    public CanvasSceneTransition sceneTransition; // Reference to the scene transition script
    [Header("Hero Data")]
    public HeroData heroData; // Drag in via Inspector

    // Runtime values
    [SerializeField] public int currentHealth;
    [SerializeField] public int currentMana;
    [SerializeField] public int currentEnergy;
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    [Header("Dash Settings")]
    public float dashSpeed = 15f;
    public float dashDuration = 1f;
    public float dashCooldown = 0.5f;

    [HideInInspector]
    public bool canDash = true;

    [Header("Energy Settings")]
    [SerializeField] public int RegenerateResourcesRate = 1; // How often to regenerate resources (in seconds)
    public int dashEnergyCost = 40;

    [Header("State Machine")]
    public StateMachine<Player> stateMachine { get; private set; }
    public StateMachine<Player> AttackStateMachine { get; private set; }

    // Player States (Add more later)
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerStunState stunState { get; private set; }
    public PlayerAttackState attackState { get; private set; }
    public PlayerNoAttackState noAttackState { get; private set; }
    public PlayerSpellState spellState { get; private set; } // For future use, e.g., casting spells

    // public PlayerAttackState attackState { get; private set; } // for future

    [Header("Components")]
    public Rigidbody2D RB { get; private set; }
    public Animator Animator { get; private set; }
    public SpriteRenderer spriteRenderer { get; private set; }
    private attackHandler attackHandler; // Reference to the attack handler script

    [SerializeField] private BoxCollider2D attackHitbox; // Reference to the attack hitbox GameObject
    private BoxCollider2D upHitbox;
    private BoxCollider2D downHitbox;
    [SerializeField] private LayerMask enemyLayer;
    private GameObject playerObject;
    public Vector2 CurrentVelocity => RB.linearVelocity;
    public bool IsFacingRight = true;
    private float RegenTimer = 0f;
    public bool isInvincible = false; // For future use, e.g., after taking damage
    private bool flash = true; // Flash on hit effect
    private Transform _cameraTransform;
    private Transform spawnPoint;

    private void Awake()
    {
        RB = transform.GetComponent<Rigidbody2D>();
        Animator = transform.GetComponent<Animator>();
        Enable_DisableInput(true); // Enable input by default
        currentHealth = heroData.defensiveStats.maxHealth;
        currentMana = heroData.specialStats.maxMana;
        currentEnergy = heroData.specialStats.maxEnergy;
        _cameraTransform = Camera.main.transform;
        stateMachine = new StateMachine<Player>();
        AttackStateMachine = new StateMachine<Player>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        attackHandler = GetComponentInChildren<attackHandler>();

        idleState = new PlayerIdleState(this, stateMachine);
        moveState = new PlayerMoveState(this, stateMachine);
        dashState = new PlayerDashState(this, stateMachine);
        stunState = new PlayerStunState(this, stateMachine);
        attackState = new PlayerAttackState(this, AttackStateMachine);
        noAttackState = new PlayerNoAttackState(this, AttackStateMachine);
        spellState = new PlayerSpellState(this, stateMachine);

        spawnPoint = GetComponent<Transform>();
        upHitbox = attackHandler.upHitbox;
        downHitbox = attackHandler.downHitbox;
    }
    private void Start()
    {
        AttackStateMachine.Initialize(noAttackState); // Start with no attack state
        stateMachine.Initialize(idleState);
    }
    private void Update()
    {

        //Debug.Log(AttackStateMachine.CurrentState.ToString());

        stateMachine.HandleInput();
        stateMachine.LogicUpdate();
        AttackStateMachine.HandleInput();
        AttackStateMachine.LogicUpdate();
    }

    private void FixedUpdate()
    {
        RegenTimer += Time.fixedDeltaTime;
        if (RegenTimer >= RegenerateResourcesRate)
        {
            RegenerateResources();
            RegenTimer = 0f;
        }
        stateMachine.PhysicsUpdate();
        AttackStateMachine.PhysicsUpdate();
    }

    #region Utility Methods — Called by States

    public void SetVelocity(Vector2 velocity)
    {
        RB.linearVelocity = velocity;
    }

    public void SetXVelocity(float x)
    {
        RB.linearVelocity = new Vector2(x, RB.linearVelocity.y);
    }

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

    public void PlayAnimation(string animationName)
    {
        if (Animator != null)
        {
            Animator.Play(animationName);
        }
    }

    public void Invincibilty(bool isInvincibleee)
    {
        isInvincible = isInvincibleee;
    }
    public void TakeDamage(int damage, Vector3 sourcePos,float knockbackForce=10f, bool applyKnockback = true, bool applyStun = true, string damageType = "Physical")
    {
        if (isInvincible) return; // Ignore damage if invincible
        //TODO : Handle different damage types (e.g., Physical, Magical)
        int effectiveDamage = Mathf.Max(0, damage - heroData.defensiveStats.defense);
        currentHealth = Mathf.Max(0, currentHealth - effectiveDamage);
        if (applyKnockback)
        {
            ApplyKnockback((transform.position - sourcePos).normalized, knockbackForce, applyStun);
            if (knockbackForce > 10f)
                ShakeCamera();// Shake camera 

            if (flash)
                StartCoroutine(FlashOnHit());
        }

        if (currentHealth == 0)
            Die();
    }
    private IEnumerator FlashOnHit()
    {
        
        if (spriteRenderer == null) yield break; // No sprite renderer to flash

        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.red*2; // Flash color
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor; // Reset to original color
    }
    public void UseMana(int amount)
    {
        currentMana = Mathf.Max(0, currentMana - amount);
    }

    public void UseEnergy(int amount)
    {
        currentEnergy = Mathf.Max(0, currentEnergy - amount);
    }
    public void Enable_DisableInput(bool check)
    {
        // This method can be used to disable player input during certain states (e.g., stunned, dashing)
        if (InputManager.Instance == null)
        {
            Debug.Log("Inououotupp");
            return;
        }
        if (check)
        {
            InputManager.Instance.EnableInput();
        }
        else
        {
            InputManager.Instance.DisableInput();
        }
    }
    public void ShakeCamera(float duration = 0.1f, float magnitude = 0.2f)
    {
        StartCoroutine(ShakeCameraCoroutine(duration, magnitude));
    }
    public IEnumerator ShakeCameraCoroutine(float duration, float magnitude)
    {
        Vector3 originalLocalPosition = _cameraTransform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * magnitude;
            _cameraTransform.localPosition = originalLocalPosition + new Vector3(randomOffset.x, randomOffset.y, 0f);

            yield return null;
        }

        _cameraTransform.localPosition = originalLocalPosition; // Reset to local position
    }

    public void RegenerateResources()
    {
        currentHealth = Mathf.Min(heroData.defensiveStats.maxHealth, currentHealth + heroData.defensiveStats.healthRegeneration);
        currentMana = Mathf.Min(heroData.specialStats.maxMana, currentMana + heroData.specialStats.manaRegeneration);
        currentEnergy = Mathf.Min(heroData.specialStats.maxEnergy, currentEnergy + heroData.specialStats.energyRegeneration);
    }
    public void ApplyKnockback(Vector2 direction, float force, bool applyStun, float duration = 0.2f)
    {
        stateMachine.ChangeState(idleState);
        StartCoroutine(KnockbackRoutine(direction, force, duration, applyStun));
    }
    private IEnumerator KnockbackRoutine(Vector2 direction, float force, float duration, bool applyStun)
    {
        // Freeze current velocity and apply impulse
        RB.linearVelocity = Vector2.zero;
        Enable_DisableInput(false); // Disable input during knockback
        RB.AddForce(direction.normalized * force, ForceMode2D.Impulse);
        // Wait for knockback duration
        yield return new WaitForSeconds(duration);
        Enable_DisableInput(true); // Re-enable input after knockback
        if (applyStun)
        {
            stateMachine.ChangeState(stunState);
        }
        else
        {
            // Resume movement or idle based on input
            if (InputManager.Instance.MoveDirection != Vector2.zero)
                stateMachine.ChangeState(moveState);
            else
                stateMachine.ChangeState(idleState);
        }
    }
    private void Die()
    {   
        Enable_DisableInput(false); // Disable input on death
        // Add animation, disable movement, etc.
        Debug.Log($"{heroData.playerName} has died.");
        if(sceneTransition != null)
            sceneTransition.StartTransition();
        else
            Debug.LogWarning("SceneTransition reference is missing on Player.");
        playerObject = this.gameObject;
        Destroy(playerObject, 1f);

        //respawn();
    }

    public void respawn()
    {
        currentHealth = heroData.defensiveStats.maxHealth;
        currentEnergy = heroData.specialStats.maxEnergy;
        currentMana = heroData.specialStats.maxMana;
        playerObject.transform.position = spawnPoint.position;

        stateMachine.Initialize(idleState);


        playerObject.SetActive(true);
    }
    internal void Move(float inputX)
    {
        throw new NotImplementedException();
    }

    public void EnableHitboxDef(bool value)
    {
        attackHitbox.enabled = value;
        if (value)
            ClearHitEnemies(); // reset before each swing
    }
    public void EnableHitbox()
    {
        EnableHitboxDef(true);
    }

    public void DisableHitbox()
    {
        EnableHitboxDef(false);
    }
    public void EnableUpHitbox()
    {
        upHitbox.enabled = true;
        ClearHitEnemies();
    }
    public void DisableUpHitBox()
    {
        upHitbox.enabled = false;
    }
    public void EnableDownHitbox()
    {
        downHitbox.enabled = true;
        ClearHitEnemies();
    }
    public void DisableDownHitBox()
    {
        downHitbox.enabled = false;
    }

    public void ClearHitEnemies()
    {
        attackHandler.ClearHitEnemies();
    }

    public void AttackDone()
    {
        (AttackStateMachine.CurrentState as PlayerAttackState)?.OnAttackAnimationComplete();
    }


    #endregion
}
