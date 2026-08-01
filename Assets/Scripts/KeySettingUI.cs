// 박세은 작성

using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class KeySettingUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Dropdown actionDropdown;
    [SerializeField] private TMP_Text keyText;

    private readonly List<string> actionNames = new()
    {
        "뒤로가기",
        "미니게임",
        "집기",
        "회전",
        "메뉴"
    };

    private readonly List<string> defaultKeyNames = new()
    {
        //"ESC,
        "SPACE",
        "E",
        "R",
        "ESC"
    };

    private void Start()
    {
        if (actionDropdown == null || keyText == null)
        {
            Debug.LogError(
                "[KeySettingUI] Dropdown 또는 KeyText가 연결되지 않았습니다.");
            return;
        }

        SetupDropdown();

        actionDropdown.onValueChanged.AddListener(OnActionChanged);

        OnActionChanged(actionDropdown.value);
    }

    private void OnDestroy()
    {
        if (actionDropdown != null)
        {
            actionDropdown.onValueChanged.RemoveListener(OnActionChanged);
        }
    }

    private void SetupDropdown()
    {
        actionDropdown.ClearOptions();
        actionDropdown.AddOptions(actionNames);

        actionDropdown.value = 0;
        actionDropdown.RefreshShownValue();
    }

    private void OnActionChanged(int selectedIndex)
    {
        if (selectedIndex < 0 || selectedIndex >= defaultKeyNames.Count)
        {
            keyText.text = "-";
            return;
        }

        keyText.text = defaultKeyNames[selectedIndex];
    }
}
