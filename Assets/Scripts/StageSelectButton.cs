// 박세은 작성
using UnityEngine;

public class StageSelectButton : MonoBehaviour
{
    [Header("Stage Info")]
    [SerializeField] private int chapterNumber = 1;
    [SerializeField] private int stageNumber = 1;

    public void SelectStage()
    {
        GameManager gameManager = GameManager.Instance;

        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>();
        }

        if (gameManager == null)
        {
            Debug.LogError("GameManager를 찾을 수 없습니다.");
            return;
        }

        bool isUnlocked =
            stageNumber == 1 || PlayerPrefs.GetInt($"Chapter{chapterNumber}Stage{stageNumber - 1}Star", 0) > 0;
    
        if (!isUnlocked)
        {
            Debug.Log($"챕터 {chapterNumber}의 스테이지 {stageNumber}는 잠겨 있습니다.");
            return;
        }

        gameManager.SelectStage(chapterNumber, stageNumber);
    }
}
