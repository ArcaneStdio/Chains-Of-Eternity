using System;
using Unity.Properties;
using UnityEngine;

namespace Unity.Behavior
{
    [Serializable, GeneratePropertyBag]
    [Condition(
        name: "Can See Player",
        category: "Conditions/Enemy",
        story: "[Enemy] can see player within range",
        id: "aa1234567890abcdef1234567890abcd")]
    internal partial class CanSeePlayerCondition : Condition
    {
        [SerializeReference] public BlackboardVariable<GameObject> Enemy;

        public override bool IsTrue()
        {
            if (Enemy.Value == null)
                return false;

            Enemy enemyComponent = Enemy.Value.GetComponent<Enemy>();
            if (enemyComponent == null)
                return false;

            return enemyComponent.CanSeePlayer();
        }
    }
}

