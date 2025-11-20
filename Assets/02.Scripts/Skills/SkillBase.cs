using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillBase : ScriptableObject
{
    public string skillName;
    public string description;
    public int unlockStage;
    public Sprite icon;
    public bool isUnlocked = false;

    public virtual void TryUnlock(int currentStage)
    {
        if (isUnlocked) return;

        if (currentStage >= unlockStage)
        {
            SkillManager.Instance.UnlockSkill(this);
            OnUnlocked();
        }
    }

    protected virtual void OnUnlocked()
    {
        Debug.Log($"{skillName} 스킬 해금됨!");
        BattleUIManager.Instance.ShowSkillUnlock(skillName);
    }

    // --- 스킬 기본 훅(모든 스킬이 공통적으로 가짐) ---
    public virtual void OnStageClear(PlayerController player) { }
    public virtual void OnBattleStart(PlayerController player) { }
    public virtual void OnPlayerDamaged(PlayerController player) { }
}
