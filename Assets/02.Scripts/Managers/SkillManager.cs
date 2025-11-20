using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance;

    public List<SkillBase> skills = new List<SkillBase>();
    public List<SkillSlot> allSkillSlots = new List<SkillSlot>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void OnStageClear(PlayerController player)
    {
        foreach (var skill in skills)
            if (skill.isUnlocked)
                skill.OnStageClear(player);
    }

    public void OnBattleStart(PlayerController player)
    {
        foreach (var skill in skills)
            if (skill.isUnlocked)
            {
                skill.OnBattleStart(player);

                if (skill is ActiveSkill a)
                    a.ResetUsage();
            }
    }

    public void OnPlayerDamaged(PlayerController player)
    {
        foreach (var skill in skills)
            if (skill.isUnlocked)
                skill.OnPlayerDamaged(player);
    }

    public void CheckUnlock(int currentStage)
    {
        foreach (var skill in skills)
        {
            skill.TryUnlock(currentStage);
        }
    }

    public void UnlockSkill(SkillBase data)
    {
        data.isUnlocked = true;

        // 해당 데이터를 가진 SkillSlot 찾아서 연출 실행
        foreach (var slot in allSkillSlots)
        {
            if (slot.data == data)
            {
                StartCoroutine(slot.UnlockRoutine());
                slot.RefreshUI();
                break;
            }
        }
    }
}
