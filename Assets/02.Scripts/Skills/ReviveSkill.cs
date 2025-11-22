using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/ReviveSkill")]
public class ReviveSkill : PassiveSkill
{
    public override void OnPlayerDamaged(PlayerController player)
    {
        SkillState state = SkillManager.Instance.GetState(this);
        if (state == null || !state.unlocked) return;

        // 사용 가능 여부 체크
        if (!state.reviveReady) return;

        // HP 0 이하일 때만 발동
        if (player.hp <= 0)
        {
            player.hp = player.maxHP / 2;

            state.reviveReady = false;
            state.reviveCooldownRemain = state.reviveCooldown;

            Debug.Log("[REVIVE] 부활 발동!");
            BattleUIManager.Instance.ShowReviveEffect();
        }
    }

    public override void OnStageClear(PlayerController player)
    {
        SkillState state = SkillManager.Instance.GetState(this);
        if (state == null || !state.unlocked) return;

        if (!state.reviveReady)
        {
            state.reviveCooldownRemain--;
            if (state.reviveCooldownRemain <= 0)
            {
                state.reviveReady = true;
                Debug.Log("[REVIVE] 쿨다운 완료, 다시 사용 가능!");
            }
        }
    }
}
