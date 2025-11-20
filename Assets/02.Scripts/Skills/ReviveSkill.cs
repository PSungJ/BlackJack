using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/ReviveSkill")]
public class ReviveSkill : PassiveSkill
{
    private bool isReady = true;
    private int cooldown = 5;
    private int cooldownRemain = 0;

    public override void OnStageClear(PlayerController player)
    {
        if (!isReady)
        {
            cooldownRemain--;
            if (cooldownRemain <= 0)
                isReady = true;
        }
    }

    public override void OnPlayerDamaged(PlayerController player)
    {
        if (!isReady) return;

        if (player.hp <= 0)
        {
            player.hp = player.maxHP;
            isReady = false;
            cooldownRemain = cooldown;

            BattleUIManager.Instance.ShowReviveEffect();
            Debug.Log("[REVIVE] 부활 발동!");
        }
    }
}
