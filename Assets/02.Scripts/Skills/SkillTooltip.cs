using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillTooltip : MonoBehaviour
{
    public static SkillTooltip Instance;

    public GameObject root;
    public TMP_Text titleText;
    public Text descText;

    private void Awake()
    {
        Instance = this;
        Hide();
    }

    public void Show(SkillBase skill, Vector3 pos)
    {
        root.SetActive(true);
        transform.position = pos;

        titleText.text = skill.skillName;
        descText.text = skill.description;
    }

    public void Hide()
    {
        root.SetActive(false);
    }
}
