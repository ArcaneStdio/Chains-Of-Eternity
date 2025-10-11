using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleHandler : MonoBehaviour
{
    [Header("References")]
    private ParticleSystem footstepParticles;     // assign (or it will use the attached PS)
    [SerializeField]
    private Tilemap groundTilemap;                // assign the Tilemap that contains walkable ground tiles
    [SerializeField]
    private Transform playerFeet;                 // small transform at player's feet

    [Header("Database")]
    public TileColorDatabase colorDatabase;     // assign the generated TileColorDatabase.asset

    [Header("Behavior")]
    public float colorLerpSpeed = 20f;          // how fast particles blend to new ground color
    public Color fallbackColor = new Color(0.8f, 0.8f, 0.8f, 1f);

    private ParticleSystem.MainModule mainModule;
    private Color currentColor;

    private void Awake()
    {
        footstepParticles = GetComponent<ParticleSystem>();
    }

    void Reset()
    {
        footstepParticles = GetComponent<ParticleSystem>();
    }

    void Start()
    {
        if (footstepParticles == null) footstepParticles = GetComponent<ParticleSystem>();
        mainModule = footstepParticles.main;
        currentColor = mainModule.startColor.mode == ParticleSystemGradientMode.Color
            ? mainModule.startColor.color
            : fallbackColor;
    }

    void Update()
    {
        if (groundTilemap == null || playerFeet == null) return;
        Vector3Int cellPos = groundTilemap.WorldToCell(playerFeet.position);
        TileBase tile = groundTilemap.GetTile(cellPos);

        Color target = fallbackColor;
        if (colorDatabase != null) target = colorDatabase.GetColor(tile);

        // Smooth blend so particles don't snap harshly when stepping between textures
        currentColor = Color.Lerp(currentColor, target, 1f - Mathf.Exp(-colorLerpSpeed * Time.deltaTime));

        mainModule.startColor = new ParticleSystem.MinMaxGradient(currentColor);
    }
}
