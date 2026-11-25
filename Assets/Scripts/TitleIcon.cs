// 박세은 작성
using UnityEngine;
using UnityEngine.EventSystems;

public class TitleIcon : MonoBehaviour
{
    [SerializeField] private bool isStartIcon = false;
    [SerializeField] private bool isQuitIcon = false;

    [SerializeField] private bool isOptionIcon = false; // 강혜원 수정, OptionIcon 추가

    private void OnMouseUpAsButton()
    {
        // 강혜원 작성, 설정창 위에서 클릭한 경우 무시
        if (OptionManager.Instance != null && OptionManager.Instance.IsPanelOpen)
            return;

        if (isStartIcon)
        {
            GameManager.Instance.StartGame();
        }

        if (isQuitIcon)
        {
            GameManager.Instance.QuitGame();
        }

        if (isOptionIcon)
        {
            OptionManager.Instance.OpenSettingsPanel();
        }
    }
}
