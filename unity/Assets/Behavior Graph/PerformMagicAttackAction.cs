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
        private Animator m_Animator;
        private UnityEngine.AI.NavMeshAgent m_NavMeshAgent;
        private bool m_WasAgentStopped;

        protected override Status OnStart()
        {
            if (Wizard.Value == null)
            {
                LogFailure("No wizard assigned.");
                return Status.Failure;
            }

            m_Animator = Wizard.Value.GetComponentInChildren<Animator>();
            m_NavMeshAgent = Wizard.Value.GetComponent<UnityEngine.AI.NavMeshAgent>();

            // TODO: STOP movement completely during attack to prevent flickering
            // @RYUGA PLEASE CHECK FOR THIS GLITCHING EFFECT
            if (m_NavMeshAgent != null && m_NavMeshAgent.isOnNavMesh)
            {
                m_WasAgentStopped = m_NavMeshAgent.isStopped;
                m_NavMeshAgent.isStopped = true;
                m_NavMeshAgent.velocity = Vector3.zero;
            }

            if (m_Animator != null)
            {
                m_Animator.SetBool("isAttacking", true);
                m_Animator.SetBool("followPlayer", false);
                m_Animator.SetBool("freeRoam", false);
            }

            m_Timer = 0f;
            m_HasAttacked = false;
            
            UnityEngine.Debug.Log($"[PerformMagicAttack] Started attack, agent stopped");
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (Wizard.Value == null)
                return Status.Failure;

            // Keep agent stopped during entire attack sequence
            if (m_NavMeshAgent != null && m_NavMeshAgent.isOnNavMesh && !m_NavMeshAgent.isStopped)
            {
                m_NavMeshAgent.isStopped = true;
                m_NavMeshAgent.velocity = Vector3.zero;
            }

            // Perform attack once at the start
            if (!m_HasAttacked && m_Timer < 0.2f) // TODO: adjust this timing based on animation
            {
                
            }
            else if (!m_HasAttacked)
            {
                Enemy enemyComponent = Wizard.Value.GetComponent<Enemy>();
                if (enemyComponent != null)
                {
                    enemyComponent.PerformAttack();
                    m_HasAttacked = true;
                    UnityEngine.Debug.Log($"[PerformMagicAttack] Attack executed at t={m_Timer}");
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
                UnityEngine.Debug.Log($"[PerformMagicAttack] Attack complete, resuming movement");
                return Status.Success;
            }

            return Status.Running;
        }

        protected override void OnEnd()
        {
            if (m_NavMeshAgent != null && m_NavMeshAgent.isOnNavMesh)
            {
                m_NavMeshAgent.isStopped = m_WasAgentStopped;
            }
            
            if (m_Animator != null)
            {
                m_Animator.SetBool("isAttacking", false);
            }

            m_Timer = 0f;
            m_HasAttacked = false;
        }
    }
}

