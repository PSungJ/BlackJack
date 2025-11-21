using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlot : MonoBehaviour
{
    [Header("UI 연결")]
    public SkillState state;       // SkillManager에서 직접 연결
    public Image icon;
    public GameObject lockObj;

    [Header("애니메이션 옵션")]
    public float unlockScaleTime = 0.25f;

    private void Awake()
    {
        RefreshUI();
    }

    // UI 갱신 (잠금 여부 반영)
    public void RefreshUI()
    {
        bool unlocked = state.unlocked;

        icon.color = unlocked ? Color.white : new Color(0.4f, 0.4f, 0.4f);
        lockObj.SetActive(!unlocked);
    }

    // SkillManager -> 해금될 때 호출
    public IEnumerator UnlockRoutine()
    {
        // 아이콘 밝게
        icon.color = Color.white;

        // 자물쇠 애니메이션
        if (lockObj != null)
        {
            RectTransform rt = lockObj.GetComponent<RectTransform>();
            Vector3 originalScale = rt.localScale;

            // 확대
            float t = 0;
            while (t < unlockScaleTime)
            {
                t += Time.deltaTime;
                rt.localScale = Vector3.Lerp(originalScale, originalScale * 1.3f, t);
                yield return null;
            }

            t = 0;
            while (t < unlockScaleTime)
            {
                t += Time.deltaTime;
                rt.localScale = Vector3.Lerp(originalScale * 1.3f, Vector3.zero, t);
                yield return null;
            }

            lockObj.SetActive(false);
        }
    }
}
