// 박세은 작성

using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SoundSettingUI : MonoBehaviour
{
    public enum SoundType
    {
        BGM,
        SFX
    }

    [Header("Sound Type")]
    [SerializeField] private SoundType soundType;

    [Header("UI")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TMP_InputField volumeInputField;

    [Header("Default")]
    [Range(0, 100)]
    [SerializeField] private int defaultVolume = 50;

    private string SaveKey
    {
        get
        {
            return soundType == SoundType.BGM
                ? "BGMVolume"
                : "SFXVolume";
        }
    }

    private void Awake()
    {
        if (volumeSlider == null)
        {
            Debug.LogError($"[{name}] Slider가 연결되지 않았습니다.");
            enabled = false;
            return;
        }

        volumeSlider.minValue = 0;
        volumeSlider.maxValue = 100;
        volumeSlider.wholeNumbers = true;

        volumeSlider.onValueChanged.AddListener(OnSliderValueChanged);

        if (volumeInputField != null)
        {
            volumeInputField.contentType = TMP_InputField.ContentType.IntegerNumber;

            volumeInputField.characterLimit = 3;
            volumeInputField.onEndEdit.AddListener(OnInputFieldEndEdit);
        }
    }

    private void Start()
    {
        int savedVolume = Mathf.RoundToInt(PlayerPrefs.GetFloat(SaveKey, defaultVolume));
        ApplyVolume(savedVolume, false);
    }

    private void OnDestroy()
    {
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }

        if (volumeInputField != null)
        {
            volumeInputField.onEndEdit.RemoveListener(OnInputFieldEndEdit);
        }
    }

    private void OnSliderValueChanged(float value)
    {
        int volume = Mathf.RoundToInt(value);
        ApplyVolume(volume, true);
    }

    private void OnInputFieldEndEdit(string inputText)
    {
        if (!int.TryParse(inputText, out int volume))
        {
            UpdateInputField(Mathf.RoundToInt(volumeSlider.value));
            return;
        }

        volume = Mathf.Clamp(volume, 0, 100);
        ApplyVolume(volume, true);
    }

    private void ApplyVolume(int volume, bool save)
    {
        volume = Mathf.Clamp(volume, 0, 100);

        volumeSlider.SetValueWithoutNotify(volume);

        UpdateInputField(volume);
        ApplyToAudioManager(volume, save);
    }

    private void UpdateInputField(int volume)
    {
        if (volumeInputField == null)
        {
            return;
        }

        volumeInputField.SetTextWithoutNotify(volume.ToString());
    }

    private void ApplyToAudioManager(int volume, bool save)
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning(
                $"[{name}] AudioManager.Instance가 없습니다. "
                + "TitleScene을 거쳐 실행했는지 확인해주세요.");

            if (save)
            {
                PlayerPrefs.SetFloat(SaveKey, volume);
                PlayerPrefs.Save();
            }

            return;
        }

        if (soundType == SoundType.BGM)
        {
            AudioManager.Instance.SetBgmVolume(volume, save);
        }
        else if (soundType == SoundType.SFX)
        {
            AudioManager.Instance.SetSfxVolume(volume, save);
        }
    }

    public void ResetVolume()
    {
        ApplyVolume(defaultVolume, true);
    }
}
