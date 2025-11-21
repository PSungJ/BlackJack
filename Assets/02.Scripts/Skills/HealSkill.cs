using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/HealSkill")]
public class HealSkill : PassiveSkill
{
    public int healAmount = 5;

    public override void OnStageClear(PlayerController player)
    {
        player.hp = Mathf.Min(player.maxHP, player.hp + healAmount);
        Debug.Log($"[HealSkill] 스테이지 보상으로 HP +{healAmount}");
    }
}
