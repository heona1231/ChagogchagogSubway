// 박세은 작성
using UnityEngine;

public class StageManager : MonoBehaviour
{
    //[SerializeField] private bool isSpecialSeatSuccess = false;
    [SerializeField] private StageData[] stageDatas;

    [Header("Timer")]
    [SerializeField] private Transform timerFill;
    [SerializeField] private Transform targetTimeMarker;

    [Header("Panels")]
    [SerializeField] private GameObject clearPanel;

    [Header("Clear Star")]
    [SerializeField] private SpriteRenderer clearStarRenderer;
    [SerializeField] private Sprite star0Sprite;
    [SerializeField] private Sprite star1Sprite;
    [SerializeField] private Sprite star2Sprite;
    [SerializeField] private Sprite star3Sprite;

    private StageData currentStageData;
    private bool isCleared = false;
    private Vector3 timerFillStartScale;
    //private Vector3 timerFillStartPosition;
    private float timerFillStartLeftX;
    private float timerFillStartWidth;

    private void Start()
    {
        int currentStageNumber = GameManager.Instance.CurrentStageNumber;
        currentStageData = FindStageData(currentStageNumber);

        if (currentStageData == null) return;

        if (clearPanel != null)
        {
            clearPanel.SetActive(false);
        }

        if (timerFill != null)
        {
            timerFillStartScale = timerFill.localScale;

            SpriteRenderer timerRenderer = timerFill.GetComponent<SpriteRenderer>();
            timerFillStartLeftX = timerRenderer.bounds.min.x;
            timerFillStartWidth = timerRenderer.bounds.size.x;
            //timerFillStartPosition = timerFill.position;
        }

        SetTargetTimeMarker();

        GameManager.Instance.StartStage(
            currentStageData.limitTime,
            currentStageData.targetTime
        );

        Debug.Log($"챕터{currentStageData.chapterNumber} 스테이지{currentStageData.stageNumber} 시작");
    }

    private void Update()
    {
        UpdateTimerBar();
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

    private void SetTargetTimeMarker()
    {
        if (targetTimeMarker == null || timerFill == null || currentStageData == null)
        {
            return;
        }

        SpriteRenderer timerRenderer = timerFill.GetComponent<SpriteRenderer>();

        float ratio = currentStageData.targetTime / currentStageData.limitTime;

        float leftX = timerRenderer.bounds.min.x;
        float rightX = timerRenderer.bounds.max.x;

        Vector3 markerPosition = targetTimeMarker.position;
        markerPosition.x = Mathf.Lerp(leftX, rightX, ratio);

        targetTimeMarker.position = markerPosition;
    }

    private void UpdateTimerBar()
    {
        if (timerFill == null) return;

        float ratio = GameManager.Instance.GetRemainingTimeRatio();

        Vector3 scale = timerFillStartScale;
        scale.x = timerFillStartScale.x * ratio;
        timerFill.localScale = scale;

        Vector3 position = timerFill.position;
        position.x = timerFillStartLeftX + (timerFillStartWidth * ratio / 2f);
        //timerFill.localScale = scale;
        timerFill.position = position;
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

    public void OpenClearPanel(int starCount)
    {
        if (clearPanel == null)
        {
            Debug.LogError("ClearPanel이 StageManager에 연결되지 않았습니다.");
            return;
        }

        if (clearStarRenderer == null)
        {
            Debug.LogError("ClearStarRenderer가 StageManager에 연결되지 않았습니다.");
            return;
        }

        Debug.Log($"OpenClearPanel 실행됨 / 별 {starCount}개");

        clearPanel.SetActive(true);
        //clearPanel.transform.SetAsLastSibling();
        GameManager.Instance.PauseStage();

        if (starCount == 0)
        {
            clearStarRenderer.sprite = star0Sprite;
        }
        else if (starCount == 1)
        {
            clearStarRenderer.sprite = star1Sprite;
        }
        else if (starCount == 2)
        {
            clearStarRenderer.sprite = star2Sprite;
        }
        else if (starCount == 3)
        {
            clearStarRenderer.sprite = star3Sprite;
        }
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
