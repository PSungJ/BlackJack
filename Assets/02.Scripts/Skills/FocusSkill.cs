using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/FocusSkill")]
public class FocusSkill : ActiveSkill
{
    public override void Activate(BattleManager battle, BattleUIManager ui)
    {
        if (isUsedThisStage)
        {
            Debug.Log("[FOCUS] ÀÌ¹Ì »ç¿ëµÊ");
            return;
        }

        int min = battle.boss.GetMinPossibleScore();
        int max = battle.boss.GetMaxPossibleScore();

        ui.ShowFocusPrediction(min, max);

        isUsedThisStage = true;
    }
}
