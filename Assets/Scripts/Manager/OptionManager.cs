// 박세은 작성

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionManager : MonoBehaviour
{ 
    public enum LanguageType
    {
        Korean,
        English
    }

    public enum PickupType
    {
        Click,
        Drag
    }

    public enum ScreenModeType
    {
        Windowed,
        Fullscreen
    }

    private LanguageType currentLanguage;
    private PickupType currentPickupType;
    private ScreenModeType currentScreenMode;

    [Header("설정창")]
    [SerializeField] private GameObject settingsPanel;

    [Header("언어 버튼")]
    [SerializeField] private Image koreanButtonImage;
    [SerializeField] private Sprite koreanSelectedSprite;
    [SerializeField] private Sprite koreanUnselectedSprite;
    [SerializeField] private Image englishButtonImage;
    [SerializeField] private Sprite englishSelectedSprite;
    [SerializeField] private Sprite englishUnselectedSprite;

    [Header("집기 방식 버튼")]
    [SerializeField] private Image clickButtonImage;
    [SerializeField] private Sprite clickSelectedSprite;
    [SerializeField] private Sprite clickUnselectedSprite;
    [SerializeField] private Image dragButtonImage;
    [SerializeField] private Sprite dragSelectedSprite;
    [SerializeField] private Sprite dragUnselectedSprite;

    [Header("화면 모드 버튼")]
    [SerializeField] private Image windowedButtonImage;
    [SerializeField] private Sprite windowedSelectedSprite;
    [SerializeField] private Sprite windowedUnselectedSprite;
    [SerializeField] private Image fullscreenButtonImage;
    [SerializeField] private Sprite fullscreenSelectedSprite;
    [SerializeField] private Sprite fullscreenUnselectedSprite;

    private void Start()
    {
        settingsPanel.SetActive(false);

        LoadOptions();
        RefreshAllButtons();
        ApplyAllOptions();
    }

    public void SelectKorean()
    {
        currentLanguage = LanguageType.Korean;

        PlayerPrefs.SetInt("Language", (int)currentLanguage);
        PlayerPrefs.Save();

        RefreshLanguageButtons();

        Debug.Log("[OptionManager] 한국어 선택");
    }

    public void SelectEnglish()
    {
        currentLanguage = LanguageType.English;

        PlayerPrefs.SetInt("Language", (int)currentLanguage);
        PlayerPrefs.Save();

        RefreshLanguageButtons();

        Debug.Log("[OptionManager] English 선택");
    }

    private void RefreshLanguageButtons()
    {
        bool isKoreanSelected =
            currentLanguage == LanguageType.Korean;

        koreanButtonImage.sprite = isKoreanSelected
            ? koreanSelectedSprite
            : koreanUnselectedSprite;

        englishButtonImage.sprite = isKoreanSelected
            ? englishUnselectedSprite
            : englishSelectedSprite;
    }

    public void SelectClickPickup()
    {
        currentPickupType = PickupType.Click;

        PlayerPrefs.SetInt("PickupType", (int)currentPickupType);
        PlayerPrefs.Save();

        RefreshPickupButtons();

        Debug.Log("[OptionManager] 클릭하여 집기 선택");
    }

    public void SelectDragPickup()
    {
        currentPickupType = PickupType.Drag;

        PlayerPrefs.SetInt("PickupType", (int)currentPickupType);
        PlayerPrefs.Save();

        RefreshPickupButtons();

        Debug.Log("[OptionManager] 끌어서 집기 선택");
    }

    private void RefreshPickupButtons()
    {
        bool isClickSelected =
            currentPickupType == PickupType.Click;

        clickButtonImage.sprite = isClickSelected
            ? clickSelectedSprite
            : clickUnselectedSprite;

        dragButtonImage.sprite = isClickSelected
            ? dragUnselectedSprite
            : dragSelectedSprite;
    }

    public void SelectWindowedMode()
    {
        currentScreenMode = ScreenModeType.Windowed;

        PlayerPrefs.SetInt("ScreenMode", (int)currentScreenMode);
        PlayerPrefs.Save();

        ApplyScreenMode();
        RefreshScreenModeButtons();

        Debug.Log("[OptionManager] 창 모드 선택");
    }

    public void SelectFullscreenMode()
    {
        currentScreenMode = ScreenModeType.Fullscreen;

        PlayerPrefs.SetInt("ScreenMode", (int)currentScreenMode);
        PlayerPrefs.Save();

        ApplyScreenMode();
        RefreshScreenModeButtons();

        Debug.Log("[OptionManager] 전체 화면 선택");
    }

    private void ApplyScreenMode()
    {
        if (currentScreenMode == ScreenModeType.Fullscreen)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
    }

    private void RefreshScreenModeButtons()
    {
        bool isWindowedSelected =
            currentScreenMode == ScreenModeType.Windowed;

        windowedButtonImage.sprite = isWindowedSelected
            ? windowedSelectedSprite
            : windowedUnselectedSprite;

        fullscreenButtonImage.sprite = isWindowedSelected
            ? fullscreenUnselectedSprite
            : fullscreenSelectedSprite;
    }

    private void LoadOptions()
    {
        // 저장된 값이 없을 때의 기본값
        currentLanguage = (LanguageType)PlayerPrefs.GetInt(
            "Language",
            (int)LanguageType.Korean
        );

        currentPickupType = (PickupType)PlayerPrefs.GetInt(
            "PickupType",
            (int)PickupType.Click
        );

        currentScreenMode = (ScreenModeType)PlayerPrefs.GetInt(
            "ScreenMode",
            (int)ScreenModeType.Windowed
        );
    }

    private void ApplyAllOptions()
    {
        ApplyScreenMode();
    }

    private void RefreshAllButtons()
    {
        RefreshLanguageButtons();
        RefreshPickupButtons();
        RefreshScreenModeButtons();
    }

    public void ResetSaveData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        Debug.Log("[OptionManager] 저장 데이터 초기화 완료");

        // 초기화된 기본 설정을 바로 화면에 반영하기 위해
        // 현재 장면을 다시 불러옴
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public LanguageType GetCurrentLanguage()
    {
        return currentLanguage;
    }

    public PickupType GetCurrentPickupType()
    {
        return currentPickupType;
    }

    public ScreenModeType GetCurrentScreenMode()
    {
        return currentScreenMode;
    }

    public void OpenSettingsPanel()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettingsPanel()
    {
        settingsPanel.SetActive(false);
    }
}