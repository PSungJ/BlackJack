using System;

public abstract class ActiveSkill : SkillBase
{
    public bool isUsedThisStage = false;

    public abstract void Activate(BattleManager bm, BattleUIManager ui);

    public virtual void ResetUsage()
    {
        isUsedThisStage = false;
    }
}
