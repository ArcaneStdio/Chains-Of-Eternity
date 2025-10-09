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

        // TODO: Debouncing to prevent rapid state switching
        private static System.Collections.Generic.Dictionary<GameObject, float> s_LastCheckTime = 
            new System.Collections.Generic.Dictionary<GameObject, float>();
        private static System.Collections.Generic.Dictionary<GameObject, bool> s_LastResult = 
            new System.Collections.Generic.Dictionary<GameObject, bool>();
        
        private const float CHECK_INTERVAL = 0.3f;

        public override bool IsTrue()
        {
            if (Enemy.Value == null)
                return false;

            Enemy enemyComponent = Enemy.Value.GetComponent<Enemy>();
            if (enemyComponent == null)
                return false;

            // TODO: Debouncing logic to prevent jittery behavior
            // @RYUGA PLEASE CHECK THIS LOGIC to get the effect problem
            float currentTime = Time.time;
            if (s_LastCheckTime.ContainsKey(Enemy.Value))
            {
                float timeSinceLastCheck = currentTime - s_LastCheckTime[Enemy.Value];
                if (timeSinceLastCheck < CHECK_INTERVAL)
                {
                    return s_LastResult.ContainsKey(Enemy.Value) ? s_LastResult[Enemy.Value] : false;
                }
            }

            bool canSeePlayer = enemyComponent.CanSeePlayer();
            
            s_LastCheckTime[Enemy.Value] = currentTime;
            s_LastResult[Enemy.Value] = canSeePlayer;

            return canSeePlayer;
        }
    }
}

