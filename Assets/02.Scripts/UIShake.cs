using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIShake : MonoBehaviour
{
    public IEnumerator Shake(float duration = 0.2f, float strength = 10f)
    {
        Vector3 origin = transform.localPosition;
        float t = 0;

        while (t < duration)
        {
            t += Time.deltaTime;
            transform.localPosition = origin + (Vector3)Random.insideUnitCircle * strength;
            yield return null;
        }

        transform.localPosition = origin;
    }
}
