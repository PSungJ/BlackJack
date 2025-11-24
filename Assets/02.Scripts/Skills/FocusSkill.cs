using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/FocusSkill")]
public class FocusSkill : ActiveSkill
{
    public int minError = 2;   // 최소 오차
    public int maxError = 8;   // 최대 오차

    public override void Activate(BattleManager battle, BattleUIManager ui)
    {
        if (isUsedThisStage)
        {
            Debug.Log("[FOCUS] 이미 사용됨");
            return;
        }

        int realScore = battle.boss.GetTotalValue(battle.GetRevealedCommunityCards());

        int error = Random.Range(minError, maxError + 1);

        int min = Mathf.Max(0, realScore - error);
        int max = realScore + error;

        ui.ShowFocusPrediction(min, max);

        Debug.Log($"[FOCUS] 실제 점수 {realScore}, 예측 범위 {min} ~ {max} (error {error})");

        isUsedThisStage = true;
    }
}
