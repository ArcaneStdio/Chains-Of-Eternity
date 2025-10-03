using UnityEngine;

public class SpellCaster : MonoBehaviour
{
    public SpellObject[] spellbook; // Assign in Inspector
    public PlayerStats playerstats; // Reference to the Player script
    void Update()
    {
        if (InputManager.Instance.Spell1Pressed)
        {
            if (playerstats.currentMana < spellbook[0].manaCost)
            {
                Debug.LogWarning("Not enough mana to cast this spell.");
                return;
            }
            SpellObject spell = spellbook[0];
            SpellFactory.CastSpell(spell, transform);
            playerstats.currentMana.value -= (int)spell.manaCost;
            Debug.Log($"Casting spell: {spell.name}, Mana cost: {spell.manaCost}, Remaining Mana: {playerstats.currentMana}");
        }

        if (InputManager.Instance.Spell2Pressed)
        {
            if (playerstats.currentMana < spellbook[1].manaCost)
            {
                Debug.LogWarning("Not enough mana to cast this spell.");
                return;
            }
            SpellObject spell = spellbook[1];
            SpellFactory.CastSpell(spell, transform);
            playerstats.currentMana.value -= (int)spell.manaCost;
            Debug.Log($"Casting spell: {spell.name}, Mana cost: {spell.manaCost}, Remaining Mana: {playerstats.currentMana}");
        }
        if (InputManager.Instance.Spell3Pressed)
        {
            if (playerstats.currentMana < spellbook[2].manaCost)
            {
                Debug.LogWarning("Not enough mana to cast this spell.");
                return;
            }
            SpellObject spell = spellbook[2];
            SpellFactory.CastSpell(spell, transform);
            playerstats.currentMana.value -= (int)spell.manaCost;
            Debug.Log($"Casting spell: {spell.name}, Mana cost: {spell.manaCost}, Remaining Mana: {playerstats.currentMana}");
        }
        if (InputManager.Instance.Spell4Pressed)
        {
            if (playerstats.currentMana < spellbook[3].manaCost)
            {
                Debug.LogWarning("Not enough mana to cast this spell.");
                return;
            }
            SpellObject spell = spellbook[3];
            SpellFactory.CastSpell(spell, transform);
            playerstats.currentMana.value -= (int)spell.manaCost;
            Debug.Log($"Casting spell: {spell.name}, Mana cost: {spell.manaCost}, Remaining Mana: {playerstats.currentMana}");
        }
    }
}