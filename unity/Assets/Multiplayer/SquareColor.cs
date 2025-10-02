using PurrNet;
using UnityEngine;

public class SquareColor : NetworkBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            SetColor(Random.ColorHSV());
        }
    }

    [ServerRpc(requireOwnership: false)]
    private void SetColor(Color color)
    {
        RpcSetColor(color);
    }

    [ObserversRpc]
    private void RpcSetColor(Color color)
    {
        GetComponent<Renderer>().material.color = color;
    }
}
