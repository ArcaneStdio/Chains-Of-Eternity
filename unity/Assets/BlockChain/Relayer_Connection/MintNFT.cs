    using UnityEngine;
    using UnityEngine.Networking;
    using System.Collections;
    using System.Text;

    // Remove MonoBehaviour inheritance - these are just data containers

    public class Mint_NFT : MonoBehaviour
    {
        private string serverUrl = "http://localhost:3000/mint-nft";
        
        public IEnumerator MintNFT(string recipientAddr)
        {
            Debug.Log("MintNFT called with recipient: " + recipientAddr);
            // Prepare JSON body
            MintRequest requestData = new MintRequest { recipientAddr = recipientAddr };
            string jsonData = JsonUtility.ToJson(requestData);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

            using (UnityWebRequest request = new UnityWebRequest(serverUrl, "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("MintNFT Failed: " + request.error);
                    Debug.LogError("Server Response: " + request.downloadHandler.text);
                }
                else
                {
                    Debug.Log("MintNFT Success: " + request.downloadHandler.text);

                    string json = request.downloadHandler.text;

                }
            }
        }
    }
    [System.Serializable]
    public class MintRequest : MonoBehaviour
    {
        public string recipientAddr;
    }

    // Remove MonoBehaviour inheritance - these are just data containers
    