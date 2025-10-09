using System;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;

namespace Unity.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Navigate To Target",
        description: "Makes an agent navigate to a target using NavMeshAgent",
        category: "Action/Navigation",
        story: "[Agent] navigates to [Target]",
        id: "cc1234567890abcdef1234567890abcd")]
    internal partial class NavigateToTargetAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        [SerializeReference] public BlackboardVariable<float> Speed = new BlackboardVariable<float>(3f);
        [SerializeReference] public BlackboardVariable<float> DistanceThreshold = new BlackboardVariable<float>(1.5f);

        private NavMeshAgent m_NavMeshAgent;
        private Animator m_Animator;
        private float m_OriginalSpeed;
        private float m_OriginalStoppingDistance;

        protected override Status OnStart()
        {
            if (Agent.Value == null)
            {
                LogFailure("No agent assigned.");
                UnityEngine.Debug.LogError("[NavigateToTarget] Agent.Value is NULL!");
                return Status.Failure;
            }

            if (Target.Value == null)
            {
                LogFailure("No target assigned.");
                UnityEngine.Debug.LogError("[NavigateToTarget] Target.Value is NULL!");
                return Status.Failure;
            }

            m_NavMeshAgent = Agent.Value.GetComponent<NavMeshAgent>();
            if (m_NavMeshAgent == null)
            {
                LogFailure("Agent does not have a NavMeshAgent component.");
                UnityEngine.Debug.LogError($"[NavigateToTarget] No NavMeshAgent on {Agent.Value.name}!");
                return Status.Failure;
            }

            m_Animator = Agent.Value.GetComponentInChildren<Animator>();

            UnityEngine.Debug.Log($"[NavigateToTarget] Starting! Agent={Agent.Value.name}, Target={Target.Value.name}, Speed={Speed.Value}");

            // Store original values
            m_OriginalSpeed = m_NavMeshAgent.speed;
            m_OriginalStoppingDistance = m_NavMeshAgent.stoppingDistance;

            // Set navigation parameters
            m_NavMeshAgent.speed = Speed.Value;
            m_NavMeshAgent.stoppingDistance = DistanceThreshold.Value;
            
            if (m_Animator != null)
            {
                m_Animator.SetBool("followPlayer", true);
                m_Animator.SetBool("freeRoam", false);
                m_Animator.SetBool("isAttacking", false);
            }
            
            UnityEngine.Debug.Log($"[NavigateToTarget] NavMeshAgent configured. IsOnNavMesh={m_NavMeshAgent.isOnNavMesh}, Speed={m_NavMeshAgent.speed}");

            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (Agent.Value == null || Target.Value == null || m_NavMeshAgent == null)
                return Status.Failure;

            // Update destination to follow moving target
            if (m_NavMeshAgent.isActiveAndEnabled && m_NavMeshAgent.isOnNavMesh)
            {
                m_NavMeshAgent.SetDestination(Target.Value.transform.position);

                // Check if we've reached the target
                if (!m_NavMeshAgent.pathPending)
                {
                    float distanceToTarget = Vector3.Distance(Agent.Value.transform.position, Target.Value.transform.position);
                    if (distanceToTarget <= DistanceThreshold.Value)
                    {
                        return Status.Success;
                    }
                }
            }

            return Status.Running;
        }

        protected override void OnEnd()
        {
            if (m_Animator != null)
            {
                m_Animator.SetBool("followPlayer", false);
            }

            if (m_NavMeshAgent != null)
            {
                if (m_NavMeshAgent.isOnNavMesh)
                {
                    m_NavMeshAgent.ResetPath();
                }
                m_NavMeshAgent.speed = m_OriginalSpeed;
                m_NavMeshAgent.stoppingDistance = m_OriginalStoppingDistance;
                
                m_NavMeshAgent.isStopped = false;
            }
            
            UnityEngine.Debug.Log($"[NavigateToTarget] Ended for {Agent.Value?.name}");
        }
    }
}

