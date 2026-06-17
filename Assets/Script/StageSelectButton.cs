// 박세은 작성
using UnityEngine;

public class StageSelectButton : MonoBehaviour
{
    [SerializeField] private int stageNumber;
    [SerializeField] private bool isLocked;

    public void SelectStage()
    {
        int currentChapterNumber = GameManager.Instance.CurrentChapterNumber;

        bool isUnlocked = stageNumber == 1 ||
            PlayerPrefs.GetInt($"Chapter{currentChapterNumber}Stage{stageNumber - 1}Star", 0) > 0;

        if (!isUnlocked)
        {
            Debug.Log("잠긴 스테이지입니다.");
            return;
        }

        GameManager gameManager = FindFirstObjectByType<GameManager>();

        gameManager.SelectStage(stageNumber);
    }
}
