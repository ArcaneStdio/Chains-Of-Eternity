using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
public class FlowUnityBridgeHero : MonoBehaviour
{
    //public LoginUI Ref;
    private string apiBase = "http://localhost:3000"; // Replace with your hosted backend

    public IEnumerator MintHero(string recipientAddress)
    {
        Debug.Log("Reached here -------- 0");
        string apiBase = "http://localhost:3000";
        MintHeroRequest reqData = new MintHeroRequest { recipientAddr = recipientAddress };
        string json = JsonUtility.ToJson(reqData);
        Debug.Log("Reached here -------- 1");
        UnityWebRequest request = new UnityWebRequest($"{apiBase}/mint-hero", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        Debug.Log("Reached here -------- 2");

        yield return request.SendWebRequest();

        Debug.Log("Reached here -------- 3");
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("MintHero Success: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("MintHero Failed: " + request.error);
        }
    }
}

[System.Serializable]
public class MintHeroRequest
{
    public string recipientAddr;
}

