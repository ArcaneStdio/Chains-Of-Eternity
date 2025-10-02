using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
public class FlowUnityBridgeHero : MonoBehaviour
{
    public LoginUI Ref;
    private string apiBase = "http://localhost:3000"; // Replace with your hosted backend

    public IEnumerator MintHero(string recipientAddress)
    {
        MintHeroRequest reqData = new MintHeroRequest { recipientAddr = Ref.connectedAccount };
        string json = JsonUtility.ToJson(reqData);

        UnityWebRequest request = new UnityWebRequest($"{apiBase}/mint-hero", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

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

