using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/HealSkill")]
public class HealSkill : PassiveSkill
{
    public int healAmount = 5;

    public override void OnStageClear(PlayerController player)
    {
        // 이미 풀피면 아무것도 하지 않음
        if (player.hp >= player.maxHP)
        {
            Debug.Log("[HEAL] 풀피 상태, 회복 효과 미발동");
            return;
        }

        player.hp = Mathf.Clamp(player.hp + healAmount, 0, player.maxHP);
        Debug.Log($"[HEAL] {healAmount} 회복!");

        // UI 즉시 업데이트
        BattleUIManager.Instance.UpdateStatusUI();

        // Heal 효과 연출 실행
        BattleUIManager.Instance.StartCoroutine(
            BattleUIManager.Instance.ShowHealEffect(healAmount));

        // 사운드 재생
        SoundManager.Instance.PlayHeal();
    }
}
