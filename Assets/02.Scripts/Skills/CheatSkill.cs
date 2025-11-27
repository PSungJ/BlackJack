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

        // 다음에 뒤집힐 커뮤니티 카드가 없는 경우
        if (idx < 0 || idx >= battle.communityCards.Count)
        {
            Debug.Log("[CHEAT] 뒤집을 공용 카드가 없음");
            return;
        }

        // UI에서 실제 투시 성공했을 때만 사용 처리
        bool success = ui.ShowCheatPreview(idx);

        if (success)
        {
            isUsedThisStage = true;
        }
        else
        {
            Debug.LogWarning("[CHEAT] 미리보기 실패 - 사용 처리 안함");
        }
    }
}
