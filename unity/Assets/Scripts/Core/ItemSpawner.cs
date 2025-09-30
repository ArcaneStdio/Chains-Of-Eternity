using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [System.Serializable]
    public class ItemSpawnData
    {
        public GameObject prefab;
        public int count; // how many of this prefab to spawn
    }

    [Header("Items To Spawn")]
    public List<ItemSpawnData> itemsToSpawn = new();

    [Header("Spawn Area")]
    public Vector2 spawnAreaSize = new Vector2(10f, 10f);
    public float minDistanceBetweenItems = 1.5f;
    public int maxPlacementAttempts = 100;

    private List<Vector2> usedPositions = new();

    void Start()
    {
        SpawnItems();
    }

    void SpawnItems()
    {
        foreach (var itemData in itemsToSpawn)
        {
            for (int i = 0; i < itemData.count; i++)
            {
                Vector2 spawnPos = FindNonOverlappingPosition();
                if (spawnPos != Vector2.positiveInfinity)
                {
                    Instantiate(itemData.prefab, spawnPos, Quaternion.identity);
                    usedPositions.Add(spawnPos);
                }
                else
                {
                    Debug.LogWarning($"Could not find a non-overlapping position for {itemData.prefab.name}");
                }
            }
        }
    }

    Vector2 FindNonOverlappingPosition()
    {
        for (int attempt = 0; attempt < maxPlacementAttempts; attempt++)
        {
            Vector2 offset = new Vector2(
                Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f),
                Random.Range(-spawnAreaSize.y / 2f, spawnAreaSize.y / 2f)
            );
            Vector2 candidatePos = (Vector2)transform.position + offset;

            bool tooClose = false;
            foreach (var pos in usedPositions)
            {
                if (Vector2.Distance(pos, candidatePos) < minDistanceBetweenItems)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
                return candidatePos;
        }

        // If we can't find a valid spot
        return Vector2.positiveInfinity;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, spawnAreaSize);
    }
}
