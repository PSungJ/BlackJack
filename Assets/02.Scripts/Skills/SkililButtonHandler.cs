using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillButtonHandler : MonoBehaviour
{
    public BattleManager battleManager;
    public BattleUIManager uiManager;

    public void OnClick_Cheat()
    {
        SkillManager.Instance.UseActiveSkill("Cheat", battleManager, uiManager);
    }

    public void OnClick_Focus()
    {
        SkillManager.Instance.UseActiveSkill("Focus", battleManager, uiManager);
    }
}
