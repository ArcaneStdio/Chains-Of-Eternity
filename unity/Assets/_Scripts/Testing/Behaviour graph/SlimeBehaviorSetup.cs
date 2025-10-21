using UnityEngine;
[RequireComponent(typeof(Unity.Behavior.BehaviorGraphAgent))]
public class SlimeBehaviorSetup : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The slime GameObject itself (usually 'this.gameObject')")]
    public GameObject slimeSelf;

    [Header("Roaming Settings")]
    [Tooltip("Radius for random roaming")]
    public float roamRadius = 3f;
    
    [Tooltip("Movement speed while roaming")]
    public float roamSpeed = 1.5f;

    [Header("Chase Settings")]
    [Tooltip("Movement speed while chasing player")]
    public float chaseSpeed = 2.5f;

    [Tooltip("How close the slime gets to player before collision")]
    public float chaseDistanceThreshold = 0.3f;

    private Unity.Behavior.BehaviorGraphAgent behaviorAgent;

    private void Awake()
    {
        if (slimeSelf == null)
            slimeSelf = gameObject;

        behaviorAgent = GetComponent<Unity.Behavior.BehaviorGraphAgent>();
    }

    private void Start()
    {
        if (behaviorAgent != null && behaviorAgent.BlackboardReference != null)
        {
            var blackboard = behaviorAgent.BlackboardReference.Blackboard;
            
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject == null)
            {
                Debug.LogError("[SlimeBehaviorSetup] Could not find Player! Make sure your player has the 'Player' tag.");
            }
            
            foreach (var variable in blackboard.Variables)
            {
                if (variable.Name == "Slime" || variable.Name == "Enemy")
                {
                    if (variable is Unity.Behavior.BlackboardVariable<GameObject> gameObjectVar)
                    {
                        gameObjectVar.Value = slimeSelf;
                        Debug.Log($"[SlimeBehaviorSetup] Set '{variable.Name}' = {slimeSelf.name}");
                    }
                }
                else if (variable.Name == "Player" || variable.Name == "Target")
                {
                    if (variable is Unity.Behavior.BlackboardVariable<GameObject> gameObjectVar && playerObject != null)
                    {
                        gameObjectVar.Value = playerObject;
                        Debug.Log($"[SlimeBehaviorSetup] ✓ Set '{variable.Name}' = {playerObject.name}");
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
                else if (variable.Name == "ChaseSpeed")
                {
                    if (variable is Unity.Behavior.BlackboardVariable<float> floatVar)
                    {
                        floatVar.Value = chaseSpeed;
                    }
                }
                else if (variable.Name == "ChaseDistanceThreshold")
                {
                    if (variable is Unity.Behavior.BlackboardVariable<float> floatVar)
                    {
                        floatVar.Value = chaseDistanceThreshold;
                    }
                }
            }
        }
    }
}
