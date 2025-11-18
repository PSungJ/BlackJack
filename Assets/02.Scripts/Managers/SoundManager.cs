using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("오디오 소스")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("오디오 클립")]
    public AudioClip bgm;
    public AudioClip hitButtonSfx;
    public AudioClip standButtonSfx;
    public AudioClip damageSfx;
    public AudioClip thinkingSfx;

    private float idleTimer = 0f;
    private bool isThinkingPlayed = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PlayBGM();
    }

    private void Update()
    {
        HandleIdleSound();
    }

    // -------------------------------
    // BGM
    // -------------------------------
    public void PlayBGM()
    {
        if (bgmSource == null || bgm == null) return;

        bgmSource.loop = true;
        bgmSource.clip = bgm;
        bgmSource.Play();
    }

    // -------------------------------
    // SFX
    // -------------------------------
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);

        ResetIdleTimer();
    }

    public void PlayHitButton() => PlaySFX(hitButtonSfx);
    public void PlayStandButton() => PlaySFX(standButtonSfx);
    public void PlayDamage() => PlaySFX(damageSfx);

    // -------------------------------
    // 플레이어가 아무것도 안할 때
    // -------------------------------
    private void HandleIdleSound()
    {
        idleTimer += Time.deltaTime;

        if (!isThinkingPlayed && idleTimer >= 10f)
        {
            PlaySFX(thinkingSfx);
            isThinkingPlayed = true;
        }
    }

    public void ResetIdleTimer()
    {
        idleTimer = 0f;
        isThinkingPlayed = false;
    }
}
