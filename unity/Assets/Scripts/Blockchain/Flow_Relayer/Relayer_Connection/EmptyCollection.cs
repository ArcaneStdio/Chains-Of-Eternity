//using UnityEngine;
//using UnityEngine.Networking;
//using System.Collections;
//using System.Text;
//
//[System.Serializable]
//public class MintEmptyCollection : MonoBehaviour
//{}
//
//public class FlowUnityBridgeEmptyCollection : MonoBehaviour
//{
//    private string apiBase = "http://localhost:3000"; // Replace with your hosted backend
//
//    public IEnumerator MintHero(string recipientAddress)
//    {
//        MintEmptyCollection reqData = new MintEmptyCollection {};
//        string json = JsonUtility.ToJson(reqData);
//
//        UnityWebRequest request = new UnityWebRequest($"{apiBase}/empty-collection", "POST");
//        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
//        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
//        request.downloadHandler = new DownloadHandlerBuffer();
//        request.SetRequestHeader("Content-Type", "application/json");
//
//        yield return request.SendWebRequest();
//
//        if (request.result == UnityWebRequest.Result.Success)
//        {
//            Debug.Log("Empty Collection Success: " + request.downloadHandler.text);
//        }
//        else
//        {
//            Debug.LogError("Empty Collection Failed: " + request.error);
//        }
//    }
//}
//