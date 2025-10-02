// using UnityEngine;
// using PurrNet; // Make sure you reference PurrNet, not UNet

// public class TriangleSpawner : NetworkBehaviour
// {
//     public GameObject trianglePrefab;

//     protected override void OnSpawned(bool asServer)
//     {
//         base.OnSpawned(asServer);

//         // Only the owner can control spawning
//         //enabled = IsOwner;
//     }

//     private void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.H))
//         {
//                 var obj = Instantiate(trianglePrefab, transform.position, Quaternion.identity);
//         }
//     }
// }
