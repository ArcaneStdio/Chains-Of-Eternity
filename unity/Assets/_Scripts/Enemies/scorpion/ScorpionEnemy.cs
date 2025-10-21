using UnityEngine;

public class ScorpionEnemy : Enemy
{
    public SpellObject Scorpionspell; // Assign in Inspector

    public override void PerformAttack()
    {   
        Debug.Log("Scorpion is attacking with spell: " + Scorpionspell.name);
        SpellFactory.CastSpell(Scorpionspell, transform);
    }




}
