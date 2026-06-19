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
}