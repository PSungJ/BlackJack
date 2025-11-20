using System.Collections.Generic;
using UnityEngine;

public class BossController : PlayerController
{
    [Header("보스 설정")]
    public int stageLevel = 1;
    public int baseHP = 50;
    public float hpGrowthRate = 1.2f;
    public int bossMaxHP;

    public void InitBoss(DeckManager deck, int stage)
    {
        stageLevel = stage;
        bossMaxHP = Mathf.RoundToInt(baseHP * Mathf.Pow(hpGrowthRate, stageLevel - 1));
        hp = bossMaxHP;

        handCards.Clear();
        handCards.Add(deck.DrawCard());
        handCards.Add(deck.DrawCard());

        Debug.Log($"보스 등장! 스테이지 {stageLevel}, HP: {hp}");

        base.Init(deck);
    }

    public int GetMinPossibleScore()
    {
        return GetTotalValue(new List<Card>()); // 현재 점수 기준 최소
    }

    public int GetMaxPossibleScore()
    {
        return GetTotalValue(new List<Card>()) + 10; // 최대 10 정도 여유 (Ace 등)
    }

    public void ResetHand()
    {
        handCards.Clear();
    }

    public bool IsDefeated()
    {
        return hp <= 0;
    }
}
