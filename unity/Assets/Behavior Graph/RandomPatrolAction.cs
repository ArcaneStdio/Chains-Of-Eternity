using System;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;

namespace Unity.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "RandomPatrol",
        description: "Moves an Agent randomly within a radius using NavMeshAgent or transform-based movement.",
        category: "Action/Navigation",
        story: "[Agent] roams randomly within [RoamRadius]",
        id: "ff1466e917bd04d6055a5b3089cf3c61")]
    internal partial class RandomPatrolAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<float> RoamRadius = new(5f);
        [SerializeReference] public BlackboardVariable<float> Speed = new(3f);
        [SerializeReference] public BlackboardVariable<float> WaitTime = new(1f);
        [SerializeReference] public BlackboardVariable<float> DistanceThreshold = new(0.2f);
        [SerializeReference] public BlackboardVariable<string> AnimatorSpeedParam = new("SpeedMagnitude");

        private NavMeshAgent m_NavMeshAgent;
        private Animator m_Animator;
        private Vector3 m_CurrentTarget;
        private float m_CurrentSpeed;
        private float m_OriginalStoppingDistance = -1f;
        private float m_OriginalSpeed = -1f;
        private float m_WaitTimer;
        private bool m_Waiting;

        protected override Status OnStart()
        {
            if (Agent.Value == null)
            {
                LogFailure("No agent assigned.");
                return Status.Failure;
            }

            Initialize();

            PickNewDestination();
            m_Waiting = false;
            m_WaitTimer = 0f;

            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (Agent.Value == null) return Status.Failure;

            if (m_Waiting)
            {
                if (m_WaitTimer > 0f)
                {
                    m_WaitTimer -= Time.deltaTime;
                }
                else
                {
                    m_Waiting = false;
                    PickNewDestination();
                }
            }
            else
            {
                float distance = GetDistanceToTarget();
                bool destinationReached = distance <= DistanceThreshold;

                if (destinationReached && (m_NavMeshAgent == null || !m_NavMeshAgent.pathPending))
                {
                    m_CurrentSpeed = 0;
                    m_WaitTimer = WaitTime.Value;
                    m_Waiting = true;
                }
                else if (m_NavMeshAgent == null) // transform-based movement
                {
                    Agent.Value.transform.position =
    Vector3.MoveTowards(Agent.Value.transform.position, m_CurrentTarget, Speed.Value * Time.deltaTime);

                }
            }

            UpdateAnimatorSpeed();
            return Status.Running;
        }

        protected override void OnEnd()
        {
            UpdateAnimatorSpeed(0f);

            if (m_NavMeshAgent != null)
            {
                if (m_NavMeshAgent.isOnNavMesh)
                {
                    m_NavMeshAgent.ResetPath();
                }
                m_NavMeshAgent.speed = m_OriginalSpeed;
                m_NavMeshAgent.stoppingDistance = m_OriginalStoppingDistance;
            }
        }

        private void Initialize()
        {
            m_Animator = Agent.Value.GetComponentInChildren<Animator>();
            m_NavMeshAgent = Agent.Value.GetComponentInChildren<NavMeshAgent>();
            if (m_NavMeshAgent != null)
            {
                if (m_NavMeshAgent.isOnNavMesh)
                    m_NavMeshAgent.ResetPath();

                m_OriginalSpeed = m_NavMeshAgent.speed;
                m_NavMeshAgent.speed = Speed.Value;
                m_OriginalStoppingDistance = m_NavMeshAgent.stoppingDistance;
                m_NavMeshAgent.stoppingDistance = DistanceThreshold;
            }

            UpdateAnimatorSpeed(0f);
        }

        private float GetDistanceToTarget()
        {
            if (m_NavMeshAgent != null)
                return m_NavMeshAgent.remainingDistance;

            Vector3 agentPos = Agent.Value.transform.position;
            agentPos.y = m_CurrentTarget.y; // ignore height
            return Vector3.Distance(agentPos, m_CurrentTarget);
        }

        private void PickNewDestination()
        {
            Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * RoamRadius.Value;
            m_CurrentTarget = Agent.Value.transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);

            if (m_NavMeshAgent != null && m_NavMeshAgent.isOnNavMesh)
            {
                m_NavMeshAgent.SetDestination(m_CurrentTarget);
            }
        }

        private void UpdateAnimatorSpeed(float explicitSpeed = -1f)
        {
            
        }
    }
}
    