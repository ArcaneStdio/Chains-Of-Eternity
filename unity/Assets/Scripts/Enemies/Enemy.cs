using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] Transform playerTransform; // Reference to the player's transform
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float followOffset = 0.5f; // Offset to maintain while following the player    
    [Header("Enemy Stats")]
    [SerializeField] private bool isYFlip = false;
    public int maxHealth = 100;
    public int currentHealth;
    public int damage = 10;
    public float attackSpeed = 1f; // Attacks per second
    public float AttackRange = 1.5f;
    public float AttackCooldown = 1f;
    public float knockbackForce = 7f; // Force applied when knocked back
    public float attackanimationtime = 1f; // Duration of stun effect
    public UInt32 maxExpAward;
    public UInt32 minExpAward;
    private int expAward;
    public String Name = "Enemy";

    private bool hasDied = false;
    private bool fetchedExp = false;

    // Adding this variable so we can shift to cooldown state properly.
    public bool performedAttack = false;

    private float timer;

    private TaskCompletionSource<UInt32> expTCS;

    public NavMeshAgent agent;

    private bool movingUp;
    private bool movingDown;

    private bool inknockback = false;

    public GameObject deathEffect = null;
    [SerializeField] private float verticalAngleThreshold = 10f; // degrees


    // fields for player detection
    [Header("Player Detection")]
    [SerializeField] private float visionRange = 5f;
    [SerializeField] private float visionRadius = 0.5f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer;


    // fields for random roaming positions
    [Header("Roaming Settings")]
    [SerializeField] private float roamRadius = 5f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Behavior System")]
    [Tooltip("If true, uses Behavior Graph. If false, uses State Machine.")]
    public bool useBehaviorGraph = false;

    // Animator for the enemy reference
    public Animator animator;

    public Rigidbody2D RB { get; private set; }

    public Transform PlayerTransform => playerTransform;
    public float MoveSpeed => moveSpeed;
    public float FollowOffset => followOffset;
    public float attackRange => AttackRange;

    public float attackCooldown => AttackCooldown;

    public StateMachine<Enemy> StateMachine { get; private set; }

    public IdleState IdleState { get; private set; }
    public FreeRoamingState FreeRoamingState { get; private set; }
    public FollowState FollowState { get; private set; }
    public AttackState AttackState { get; private set; }
    public StunState StunState { get; private set; }
    public CooldownState CooldownState { get; private set; }
    public SpriteRenderer spriteRenderer { get; private set; }

    //private SimpleRandomContract simpleRandomContract;

    public bool IsFacingRight = true;

    private void Awake()
    {
        //Debug.Log("qwertyuiop " + agent.updateRotation);
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        if (playerTransform == null)
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        RB = transform.GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateUpAxis = false; // Disable automatic up-axis adjustment if not needed
        agent.updateRotation = false; // Disable automatic rotation to control it manually
        StartCoroutine(FixRotationNextFrame());
        StartCoroutine(SelectingPlayer());
    }
    private IEnumerator FixRotationNextFrame()
    {
        yield return null; // wait one frame for NavMeshAgent internal setup
        transform.rotation = Quaternion.identity; // reset to upright
    }

    // void OnEnable()
    // {
    //     if (VRFResultRouter.Instance != null)
    //         VRFResultRouter.Instance.OnVRFResult += OnVRFResult;
    // }

    // void OnDisable()
    // {
    //     if (VRFResultRouter.Instance != null)
    //         VRFResultRouter.Instance.OnVRFResult -= OnVRFResult;
    // }
    void Start()
    {
        //Debug.Log("zzzzzzzzzzzzzzzzzzzzzzzzzz");
        //Debug.Log("qwertyuiop " + agent.updateRotation);
        if (agent.updateRotation)
        {
            agent.updateRotation = false; // Disable automatic rotation to control it manually
            //Debug.Log("changed value of update rotation " + agent.updateRotation);
        }
        if (agent.updateUpAxis)
        {
            agent.updateUpAxis = false; // Disable automatic rotation to control it manually
            //Debug.Log("chagned value of update up axis " + agent.updateUpAxis);
        }
        if (RB == null)
        {
            //Debug.LogError("Rigidbody2D component is missing on the enemy!");
        }
        else
        {
            RB.freezeRotation = true;
        }
        //Debug.Log("yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy");
        currentHealth = maxHealth; // Initialize current health to max health

        // Only initialize state machine if not using Behavior Graph
        if (!useBehaviorGraph)
        {
            Debug.Log($"[{Name}] Using STATE MACHINE AI");
            //Debug.Log("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
            StateMachine = new StateMachine<Enemy>();
            //simpleRandomContract = new SimpleRandomContract();

            //Debug.Log("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb");
            IdleState = new IdleState(this, StateMachine);
            FreeRoamingState = new FreeRoamingState(this, StateMachine);
            FollowState = new FollowState(this, StateMachine);
            AttackState = new AttackState(this, StateMachine);
            StunState = new StunState(this, StateMachine);
            CooldownState = new CooldownState(this, StateMachine, AttackCooldown);
            //Debug.Log("ccccccccccccccccccccccccccccccccccccccccc");
            StateMachine.Initialize(IdleState);
            //Debug.Log("Current state of enemyh in start func " + StateMachine.CurrentState);
        }
        else
        {
            Debug.Log($"[{Name}] Using BEHAVIOR GRAPH AI ✓");
            
            // Check if BehaviorGraphAgent exists
            var behaviorAgent = GetComponent<Unity.Behavior.BehaviorGraphAgent>();
            if (behaviorAgent != null)
            {
                Debug.Log($"[{Name}] BehaviorGraphAgent found! Graph={behaviorAgent.Graph?.name ?? "NULL"}");
            }
            else
            {
                Debug.LogError($"[{Name}] NO BehaviorGraphAgent component!");
            }
        }
        
        expAward = (int)UnityEngine.Random.Range(minExpAward, maxExpAward);
        // StartCoroutine(enumerator());
    }

    public void Update()
    {
        // Only run state machine if not using Behavior Graph
        if (!useBehaviorGraph)
        {
            StateMachine?.LogicUpdate();
        }
        
        CheckVerticalMovement();
        
        // Only set animator parameters if they exist
        if (animator != null)
        {
            foreach (AnimatorControllerParameter param in animator.parameters)
            {
                if (param.name == "movingUp")
                {
                    animator.SetBool("movingUp", movingUp);
                }
                if (param.name == "movingDown")
                {
                    animator.SetBool("movingDown", movingDown);
                }
            }
        }
    }
    private IEnumerator SelectingPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Transform closest = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (GameObject p in players)
        {
            float dist = Vector3.Distance(currentPos, p.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = p.transform;
            }
        }

        playerTransform = closest;
        yield return new WaitForSeconds(1f);
        StartCoroutine(SelectingPlayer());
    }

    private void FixedUpdate()
    {
        // Only run state machine physics if not using Behavior Graph
        if (!useBehaviorGraph)
        {
            StateMachine?.PhysicsUpdate();
        }
        
        FlipIfNeeded();
    }

    public bool CanSeePlayer()
    {
        if (playerTransform == null)
            return false;

        Vector2 direction = (playerTransform.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, playerTransform.position);

        // Don't bother if player too far
        if (distance > visionRange)
            return false;

        RaycastHit2D hit = Physics2D.CircleCast(
            origin: transform.position,
            radius: visionRadius,
            direction: direction,
            distance: distance,
            layerMask: playerLayer | obstacleLayer // Combine masks
        );

        //Debug.Log("Performed raycast for player detection");

        if (hit)
        {
            // If the hit is the player, return true; if it's an obstacle, return false.
            if (((1 << hit.collider.gameObject.layer) & playerLayer) != 0)
                return true;
        }
        //Debug.Log("Player not detected within vision range or blocked by an obstacle.");
        return false;
    }

    public Vector2 GetRandomRoamPosition()
    {
        Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * roamRadius;
        Vector2 targetPos = (Vector2)transform.position + randomOffset;
        for (int i = 0; i < 100; i++) // Retry a few times in case of invalid points
        {
            randomOffset = UnityEngine.Random.insideUnitCircle * roamRadius;
            targetPos = (Vector2)transform.position + randomOffset;
            //Debug.Log(randomOffset);
            //Debug.Log(targetPos);

            // Check ground layer
            RaycastHit2D hit = Physics2D.Raycast(
                origin: transform.position,
                direction: (targetPos - (Vector2)transform.position).normalized,
                distance: (targetPos - (Vector2)transform.position).magnitude,
                layerMask: obstacleLayer
            );
            if (hit == false)
            {
                return targetPos;
            }

        }
        return targetPos;
    }
    public void TakeDamage(int damage, Vector3 sourcePos, float knockbackForce, bool applyKnockback = true, bool applyStun = true, bool flash = true, string damageType = "Physical")
    {
        //TODO : Handle different damage types (e.g., Physical, Magical)
        int effectiveDamage = Mathf.Max(0, damage);

        currentHealth = Mathf.Max(0, currentHealth - effectiveDamage);
        animator.SetBool("isHit", true);
        
        //backup[s]
        StartCoroutine(ResetHitAnimation(0.5f));
        
        //Debug.Log(sourcePos);
        //Debug.Log(transform.position);
        //Debug.Log(transform.position - sourcePos);
        Vector2 knockbackDirection = ((Vector2)transform.position - (Vector2)sourcePos).normalized;
        //Debug.Log(knockbackDirection);
        //knockbackDirection = new Vector2(knockbackDirection.x, -knockbackDirection.y);
        //Debug.Log(knockbackDirection);
        if (applyKnockback && !inknockback)
            ApplyKnockback(knockbackDirection, applyStun, knockbackForce);
        if (flash)
            StartCoroutine(FlashOnHit());

        if (currentHealth == 0 && !hasDied)
            Die();
    }
    public virtual IEnumerator FlashOnHit()
    {

        if (spriteRenderer == null) yield break; // No sprite renderer to flash

        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.red * 2; // Flash color
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor; // Reset to original color
    }
    public void ApplyKnockback(Vector2 direction, bool applyStun, float force = 0.1f, float duration = 0.1f)
    {
        StartCoroutine(KnockbackRoutine(direction, force, duration, applyStun));
    }
    private IEnumerator KnockbackRoutine(Vector2 direction, float force, float duration, bool applyStun)
    {
        inknockback = true;
        agent.enabled = false;
        RB.AddForce(direction.normalized * force, ForceMode2D.Impulse);
        yield return new WaitForSeconds(duration);
        RB.linearVelocity = Vector2.zero;
        agent.enabled = true;
        
        // Only use state machine if not using Behavior Graph
        if (!useBehaviorGraph && StateMachine != null)
        {
            if (applyStun)
            {
                StateMachine.ChangeState(StunState);
            }
            else
            {
                StateMachine.ChangeState(FreeRoamingState); // Return to idle state if not stunned
            }
        }
        inknockback = false;
    }
    public void FlipIfNeeded()
    {
        if (Mathf.Abs(agent.velocity.x) < 0.5f)
        {
            bool shouldFlip = (agent.velocity.x > 0 && !IsFacingRight) || (agent.velocity.x < 0 && IsFacingRight);
            if (shouldFlip)
            {
                Flip();
            }
        }
    }
    public void Flip()
    {
        //Debug.Log("Flipping" + Name);
        IsFacingRight = !IsFacingRight;
        Debug.Log("This is value of isYFlip: " + isYFlip);
        if (isYFlip)
        {
            Debug.Log("------------------------------------------------------------------------------------");
            transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
        }
        else
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    public void Die()
    {
        hasDied = true;

        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        animator.SetBool("isDead", true);
        animator.SetBool("isAttacking", false);
        animator.SetBool("followPlayer", false);
        animator.SetBool("freeRoam", false);
        
        Debug.Log(Name + " has died.");
        
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
        onDeath();
        
        // Delay destruction to let death animation play
        StartCoroutine(DestroyAfterAnimation(2f)); // TODO: adjust delay based on actual animation length
    }

    private System.Collections.IEnumerator DestroyAfterAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
    public virtual void PerformAttack()
    {
        Debug.Log("Base Enemy Attack");
    }
    public virtual void onDeath()
    {
        // Override this method to implement custom death behavior
        Debug.Log(Name + " has been defeated!");
    }

    public void hitAnimComplete()
    {
        animator.SetBool("isHit", false);
    }

    private System.Collections.IEnumerator ResetHitAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.SetBool("isHit", false);
    }

    // void OnVRFResult(uint exp)
    // {
    //     Debug.Log($"?? Awarding {exp} EXP to player");

    //     expAward = (int)exp;
    // }

    // void HandleVRFResult(uint result)
    // {
    //     string myAddress = Web3AuthManager.Instance.GetWalletAddress();

    //     // Optional: if using `requestId` later, you can match here
    //     // For now, just resolve the Task with the exp
    //     if (expTCS != null && !expTCS.Task.IsCompleted)
    //     {
    //         expTCS.SetResult(result);
    //     }
    // }

    // private IEnumerator enumerator()
    // {
    //     yield return new WaitForSeconds(1f);

    //     fetchExpFromContract();
    // }

    // private async void fetchExpFromContract()
    // {
    //     expTCS = new TaskCompletionSource<UInt32>();
    //     VRFResultRouter.Instance.OnVRFResult += HandleVRFResult;
    //     Debug.Log("Requesting random experience award...");
    //     string txnHash = await simpleRandomContract.RequestRandom(minExpAward, maxExpAward);
    //     Debug.Log($"VRF request sent with transaction hash: {txnHash}");
    //     Debug.Log("Waiting for VRF result...");
    //     UInt32 exp = await expTCS.Task;
    //     Debug.Log($"Received EXP: {exp} from VRF request {txnHash}");
    //     ProgressManager.Instance.AddExperience((int)exp);
    // }
    private void CheckVerticalMovement()
    {
        // Reset flags
        movingUp = false;
        movingDown = false;
        
        if (agent == null || !agent.isActiveAndEnabled || !agent.hasPath)
            return;
            
        Vector2 directionToDestination = agent.destination - transform.position;
        
        // Check if destination is valid
        if (directionToDestination.magnitude < 0.01f)
            return;
            
        directionToDestination.Normalize();

        // Get the signed angle relative to the y-axis
        float angle = Vector2.SignedAngle(Vector2.up, directionToDestination);

        // Check if within threshold
        if (Mathf.Abs(angle) <= verticalAngleThreshold)
        {
            movingUp = true;
            movingDown = false;
        }
        else if (Mathf.Abs(angle) >= 180f - verticalAngleThreshold)
        {
            movingDown = true;
            movingUp = false;
        }
    }

}

