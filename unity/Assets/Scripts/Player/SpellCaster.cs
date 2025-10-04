using UnityEngine;
using PurrNet; // import for [ServerRpc], NetworkBehaviour, etc.

public class SpellCaster : NetworkBehaviour
{
    public SpellObject[] spellbook; // Assign in Inspector
    public PlayerStats playerStats; // Reference to PlayerStats (with SyncVars)

    void Update()
    {
        if (!isOwner) return; // Only local player can issue input

        if (InputManager.Instance.Spell1Pressed) TryCastSpell(0);
        if (InputManager.Instance.Spell2Pressed) TryCastSpell(1);
        if (InputManager.Instance.Spell3Pressed) TryCastSpell(2);
        if (InputManager.Instance.Spell4Pressed) TryCastSpell(3);
    }

    private void TryCastSpell(int index)
    {
        if (index < 0 || index >= spellbook.Length) return;
        SpellObject spell = spellbook[index];

        // Local pre-check for responsiveness (optional, actual validation is on server)
        if (playerStats.currentMana < spell.manaCost)
        {
            Debug.LogWarning("Not enough mana to cast this spell.");
            return;
        }

        // Ask the server to cast (authoritative)
        RequestCastSpellServerRpc(index, transform.position, transform.forward);
    }

    // Client → Server
    [ServerRpc(requireOwnership:true)]
    private void RequestCastSpellServerRpc(int spellIndex, Vector3 spawnPos, Vector3 forward, RPCInfo info = default)
    {
        if (spellIndex < 0 || spellIndex >= spellbook.Length) return;
        SpellObject spell = spellbook[spellIndex];

        // Validate mana on server
        if (playerStats.currentMana < spell.manaCost)
        {
            Debug.Log($"Player tried to cast {spell.name} but not enough mana.");
            return;
        }

        // Deduct mana server-side (SyncVar will sync to all)
        playerStats.currentMana.value -= (int)spell.manaCost;

        // Create projectile/spell effect server-side
        SpellFactory.CastSpell(spell, transform);

        Debug.Log($"[SERVER] Casting spell: {spell.name}, Cost: {spell.manaCost}, Remaining Mana: {playerStats.currentMana}");
    }
}
