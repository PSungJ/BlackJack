using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerController : MonoBehaviour
{
    public List<Card> handCards = new List<Card>();     // 기본 2장
    public List<Card> activeCards = new List<Card>();   // 손패 + 오픈된 공용카드
    public int hp = 100;

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
    public int GetTotalValue(List<Card> community)
    {
        int total = 0;

        // 개인 카드
        foreach (var card in handCards)
            total += card.value;

        // 공개된 커뮤니티 카드
        foreach (var card in community)
            total += card.value;

        return total;
    }

    public void TakeDamage(int dmg)
    {
        hp -= dmg;
        hp = Mathf.Max(hp, 0);
        Debug.Log($"플레이어 HP: {hp}");
    }
}