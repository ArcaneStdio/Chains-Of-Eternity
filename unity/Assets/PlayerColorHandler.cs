using UnityEngine;
using PurrNet;

public class PlayerColorHandler : NetworkBehaviour
{
    [Header("Material Reference")]
    [SerializeField] private Material playerMaterial; // assign in inspector

    //[SyncVar(OnChanged = nameof(OnShirtColorChanged))]
    private SyncVar<Color> shirtColor;

    //[SyncVar(OnChanged = nameof(OnHairColorChanged))]
    private SyncVar<Color> hairColor;

    private void Awake()
    {
        // Make sure each player has a unique material instance
        if (playerMaterial != null)
        {
            playerMaterial = Instantiate(playerMaterial);
            GetComponent<SpriteRenderer>().material = playerMaterial;

            // Read initial colors from material
            shirtColor.value = playerMaterial.GetColor("_ShirtColor");
            hairColor.value = playerMaterial.GetColor("_HairColor");
        }
    }

    public void Start()
    {
        //base.OnStartLocalPlayer();

        // Local player sends their existing colors to the server
        if (isClient)
        {
            CmdSetColors(
                playerMaterial.GetColor("_ShirtColor"),
                playerMaterial.GetColor("_HairColor")
            );
        }
    }

    [ServerRpc]
    private void CmdSetColors(Color newShirt, Color newHair)
    {
        shirtColor.value = newShirt;
        hairColor.value = newHair;
    }

    // SyncVar hooks
    private void OnShirtColorChanged(Color prev, Color next)
    {
        if (playerMaterial != null)
            playerMaterial.SetColor("_ShirtColor", next);
    }

    private void OnHairColorChanged(Color prev, Color next)
    {
        if (playerMaterial != null)
            playerMaterial.SetColor("_HairColor", next);
    }
}
