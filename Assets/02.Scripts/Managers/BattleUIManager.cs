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
    public Transform playerCardParent;
    public Transform bossCardParent;
    public Transform communityCardParent;
    public GameObject cardPrefab; // 카드 UI 프리팹

    private List<GameObject> cardUIs = new List<GameObject>();

    private void Start()
    {
        hitButton.onClick.AddListener(OnHit);
        standButton.onClick.AddListener(OnStand);
        nextStageButton.onClick.AddListener(OnNextStage);

        resultText.gameObject.SetActive(false);
        nextStageButton.gameObject.SetActive(false);
        roundText.gameObject.SetActive(false);
    }

    public void UpdateStatusUI(int stage)
    {
        playerHPText.text = $"HP : {player.hp}";
        bossHPText.text = $"HP : {boss.hp}";
        stageText.text = $"Stage {stage}";

        //HP Bar 채우기 갱신
        AnimateHPBar(playerHPBar, (float)player.hp / player.maxHP);
        AnimateHPBar(bossHPBar, (float)boss.hp / boss.baseHP);
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
        ClearCardUI();

        // 플레이어는 항상 앞면
        int total = 0;
        foreach (var card in playerHand)
        {
            int displayValue = battleManager.player.GetCardDisplayValue(card, total);
            total += displayValue; // 누적 합
            CreateCardUI(card, playerCardParent, displayValue, true);
        }

        // 보스 카드: revealBoss가 true일 때만 앞면
        total = 0;
        foreach (var card in bossHand)
        {
            int displayValue = battleManager.boss.GetCardDisplayValue(card, total);
            total += displayValue;
            CreateCardUI(card, bossCardParent, displayValue, revealBoss);
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
            CreateCardUI(card, communityCardParent, displayValue, isFaceUp);
        }
    }

    private void CreateCardUI(Card card, Transform parent, int displayValue, bool isFaceUp = true)
    {
        var obj = Instantiate(cardPrefab, parent);
        //obj.GetComponentInChildren<TMP_Text>().text = displayValue.ToString();
        obj.transform.localScale = Vector3.one;

        //// CardFlip 컴포넌트 안전히 가져오기
        var flip = obj.GetComponent<CardFlip>();
        if (flip == null)
        {
            Debug.LogError("CardPrefab에 CardFlip 컴포넌트를 붙이세요!");
            return;
        }

        // 반드시 Init 호출 (front/back sprite와 라벨 전달)
        flip.Init(card.frontSprite, card.backSprite, $"{card.value}", isFaceUp);

        //카드 눕히기 효과 추가
        obj.transform.localRotation = Quaternion.Euler(20f, 0f, 0f);
        // X축 기울기 → 카드가 바닥에 놓인 느낌
        // Z축 랜덤 기울기 → 자연스러움

        cardUIs.Add(obj);
    }

    private void ClearCardUI()
    {
        foreach (var obj in cardUIs)
            Destroy(obj);
        cardUIs.Clear();
    }

    // 특정 커뮤니티 카드(인덱스)만 뒤집기 (재생성하지 않음)
    public void FlipCommunityCard(int index)
    {
        if (index < 0 || index >= communityCardParent.childCount) return;
        var child = communityCardParent.GetChild(index);
        var flip = child.GetComponent<CardFlip>();
        if (flip != null)
        {
            // 비동기적으로 뒤집고 끝난 뒤 원하면 콜백 수행
            StartCoroutine(FlipAndDo(() => {
                // Flip 끝난 뒤 필요하면 할 작업 (예: 점수 갱신 등)
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
            var child = bossCardParent.GetChild(i);
            var flip = child.GetComponent<CardFlip>();
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
        yield return new WaitForSeconds(1.8f);

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
        yield return new WaitForSeconds(2f);

        // Fade Out
        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            c.a = Mathf.Lerp(1, 0, t);
            resultText.color = c;
            yield return null;
        }

        resultText.gameObject.SetActive(false);
    }
}
