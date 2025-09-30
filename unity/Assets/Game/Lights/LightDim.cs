using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightPulse : MonoBehaviour
{
    public Light2D[] targetLights;           // Reference to your 2D light
    public float minIntensity = 0.5f;     // Lowest brightness
    public float maxIntensity = 1.5f;     // Highest brightness
    public float speed = 2f;              // How fast it pulses

    private float timeOffset;

     void Start()
    {
        if (targetLights == null || targetLights.Length == 0)
        {
            // Automatically find all Light2D components in children
            targetLights = GetComponentsInChildren<Light2D>();
        }

        // Random offset so if multiple scripts exist, they won't sync perfectly
        timeOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        // Sine wave oscillates between -1 and 1
        float sineValue = Mathf.Sin((Time.time + timeOffset) * speed);

        // Normalize to 0–1
        float normalized = (sineValue + 1f) / 2f;

        // Map to intensity range
        float newIntensity = Mathf.Lerp(minIntensity, maxIntensity, normalized);

        // Apply to all lights
        foreach (Light2D light in targetLights)
        {
            if (light != null)
                light.intensity = newIntensity;
        }
    }
}
