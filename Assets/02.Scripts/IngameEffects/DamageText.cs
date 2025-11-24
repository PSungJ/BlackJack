using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    public TMP_Text text;
    public float moveY = 60f;
    public float duration = 0.6f;
    public float fadeTime = 0.3f;

    public void Init(int amount, bool isHeal = false)
    {
        text = GetComponent<TextMeshProUGUI>();

        if (isHeal)
        {
            text.text = $"+{amount}";
            text.color = Color.green;
        }
        else
        {
            text.text = $"-{amount}";
            text.color = Color.red;
        }
        StartCoroutine(Play());
    }

    private IEnumerator Play()
    {
        Vector3 start = transform.localPosition;
        Vector3 end = start + new Vector3(0, moveY, 0);

        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(start, end, t / duration);
            yield return null;
        }

        // Fade out
        float ft = 0;
        Color c = text.color;
        while (ft < fadeTime)
        {
            ft += Time.deltaTime;
            c.a = Mathf.Lerp(1, 0, ft / fadeTime);
            text.color = c;
            yield return null;
        }

        Destroy(gameObject);
    }
}
