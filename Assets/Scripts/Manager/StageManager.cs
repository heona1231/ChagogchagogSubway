// 박세은 작성
using UnityEngine;

public class StageManager : MonoBehaviour
{
    //[SerializeField] private bool isSpecialSeatSuccess = false;
    [SerializeField] private StageData[] stageDatas;
    
    private StageData currentStageData;
    private bool isCleared = false;

    private void Start()
    {
        int currentStageNumber = GameManager.Instance.CurrentStageNumber;
        currentStageData = FindStageData(currentStageNumber);

        if (currentStageData == null) return;

        GameManager.Instance.StartStage(
            currentStageData.limitTime,
            currentStageData.targetTime
        );

        Debug.Log($"챕터{currentStageData.chapterNumber} 스테이지{currentStageData.stageNumber} 시작");
    }

    private StageData FindStageData(int stageNumber)
    {
        int chapterNumber = GameManager.Instance.CurrentChapterNumber;

        foreach (StageData data in stageDatas)
        {
            if (data.chapterNumber == chapterNumber && data.stageNumber == stageNumber)
            {
                return data;
            }
        }

        Debug.LogError($"챕터{chapterNumber} {stageNumber}번 스테이지 데이터를 찾을 수 없습니다.");
        return null;
    }

    public void CheckClear()
    {
        if (isCleared) return;
        if (!IsPuzzleCleared())
        {
            Debug.Log("퍼즐 미완성");
            return;
        }

        isCleared = true; // 확인용

        bool isSpecialSeatSuccess = IsSpecialSeatSuccess();

        GameManager.Instance.ClearStage(isSpecialSeatSuccess);
    }

    private bool IsPuzzleCleared()
    {
        // 모든 블럭이 정답 위치에 놓였는지 확인
        return false;
    }

    private bool IsSpecialSeatSuccess()
    {
        // 특수 승객이 특수 좌석에 맞게 배치됐는지 확인
        return false;
    }

    //public void SaveStageStar(int stageNumber, int starCount)
    //{
    //    string key = "Chapter1Stage" + stageNumber + "Star";

    //    int savedStar = PlayerPrefs.GetInt(key, 0);

    //    if (starCount > savedStar)
    //    {
    //        PlayerPrefs.SetInt(key, starCount);
    //        PlayerPrefs.Save();
    //    }
    //}
    // 별 저장을 StageManager가 아니라 GameManager에서 할 거기 때문에!
}
