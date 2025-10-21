using UnityEngine;
[RequireComponent(typeof(Unity.Behavior.BehaviorGraphAgent))]
public class WizardBehaviorSetup : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The wizard GameObject itself (usually 'this.gameObject')")]
    public GameObject wizardSelf;

    [Header("Roaming Settings")]
    [Tooltip("Radius for random roaming")]
    public float roamRadius = 5f;
    
    [Tooltip("Movement speed while roaming")]
    public float roamSpeed = 2f;

    [Header("Follow Settings")]
    [Tooltip("Movement speed while following player")]
    public float followSpeed = 3f;

    [Header("Attack Settings")]
    [Tooltip("Time between attacks")]
    public float attackCooldown = 2f;

    private Unity.Behavior.BehaviorGraphAgent behaviorAgent;

    private void Awake()
    {
        if (wizardSelf == null)
            wizardSelf = gameObject;

        behaviorAgent = GetComponent<Unity.Behavior.BehaviorGraphAgent>();
    }

    private void Start()
    {
        // Set blackboard variables programmatically
        if (behaviorAgent != null && behaviorAgent.BlackboardReference != null)
        {
            var blackboard = behaviorAgent.BlackboardReference.Blackboard;
            
            // Iterate through all variables and set values by name
            foreach (var variable in blackboard.Variables)
            {
                if (variable.Name == "Wizard" || variable.Name == "Enemy")
                {
                    if (variable is Unity.Behavior.BlackboardVariable<GameObject> gameObjectVar)
                    {
                        gameObjectVar.Value = wizardSelf;
                    }
                }
                else if (variable.Name == "RoamRadius")
                {
                    if (variable is Unity.Behavior.BlackboardVariable<float> floatVar)
                    {
                        floatVar.Value = roamRadius;
                    }
                }
                else if (variable.Name == "RoamSpeed")
                {
                    if (variable is Unity.Behavior.BlackboardVariable<float> floatVar)
                    {
                        floatVar.Value = roamSpeed;
                    }
                }
                else if (variable.Name == "FollowSpeed")
                {
                    if (variable is Unity.Behavior.BlackboardVariable<float> floatVar)
                    {
                        floatVar.Value = followSpeed;
                    }
                }
                else if (variable.Name == "AttackCooldown")
                {
                    if (variable is Unity.Behavior.BlackboardVariable<float> floatVar)
                    {
                        floatVar.Value = attackCooldown;
                    }
                }
            }
        }
    }
}

