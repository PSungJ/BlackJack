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

    [Header("버튼")]
    public Button hitButton;
    public Button standButton;
    public Button nextStageButton;

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
    }

    public void UpdateStatusUI(int stage)
    {
        playerHPText.text = $"Player HP: {player.hp}";
        bossHPText.text = $"Boss HP: {boss.hp}";
        stageText.text = $"Stage {stage}";
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
        hitButton.interactable = true;
        standButton.interactable = true;

        battleManager.NextStage();
    }

    public void ShowResult(bool bossDefeated, bool playerDefeated, int playerScore, int bossScore)
    {
        if (playerDefeated)
        {
            resultText.text = "<color=purple>Dealer Win!</color>";
        }
        else if (bossDefeated)
        {
            resultText.text = $"<color=purple>Player Win!</color>\nStage {battleManager.currentStage} Clear!";
            nextStageButton.gameObject.SetActive(true);
        }
        else
        {
            string playerColor = (playerScore > bossScore && playerScore <= 21) ? "#00FF66" : "#CCCCCC";
            string bossColor = (bossScore > playerScore && bossScore <= 21) ? "#FF4444" : "#CCCCCC";

            resultText.text =
                $"<b><color={playerColor}>Player: {playerScore}</color></b>\n" +
                $"<b><color={bossColor}>Dealer: {bossScore}</color></b>";

            if (playerScore > 21)
                resultText.text = resultText.text.Replace(
                    $"Player: {playerScore}",
                    $"<color=#FF6666>Player (BURST): {playerScore}</color>");

            if (bossScore > 21)
                resultText.text = resultText.text.Replace(
                    $"Dealer: {bossScore}",
                    $"<color=#FF6666>Dealer (BURST): {bossScore}</color>");
        }

        resultText.gameObject.SetActive(true);
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
}
