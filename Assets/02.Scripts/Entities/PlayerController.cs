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

    public int armor = 0;   // 방어막

    public void AddArmor(int amount)
    {
        armor += amount;
        if (armor < 0) armor = 0;
    }

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

    // 데미지 계산을 방어막 먼저 소모하도록 수정
    public void TakeDamage(int dmg)
    {
        int damage = dmg;

        // 방어막 처리
        if (armor > 0)
        {
            int absorbed = Mathf.Min(armor, damage);
            armor -= absorbed;
            damage -= absorbed;
        }

        // 일반적인 데미지 처리
        hp = Mathf.Clamp(hp - damage, 0, maxHP);

        // Revive 체크 구간
        if (hp <= 0)
        {
            StartCoroutine(HandleDeath());
            return;
        }
    }

    private IEnumerator HandleDeath()
    {
        Debug.Log("[Damage] Player Down. Checking revive...");

        // ReviveSkill 호출
        SkillManager.Instance.OnPlayerDamaged(this);

        // ReviveSkill이 hp를 되살렸다면 코루틴 실행
        if (hp > 0)
        {
            Debug.Log("[REVIVE] revive detected → play revive VFX");

            // 부활 이펙트 + UI 갱신까지 포함한 코루틴 호출
            yield return StartCoroutine(BattleUIManager.Instance.ShowReviveEffect());

            BattleUIManager.Instance.UpdateStatusUI();   // UI 업데이트
            Debug.Log($"[REVIVE] revival complete! Current HP: {hp}");
            yield break;
        }

        // Revive 실패 시
        hp = 0;
        Debug.Log("[Damage] Player Dead. No revive available.");
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