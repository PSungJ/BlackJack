using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillState
{
    public SkillBase data;
    public bool unlocked;

    // ActiveSkill 전용
    public bool usedThisStage = false;

    // ReviveSkill용
    public bool reviveReady = true;
    public int reviveCooldown = 5;
    public int reviveCooldownRemain = 0;
}

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance;

    public List<SkillBase> skills = new List<SkillBase>();
    public List<SkillSlot> allSkillSlots = new List<SkillSlot>();

    [Header("플레이어 진행용 상태 저장")]
    public List<SkillState> skillStates = new List<SkillState>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // SkillState 생성 (ScriptableObject는 상태를 가지지 않음)
        if (skillStates.Count == 0)
        {
            foreach (var skill in skills)
            {
                skillStates.Add(new SkillState
                {
                    data = skill,
                    unlocked = false
                });
            }
        }
    }

    // ============================
    //   전투 이벤트 콜백 전달
    // ===================

    public void OnStageClear(PlayerController player)
    {
        foreach (var s in skillStates)
            if (s.unlocked)
                s.data.OnStageClear(player);
    }

    public void OnBattleStart(PlayerController player)
    {
        foreach (var s in skillStates)
        {
            if (s.unlocked)
            {
                s.data.OnBattleStart(player);

                // ActiveSkill은 스테이지 시작마다 사용 가능하도록 초기화
                if (s.data is ActiveSkill active)
                {
                    active.ResetUsage();
                }
            }
        }
    }

    public void OnAcivateSkill(PlayerController player)
    {
        foreach (var s in skillStates)
        {
            if (s.data is ActiveSkill)
                s.usedThisStage = false;
        }
    }

    public void OnPlayerDamaged(PlayerController player)
    {
        foreach (var s in skillStates)
            if (s.unlocked)
                s.data.OnPlayerDamaged(player);
    }

    // ============================
    //   스킬 해금 체크
    // ============================
    public void CheckUnlock(int stage)
    {
        foreach (var state in skillStates)
        {
            if (!state.unlocked && stage >= state.data.unlockStage)
            {
                UnlockSkill(state);
            }
        }
    }

    public void UnlockSkill(SkillBase baseSkill)
    {
        // SkillState 찾기
        SkillState state = skillStates.Find(x => x.data == baseSkill);
        if (state == null) return;

        state.unlocked = true;

        Debug.Log($"{baseSkill.skillName} 스킬 해금!");

        // ScriptableObject 훅 실행
        baseSkill.OnUnlocked();

        // SkillSlot UI 갱신
        foreach (SkillSlot slot in allSkillSlots)
        {
            if (slot.state.data == baseSkill)
            {
                slot.RefreshUI();           // 밝기, lock 표시 갱신
                StartCoroutine(slot.UnlockRoutine());       // 해금 애니메이션 실행
                break;
            }
        }
    }

    public void UnlockSkill(SkillState state)
    {
        UnlockSkill(state.data);
    }

    public void UseActiveSkill(string skillName, BattleManager bm, BattleUIManager ui)
    {
        // 1) 스킬 찾기
        SkillState state = skillStates.Find(x => x.data.skillName == skillName);
        if (state == null || !state.unlocked) return;

        // 2) 패시브는 무시
        ActiveSkill active = state.data as ActiveSkill;
        if (active == null) return;

        if (state.usedThisStage)
        {
            Debug.Log($"{skillName} 이미 사용됨");
            return;
        }

        // 3) 스킬 사용
        active.Activate(bm, ui);
        state.usedThisStage = true;
    }

    public SkillState GetState(SkillBase baseSkill)
    {
        return skillStates.Find(x => x.data == baseSkill);
    }
}