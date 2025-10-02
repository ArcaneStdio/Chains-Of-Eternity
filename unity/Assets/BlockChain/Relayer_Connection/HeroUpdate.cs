using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;



public class FlowUnityBridgeUpdateHero : MonoBehaviour
{
    private string apiBase = "http://localhost:3000"; // Your backend

    public IEnumerator UpdateHero(UpdateHeroRequest heroData)
    {
        string json = JsonUtility.ToJson(heroData);

        UnityWebRequest request = new UnityWebRequest($"{apiBase}/update-hero", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("UpdateHero Success: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("UpdateHero Failed: " + request.error);
            Debug.LogError("Response: " + request.downloadHandler.text);
        }
    }
}
[System.Serializable]
public class UpdateHeroRequest: MonoBehaviour
{
    public ulong nftID;

    // Offensive
    public uint damage;
    public uint attackSpeed;
    public uint criticalRate;
    public uint criticalDamage;

    // Defensive
    public uint maxHealth;
    public uint defense;
    public uint healthRegeneration;
    public uint[] resistances;

    // Special
    public uint maxEnergy;
    public uint energyRegeneration;
    public uint maxMana;
    public uint manaRegeneration;

    // Stat Points
    public uint constitution;
    public uint strength;
    public uint dexterity;
    public uint intelligence;
    public uint stamina;
    public uint agility;
    public uint remainingPoints;
}
