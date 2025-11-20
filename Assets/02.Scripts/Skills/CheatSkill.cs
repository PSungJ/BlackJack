using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/CheatSkill")]
public class CheatSkill : ActiveSkill
{
    public override void Activate(BattleManager battle, BattleUIManager ui)
    {
        if (isUsedThisStage)
        {
            Debug.Log("[CHEAT] 이미 사용함");
            return;
        }

        int idx = battle.GetNextCommunityIndex();
        Card next = battle.GetCommunityCardAt(idx);

        if (next != null)
            ui.ShowCheatPreview(next);

        isUsedThisStage = true;
    }
}
