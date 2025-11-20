using System;

public abstract class PassiveSkill : SkillBase
{
    // 패시브는 기본적으로 OnStageClear / OnBattleStart / OnPlayerDamaged 중 필요한 것만 override
}
