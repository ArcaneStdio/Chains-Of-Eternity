using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpellSlotUI : MonoBehaviour
{
    [Header("UI References")]
    public Image spellIcon;
    public Button selectButton;

    private SpellObject spellData;
    private ProjectileData projectileData;
    private SpellMarketplaceUI marketplace;

    public void Setup(SpellObject spell,ProjectileData projectile ,SpellMarketplaceUI market)
    {
        spellData = spell;
        projectileData = projectile;
        marketplace = market;

        // Prefer a direct sprite reference stored in SpellObject
        if (spellData != null && spellData.visualPrefab != null)
        {
            // Try to fetch a SpriteRenderer sprite from the prefab (if it has one)
            SpriteRenderer sr = spellData.visualPrefab.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                spellIcon.sprite = sr.sprite;
            }
            else
            {
                Debug.LogWarning($"No SpriteRenderer found on prefab for spell: {spellData.spellName}");
                spellIcon.sprite = null; // fallback
            }
        }
        else
        {
            spellIcon.sprite = null; // empty slot
        }

        // Hook up button
        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => marketplace.ShowSpellDetails(spellData,projectileData));
    }
}
