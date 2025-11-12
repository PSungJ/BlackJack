using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : PlayerController
{
    [Header("보스 설정")]
    public int stageLevel = 1;      // 현재 스테이지 번호
    public int baseHP = 50;         // 1스테이지 기준 기본 HP
    public float hpGrowthRate = 1.2f; // 스테이지마다 HP 증가 비율 (20%씩 증가 예시)

    /// 보스 초기화 (스테이지 정보를 기반으로 HP 세팅 + 카드 초기화)
    public void InitBoss(DeckManager deck, int stage)
    {
        stageLevel = stage;

        // 스테이지에 따라 체력 증가 (50 → 60 → 72 → 86 → ...)
        hp = Mathf.RoundToInt(baseHP * Mathf.Pow(hpGrowthRate, stageLevel - 1));

        // 부모 클래스의 Init() 호출 → 카드 2장 받기
        base.Init(deck);

        Debug.Log($"보스 등장! [스테이지 {stageLevel}] HP: {hp}");
    }

    /// 전투 후 체력 남았는지 체크
    public bool IsDefeated()
    {
        return hp <= 0;
    }

    /// 보스가 패배 시 호출 (다음 스테이지 진행 등)
    public void OnDefeated()
    {
        Debug.Log($"보스 처치! 스테이지 {stageLevel} 클리어!");
    }
}
