using UnityEngine;

public class SpellCaster : MonoBehaviour
{
    public SpellObject[] spellbook; // Assign in Inspector (size = 8)
    public PlayerStats playerstats; // Reference to the Player script

    void Update()
    {
        // Loop through 8 spell slots
        for (int i = 0; i < spellbook.Length && i < 8; i++)
        {
            if (spellbook[i] == null) continue; // Skip empty slots

            // Map keys 1-8 -> Alpha1 to Alpha8
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                TryCastSpell(i);
            }
        }
    }

    private void TryCastSpell(int index)
    {
        SpellObject spell = spellbook[index];

        if (playerstats.currentMana < spell.manaCost)
        {
            Debug.LogWarning($"Not enough mana to cast spell in slot {index + 1}.");
            return;
        }

        SpellFactory.CastSpell(spell, transform);
        playerstats.currentMana.value -= (int)spell.manaCost;

        Debug.Log($"Casting spell from slot {index + 1}: {spell.name}, Mana cost: {spell.manaCost}, Remaining Mana: {playerstats.currentMana}");
    }
}
