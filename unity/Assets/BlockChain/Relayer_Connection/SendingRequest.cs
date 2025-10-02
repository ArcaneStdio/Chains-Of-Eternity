using UnityEngine;

public class FlowTester : MonoBehaviour
{
    private FlowUnityBridgeHero flowUnityBridgehero;
    
    public FlowUnityBridgeUpdateHero updateHeroRequest;

    public Mint_NFT mintnft;

    // private void Awake() {
    //     updateHeroRequest = new FlowUnityBridgeUpdateHero();
    // }

    private void Start()
    {

        //flowUnityBridgehero = new FlowUnityBridgeHero();
        //StartCoroutine(flowUnityBridgehero.MintHero("0x2671f0332b22a748"));

        UpdateHeroRequest req = new UpdateHeroRequest()
        {
            nftID = 1,

            damage = 15,
            attackSpeed = 2,
            criticalRate = 6,
            criticalDamage = 60,

            maxHealth = 120,
            defense = 12,
            healthRegeneration = 6,
            resistances = new uint[] { 1, 3, 5 },

            maxEnergy = 60,
            energyRegeneration = 6,
            maxMana = 60,
            manaRegeneration = 6,

            constitution = 12,
            strength = 11,
            dexterity = 11,
            intelligence = 11,
            stamina = 11,
            agility = 11,
            remainingPoints = 5
        };

        //        StartCoroutine(updateHeroRequest.UpdateHero(req));

        if (mintnft != null)
        {
            StartCoroutine(mintnft.MintNFT("0xe16751ab754afa70"));
        }

    }
}
