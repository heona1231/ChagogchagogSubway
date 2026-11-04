//서현아 작성
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    [Header("오디오 소스")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("배경음악")]
    public AudioClip bgm1Sound;
    public AudioClip bgm2Sound;

    [Header("효과음 클립")]
    public AudioClip clickSound;
    public AudioClip blockPlaceSound;

    [Header("볼륨")]
    public float bgmVolume;
    public float sfxVolume;

    // 박세은 추가(SettingsPanel이랑 연결하기 위함)
    [Header("기본 볼륨(0~100)")]
    [Range(0f, 100f)]
    [SerializeField] private float defaultBgmVolume = 50f;
    [SerializeField] private float defaultSfxVolume = 50f;

    public float BgmVolume { get; private set; }
    public float SfxVolume { get; private set; }

    private const string BGM_VOLUME_KEY = "BGMVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        LoadVolumeSettings();
    }

    //시작시 bgm1Sound를 bgm으로 재생
    private void Start()
    {
        PlayBGM(bgm1Sound);
    }

    private void LoadVolumeSettings()
    {
        float savedBgmVolume = PlayerPrefs.GetFloat(
            BGM_VOLUME_KEY,
            defaultBgmVolume
            );
        float savedSfxVolume = PlayerPrefs.GetFloat(
            SFX_VOLUME_KEY,
            defaultSfxVolume
            );

        SetBgmVolume(savedBgmVolume, false);
        SetSfxVolume(savedSfxVolume, false);
    }

    public void PlayBGM(AudioClip clip)
    {
        if(clip != null && bgmSource != null)
        {
            bgmSource.clip = clip;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    //효과음 재생 함수
    //사용법 : AudioManager.Instance.PlaySFX(AudioManager.Instance.[스크립트 내 효과음 클립 이름 ex>clickSound]);
    public void PlaySFX(AudioClip clip)
    {
        if(clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }


    //bgm 볼륨 조절 함수
    //사용법 : AudioManager.Instance.SetBgmVolume([0~100사이의 숫자])
    // 박세은 추가: UI 기준으로 0~100 받도록 수정 (파라미터도 추가)
    public void SetBgmVolume(float volume, bool save = true)
    {
        //bgmVolume = volume;
        BgmVolume = Mathf.Clamp(volume, 0f, 100f);

        if (bgmSource != null)
        {
            // AudioSource.volume이 0~1을 사용하기 때문에 100f로 나누기
            bgmSource.volume = BgmVolume / 100f;
        }

        if (save)
        {
            PlayerPrefs.SetFloat(BGM_VOLUME_KEY, BgmVolume);
            PlayerPrefs.Save();
        }
    }

    //효과음 볼륨 조절 함수
    //사용법 : AudioManager.Instance.SetSfxVolume([0~100사이의 숫자])
    // 박세은 추가: SetBGMVolume에서의 설명으로 대체
    public void SetSfxVolume(float volume, bool save = true)
    {
        //sfxVolume = volume;
        SfxVolume = Mathf.Clamp(volume, 0f, 100f);

        if (sfxSource != null)
        {
            sfxSource.volume = SfxVolume / 100f;
        }

        if (save)
        {
            PlayerPrefs.SetFloat(SFX_VOLUME_KEY, SfxVolume);
            PlayerPrefs.Save();
        }
    }

    public void ResetVolumeSettings()
    {
        SetBgmVolume(defaultBgmVolume);
        SetSfxVolume(defaultSfxVolume);
    }
}
