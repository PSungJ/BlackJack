using UnityEngine;

public class BossController : PlayerController
{
    [Header("보스 설정")]
    public int stageLevel = 1;
    public int baseHP = 50;
    public float hpGrowthRate = 1.2f;

    public void InitBoss(DeckManager deck, int stage)
    {
        stageLevel = stage;
        hp = Mathf.RoundToInt(baseHP * Mathf.Pow(hpGrowthRate, stageLevel - 1));
        hp = baseHP;

        handCards.Clear();
        handCards.Add(deck.DrawCard());
        handCards.Add(deck.DrawCard());

        Debug.Log($"보스 등장! 스테이지 {stageLevel}, HP: {hp}");

        base.Init(deck);
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
