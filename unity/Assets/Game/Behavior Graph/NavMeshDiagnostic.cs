using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Attach to wizard to diagnose NavMesh issues
/// Shows big red text on screen with diagnostic info
/// </summary>
public class NavMeshDiagnostic : MonoBehaviour
{
    private NavMeshAgent agent;
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    
    void OnGUI()
    {
        if (!Application.isPlaying) return;
        
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 20;
        style.normal.textColor = Color.red;
        
        GUILayout.BeginArea(new Rect(Screen.width - 400, 10, 390, 500));
        
        GUILayout.Label("=== WIZARD NAVMESH DIAGNOSTIC ===", style);
        
        if (agent == null)
        {
            GUILayout.Label("NO NavMeshAgent COMPONENT!", style);
        }
        else
        {
            style.normal.textColor = agent.enabled ? Color.green : Color.red;
            GUILayout.Label($"Agent Enabled: {agent.enabled}", style);
            
            style.normal.textColor = agent.isOnNavMesh ? Color.green : Color.red;
            GUILayout.Label($"Is On NavMesh: {agent.isOnNavMesh}", style);
            
            style.normal.textColor = agent.speed > 0 ? Color.green : Color.red;
            GUILayout.Label($"Speed: {agent.speed}", style);
            
            style.normal.textColor = Color.yellow;
            GUILayout.Label($"Has Path: {agent.hasPath}", style);
            GUILayout.Label($"Path Status: {agent.pathStatus}", style);
            GUILayout.Label($"Velocity: {agent.velocity.magnitude:F2}", style);
            
            if (agent.hasPath)
            {
                GUILayout.Label($"Remaining Distance: {agent.remainingDistance:F2}", style);
            }
            
            style.normal.textColor = Color.white;
            GUILayout.Label("\n--- SOLUTIONS ---", style);
            
            if (!agent.isOnNavMesh)
            {
                style.normal.textColor = Color.red;
                GUILayout.Label("Window → AI → Navigation → Bake", style);
            }
            
            if (agent.speed == 0)
            {
                style.normal.textColor = Color.red;
                GUILayout.Label("Set NavMeshAgent Speed > 0", style);
            }
        }
        
        GUILayout.EndArea();
    }
}

