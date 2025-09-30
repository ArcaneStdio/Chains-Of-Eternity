using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Dash Settings")]
    public float dashSpeed = 10f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    private bool isDashing = false;
    private float dashTimeRemaining = 0f;
    private float dashCooldownRemaining = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        moveInput = InputManager.Instance.MoveDirection;

        // Dash input
        if (InputManager.Instance.DashPressed && dashCooldownRemaining <= 0f && moveInput != Vector2.zero)
        {
            isDashing = true;
            dashTimeRemaining = dashDuration;
            dashCooldownRemaining = dashCooldown;
        }

        // Attack input
        if (InputManager.Instance.AttackPressed)
        {
            if (InputManager.Instance.IsAttackHeld)
                Debug.Log("Charged Attack");
            else
                Debug.Log("Basic Attack");
        }

        // Update dash cooldown
        if (dashCooldownRemaining > 0f)
            dashCooldownRemaining -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            rb.linearVelocity = moveInput * dashSpeed;
            dashTimeRemaining -= Time.fixedDeltaTime;

            if (dashTimeRemaining <= 0f)
                isDashing = false;
        }
        else
        {
            rb.linearVelocity = moveInput * moveSpeed;
        }
    }
}
