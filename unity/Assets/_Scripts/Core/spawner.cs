using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public List<Spawnable> spawnables = new(); // n is just spawnables.Count

    [Header("Spawn Area")]
    public Vector2 spawnAreaSize = new Vector2(10f, 10f); // Width and height of the spawn area

    void Update()
    {
        foreach (var spawnable in spawnables)
        {
            spawnable.timer += Time.deltaTime;

            if (spawnable.timer >= spawnable.spawnInterval && spawnable.currentCount < spawnable.maxSpawnCount)
            {
                Spawn(spawnable);
                spawnable.timer = 0f;
            }
        }
    }

    void Spawn(Spawnable s)
    {
        // Calculate a random position within the spawn area rectangle centered around this spawner's position
        Vector2 offset = new Vector2(
            Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f),
            Random.Range(-spawnAreaSize.y / 2f, spawnAreaSize.y / 2f)
        );

        Vector2 spawnPosition = (Vector2)transform.position + offset;

        GameObject obj = Instantiate(s.prefab, spawnPosition, Quaternion.identity);
        s.currentCount++;

        // Optional: Handle death/cleanup to decrement count later
        // obj.GetComponent<SpawnedObject>()?.Init(() => s.currentCount--);
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        
        Gizmos.DrawWireCube(transform.position, spawnAreaSize);
    }
}
