using UnityEngine;
using UnityEngine.AI;

public class WizardDebugHelper : MonoBehaviour
{
    private Enemy enemy;
    private NavMeshAgent agent;
    private Transform player;
    
    void Start()
    {
        enemy = GetComponent<Enemy>();
        agent = GetComponent<NavMeshAgent>();
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }
    
    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        
        // Draw vision range
        if (enemy != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 5f); // vision range
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 1.5f); // attack range
        }
        
        // Draw line to player
        if (player != null)
        {
            Gizmos.color = enemy != null && enemy.CanSeePlayer() ? Color.green : Color.red;
            Gizmos.DrawLine(transform.position, player.position);
        }
        
        // Draw NavMesh path
        if (agent != null && agent.hasPath)
        {
            Gizmos.color = Color.cyan;
            var path = agent.path;
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                Gizmos.DrawLine(path.corners[i], path.corners[i + 1]);
            }
        }
    }
    
    void OnGUI()
    {
        if (!Application.isPlaying) return;
        
        GUILayout.BeginArea(new Rect(10, 10, 300, 400));
        GUILayout.Label("=== WIZARD DEBUG ===");
        
        if (enemy != null)
        {
            GUILayout.Label($"Using Behavior Graph: {enemy.useBehaviorGraph}");
            GUILayout.Label($"Can See Player: {enemy.CanSeePlayer()}");
            GUILayout.Label($"Health: {enemy.currentHealth}/{enemy.maxHealth}");
        }
        
        if (agent != null)
        {
            GUILayout.Label($"Agent Enabled: {agent.enabled}");
            GUILayout.Label($"Is On NavMesh: {agent.isOnNavMesh}");
            GUILayout.Label($"Has Path: {agent.hasPath}");
            GUILayout.Label($"Agent Speed: {agent.speed}");
            GUILayout.Label($"Velocity: {agent.velocity.magnitude:F2}");
            
            if (agent.hasPath)
            {
                GUILayout.Label($"Remaining Distance: {agent.remainingDistance:F2}");
                GUILayout.Label($"Path Status: {agent.pathStatus}");
            }
        }
        
        if (player != null)
        {
            float dist = Vector3.Distance(transform.position, player.position);
            GUILayout.Label($"Distance to Player: {dist:F2}");
        }
        
        GUILayout.EndArea();
    }
}

