using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleUIManager : MonoBehaviour
{
    [Header("참조")]
    public BattleManager battleManager;
    public StageManager stageManager;
    public PlayerController player;
    public BossController boss;

    [Header("UI 요소")]
    public TMP_Text playerHPText;
    public TMP_Text bossHPText;
    public TMP_Text stageText;
    public TMP_Text resultText;
    public TMP_Text roundText;
    public TMP_Text stageClearText;
    public Image playerHPBar;
    public Image bossHPBar;

    [Header("버튼")]
    public Button hitButton;
    public Button standButton;
    public Button nextStageButton;
    public Button goLobbyButton;

    [Header("카드 슬롯")]
    public Transform deckPosition; // 카드가 날아오는 위치
    public Transform playerCardParent;
    public Transform bossCardParent;
    public Transform communityCardParent;
    public GameObject cardPrefab; // 카드 UI 프리팹

    [Header("카드 섞기 연출")]
    public GameObject shuffleLeft;
    public GameObject shuffleRight;

    private List<GameObject> cardUIs = new List<GameObject>();

    private void Start()
    {
        hitButton.onClick.AddListener(OnHit);
        standButton.onClick.AddListener(OnStand);
        nextStageButton.onClick.AddListener(OnNextStage);

        resultText.gameObject.SetActive(false);
        nextStageButton.gameObject.SetActive(false);
        goLobbyButton.gameObject.SetActive(false);
        roundText.gameObject.SetActive(false);
    }

    public IEnumerator ShowShuffleAnimation()
    {
        shuffleLeft.SetActive(true);
        shuffleRight.SetActive(true);

        RectTransform left = shuffleLeft.GetComponent<RectTransform>();
        RectTransform right = shuffleRight.GetComponent<RectTransform>();

        Vector3 leftStart = new Vector3(-240f, 0, 0);
        Vector3 rightStart = new Vector3(240f, 0, 0);
        Vector3 center = new Vector3(0, 0, 0);

        left.localPosition = leftStart;
        right.localPosition = rightStart;

        int shuffleCount = Random.Range(6, 10);

        for (int i = 0; i < shuffleCount; i++)
        {
            float duration = Random.Range(0.18f, 0.28f);
            float t = 0f;

            // Z축(깊이) 튀어나오는 값
            float zDepth = Random.Range(30f, 60f);

            // X축/Y축 기울기 (입체감)
            Quaternion leftTiltStart = Quaternion.Euler(Random.Range(-10f, -3f), Random.Range(-15f, -5f), Random.Range(-5f, 5f));
            Quaternion leftTiltEnd = Quaternion.Euler(Random.Range(-3f, 10f), Random.Range(5f, 15f), Random.Range(-5f, 5f));

            Quaternion rightTiltStart = Quaternion.Euler(Random.Range(-10f, -3f), Random.Range(5f, 15f), Random.Range(-5f, 5f));
            Quaternion rightTiltEnd = Quaternion.Euler(Random.Range(-3f, 10f), Random.Range(-15f, -5f), Random.Range(-5f, 5f));

            // 1) 중앙으로 모이며 깊이 + 기울어짐
            while (t < duration)
            {
                t += Time.deltaTime;
                float p = t / duration;

                Vector3 lPos = Vector3.Lerp(leftStart, center, p);
                Vector3 rPos = Vector3.Lerp(rightStart, center, p);

                // Z축 깊이 추가 (입체감 핵심)
                lPos.z = -Mathf.Sin(p * Mathf.PI) * zDepth;
                rPos.z = -Mathf.Sin(p * Mathf.PI) * zDepth;

                left.localPosition = lPos;
                right.localPosition = rPos;

                left.localRotation = Quaternion.Slerp(leftTiltStart, leftTiltEnd, p);
                right.localRotation = Quaternion.Slerp(rightTiltStart, rightTiltEnd, p);

                yield return null;
            }

            yield return new WaitForSeconds(Random.Range(0.03f, 0.07f));

            // 2) 다시 원래 자리로 복귀
            t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                float p = t / duration;

                Vector3 lPos = Vector3.Lerp(center, leftStart, p);
                Vector3 rPos = Vector3.Lerp(center, rightStart, p);

                lPos.z = -Mathf.Sin((1 - p) * Mathf.PI) * zDepth;
                rPos.z = -Mathf.Sin((1 - p) * Mathf.PI) * zDepth;

                left.localPosition = lPos;
                right.localPosition = rPos;

                left.localRotation = Quaternion.Slerp(leftTiltEnd, leftTiltStart, p);
                right.localRotation = Quaternion.Slerp(rightTiltEnd, rightTiltStart, p);

                yield return null;
            }

            yield return new WaitForSeconds(Random.Range(0.05f, 0.12f));
        }

        shuffleLeft.SetActive(false);
        shuffleRight.SetActive(false);
    }

    public IEnumerator DealCardAnimation(GameObject cardObj, Transform targetSlot)
    {
        float t = 0;
        Vector3 start = deckPosition.position;
        Quaternion startRot = Quaternion.Euler(0, 180, 0);
        Quaternion endRot = Quaternion.Euler(20, 0, 0);

        cardObj.transform.position = start;
        cardObj.transform.rotation = startRot;

        while (t < 1f)
        {
            t += Time.deltaTime * 3f;
            cardObj.transform.position = Vector3.Lerp(start, targetSlot.position, t);
            cardObj.transform.rotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }

        // 애니메이션 끝났으니 Slot 밑으로 이동
        cardObj.transform.SetParent(targetSlot, true);

        // 필요한 경우, localPosition / rotation 맞추기
        cardObj.transform.localPosition = Vector3.zero;
    }

    public void UpdateStatusUI(int stage)
    {
        playerHPText.text = $"HP : {player.hp}";
        bossHPText.text = $"HP : {boss.hp}";
        stageText.text = $"Stage {stage}";

        //HP Bar 채우기 갱신
        AnimateHPBar(playerHPBar, (float)player.hp / player.maxHP);
        AnimateHPBar(bossHPBar, (float)boss.hp / boss.bossMaxHP);
    }

    public void AnimateHPBar(Image bar, float targetFill)
    {
        StartCoroutine(AnimateFill(bar, targetFill));
    }

    private IEnumerator AnimateFill(Image bar, float target)
    {
        float start = bar.fillAmount;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * 3f; // 속도 (3 = 0.33초)
            bar.fillAmount = Mathf.Lerp(start, target, t);
            yield return null;
        }
    }

    public void RefreshCards(List<Card> playerHand, List<Card> bossHand, List<Card> community, bool revealBoss = false)
    {
        // 플레이어는 항상 앞면
        int total = 0;
        foreach (var card in playerHand)
        {
            int displayValue = battleManager.player.GetCardDisplayValue(card, total);
            total += displayValue; // 누적 합
        }

        // 보스 카드: revealBoss가 true일 때만 앞면
        total = 0;
        foreach (var card in bossHand)
        {
            int displayValue = battleManager.boss.GetCardDisplayValue(card, total);
            total += displayValue;
        }

        // 커뮤니티 카드: Hit 전은 뒷면, 공개된 카드만 앞면
        total = 0;
        for (int i = 0; i < battleManager.communityCards.Count; i++)
        {
            Card card = battleManager.communityCards[i];
            bool isFaceUp = i < community.Count; // 공개 여부
            int displayValue = battleManager.player.GetCardDisplayValue(card, total);
            if (isFaceUp)
                total += displayValue; // 공개된 카드만 누적
        }
    }

    public GameObject CreateCardInstant(Card card, Transform parent, bool startFaceUp)
    {
        var obj = Instantiate(cardPrefab, parent);
        var flip = obj.GetComponent<CardFlip>();
        flip.Init(card.frontSprite, card.backSprite, $"{card.value}", startFaceUp);
        return obj;
    }

    // 특정 커뮤니티 카드(인덱스)만 뒤집기 (재생성하지 않음)
    public void FlipCommunityCard(int index)
    {
        if (index < 0 || index >= communityCardParent.childCount) return;

        // 슬롯 → 슬롯의 첫번째 자식(카드)
        var slot = communityCardParent.GetChild(index);

        if (slot.childCount == 0) return;  // 카드 없으면 종료

        var card = slot.GetChild(0); // ← 카드 오브젝트
        var flip = card.GetComponent<CardFlip>();

        if (flip != null)
        {
            StartCoroutine(FlipAndDo(() => {
                UpdateStatusUI(battleManager.currentStage);
            }, flip));
        }
    }

    private IEnumerator FlipAndDo(System.Action onComplete, CardFlip flip)
    {
        yield return StartCoroutine(flip.FlipToFrontCoroutine());
        onComplete?.Invoke();
    }

    private void OnHit()
    {
        battleManager.PlayerHit();
    }

    private void OnStand()
    {
        hitButton.interactable = false;
        standButton.interactable = false;

        battleManager.PlayerStand();
    }

    public void ClearAllCards()
    {
        // 카드 인스턴스 리스트 정리
        foreach (var obj in cardUIs)
            if (obj != null) Destroy(obj);

        cardUIs.Clear();

        // 슬롯 밑 자식 모두 삭제
        ClearChildren(playerCardParent);
        ClearChildren(bossCardParent);
        ClearChildren(communityCardParent);
    }

    private void ClearChildren(Transform parent)
    {
        foreach (Transform slot in parent)
        {
            for (int i = slot.childCount - 1; i >= 0; i--)
            {
                Destroy(slot.GetChild(i).gameObject);
            }
        }
    }

    private void OnNextStage()
    {
        resultText.gameObject.SetActive(false);
        nextStageButton.gameObject.SetActive(false);
        stageClearText.gameObject.SetActive(false);
        hitButton.interactable = true;
        standButton.interactable = true;

        battleManager.NextStage();
    }

    public void ShowResult(bool bossDefeated, bool playerDefeated, int playerScore, int bossScore)
    {
        resultText.gameObject.SetActive(false);
        nextStageButton.gameObject.SetActive(false);
        goLobbyButton.gameObject.SetActive(false);

        if (playerDefeated)
        {
            StartCoroutine(ShowPlayerDie());
            goLobbyButton.gameObject.SetActive(true);
            return;
        }
        else if (bossDefeated)
        {
            StartCoroutine(ShowStageClear());
            nextStageButton.gameObject.SetActive(true);
            return;
        }
        else
        {
            // 일반 라운드 결과 표시
            string playerColor = (playerScore > bossScore && playerScore <= 21) ? "#00FF66" : "#CCCCCC";
            string bossColor = (bossScore > playerScore && bossScore <= 21) ? "#FF4444" : "#CCCCCC";

            string text =
                $"<b><color={playerColor}>Player: {playerScore}</color></b>\n" +
                $"<b><color={bossColor}>Dealer: {bossScore}</color></b>";

            if (playerScore > 21)
                text = text.Replace($"Player: {playerScore}", $"<color=#FF6666>Player (BURST): {playerScore}</color>");

            if (bossScore > 21)
                text = text.Replace($"Dealer: {bossScore}", $"<color=#FF6666>Dealer (BURST): {bossScore}</color>");

            // 일반 라운드 결과는 페이드 연출
            StartCoroutine(ShowRoundResultWithFade(text));
        }
    }

    public void RevealBossCards()
    {
        for (int i = 0; i < bossCardParent.childCount; i++)
        {
            Transform slot = bossCardParent.GetChild(i);

            if (slot.childCount == 0) continue;

            Transform card = slot.GetChild(0);
            var flip = card.GetComponent<CardFlip>();

            if (flip != null)
            {
                flip.FlipToFront();
            }
        }
    }

    public IEnumerator ShowRoundStart()
    {
        roundText.text = "ROUND START";
        roundText.gameObject.SetActive(true);

        // 시작은 Alpha = 0
        Color c = roundText.color;
        c.a = 0;
        roundText.color = c;

        // 1) Fade In
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f; // 0.5초 정도
            c.a = Mathf.Lerp(0, 1, t);
            roundText.color = c;
            yield return null;
        }

        // 2) 텍스트 유지
        yield return new WaitForSeconds(1.5f);

        // 3) Fade Out
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            c.a = Mathf.Lerp(1, 0, t);
            roundText.color = c;
            yield return null;
        }

        roundText.gameObject.SetActive(false);
    }

    public IEnumerator ShowStageClear()
    {
        stageClearText.text = $"<color=purple>Player Win!</color>\nStage {battleManager.currentStage} Clear!";
        stageClearText.gameObject.SetActive(true);

        Color c = stageClearText.color;
        c.a = 0;
        stageClearText.color = c;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            c.a = Mathf.Lerp(0, 1, t);
            stageClearText.color = c;
            yield return null;
        }

        yield return new WaitForSeconds(1f);
    }

    public IEnumerator ShowPlayerDie()
    {
        stageClearText.text = $"<color=Red>You Die...</color>";
        stageClearText.gameObject.SetActive(true);

        Color c = stageClearText.color;
        c.a = 0;
        stageClearText.color = c;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            c.a = Mathf.Lerp(0, 1, t);
            stageClearText.color = c;
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            c.a = Mathf.Lerp(1, 0, t);
            stageClearText.color = c;
            yield return null;
        }

        stageClearText.gameObject.SetActive(false);
    }

    public IEnumerator ShowRoundResultWithFade(string text)
    {
        resultText.text = text;
        resultText.gameObject.SetActive(true);

        Color c = resultText.color;
        c.a = 0;
        resultText.color = c;

        // Fade In
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            c.a = Mathf.Lerp(0, 1, t);
            resultText.color = c;
            yield return null;
        }

        // 표시 유지 시간
        yield return new WaitForSeconds(10f);

        resultText.gameObject.SetActive(false);
    }
}
