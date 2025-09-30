using ColorPicker;
using UnityEngine;

public class ColorSwapper : MonoBehaviour
{
     [SerializeField] private SpriteRenderer spriteRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public ColorPicker.ColorPicker shirtcolorPicker;
    public ColorPicker.ColorPicker haircolorPicker;
    private Color HairColor,ShirtColor;
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        ShirtColor = shirtcolorPicker.CurrentSelectedColor;
        HairColor = haircolorPicker.CurrentSelectedColor;
        Material mat = spriteRenderer.material;

        // Set the colors (property names must match Shader Graph references)
        mat.SetColor("_ShirtColor", ShirtColor);
        mat.SetColor("_HairColor", HairColor);
    }
}
