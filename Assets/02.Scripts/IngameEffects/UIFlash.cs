using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFlash : MonoBehaviour
{
    public Image targetImage;

    public IEnumerator Flash(Color flashColor, float duration = 0.15f)
    {
        Color original = targetImage.color;

        targetImage.color = flashColor;
        yield return new WaitForSeconds(duration);

        targetImage.color = original;
    }
}
