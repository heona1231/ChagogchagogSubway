// 박세은 작성
using UnityEngine;
using UnityEngine.UI;

public class StageIcon : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject clearPanel;

    [Header("Icons")]
    [SerializeField] private bool isMenuIcon = false;
    [SerializeField] private bool isRestartIcon = false;
    [SerializeField] private bool isReturnIcon = false;
    [SerializeField] private bool isMainMenuIcon = false;
    [SerializeField] private bool isRestartButton = false;
    [SerializeField] private bool isNextButton = false;

    [Header("Clear Star")]
    [SerializeField] private SpriteRenderer clearStarRenderer;
    [SerializeField] private Sprite star0Sprite;
    [SerializeField] private Sprite star1Sprite;
    [SerializeField] private Sprite star2Sprite;
    [SerializeField] private Sprite star3Sprite;

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

    private void OnMouseDown()
    {
        if (isMenuIcon)
        {
            menuPanel.SetActive(true);
            GameManager.Instance.PauseStage();
            return;
        }

        if (isRestartIcon)
        {
            GameManager.Instance.RestartGame();
            return;
        }

        if (isReturnIcon)
        {
            menuPanel.SetActive(false);
            GameManager.Instance.ResumeStage();
            return;
        }

        if (isMainMenuIcon)
        {
            GameManager.Instance.BackToChapter();
            return;
        }

        if (isRestartButton)
        {
            GameManager.Instance.RestartGame();
            return;
        }

        if (isNextButton)
        {
            GameManager.Instance.NextStage();
            return;
        }
    }

    //public void OpenClearPanel(int starCount)
    //{
    //    clearPanel.SetActive(true);
    //    GameManager.Instance.PauseStage();

    //    if (starCount == 0)
    //    {
    //        clearStarRenderer.sprite = star0Sprite;
    //    }
    //    else if (starCount == 1)
    //    {
    //        clearStarRenderer.sprite = star1Sprite;
    //    }
    //    else if (starCount == 2)
    //    {
    //        clearStarRenderer.sprite = star2Sprite;
    //    }
    //    else if (starCount == 3)
    //    {
    //        clearStarRenderer.sprite = star3Sprite;
    //    }
    //}
}