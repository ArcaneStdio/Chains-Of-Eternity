using System.Collections;
using UnityEngine;
using PurrNet;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerStats : NetworkBehaviour
{
    [Header("Hero Data")]
    public HeroData heroData; // ScriptableObject reference
    public Animator Animator { get; private set; }
    // Synced values
    [SerializeField] public SyncVar<float> currentHealth;
    [SerializeField] public SyncVar<float> currentMana;
    [SerializeField] public SyncVar<float> currentEnergy;

    [Header("Settings")]
    public int RegenerateResourcesRate = 1;
    private float regenTimer = 0f;

    // Components
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Transform cameraTransform;
    public bool isInvincible = false;
    private bool flash = true;
    private Transform spawnPoint;
    private GameObject playerObject;
    private Color originalColor;
    private bool isFlashing = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        cameraTransform = Camera.main.transform;
        spawnPoint = transform;
        originalColor = spriteRenderer.color;
        StartCoroutine(StartAfter2s());
    }
    private void Start(){
        StartCoroutine(StartAfter2s());
    }
    private IEnumerator StartAfter2s()
    {
        // Initialize values on server
        yield return new WaitForSeconds(1f);
        ChangeHealth(heroData.defensiveStats.maxHealth);
        currentMana.value = heroData.specialStats.maxMana;
        currentEnergy.value = heroData.specialStats.maxEnergy;
    }

    [ServerRpc(requireOwnership:false)]
    public void ChangeHealth(float newHealth)
    {
        currentHealth.value = newHealth;
    }

    private void FixedUpdate()
    {
        regenTimer += Time.fixedDeltaTime;
        if (regenTimer >= RegenerateResourcesRate)
        {
            RegenerateResources();
            regenTimer = 0f;
        }
    }

    #region Resource Management
    public void TakeDamage(int damage, Vector3 sourcePos, float knockbackForce = 10f, bool applyKnockback = true, bool applyStun = true)
    {
        if (isInvincible) return;

        int effectiveDamage = Mathf.Max(0, damage - heroData.defensiveStats.defense);
        float newHealth = Mathf.Max(0, currentHealth.value - effectiveDamage);
        ChangeHealth(newHealth);
        Debug.Log($"{heroData.playerName} took {effectiveDamage} damage. Current Health: {currentHealth.value}/{heroData.defensiveStats.maxHealth}");

        if (applyKnockback)
        {
            ApplyKnockback((transform.position - sourcePos).normalized, knockbackForce, applyStun);
            if (knockbackForce > 10f) ShakeCamera();
            if (flash){ 
                if(!isFlashing){
                StartCoroutine(FlashOnHit());}
            }
        }

        if (currentHealth.value == 0)
            Die();
    }

    public void UseMana(int amount) =>
        currentMana.value = Mathf.Max(0, currentMana.value - amount);

    public void UseEnergy(int amount) =>
        currentEnergy.value = Mathf.Max(0, currentEnergy.value - amount);

    public void RegenerateResources()
    {
        float newHealth = Mathf.Min(heroData.defensiveStats.maxHealth, currentHealth.value + heroData.defensiveStats.healthRegeneration);
        ChangeHealth(newHealth);
        currentMana.value = Mathf.Min(heroData.specialStats.maxMana, currentMana.value + heroData.specialStats.manaRegeneration);
        currentEnergy.value = Mathf.Min(heroData.specialStats.maxEnergy, currentEnergy.value + heroData.specialStats.energyRegeneration);
    }
    #endregion

    #region Knockback + Death
    public void ApplyKnockback(Vector2 direction, float force, bool applyStun, float duration = 0.2f)
    {
        StartCoroutine(KnockbackRoutine(direction, force, duration, applyStun));
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
    private IEnumerator KnockbackRoutine(Vector2 direction, float force, float duration, bool applyStun)
    {
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);

        yield return new WaitForSeconds(duration);
        // Re-enable input via controller later
        rb.linearVelocity = Vector2.zero;
    }

    private void Die()
    {
        Debug.Log($"{heroData.playerName} has died.");
        playerObject = gameObject;
        Destroy(playerObject, 1f);
    }

    public void Respawn()
    {
        ChangeHealth(heroData.defensiveStats.maxHealth);
        currentEnergy.value = heroData.specialStats.maxEnergy;
        currentMana.value = heroData.specialStats.maxMana;
        transform.position = spawnPoint.position;
        gameObject.SetActive(true);
    }
    #endregion

    #region Visual Effects
    private IEnumerator FlashOnHit()
    {   
        isFlashing = true;
        if (spriteRenderer == null) yield break;
        spriteRenderer.color = Color.red * 2;
        yield return new WaitForSeconds(0.2f);
        isFlashing = false;
        spriteRenderer.color = originalColor;
    }

    public void ShakeCamera(float duration = 0.1f, float magnitude = 0.2f)
    {
        StartCoroutine(ShakeCameraCoroutine(duration, magnitude));
    }

    private IEnumerator ShakeCameraCoroutine(float duration, float magnitude)
    {
        Vector3 originalLocalPosition = cameraTransform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * magnitude;
            cameraTransform.localPosition = originalLocalPosition + new Vector3(randomOffset.x, randomOffset.y, 0f);
            yield return null;
        }

        cameraTransform.localPosition = originalLocalPosition;
    }
    #endregion
}
