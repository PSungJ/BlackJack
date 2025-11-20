using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/ArmorSkill")]
public class ArmorSkill : PassiveSkill
{
    public int armorGain = 10;

    public override void OnStageClear(PlayerController player)
    {
        player.AddArmor(armorGain);
        Debug.Log($"[ARMOR] 스테이지 클리어 → 방어막 +{armorGain}");
    }
}
