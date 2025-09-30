using UnityEngine;

public enum ElementType { Fire, Water, Lightning, Wind }
public enum AttackSubtype { Projectile, AoE }

[CreateAssetMenu(fileName = "New Spell", menuName = "Spell System/Spell")]
public class SpellObject : ScriptableObject
{
    [Header("Core Info")]
    public string spellName;
    public ElementType element;
    public GameObject visualPrefab;
    public float manaCost;
    public float cooldown;

    [Header("Attack Type")]
    public AttackSubtype attackSubtype;
    public AttackData attackData;
}
