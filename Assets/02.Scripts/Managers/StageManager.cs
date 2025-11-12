using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [Header("참조")]
    public BattleManager battleManager;
    public PlayerController player;
    private int currentStage = 1;

    /// 첫 전투 시작
    public void StartFirstStage()
    {
        currentStage = 1;
        battleManager.currentStage = currentStage;
        battleManager.StartBattle();
    }

    /// 보스 처치 후 다음 스테이지로 진행
    public void NextStage()
    {
        currentStage++;
        battleManager.currentStage = currentStage;
        battleManager.StartBattle();

        Debug.Log($"다음 스테이지로 이동! (Stage {currentStage})");
    }

    /// 플레이어 사망 시 처리
    public void OnPlayerDefeated()
    {
        Debug.Log("플레이어 패배! 게임 오버");
    }
}
