using System;
using Unity.Properties;
using UnityEngine;

namespace Unity.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Perform Magic Attack",
        description: "Makes the wizard enemy perform a magic attack",
        category: "Action/Combat",
        story: "[Wizard] performs magic attack",
        id: "bb1234567890abcdef1234567890abcd")]
    internal partial class PerformMagicAttackAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Wizard;
        [SerializeReference] public BlackboardVariable<float> AttackCooldown = new BlackboardVariable<float>(1.5f);

        private float m_Timer;
        private bool m_HasAttacked;

        protected override Status OnStart()
        {
            if (Wizard.Value == null)
            {
                LogFailure("No wizard assigned.");
                return Status.Failure;
            }

            m_Timer = 0f;
            m_HasAttacked = false;
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (Wizard.Value == null)
                return Status.Failure;

            // Perform attack once
            if (!m_HasAttacked)
            {
                Enemy enemyComponent = Wizard.Value.GetComponent<Enemy>();
                if (enemyComponent != null)
                {
                    enemyComponent.PerformAttack();
                    m_HasAttacked = true;
                }
                else
                {
                    LogFailure("No Enemy component found on wizard.");
                    return Status.Failure;
                }
            }

            // Wait for cooldown
            m_Timer += Time.deltaTime;
            if (m_Timer >= AttackCooldown.Value)
            {
                return Status.Success;
            }

            return Status.Running;
        }

        protected override void OnEnd()
        {
            m_Timer = 0f;
            m_HasAttacked = false;
        }
    }
}

