using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static ProjectileData;

public class SpellMarketplaceUI : MonoBehaviour
{
    [Header("Export Target")]
    public SpellObject targetSpellObject;   // assign in Inspector

    private int currentIndex = -1;
    [Header("UI References")]
    public UnityEngine.UI.Button[] spellButtons;
    public SpellSlotUI[] spellSlots;
    public TMP_Text spellNameText;
    public TMP_Text spellDetailsText;
    public UnityEngine.UI.Image spellImage;

    [Header("Available Spells")]
    public SpellObject[] spells;
    public ProjectileData[] projectileData;

    [Header("Visual Prefabs (Manual Assign)")]
    public GameObject fireStraightPrefab;
    public GameObject fireCircularPrefab;
    public GameObject waterStraightPrefab;
    public GameObject waterCircularPrefab;
    public GameObject lightningStraightPrefab;
    public GameObject lightningCircularPrefab;
    public GameObject windStraightPrefab;
    public GameObject windCircularPrefab;

    private void Start()
    {
        for (int i = 0; i < spellButtons.Length; i++)
        {
            int index = i;
            ShowSpellDetails(i); // Show details of the first spell by default
            spellButtons[i].onClick.AddListener(() => ShowSpellDetails(index));
            spellSlots[i].Setup(spells[i], projectileData[i], this);
        }
        StartCoroutine(WaitandUpdate());
    }

    private IEnumerator WaitandUpdate()
    {
        yield return new WaitForSeconds(2f); // wait for 0.1 seconds to ensure all UI elements are initialized
        for (int i = 0; i < spellButtons.Length; i++)
        {
            int index = i;
            ShowSpellDetails(i); // Show details of the first spell by default
            spellButtons[i].onClick.AddListener(() => ShowSpellDetails(index));
            spellSlots[i].Setup(spells[i], projectileData[i], this);
        }
    }
    private void ShowSpellDetails(int index)
    {
        if (index < 0 || index >= spells.Length) return;

        SpellObject spell = spells[index];
        ProjectileData projdata = projectileData[index];
        // text
        currentIndex = index;
        spellNameText.text = spell.spellName;
        spellDetailsText.text =
            $"Element: {spell.element}\n" +
            $"Mana Cost: {spell.manaCost}\n" +
            $"Cooldown: {spell.cooldown}\n" +
            $"Subtype: {spell.attackSubtype}\n" +
            $"Path: {projdata.movementPath}\n";

        // image (if prefab has a sprite)
        if (spellImage != null && spell.visualPrefab != null)
        {
            SpriteRenderer sr = spell.visualPrefab.GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
                spellImage.sprite = sr.sprite;
        }

        // assign the correct prefab
        spell.visualPrefab = GetVisualPrefab(spell, projdata);
    }



    private GameObject GetVisualPrefab(SpellObject spell, ProjectileData projData)
    {
        if (projData == null) return null;

        switch (spell.element)
        {
            case ElementType.Fire:
                return projData.movementPath == ProjectilePath.Circular ? fireCircularPrefab : fireStraightPrefab;

            case ElementType.Water:
                return projData.movementPath == ProjectilePath.Circular ? waterCircularPrefab : waterStraightPrefab;

            case ElementType.Lightning:
                return projData.movementPath == ProjectilePath.Circular ? lightningCircularPrefab : lightningStraightPrefab;

            case ElementType.Wind:
                return projData.movementPath == ProjectilePath.Circular ? windCircularPrefab : windStraightPrefab;
        }
        return null;
    }

    internal void ShowSpellDetails(SpellObject spellData, ProjectileData projectileData)
    {
        SpellObject spell = spellData;
        ProjectileData projdata = projectileData;

        spellNameText.text = spell.spellName;
        spellDetailsText.text =
            $"Element: {spell.element}\n" +
            $"Mana Cost: {spell.manaCost}\n" +
            $"Cooldown: {spell.cooldown}\n" +
            $"Subtype: {spell.attackSubtype}\n" +
            $"Path: {projdata.movementPath}\n";

        // image (if prefab has a sprite)
        if (spellImage != null && spell.visualPrefab != null)
        {
            SpriteRenderer sr = spell.visualPrefab.GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
                spellImage.sprite = sr.sprite;
        }

        // assign the correct prefab
        spell.visualPrefab = GetVisualPrefab(spell, projdata);
    }
    public void DumpToTarget()
    {
        if (targetSpellObject == null)
        {
            Debug.LogWarning("Target SpellObject is not assigned!");
            return;
        }
        if (currentIndex < 0 || currentIndex >= spells.Length)
        {
            Debug.LogWarning("No spell currently selected to dump!");
            return;
        }

        SpellObject sourceSpell = spells[currentIndex];
        ProjectileData sourceProjectile = projectileData[currentIndex];

        // Copy all basic data
        targetSpellObject.spellName      = sourceSpell.spellName;
        targetSpellObject.element        = sourceSpell.element;
        targetSpellObject.manaCost       = sourceSpell.manaCost;
        targetSpellObject.cooldown       = sourceSpell.cooldown;
        targetSpellObject.attackSubtype  = sourceSpell.attackSubtype;

        // Replace projectile data reference
        targetSpellObject.attackData.projectileData  = sourceProjectile;

        // Assign correct prefab
        targetSpellObject.visualPrefab   = GetVisualPrefab(sourceSpell, sourceProjectile);

        Debug.Log($"[Marketplace] Dumped spell '{sourceSpell.spellName}' into target SpellObject.");
    }
}
