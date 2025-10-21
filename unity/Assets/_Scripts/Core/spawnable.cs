using System;
using UnityEngine;

[Serializable]
public class Spawnable
{
    public GameObject prefab;
    public int maxSpawnCount = 1;
    public float spawnInterval = 1f;
    public string spawnName;

    [HideInInspector] public float timer;
    [HideInInspector] public int currentCount;
}