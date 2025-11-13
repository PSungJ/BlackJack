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
        StartBattle();
    }

    /// 전투 시작 시 초기화
    public void StartBattle()
    {
        deck.InitializeDeck();
        player.Init(deck);  // 플레이어는 체력 유지, 카드만 새로
        boss.InitBoss(deck, currentStage);  // 보스는 스테이지에 맞춰 새로 등장
        DealCommunityCards();
        revealedCardCount = 0;

        uiManager.UpdateStatusUI(currentStage);
        uiManager.RefreshCards(player.handCards, boss.handCards, GetRevealedCommunityCards());

        Debug.Log($"=== 스테이지 {currentStage} 전투 시작 ===");
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

        //점수를 직접 전달하여 UI가 다시 계산하지 않도록 수정
        uiManager.ShowResult(boss.IsDefeated(), player.hp <= 0, playerScore, bossScore);
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
        currentStage++;
        StartBattle();
    }
}