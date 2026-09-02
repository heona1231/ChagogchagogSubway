// �ڼ��� �ۼ�
using UnityEngine;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    //[SerializeField] private bool isSpecialSeatSuccess = false;
    [SerializeField] private StageData[] stageDatas;
    [SerializeField] private float limitTime = 60f;
    [SerializeField] private float targetTime = 30f;

    [Header("Timer")]
    [SerializeField] private Transform timerFill;
    [SerializeField] private Transform targetTimeMarker;

    [Header("Panels")]
    [SerializeField] private GameObject clearPanel;

    [Header("Clear Star")]
    [SerializeField] private Image clearStarImage;
    [SerializeField] private Sprite star0Sprite;
    [SerializeField] private Sprite star1Sprite;
    [SerializeField] private Sprite star2Sprite;
    [SerializeField] private Sprite star3Sprite;

    [Header("Clear(Next) Button")]
    [SerializeField] private Button nextButton;
    [SerializeField] private Image nextButtonImage;
    [SerializeField] private Sprite nextButtonActiveSprite;
    [SerializeField] private Sprite nextButtonDisabledSprite;

    [Header("Board")] // ������ �ۼ�, board ������Ʈ ���� ����
    [SerializeField] private Board bgBoard;
    [SerializeField] private Board gameBoard;

    [Header("Block")]
    [SerializeField] private Block blockPrefab;

    private StageData currentStageData;
    private bool isCleared = false;
    private Vector3 timerFillStartScale;
    //private Vector3 timerFillStartPosition;
    private float timerFillStartLeftX;
    private float timerFillStartWidth;

    private void Start()
    {
        int currentChapterNumber = GameManager.Instance.CurrentChapterNumber;
        int currentStageNumber = GameManager.Instance.CurrentStageNumber;
        currentStageData = FindStageData(currentChapterNumber, currentStageNumber);

        //if (currentStageData == null) return;

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

        // ������ �ۼ�, ���&���� ���� ������ stageData�� ������ ����
        /**
        bgBoard.boardData = stageData.bgBoardData;
        gameBoard.boardData = stageData.gameBoardData;
        bgBoard.Initialize(stageData.bgBoardData);
        gameBoard.Initialize(stageData.gameBoardData);
        **/

        SetTargetTimeMarker();

        //GameManager.Instance.StartStage(
        //    currentStageData.limitTime,
        //    currentStageData.targetTime
        //);

        //Debug.Log($"é��{currentStageData.chapterNumber} ��������{currentStageData.stageNumber} ����");

        //������ �ۼ�, block ����
        Block newBlock = Instantiate(blockPrefab, Vector3.zero, Quaternion.identity);

        foreach (var blockSpawnData in currentStageData.blockSpawnDatas)
        {
            newBlock.transform.position = new Vector3(blockSpawnData.spawnPosition.x, blockSpawnData.spawnPosition.y, 0f);
            newBlock.transform.rotation = Quaternion.Euler(blockSpawnData.spawnRotation);
            newBlock.Initialize(blockSpawnData.blockDataPrefab);
        }

        GameManager.Instance.StartStage(limitTime, targetTime);
        //Debug.Log($"{currentStageNumber}�� �������� ����");
        Debug.Log($"é�� {currentChapterNumber}��,  {currentStageNumber}�� �������� ����");

    }

    private void Update()
    {
        UpdateTimerBar();
    }

    // StageData ������� �����鼭 �ּ� ó��
    // ������ ����
    private StageData FindStageData(int chapterNumber, int stageNumber)
    {
        /*int chapterNumber = GameManager.Instance.CurrentChapterNumber;

        foreach (StageData data in stageDatas)
        {
            if (data.chapterNumber == chapterNumber && data.stageNumber == stageNumber)
            {
                return data;
            }
        }*/

        try
        {
            currentStageData = stageDatas[stageNumber];
        }
        catch
        {
            Debug.LogError($"é��{chapterNumber} {stageNumber}�� �������� �����͸� ã�� �� �����ϴ�.");
        }

        Debug.LogError($"é��{chapterNumber} {stageNumber}�� �������� �����͸� ã�� �� �����ϴ�.");
        return null;
    }

    private void SetTargetTimeMarker()
    {
        if (targetTimeMarker == null || timerFill == null) //|| currentStageData == null)
        {
            return;
        }

        SpriteRenderer timerRenderer = timerFill.GetComponent<SpriteRenderer>();

        //float ratio = currentStageData.targetTime / currentStageData.limitTime;
        float ratio = targetTime / limitTime;

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
        /**if (!IsPuzzleCleared())
        {
            Debug.Log("���� �̿ϼ�");
            return;
        }**/

        // isCleared = true; // Ȯ�ο�

        bool isSpecialSeatSuccess = Board.Main.CheckAllSpecialSeatsSatisfied();

        GameManager.Instance.ClearStage(isSpecialSeatSuccess);
    }

    private bool IsPuzzleCleared()
    {
        // ��� ������ ���� ��ġ�� �������� Ȯ��
        return false;
    }

    private bool IsSpecialSeatSuccess()
    {
        // Ư�� �°��� Ư�� �¼��� �°� ��ġ�ƴ��� Ȯ��
        return false;
    }

    private void SetNextButtonState(int starCount)
    {
        bool canGoNext = starCount > 0;

        nextButton.interactable = canGoNext;

        if (canGoNext)
        {
            nextButtonImage.sprite = nextButtonActiveSprite;
        }
        else
        {
            nextButtonImage.sprite = nextButtonDisabledSprite;
        }
    }

    public void OpenClearPanel(int starCount)
    {
        if (clearPanel == null)
        {
            Debug.LogError("ClearPanel�� StageManager�� ������� �ʾҽ��ϴ�.");
            return;
        }

        if (clearStarImage == null)
        {
            Debug.LogError("ClearStarRenderer�� StageManager�� ������� �ʾҽ��ϴ�.");
            return;
        }

        Debug.Log($"OpenClearPanel ����� / �� {starCount}��");

        clearPanel.SetActive(true);
        //clearPanel.transform.SetAsLastSibling();
        GameManager.Instance.PauseStage();

        if (starCount == 0)
        {
            clearStarImage.sprite = star0Sprite;
        }
        else if (starCount == 1)
        {
            clearStarImage.sprite = star1Sprite;
        }
        else if (starCount == 2)
        {
            clearStarImage.sprite = star2Sprite;
        }
        else if (starCount == 3)
        {
            clearStarImage.sprite = star3Sprite;
        }

        SetNextButtonState(starCount);
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
    // �� ������ StageManager�� �ƴ϶� GameManager���� �� �ű� ������!
}
