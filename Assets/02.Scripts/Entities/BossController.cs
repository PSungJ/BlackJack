using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using static Card;

public class BossController : MonoBehaviour
{
    public List<Card> handCards = new List<Card>();     // 기본 2장
    public List<Card> activeCards = new List<Card>();   // 손패 + 오픈된 공용카드

    [Header("보스 설정")]
    public int stageLevel = 1;
    public int baseHP = 50;
    public float hpGrowthRate = 1.2f;
    public int bossMaxHP;
    public int bossHp;

    public void InitBoss(DeckManager deck, int stage)
    {
        stageLevel = stage;
        bossMaxHP = Mathf.RoundToInt(baseHP * Mathf.Pow(hpGrowthRate, stageLevel - 1));
        bossHp = bossMaxHP;

        handCards.Clear();
        handCards.Add(deck.DrawCard());
        handCards.Add(deck.DrawCard());

        Debug.Log($"보스 등장! 스테이지 {stageLevel}, HP: {bossHp}");

        Init(deck);
    }

    public void Init(DeckManager deck)
    {
        handCards.Clear();
        activeCards.Clear();

        // 2장 배분
        for (int i = 0; i < 2; i++)
        {
            Card drawn = deck.DrawCard();
            handCards.Add(drawn);
        }

        // 시작 시 손패 복사
        activeCards = new List<Card>(handCards);
    }

    public int GetMinPossibleScore()
    {
        return GetTotalValue(new List<Card>()); // 현재 점수 기준 최소
    }

    public int GetMaxPossibleScore()
    {
        return GetTotalValue(new List<Card>()) + 10; // 최대 10 정도 여유 (Ace 등)
    }

    public void TakeDamage(int dmg)
    {
        int damage = dmg;

        // HP 감소
        bossHp = Mathf.Clamp(bossHp - dmg, 0, bossMaxHP);

        // 사망
        if (bossHp <= 0)
        {
            Debug.Log("보스 사망");
            return;
        }
    }

    public void ResetHand()
    {
        handCards.Clear();
    }

    public bool IsDefeated()
    {
        return bossHp <= 0;
    }

    /// 공용 카드 추가
    public void AddCommunityCard(Card card)
    {
        activeCards.Add(card);
    }

    /// 카드 합 계산 (Ace는 1 또는 11)
    public int GetTotalValue(List<Card> communityCards)
    {
        int total = 0;
        int aceCount = 0;

        // 손패 + 커뮤니티 합산
        List<Card> allCards = new List<Card>(handCards);
        allCards.AddRange(communityCards);

        foreach (Card card in allCards)
        {
            if (card.rank == Rank.Ace) // 또는 card.value == 1
            {
                aceCount++;
                total += 1; // 일단 1로 추가
            }
            else
            {
                total += card.value;
            }
        }

        // Ace를 하나씩 11로 "승격"하되, 21을 넘지 않으면 승격
        for (int i = 0; i < aceCount; i++)
        {
            if (total + 10 <= 21)
                total += 10;
            else
                break; // 21을 초과하면 더 이상 승격하지 않음
        }

        return total;
    }

    public int GetCardDisplayValue(Card card, int currentTotal)
    {
        if (card.rank == Rank.Ace)
        {
            // 21을 넘지 않으면 11, 아니면 1
            if (currentTotal + 11 <= 21)
                return 11;
            else
                return 1;
        }
        return card.value;
    }
}
