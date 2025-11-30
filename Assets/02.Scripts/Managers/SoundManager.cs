using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("오디오 소스")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("오디오 클립")]
    public AudioClip lobbyBgm;
    public AudioClip gameBgm;
    public AudioClip hitButtonSfx;
    public AudioClip standButtonSfx;
    public AudioClip damageSfx;
    public AudioClip thinkingSfx;
    public AudioClip healSfx;
    public AudioClip reviveSfx;

    private float idleTimer = 0f;
    private bool isThinkingPlayed = false;

    private bool isGameScene = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        // Scene 로드 이벤트 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        PlayBGMForCurrentScene();
    }

    private void Update()
    {
        if (!isGameScene) return;   // 로비에서는 idle 체크 안함

        HandleIdleSound();
    }

    // -------------------------------
    // 씬 변경 시 자동 BGM 변경
    // -------------------------------
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayBGMForCurrentScene();
        
        // 게임 씬 판별
        isGameScene = !scene.name.Contains("Lobby");

        // 씬 전환 시 타이머 초기화
        ResetIdleTimer();
    }

    private void PlayBGMForCurrentScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName.Contains("Lobby"))
            ChangeBGM(lobbyBgm);
        else
            ChangeBGM(gameBgm);
    }

    private void ChangeBGM(AudioClip newClip)
    {
        if (newClip == null || bgmSource.clip == newClip)
            return;

        bgmSource.Stop();
        bgmSource.clip = newClip;
        bgmSource.loop = true;
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
    public void PlayHeal() => PlaySFX(healSfx);
    public void PlayRevive() => PlaySFX(reviveSfx);

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
