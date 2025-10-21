using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System;
public class FlowUnityBridgeListed : MonoBehaviour
{
   // public LoginUI Ref;
    public string transactionId;                  // Make sure to set this value appropriately
     // Replace with your hosted backend
    public IEnumerator MarketplaceListed(string apiBase, string transactionId)
    {
        //MarketplaceListedRequest reqData = new MarketplaceListedRequest { transactionId };
        Debug.Log(apiBase);
        string url = apiBase + "/marketplace-listed";
        Debug.Log(url);
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        Debug.Log(url);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(transactionId);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Listing Success: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Listing Failed: " + request.error);
        }
    }
}

[System.Serializable]
public class MarketplaceListedRequest
{
    public string transactionId;
}

