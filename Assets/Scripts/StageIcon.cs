// 박세은 작성
using UnityEngine;

public class StageIcon : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject menuPanel;
    //[SerializeField] private GameObject clearPanel;

    private void Start()
    {
        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
        }
        //if (clearPanel != null)
        // {
        //     clearPanel.SetActive(false);
        // }
    }

    public void OpenMenu()
    {
        menuPanel.SetActive(true);
        GameManager.Instance.PauseStage();
    }

    public void RestartStage()
    {
        GameManager.Instance.RestartGame();
    }

    public void ReturnStage()
    {
        menuPanel.SetActive(false);
        GameManager.Instance.ResumeStage();
    }

    public void BackToMainMenu()
    {
        GameManager.Instance.BackToChapter();
    }

    public void GoToNextStage()
    {
        GameManager.Instance.NextStage();
    }


    public bool IsMenuOpen()
    {
        return menuPanel != null && menuPanel.activeSelf;
    }

    public void ToggleMenu()
    {
        if (menuPanel == null)
        {
            Debug.LogError("MenuPanel이 연결되지 않았습니다.");
            return;
        }

        if (menuPanel.activeSelf)
        {
            menuPanel.SetActive(false);
            GameManager.Instance.ResumeStage();
        }
        else
        {
            menuPanel.SetActive(true);
            GameManager.Instance.PauseStage();
        }
    }
}