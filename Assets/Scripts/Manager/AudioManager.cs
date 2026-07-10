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
    }

    //시작시 bgm1Sound를 bgm으로 재생
    private void Start()
    {
        PlayBGM(bgm1Sound);
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
    public void SetBgmVolume(float volume)
    {
        bgmVolume = volume;
        if (bgmSource != null)
        {
            bgmSource.volume = bgmVolume;
        }
    }

    //효과음 볼륨 조절 함수
    //사용법 : AudioManager.Instance.SetSfxVolume([0~100사이의 숫자])
    public void SetSfxVolume(float volume)
    {
        sfxVolume = volume;
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
    }
}
