//using System;
//using System.Collections;
//using System.Text;
//using TMPro;
//using UnityEngine;
//using UnityEngine.Networking;
//using UnityEngine.UI; // For Dropdown + Slider
//using Thirdweb.Unity;

//public class SpellInputManager : MonoBehaviour
//{
//    private string Spelljson;
//    private string walletAddress;
//    private string message;
//    private string signature;

//    [Header("Core Inputs")]
//    public TMP_InputField InputSpellName;
//    public TMP_InputField InputDamage;
//    public TMP_InputField InputKnockBackForce;
//    public TMP_InputField InputCooldown;
//    public TMP_InputField InputManaCost;

//    public TMP_InputField NumberOfProjectiles;
//    public TMP_InputField DelayBetweenProjectiles;
//    public TMP_InputField AngleProjectiles;
//    public TMP_InputField ZigzagAmplitude;
//    public TMP_InputField ZigzagFrequency;
//    public TMP_InputField StraightSpawnOffSet;

//    public TMP_InputField HomingUpdateRate;
//    public TMP_InputField HomingRadius;

//    public TMP_InputField CircularSpeed;
//    public TMP_InputField CircularRadius;

//    public TMP_InputField ArcGravityOffSet;
//    public TMP_InputField RandomDirectionOffset;

//    [Header("Dropdowns")]
//    public TMP_Dropdown DropdownElementTMP;
//    public Dropdown DropdownElement;
//    public TMP_Dropdown DropdownSubTypeTMP;
//    public Dropdown DropdownSubType;
//    public TMP_Dropdown DropdownMovementTMP;
//    public Dropdown DropdownMovement;

//    [Header("Slider Input")]
//    public Slider powerSlider;
//    private float powerValue;

//    [Header("Output Spell Object")]
//    public SpellObject CreateSpell;
//    public ProjectileData DataProjectile;

//    private ElementType elementType;
//    private AttackSubtype subType;

//    [Header("Relayer")]
//    [SerializeField] private string relayerUrl = "http://localhost:3000/upload-json"; // TODO replace

//    // --- Unity lifecycle ---
//    private async void Awake()
//    {
//        walletAddress = await ThirdwebManager.Instance.GetActiveWallet().GetAddress();
//    }

//    // --- Save projectile data ---
//    public void SaveProjectileData()
//    {
//        if (DataProjectile == null)
//        {
//            Debug.LogError("No ProjectileData assigned!");
//            return;
//        }

//        int.TryParse(InputDamage.text, out DataProjectile.damage);
//        float.TryParse(InputKnockBackForce.text, out DataProjectile.knockbackForce);
//        float.TryParse(InputCooldown.text, out DataProjectile.projectileLifeTime);
//        int.TryParse(InputManaCost.text, out DataProjectile.maxPierceCount);
//        int.TryParse(NumberOfProjectiles.text, out DataProjectile.numberOfProjectiles);
//        float.TryParse(DelayBetweenProjectiles.text, out DataProjectile.delayBetweenProjectiles);
//        float.TryParse(AngleProjectiles.text, out DataProjectile.staggeredLaunchAngle);
//        float.TryParse(ZigzagAmplitude.text, out DataProjectile.zigzagAmplitude);
//        float.TryParse(ZigzagFrequency.text, out DataProjectile.zigzagFrequency);
//        float.TryParse(StraightSpawnOffSet.text, out float straightOffset);
//        float.TryParse(HomingRadius.text, out DataProjectile.homingRadius);
//        float.TryParse(HomingUpdateRate.text, out DataProjectile.homingUpdateRate);
//        float.TryParse(CircularSpeed.text, out DataProjectile.circularSpeed);
//        float.TryParse(CircularRadius.text, out DataProjectile.circularInitialRadius);
//        float.TryParse(ArcGravityOffSet.text, out DataProjectile.arcGravityScale);
//        float.TryParse(RandomDirectionOffset.text, out DataProjectile.randomDirectionOffset);

//        DataProjectile.projectileSpeed = powerSlider != null ? powerSlider.value : 10f;

//        SetEnumFromDropdown<ProjectileData.ProjectilePath>(DropdownMovementTMP, DropdownMovement, ref DataProjectile.movementPath);

//        Debug.Log($"Projectile Data Updated: Damage={DataProjectile.damage}, Speed={DataProjectile.projectileSpeed}, Path={DataProjectile.movementPath}");
//    }

//    // --- Save spell data ---
//    public void SaveSpell()
//    {
//        string spellName = InputSpellName.text;

//        float.TryParse(InputCooldown.text, out float cooldown);
//        int.TryParse(InputManaCost.text, out int manaCost);

//        SetEnumFromDropdown<ElementType>(DropdownElementTMP, DropdownElement, ref elementType);
//        SetEnumFromDropdown<AttackSubtype>(DropdownSubTypeTMP, DropdownSubType, ref subType);

//        powerValue = powerSlider != null ? powerSlider.value : 0f;

//        if (CreateSpell != null)
//        {
//            CreateSpell.spellName = spellName;
//            CreateSpell.cooldown = cooldown;
//            CreateSpell.manaCost = manaCost;
//            CreateSpell.element = elementType;
//            CreateSpell.attackSubtype = subType;

//            Debug.Log($"Spell Saved: {spellName} | {elementType} | {subType} | Power={powerValue}");
//        }
//        else
//        {
//            Debug.LogWarning("No SpellObject assigned!");
//        }
//    }

//    private void SetEnumFromDropdown<T>(TMP_Dropdown tmpDropdown, Dropdown uiDropdown, ref T targetEnum) where T : struct, Enum
//    {
//        string option = string.Empty;

//        if (tmpDropdown != null)
//            option = tmpDropdown.options[tmpDropdown.value].text;
//        else if (uiDropdown != null)
//            option = uiDropdown.options[uiDropdown.value].text;

//        if (!string.IsNullOrEmpty(option) && Enum.TryParse(option, true, out T parsed))
//            targetEnum = parsed;
//        else
//            Debug.LogWarning($"Could not parse '{option}' into {typeof(T).Name}");
//    }

//    // --- Button hook ---
//    public void OnSaveButtonClicked()
//    {
//        SaveProjectileData();
//        SaveSpell();
//        Spelljson = SpellJsonConverter.ToJson(CreateSpell);

//        // fetch → sign → send
//        FetchMessage();
//    }

//    // --- Fetch message from Lighthouse ---
//    public void FetchMessage()
//    {
//        if (string.IsNullOrEmpty(walletAddress))
//        {
//            Debug.LogError("Wallet address not set yet!");
//            return;
//        }

//        string url = $"https://encryption.lighthouse.storage/api/message/{walletAddress}";
//        StartCoroutine(GetMessageFromServer(url));
//    }

//    private IEnumerator GetMessageFromServer(string url)
//    {
//        using (UnityWebRequest req = UnityWebRequest.Get(url))
//        {
//            yield return req.SendWebRequest();

//            if (req.result != UnityWebRequest.Result.Success)
//            {
//                Debug.LogError("Error fetching message: " + req.error);
//            }
//            else
//            {
//                string json = req.downloadHandler.text;
//                Debug.Log("Raw Response: " + json);

//                // Wrap JSON array for Unity JsonUtility
//                string wrappedJson = "{ \"messages\": " + json + " }";
//                MessageDataArray res = JsonUtility.FromJson<MessageDataArray>(wrappedJson);

//                if (res != null && res.messages != null && res.messages.Length > 0)
//                {
//                    message = res.messages[0].message;
//                    Debug.Log("Picked message: " + message);

//                    // sign and send
//                    SignMessageFunction(message);
//                }
//                else
//                {
//                    Debug.LogWarning("No message found in response!");
//                }
//            }
//        }
//    }

//    // --- Signing ---
//    public async void SignMessageFunction(string msg)
//    {
//        var wallet = ThirdwebManager.Instance.GetActiveWallet();
//        signature = await wallet.PersonalSign(msg);

//        Debug.Log("Signed message: " + signature);

//        // now send to relayer
//        SendToRelayer();
//    }

//    // --- Relayer call ---
//    public void SendToRelayer()
//    {
//        if (string.IsNullOrEmpty(Spelljson) || string.IsNullOrEmpty(walletAddress) || string.IsNullOrEmpty(signature))
//        {
//            Debug.LogError("Missing data for relayer call!");
//            return;
//        }

//        StartCoroutine(PostToRelayer(Spelljson, walletAddress, signature));
//    }

//    private IEnumerator PostToRelayer(string spellJson, string publicKey, string signedMessage)
//    {
//        RelayerRequest body = new RelayerRequest
//        {
//            jsonData = Spelljson,
//            publicKey = walletAddress,
//            signedMessage = signature
//        };

//        string jsonString = JsonUtility.ToJson(body);
//        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonString);

//        using (UnityWebRequest req = new UnityWebRequest(relayerUrl, "POST"))
//        {
//            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
//            req.downloadHandler = new DownloadHandlerBuffer();
//            req.SetRequestHeader("Content-Type", "application/json");

//            Debug.Log("Sending to relayer: " + jsonString);

//            yield return req.SendWebRequest();

//            if (req.result != UnityWebRequest.Result.Success)
//            {
//                Debug.LogError("Relayer call failed: " + req.error);
//            }
//            else
//            {
//                Debug.Log("Relayer response: " + req.downloadHandler.text);
//            }
//        }
//    }

//    // --- JSON classes ---
//    [System.Serializable]
//    public class MessageData
//    {
//        public string message;
//    }

//    [System.Serializable]
//    public class MessageDataArray
//    {
//        public MessageData[] messages;
//    }
//    [Serializable]
//    public class RelayerRequest
//    {
//        public string jsonData;
//        public string publicKey;
//        public string signedMessage;
//    }
//}
