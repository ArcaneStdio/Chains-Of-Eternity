using Unity.Cinemachine;
using UnityEngine;

public class ReferenceSetter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private CinemachineCamera cinemachine;
    [SerializeField] private Transform Transform;


    private void Awake()
    {
        cinemachine = GetComponent<CinemachineCamera>();
    }


    // Update is called once per frame
    void Update()
    {
        
        if(Transform == null)
        {
            Transform = GameObject.FindGameObjectWithTag("Player").transform;
            cinemachine.Follow = Transform;
        }
    }
}
