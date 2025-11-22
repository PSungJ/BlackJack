using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillBase : ScriptableObject
{
    public string skillName;
    public string description;
    public int unlockStage;
    public Sprite icon;

    // 스킬 해금 조건 체크 - 상태 변경은 SkillManager가 담당
    public virtual void TryUnlock(int currentStage)
    {
        if (currentStage >= unlockStage)
        {
            SkillManager.Instance.UnlockSkill(this);
            OnUnlocked();
        }
    }

    // 실제 해금 시 호출되는 훅
    public virtual void OnUnlocked()
    {
        Debug.Log($"{skillName} 스킬 해금됨!");
        BattleUIManager.Instance.ShowSkillUnlock(skillName);
    }

    // --- 스킬 기본 훅(모든 스킬이 공통적으로 가짐) ---
    public virtual void OnStageClear(PlayerController player) { }
    public virtual void OnBattleStart(PlayerController player) { }
    public virtual void OnPlayerDamaged(PlayerController player) { }
}
