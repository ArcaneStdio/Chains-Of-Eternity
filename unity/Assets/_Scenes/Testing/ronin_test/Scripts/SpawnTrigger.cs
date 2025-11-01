using UnityEngine;

public class PlayerSpawnHandler : MonoBehaviour
{
    void Start()
    {
        if (SpawnPoint.NextSpawnPosition != Vector3.zero)
        {
            transform.position = SpawnPoint.NextSpawnPosition;
            SpawnPoint.NextSpawnPosition = Vector3.zero; // reset after use
        }
    }
}
