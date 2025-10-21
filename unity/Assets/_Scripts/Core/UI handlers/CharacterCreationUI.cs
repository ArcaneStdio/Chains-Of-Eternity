//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;
//using System.Collections.Generic;
//using System.Collections;
//using System;
//using System.Numerics;

//public class CharacterCreationUI : MonoBehaviour
//{
//    List<BigInteger> playerIDs; 


//    [Header("UI Elements")]
//    public GameObject loadingPanel;
//    public GameObject createCharacterPanel;
//    public GameObject characterListPanel;

//    [Header("Create Character Form")]
//    public TMP_InputField playerNameInput;
//    //public TMP_Dropdown raceDropdown;
//    public Button createButton;

//    [Header("Character List")]
//    public Transform characterListParent;
//    public Button characterItemPrefab;

//    [Header("Status")]
//    public TextMeshProUGUI statusText;

//    private bool isCreatingCharacter = false;

//    private List<HeroBasicData> heroBasics = new List<HeroBasicData>();
//    private string wallet;

//    [SerializeField] TMP_InputField walletdisplay;

//    [Serializable]
//    public class HeroBasicData
//    {
//        public string playerName;
//        public uint level;
//        public bool isBanned;
//        public string raceName;

//        public HeroBasicData(string name, uint lvl, bool banned, string race)
//        {
//            playerName = name;
//            level = lvl;
//            isBanned = banned;
//            raceName = race;
//        }
//    }

//    private async void PopulateCharacterList(List<BigInteger> tokenIds)
//    {
//        if (tokenIds == null || tokenIds.Count == 0)
//        {
//            Debug.LogWarning("No token IDs to load.");
//            return;
//        }

//        // Clear previous list
//        foreach (Transform child in characterListParent)
//        {
//            Destroy(child.gameObject);
//        }

//        // Fetch basic info from the contract
//        BigInteger[] tokenArray = tokenIds.ToArray();
//        //var (names, levels, banned, races) = await HeroContractService.Instance.GetMultipleHeroesBasicInfo(tokenArray);
//        var heroInfo = await HeroContractService.Instance.GetHeroBasicInfo(tokenIds[0]);

//        heroBasics.Clear();

//        for (int i = 0; i < tokenArray.Length; i++)
//        {
//            HeroBasicData data = new HeroBasicData(heroInfo.PlayerName, heroInfo.Level, heroInfo.IsBanned, heroInfo.RaceName);
//            heroBasics.Add(data);

//            // Instantiate UI button for each character
//            Button item = Instantiate(characterItemPrefab, characterListParent);
//            //item.GetComponentInChildren<TextMeshProUGUI>().text = $"{data.playerName} (Lvl {data.level}) - {data.raceName}";
//            Debug.Log($"Creating button for {data.playerName} (Lvl {data.level}) - {data.raceName}");
//            BigInteger tokenId = tokenArray[i];
//            item.onClick.AddListener(() => OnCharacterSelected(tokenId));
//        }

//        characterListPanel.SetActive(true);
//    }

//    private void OnCharacterSelected(BigInteger tokenId)
//    {
//        Debug.Log($"Selected hero with Token ID: {tokenId}");
//        // TODO: transition or load character-specific data
//        SceneTransitionManager.Instance.OnCharacterSelected();
//    }


//    private void Start()
//    {
//        wallet = Web3AuthManager.Instance.GetWalletAddress().ToString();
//        StartCoroutine(InitializeAfterDelay());
//    }

//    private IEnumerator InitializeAfterDelay()
//    {
//        yield return new WaitForSeconds(1f);
//        InitializeUI();
//        yield return new WaitForSeconds(1f);
//        LoadExistingCharacters();
//    }

//    private void FixedUpdate()
//    {
//        walletdisplay.text = wallet;
//    }

//    private void InitializeUI()
//    {

//        // Initialize contract service
//        HeroContractService.Instance.InitializeContract();

//        // Setup UI
//        ShowLoadingState();

//        // Setup create button
//        createButton.onClick.AddListener(OnCreateCharacterClicked);

//        //// Setup race dropdown (add your races here)
//        //raceDropdown.options.Clear();
//        //raceDropdown.options.Add(new TMP_Dropdown.OptionData("Human"));
//        //raceDropdown.options.Add(new TMP_Dropdown.OptionData("Elf"));
//        //raceDropdown.options.Add(new TMP_Dropdown.OptionData("Orc"));
//        //raceDropdown.RefreshShownValue();

//        Debug.Log("CharacterCreationUI initialized. Waiting for contract setup.");
//    }

//    private async void LoadExistingCharacters()
//    {
//        statusText.text = "Loading existing characters...";

//        // Wait a moment for contract initialization
//        //new WaitForSeconds(1f);

//        // For now, we'll just show the create character option
//        // Later you can implement character loading here

//        Debug.Log($"Wallet being used in GetActiveHeroesByOwner: {wallet}");

//        playerIDs = await HeroContractService.Instance.GetActiveHeroesByOwner(wallet);

//        Debug.Log($"Found {playerIDs.Count} existing characters for wallet: {wallet}");
//        if(playerIDs.Count > 15)
//        {
//            statusText.text = "Existing characters loaded. You can create a new character.";
//            // Optionally, you can populate the character list here
//            //PopulateCharacterList(playerIDs);
//            //Debug.Log($"Found {playerIDs.Count} existing characters for wallet: {wallet}");
//        }
//        else
//        {
//            statusText.text = "No existing characters found. You can create a new character.";
//        }
//        ShowCreateCharacterState();
//        statusText.text = "Ready to create character";
//    }

//    private void ShowLoadingState()
//    {
//        loadingPanel.SetActive(true);
//        createCharacterPanel.SetActive(false);
//        characterListPanel.SetActive(false);
//    }

//    private void ShowCreateCharacterState()
//    {
//        loadingPanel.SetActive(false);
//        createCharacterPanel.SetActive(true);
//        characterListPanel.SetActive(false);
//    }

//    private void OnCreateCharacterClicked()
//    {
//        if (isCreatingCharacter) return;

//        string playerName = playerNameInput.text.Trim();
//        if (string.IsNullOrEmpty(playerName))
//        {
//            statusText.text = "Please enter a player name";
//            return;
//        }

//        string selectedRace = "elf";
//        StartCoroutine(CreateCharacter(playerName, selectedRace));
//    }

//    private IEnumerator CreateCharacter(string playerName, string raceName)
//    {
//        isCreatingCharacter = true;
//        createButton.interactable = false;
//        statusText.text = "Creating character on blockchain...";

//        // Create resistance array (you can customize this)
//        List<uint> resistances = new List<UInt32> { 10, 5, 15, 8 }; // Example resistances

//        // Create a simple token URI (you can make this more sophisticated)
//        string tokenURI = $"https://example.com/hero/{playerName}";

//        // Call mint function
//        string txHash = null;
//        yield return StartCoroutine(CallMintHero(playerName, raceName, resistances, tokenURI,
//            (result) => txHash = result));

//        if (!string.IsNullOrEmpty(txHash))
//        {
//            statusText.text = "Character created successfully!";
//            Debug.Log($"Character creation transaction hash: {txHash}");
//            yield return new WaitForSeconds(2f);

//            // Test retrieving the data (assuming token ID 0 for first mint)
//            yield return StartCoroutine(TestRetrieveHeroData(0));

//            // Transition to game scene
//            SceneTransitionManager.Instance.OnCharacterSelected();
//        }
//        else
//        {
//            statusText.text = "Character creation failed. Please try again.";
//            createButton.interactable = true;
//        }
//        isCreatingCharacter = false;
//    }

//    private IEnumerator CallMintHero(string playerName, string raceName, List<UInt32> resistances, string tokenURI, System.Action<string> callback)
//    {
//        var mintTask = HeroContractService.Instance.MintHero(playerName, raceName.ToLower(), resistances, tokenURI);

//        while (!mintTask.IsCompleted)
//        {
//            yield return null;
//        }

//        if (mintTask.Exception != null)
//        {
//            Debug.LogError($"Mint failed: {mintTask.Exception.Message}");
//            callback?.Invoke(null);
//        }
//        else
//        {
//            callback?.Invoke(mintTask.Result);
//        }
//    }

//    private IEnumerator TestRetrieveHeroData(BigInteger tokenId)
//    {
//        statusText.text = "Testing data retrieval...";

//        var getDataTask = HeroContractService.Instance.GetHeroData(tokenId);

//        while (!getDataTask.IsCompleted)
//        {
//            yield return null;
//        }

//        if (getDataTask.Exception != null)
//        {
//            Debug.LogError($"Data retrieval failed: {getDataTask.Exception.Message}");
//            statusText.text = "Data retrieval test failed";
//        }
//        else if (getDataTask.Result != null)
//        {
//            var heroData = HeroContractService.Instance.ConvertToUnityHeroData(getDataTask.Result);
//            Debug.Log($"Successfully retrieved hero: {heroData.playerName}, Race: {heroData.raceName}");
//            statusText.text = "Data retrieval test successful!";
//        }
//        else
//        {
//            statusText.text = "No data found";
//        }
//    }
//}