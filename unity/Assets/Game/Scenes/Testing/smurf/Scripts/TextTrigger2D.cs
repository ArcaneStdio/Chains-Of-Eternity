using UnityEngine;
using TMPro;
using System.Collections;

public class TextTrigger2D : MonoBehaviour
{
    public TextMeshPro text2D;         
    public float fadeSpeed = 2f;       
    public float visibleDuration = 2f; 

    private bool triggered = false;

    void Start()
    {
        if (text2D != null)
            text2D.alpha = 0;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            triggered = true;
            StartCoroutine(ShowOnce());
        }
    }

    IEnumerator ShowOnce()
    {
        
        yield return StartCoroutine(FadeTextTo(1));

        yield return new WaitForSeconds(visibleDuration);

        yield return StartCoroutine(FadeTextTo(0));

        Destroy(text2D.gameObject);
        Destroy(gameObject);
    }

    IEnumerator FadeTextTo(float targetAlpha)
    {
        float startAlpha = text2D.alpha;
        float t = 0;

        while (Mathf.Abs(text2D.alpha - targetAlpha) > 0.01f)
        {
            t += Time.deltaTime * fadeSpeed;
            text2D.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        text2D.alpha = targetAlpha;
    }
}
