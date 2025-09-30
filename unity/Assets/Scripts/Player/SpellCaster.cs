using UnityEngine;

public class SpellCaster : MonoBehaviour
{
    public SpellObject[] spellbook; // Assign in Inspector
    public Player player; // Reference to the Player script
    void Update()
    {
        if (InputManager.Instance.Spell1Pressed)
        {
            if (player.currentMana < spellbook[0].manaCost)
            {
                Debug.LogWarning("Not enough mana to cast this spell.");
                return;
            }
            SpellObject spell = spellbook[0];
            SpellFactory.CastSpell(spell, transform);
            player.currentMana -= (int)spell.manaCost;
            Debug.Log($"Casting spell: {spell.name}, Mana cost: {spell.manaCost}, Remaining Mana: {player.currentMana}");
        }

        if (InputManager.Instance.Spell2Pressed)
        {
            if (player.currentMana < spellbook[1].manaCost)
            {
                Debug.LogWarning("Not enough mana to cast this spell.");
                return;
            }
            SpellObject spell = spellbook[1];
            SpellFactory.CastSpell(spell, transform);
            player.currentMana -= (int)spell.manaCost;
            Debug.Log($"Casting spell: {spell.name}, Mana cost: {spell.manaCost}, Remaining Mana: {player.currentMana}");
        }
        if (InputManager.Instance.Spell3Pressed)
        {
            if (player.currentMana < spellbook[2].manaCost)
            {
                Debug.LogWarning("Not enough mana to cast this spell.");
                return;
            }
            SpellObject spell = spellbook[2];
            SpellFactory.CastSpell(spell, transform);
            player.currentMana -= (int)spell.manaCost;
            Debug.Log($"Casting spell: {spell.name}, Mana cost: {spell.manaCost}, Remaining Mana: {player.currentMana}");
        }
        if (InputManager.Instance.Spell4Pressed)
        {
            if (player.currentMana < spellbook[3].manaCost)
            {
                Debug.LogWarning("Not enough mana to cast this spell.");
                return;
            }
            SpellObject spell = spellbook[3];
            SpellFactory.CastSpell(spell, transform);
            player.currentMana -= (int)spell.manaCost;
            Debug.Log($"Casting spell: {spell.name}, Mana cost: {spell.manaCost}, Remaining Mana: {player.currentMana}");
        }
    }
}