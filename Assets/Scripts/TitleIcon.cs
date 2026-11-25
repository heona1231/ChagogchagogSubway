// �ڼ��� �ۼ�
using UnityEngine;
using UnityEngine.EventSystems;

public class TitleIcon : MonoBehaviour
{
    [SerializeField] private bool isStartIcon = false;
    [SerializeField] private bool isQuitIcon = false;

    [SerializeField] private bool isOptionIcon = false; // ������ ����, OptionIcon �߰�

    private void OnMouseUpAsButton()
    {
        AudioManager.Instance.AudioUiClick();

        // ������ �ۼ�, ����â ������ Ŭ���� ��� ����
        if (OptionManager.Instance != null && OptionManager.Instance.IsPanelOpen)

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
