using UnityEngine;
using Unity.Cinemachine;

public class PlayerDashController : MonoBehaviour
{
    [Header("Cinemachine")]
    [Tooltip("Drag the standard Cinemachine Impulse Source component from this GameObject")]
    [SerializeField]
    private CinemachineImpulseSource impulseSource;

    [Header("Collision Settings")]
    [Tooltip("The tag assigned to the wall colliders")]
    [SerializeField]
    private string wallTag = "wall_platue"; 

    public bool dashing = false; 
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (dashing)
        {
            if (collision.gameObject.CompareTag(wallTag)) 
            {
                Vector2 impactVelocity = collision.relativeVelocity;
                
                impulseSource.GenerateImpulse(impactVelocity); 
            }
        }
    }
}