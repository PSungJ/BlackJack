using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Card;

public class PlayerController : MonoBehaviour
{
    public List<Card> handCards = new List<Card>();     // 기본 2장
    public List<Card> activeCards = new List<Card>();   // 손패 + 오픈된 공용카드
    public int maxHP = 100;
    public int hp;

    public void ClearHand()
    {
        handCards.Clear();
    }

    /// 배틀 시작 시 초기화
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

    public void TakeDamage(int dmg)
    {
        hp -= dmg;
        hp = Mathf.Clamp(hp, 0, maxHP);
        Debug.Log($"플레이어 HP: {hp}");
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