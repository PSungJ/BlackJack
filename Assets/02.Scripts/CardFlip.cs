using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardFlip : MonoBehaviour
{
    public Image cardImage;

    private bool isFlipping = false;
    private float flipDuration = 0.4f;

    // 앞면/뒷면 Sprite
    [SerializeField] private Sprite frontSprite;
    [SerializeField] private Sprite backSprite;
    [SerializeField] private string cardLabel;
    [SerializeField] private bool isFaceUp = false;

    public void Init(Sprite front, Sprite back, string text, bool startFaceUp)
    {
        if (cardImage == null)
            cardImage = GetComponentInChildren<Image>();

        frontSprite = front;
        backSprite = back;
        cardLabel = text;
        isFaceUp = startFaceUp;
        UpdateVisual();
    }

    public IEnumerator FlipToFrontCoroutine()
    {
        if (isFaceUp || isFlipping) yield break;
        yield return StartCoroutine(FlipAnimation(true));
    }

    public void FlipToFront()
    {
        if (!isFaceUp && !isFlipping)
            StartCoroutine(FlipAnimation(true));
    }

    public void FlipToBack()
    {
        if (isFaceUp && !isFlipping)
            StartCoroutine(FlipAnimation(false));
    }

    private IEnumerator FlipAnimation(bool showFront)
    {
        isFlipping = true;
        float halfTime = flipDuration / 2f;
        float t = 0f;
        var rect = GetComponent<RectTransform>();

        //반으로 접히며 사라짐
        while (t < halfTime)
        {
            float scaleX = Mathf.Lerp(1f, 0f, t / halfTime);
            rect.localScale = new Vector3(scaleX, 1f, 1f);
            t += Time.deltaTime;
            yield return null;
        }

        //이미지 전환
        isFaceUp = showFront;
        UpdateVisual();

        //다시 펼쳐짐
        t = 0f;
        while (t < halfTime)
        {
            float scaleX = Mathf.Lerp(0f, 1f, t / halfTime);
            rect.localScale = new Vector3(scaleX, 1f, 1f);
            t += Time.deltaTime;
            yield return null;
        }

        rect.localScale = Vector3.one;
        isFlipping = false;
    }

    private void UpdateVisual()
    {
        if (isFaceUp)
            cardImage.sprite = frontSprite;
        else
            cardImage.sprite = backSprite;
    }
}
