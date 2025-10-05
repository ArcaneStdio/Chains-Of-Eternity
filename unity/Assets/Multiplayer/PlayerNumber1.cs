using PurrNet;
using TMPro;
using UnityEngine;

public class PlayerNumber1 : NetworkBehaviour
{
    public SyncVar<int> playerNumber = new(0, ownerAuth: true);
    public TMP_Text numberText;

    private void Awake()
    {   
        numberText.text = "0";
        playerNumber.onChanged += OnPlayerNumberChanged;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        playerNumber.onChanged -= OnPlayerNumberChanged;
    }
    private void OnPlayerNumberChanged(int newNumber)
    {
        numberText.text = newNumber.ToString();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            playerNumber.value++;
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            playerNumber.value--;
        }
    }
}
