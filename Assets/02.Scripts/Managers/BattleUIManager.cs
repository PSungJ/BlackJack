using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleUIManager : MonoBehaviour
{
    [Header("참조")]
    public BattleManager battleManager;
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

    public void RefreshCards(List<Card> playerHand, List<Card> bossHand, List<Card> revealedCommunity, bool revealBoss = false)
    {
        ClearCardUI();

        // 플레이어는 항상 앞면
        foreach (var card in playerHand)
            CreateCardUI(card, playerCardParent, true);

        // 보스 카드: revealBoss가 true일 때만 앞면
        foreach (var card in bossHand)
            CreateCardUI(card, bossCardParent, revealBoss);

        // 커뮤니티 카드: Hit 전은 뒷면, 공개된 카드만 앞면
        for (int i = 0; i < battleManager.communityCards.Count; i++)
        {
            bool isFaceUp = i < revealedCommunity.Count;
            CreateCardUI(battleManager.communityCards[i], communityCardParent, isFaceUp);
        }
    }

    private void ClearCardUI()
    {
        foreach (var obj in cardUIs)
            Destroy(obj);
        cardUIs.Clear();
    }

    private void CreateCardUI(Card card, Transform parent, bool isFaceUp = true)
    {
        var obj = Instantiate(cardPrefab, parent);
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = Vector3.zero; // Parent 기준 위치
        var img = obj.GetComponentInChildren<Image>();
        var txt = obj.GetComponentInChildren<TMP_Text>();

        if (isFaceUp)
        {
            img.sprite = card.frontSprite;
            txt.text = $"{card.value} {card.suit}";
        }
        else
        {
            img.sprite = card.backSprite;
            txt.text = "";
        }

        cardUIs.Add(obj);
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

    public void ShowResult(bool bossDefeated, bool playerDefeated)
    {
        if (playerDefeated)
            resultText.text = "플레이어 패배!";
        else if (bossDefeated)
        {
            resultText.text = $"보스 격파! Stage {battleManager.currentStage} 클리어!";
            nextStageButton.gameObject.SetActive(true);
        }
        else
            resultText.text = "승부 완료!";

        resultText.gameObject.SetActive(true);
    }
}
