using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlot : MonoBehaviour
{
    [Header("UI 연결")]
    public SkillBase data;     // ScriptableObject
    public Image icon;
    public GameObject lockObj;

    [Header("애니메이션 옵션")]
    public float unlockScaleTime = 0.25f;

    void Awake()
    {
        RefreshUI(); // 초기 UI 설정
    }

    /// <summary>
    /// SkillSlot UI를 갱신하는 단 하나의 함수
    /// (잠금 여부, 아이콘 밝기, LockObj 상태 전부 관리)
    /// </summary>
    public void RefreshUI()
    {
        bool locked = !data.isUnlocked;

        // 자물쇠 아이콘
        if (lockObj != null)
            lockObj.SetActive(locked);

        // 아이콘 색상
        icon.color = locked
            ? new Color(0.4f, 0.4f, 0.4f)   // 잠김 → 회색
            : Color.white;                  // 해금 → 정상
    }

    /// <summary>
    /// SkillManager에서 해금 시 호출된다.
    /// </summary>
    public void Unlock()
    {
        StartCoroutine(UnlockRoutine());
    }

    /// <summary>
    /// 해금 애니메이션
    /// </summary>
    public IEnumerator UnlockRoutine()
    {
        // 아이콘 밝아짐
        icon.color = Color.white;

        // 자물쇠 연출
        if (lockObj != null)
        {
            RectTransform rt = lockObj.GetComponent<RectTransform>();
            Vector3 originalScale = rt.localScale;

            float t = 0;
            while (t < unlockScaleTime)
            {
                t += Time.deltaTime;
                float p = t / unlockScaleTime;

                rt.localScale = Vector3.Lerp(originalScale, originalScale * 1.3f, p);
                yield return null;
            }

            t = 0;
            while (t < unlockScaleTime)
            {
                t += Time.deltaTime;
                float p = t / unlockScaleTime;

                rt.localScale = Vector3.Lerp(originalScale * 1.3f, Vector3.zero, p);
                yield return null;
            }

            lockObj.SetActive(false);
        }
    }
}
