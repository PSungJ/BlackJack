using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// 카드 전투 핵심 로직
/// 플레이어의 Hit에 따라 커뮤니티 카드가 순서대로 오픈되고,
/// 오픈된 카드들은 플레이어와 보스 모두에게 적용된다.
public class BattleManager : MonoBehaviour
{
    [Header("참조")]
    public PlayerController player;     // 플레이어 스크립트
    public BossController boss;         // 보스 스크립트
    public DeckManager deck;            // 카드 덱 매니저
    public BattleUIManager uiManager;

    [Header("공용 카드 (플레이어와 보스가 공유)")]
    public List<Card> communityCards = new List<Card>();
    private int revealedCardCount = 0;

    [Header("스테이지 관리")]
    public int currentStage = 1;

    private void Start()
    {
        StartCoroutine(StartBattleRoutine());
    }

    /// 전투 시작 시 초기화
    private IEnumerator StartBattleRoutine()
    {
        // 스테이지 시작 셔플 연출
        yield return StartCoroutine(uiManager.ShowShuffleAnimation());

        deck.InitializeDeck();
        player.Init(deck);  // 플레이어는 체력 유지, 카드만 새로
        boss.InitBoss(deck, currentStage);  // 보스는 스테이지에 맞춰 새로 등장
        DealCommunityCards();
        revealedCardCount = 0;

        uiManager.UpdateStatusUI(currentStage);

        // 라운드 카드 나누기
        yield return StartCoroutine(DealRoundCards());

        // 카드 앞/뒷면 설정
        uiManager.RefreshCards(player.handCards, boss.handCards, GetRevealedCommunityCards());

        Debug.Log($"=== 스테이지 {currentStage} 전투 시작 ===");
    }

    private IEnumerator DealRoundCards()
    {
        // 플레이어 2장 (앞면)
        for (int i = 0; i < player.handCards.Count; i++)
        {
            GameObject cardUI = uiManager.CreateCardInstant(player.handCards[i], uiManager.playerCardParent, true);
            yield return StartCoroutine(uiManager.DealCardAnimation(cardUI, uiManager.playerCardParent.GetChild(i)));
        }

        // 보스 2장 (뒷면)
        for (int i = 0; i < boss.handCards.Count; i++)
        {
            GameObject cardUI = uiManager.CreateCardInstant(boss.handCards[i], uiManager.bossCardParent, false);
            yield return StartCoroutine(uiManager.DealCardAnimation(cardUI, uiManager.bossCardParent.GetChild(i)));
        }

        // 커뮤니티 5장 (처음엔 뒷면)
        for (int i = 0; i < communityCards.Count; i++)
        {
            GameObject cardUI = uiManager.CreateCardInstant(communityCards[i], uiManager.communityCardParent, false);
            yield return StartCoroutine(uiManager.DealCardAnimation(cardUI, uiManager.communityCardParent.GetChild(i)));
        }
    }

    /// 공용 카드 5장 미리 준비 (아직 뒤집혀 있는 상태)
    private void DealCommunityCards()
    {
        communityCards.Clear();
        for (int i = 0; i < 5; i++)
        {
            communityCards.Add(deck.DrawCard());
    }
    }

    /// 플레이어가 Hit 시 — 공용카드 1장 오픈 (플레이어와 보스 둘 다 적용)
    public void PlayerHit()
    {
        if (revealedCardCount < communityCards.Count)
        {
            int index = revealedCardCount;
            revealedCardCount++;

            // UI에게 해당 인덱스 카드만 뒤집으라고 요청 (Refresh 전체 금지)
            uiManager.FlipCommunityCard(index);

            // 플레이어/보스의 합산 계산은 flip 완료 콜백에서 해도 되고, 
            // 미리 계산해서 바로 표시할 수도 있음(시각적 동기화 고려).
        }
    }

    /// 현재 공개된 커뮤니티 카드만 반환
    private List<Card> GetRevealedCommunityCards()
    {
        return communityCards.GetRange(0, revealedCardCount);
    }

    /// 플레이어가 Stand 선택 시 — 즉시 승패를 결정
    public void PlayerStand()
    {
        Debug.Log("플레이어가 Stand를 선택했습니다. 승패를 결정합니다.");

        // 보스 카드 공개 (UI 전체를 다시 그리지 말고, Flip만 실행)
        uiManager.RevealBossCards();

        // 잠깐 대기 후 (Flip 애니메이션 완료 타이밍) 승패 판정 실행
        StartCoroutine(ResolveAfterDelay(0.6f));
    }

    private IEnumerator ResolveAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResolveBattle();
    }

    /// 라운드 종료 및 승패/데미지 계산
    private void ResolveBattle()
    {
        int playerScore = player.GetTotalValue(GetRevealedCommunityCards());
        int bossScore = boss.GetTotalValue(GetRevealedCommunityCards());

        Debug.Log($"플레이어 합: {playerScore}, 보스 합: {bossScore}");
        CalculateDamage(playerScore, bossScore);

        uiManager.UpdateStatusUI(currentStage);

        //결과창 & 버튼 처리
        uiManager.ShowResult(boss.IsDefeated(), player.hp <= 0, playerScore, bossScore);

        //둘 다 죽지 않았다면 → 다음 라운드 진행
        if (!boss.IsDefeated() && player.hp > 0)
        {
            StartCoroutine(StartNextRound());
        }
    }

    private IEnumerator StartNextRound()
    {
        yield return new WaitForSeconds(3f); // 승부 순간 잠깐 보여주기

        // 이전 라운드 카드 완전 삭제
        uiManager.ClearAllCards();

        // --- UI 초기화 ---
        uiManager.resultText.gameObject.SetActive(false);
        uiManager.hitButton.interactable = true;
        uiManager.standButton.interactable = true;

        // "ROUND START" 연출
        yield return StartCoroutine(uiManager.ShowRoundStart());

        // 새로운 라운드를 위해 카드 초기화
        player.ClearHand();
        boss.ClearHand();
        communityCards.Clear();
        revealedCardCount = 0;

        // 새로 2장씩, 커뮤니티 5장
        player.Init(deck);
        boss.Init(deck);
        DealCommunityCards();

        // UI 갱신
        uiManager.RefreshCards(player.handCards, boss.handCards, GetRevealedCommunityCards());

        // 카드 나누기 애니메이션 실행
        yield return StartCoroutine(DealRoundCards());
    }

    /// 규칙에 따른 데미지 계산
    private void CalculateDamage(int playerScore, int bossScore)
    {
        if (playerScore > 21 && bossScore > 21)
        {
            // 둘 다 Burst → 더 작은 수가 승리
            if (playerScore < bossScore)
                BossTakeDamage(playerScore, bossScore);
            else if (bossScore < playerScore)
                PlayerTakeDamage(playerScore, bossScore);
            else
                Debug.Log("무승부 (둘 다 Burst)");
        }
        else if (playerScore == 21 && bossScore != 21)
        {
            // 플레이어 BlackJack
            boss.TakeDamage(10);
            Debug.Log("플레이어 BlackJack! 보스 10 데미지");
        }
        else if (bossScore == 21 && playerScore != 21)
        {
            // 보스 BlackJack
            player.TakeDamage(10);
            Debug.Log("보스 BlackJack! 플레이어 10 데미지");
        }
        else if (playerScore <= 21 && (playerScore > bossScore || bossScore > 21))
        {
            BossTakeDamage(playerScore, bossScore);
        }
        else if (bossScore <= 21 && (bossScore > playerScore || playerScore > 21))
        {
            PlayerTakeDamage(playerScore, bossScore);
        }
        else
        {
            Debug.Log("무승부");
    }
}

    private void BossTakeDamage(int playerScore, int bossScore)
    {
        int damage = Mathf.Abs(playerScore - bossScore);
        boss.TakeDamage(damage);
        Debug.Log($"플레이어 승! 보스 {damage} 데미지");
    }

    private void PlayerTakeDamage(int playerScore, int bossScore)
    {
        int damage = Mathf.Abs(playerScore - bossScore);
        player.TakeDamage(damage);
        Debug.Log($"보스 승! 플레이어 {damage} 데미지");
    }

    public void NextStage()
    {
        // 이전 스테이지 카드 완전 삭제
        uiManager.ClearAllCards();

        currentStage++;
        StartCoroutine(StartBattleRoutine());
    }
}