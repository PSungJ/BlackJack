using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    // BattleUIManager에서: yield return flip.FlipToFrontCoroutine();
    public IEnumerator FlipToFrontCoroutine()
    {
        // 오브젝트가 비활성화 상태라면 그냥 종료
        if (!gameObject.activeInHierarchy || isFlipping || isFaceUp)
            yield break;

        yield return FlipAnimation(true);
    }

    // 버튼이나 다른 곳에서 바로 호출용
    public void FlipToFront()
    {
        if (!gameObject.activeInHierarchy || isFlipping || isFaceUp)
            return;

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

        float half = flipDuration / 2f;
        float t = 0f;
        RectTransform rect = GetComponent<RectTransform>();

        // 1) 옆으로 접히면서 0까지
        while (t < half)
        {
            float sx = Mathf.Lerp(1f, 0.01f, t / half);
            rect.localScale = new Vector3(sx, 1f, 1f);
            t += Time.deltaTime;
            yield return null;
        }

        // 2) 이미지 전환
        isFaceUp = showFront;
        UpdateVisual();

        // 3) 다시 펴짐
        t = 0f;
        while (t < half)
        {
            float sx = Mathf.Lerp(0.01f, 1f, t / half);
            rect.localScale = new Vector3(sx, 1f, 1f);
            t += Time.deltaTime;
            yield return null;
        }

        rect.localScale = Vector3.one;
        isFlipping = false;
    }

    private void UpdateVisual()
    {
        if (cardImage == null) return;

        // 카드 오브젝트는 항상 active, 앞/뒷면만 Sprite로 처리
        if (isFaceUp)
            cardImage.sprite = frontSprite;
        else
            cardImage.sprite = backSprite;
    }
}
