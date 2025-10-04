// using UnityEngine;
// using PurrNet;

// public class MultiplayerDamageHandler : NetworkBehaviour
// {
//     [SerializeField] private bool enableDebugLogs = false;

//     private bool IsServer => NetworkIdentity.IsServer; // PurrNet's way to check if this is the server

//     public void ApplyDamageAndKnockback(GameObject target, int damage, Vector3 hitPosition, float knockbackForce, Transform caster)
//     {
//         if (!target)
//         {
//             if (enableDebugLogs) Debug.LogWarning("Target is null in ApplyDamageAndKnockback.");
//             return;
//         }

//         var netIdentity = target.GetComponent<NetworkIdentity>();
//         if (netIdentity == null)
//         {
//             if (enableDebugLogs) Debug.LogWarning($"Target {target.name} does not have a NetworkIdentity component.");
//             return;
//         }

//         // Check PurrNet Network Rules for authority
//         if (NetworkRules.AllowEveryone("ApplyDamage"))
//         {
//             // If "Everyone" is allowed, apply damage locally and let PurrNet sync
//             ApplyDamageLocal(target, damage, hitPosition, knockbackForce, caster);
//         }
//         else if (IsServer)
//         {
//             // Server-authoritative: apply directly
//             ApplyDamageLocal(target, damage, hitPosition, knockbackForce, caster);
//         }
//         else
//         {
//             // Client: request server to apply damage
//             RpcServer_ApplyDamage(netIdentity.netId, damage, hitPosition, knockbackForce, caster.position);
//         }
//     }

//     [ServerRpc]
//     private void RpcServer_ApplyDamage(ulong targetId, int damage, Vector3 hitPosition, float knockbackForce, Vector3 casterPosition)
//     {
//         // Replace this with PurrNet's actual method to get GameObject by network ID
//         GameObject targetObj = GetNetworkObjectById(targetId);
//         if (targetObj == null)
//         {
//             if (enableDebugLogs) Debug.LogWarning($"No GameObject found for Network ID: {targetId}");
//             return;
//         }

//         ApplyDamageLocal(targetObj, damage, hitPosition, knockbackForce, null);
//     }

//     private void ApplyDamageLocal(GameObject target, int damage, Vector3 hitPosition, float knockbackForce, Transform caster)
//     {
//         // Try to apply damage to an Enemy
//         var enemy = target.GetComponentInParent<Enemy>();
//         if (enemy != null)
//         {
//             enemy.TakeDamage(damage, caster != null ? caster.position : hitPosition, knockbackForce, knockbackForce > 0, true, true, "Magical");
//             if (enableDebugLogs) Debug.Log($"Projectile hit {target.name}, dealt {damage} damage to Enemy.");
//             return;
//         }

//         // Try to apply damage to a Player
//         var playerStats = target.GetComponent<PlayerStats>();
//         if (playerStats != null)
//         {
//             playerStats.TakeDamage(damage, hitPosition, knockbackForce);
//             if (enableDebugLogs) Debug.Log($"Player takes damage: {damage}");
//         }
//         else
//         {
//             if (enableDebugLogs) Debug.LogWarning($"No Enemy or PlayerStats component found on target: {target.name}");
//         }

//         // Apply knockback to Rigidbody2D (optional, commented out as in original)
//         // var trgRb = target.GetComponent<Rigidbody2D>();
//         // if (trgRb != null)
//         // {
//         //     trgRb.AddForce((target.transform.position - hitPosition).normalized * knockbackForce, ForceMode2D.Impulse);
//         // }
//     }

//     // Placeholder method: Replace with PurrNet's actual API to get GameObject by network ID
//     private GameObject GetNetworkObjectById(ulong targetId)
//     {
//         // TODO: Replace with PurrNet's actual method, e.g., NetworkManager.GetObjectById(targetId)
//         // Example: return NetworkManager.Instance.GetNetworkObject(targetId);
//         Debug.LogError("GetNetworkObjectById not implemented. Please replace with PurrNet's object lookup method.");
//         return null; // Return null for now, causing the RPC to log a warning and exit
//     }
// }