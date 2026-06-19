// 박세은 작성
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int CurrentChapterNumber { get; private set; } = 1;
    public int CurrentStageNumber { get; private set; }

    [Header("Stage Time")]
    [SerializeField] private float limitTime;
    [SerializeField] private float targetTime;

    private float currentTime;
    private float stageStartTime;
    private bool isPlaying;
    private bool isPaused;
    private float pauseStartTime;
    private float totalPausedTime;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (isPlaying && Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }

        if (!isPlaying || isPaused) return;

        currentTime = Time.realtimeSinceStartup - stageStartTime - totalPausedTime;

        if (currentTime >= limitTime)
        {
            EndGame();
        }
    }

    public void StartGame()
    {
        Debug.Log("게임 시작");
        SceneManager.LoadScene("ChapterScene");
    }

    public void QuitGame()
    {
        Debug.Log("게임 종료");
        Application.Quit();
    }

    public void BackToChapter()
    {
        Debug.Log("챕터 화면으로 돌아가기");
        SceneManager.LoadScene("ChapterScene");
    }

    public void BackToTitle()
    {
        Debug.Log("타이틀 화면으로 돌아가기");
        SceneManager.LoadScene("TitleScene");
    }

    public void SelectStage(int stageNumber)
    {
        CurrentStageNumber = stageNumber;
        Debug.Log($"{stageNumber}번 스테이지로 이동하기");
        SceneManager.LoadScene($"Stage{stageNumber}");
    }

    public void StartStage(float stageLimitTime, float stageTargetTime)
    {
        if (CurrentStageNumber <= 0)
        {
            CurrentStageNumber = GetStageNumberFromCurrentScene();
        }

        limitTime = stageLimitTime;
        targetTime = stageTargetTime;

        totalPausedTime = 0f;
        isPaused = false;

        stageStartTime = Time.realtimeSinceStartup;
        currentTime = 0f;
        isPlaying = true;

        Debug.Log($"스테이지 시작 | 제한 시간: {limitTime}초 / 목표 시간: {targetTime}초");
    }

    public void PauseStage()
    {
        if (!isPlaying || isPaused) return;

        isPaused = true;
        pauseStartTime = Time.realtimeSinceStartup;

        Debug.Log("스테이지 일시정지");
    }

    public void ResumeStage()
    {
        if (!isPlaying || !isPaused) return;

        totalPausedTime += Time.realtimeSinceStartup - pauseStartTime;
        isPaused = false;

        Debug.Log("스테이지 재시작");
    }

    public void ClearStage(bool isSpecialSeatSuccess)
    {
        isPlaying = false;

        int starCount = CalculateStarCount(isSpecialSeatSuccess);

        Debug.Log($"현재 스테이지 번호: {CurrentStageNumber}");
        Debug.Log($"특수 좌석 성공 여부: {isSpecialSeatSuccess}");
        Debug.Log($"계산된 별 개수: {starCount}");

        SaveStageStar(CurrentStageNumber, starCount);

        StageManager stageManager = FindFirstObjectByType<StageManager>();

        if (stageManager != null)
        {
            stageManager.OpenClearPanel(starCount);
        }

        //SceneManager.LoadScene("ChapterScene");   // 테스트용
    }

    public void NextStage()
    {
        int nextStageNumber = CurrentStageNumber + 1;

        if (nextStageNumber > 5)
        {
            BackToChapter();    // 일단 스테이지 5개만 있어서 그 이상은 '다음으로' 버튼 눌렀을 때 챕터 화면으로 돌아가게 해뒀습니다~
            return;
        }

        SelectStage(nextStageNumber);
    }

    public void EndGame()
    {
        isPlaying = false;

        StageManager stageManager = FindFirstObjectByType<StageManager>();

        if (stageManager != null)
        {
            Debug.Log("StageManager와 연결되었습니다.");
            stageManager.OpenClearPanel(0);
        }
        else
        {
            Debug.Log("StageManager를 찾을 수 없습니다.");
        }

        Debug.Log("스테이지 종료");
    }

    public void RestartGame()   // 나중에 StageScene에서 재시작 버튼 만들고 연결해주면 됨
    {
        if (CurrentStageNumber <= 0)
        {
            CurrentStageNumber = GetStageNumberFromCurrentScene();
        }

        Debug.Log($"[GameManager] {CurrentStageNumber}번 스테이지 재시작");
        SceneManager.LoadScene($"Stage{CurrentStageNumber}");
    }

    private int CalculateStarCount(bool isSpecialSeatSuccess)
    {
        int starCount = 0;

        if (currentTime <= limitTime)
        {
            starCount = 1;
        }
        if (currentTime <= targetTime)
        {
            starCount = 2;
        }
        if (isSpecialSeatSuccess)
        {
            starCount = 3;
        }

        return starCount;
    }

    public void SaveStageStar(int stageNumber, int starCount)
    {
        string key = $"Chapter{CurrentChapterNumber}Stage{stageNumber}Star";

        int saveStar = PlayerPrefs.GetInt(key, 0);

        Debug.Log($"저장 key: {key}");
        Debug.Log($"기존 별: {saveStar}, 새 별: {starCount}");

        if (starCount > saveStar)
        {
            PlayerPrefs.SetInt(key, starCount);
            PlayerPrefs.Save();

            Debug.Log("별 저장 완료");
        }
    }

    public float GetRemainingTimeRatio()
    {
        if (limitTime <= 0)
        {
            return 0f;
        }

        float remainingTime = limitTime - currentTime;
        return Mathf.Clamp01(remainingTime / limitTime);
    }

    private int GetStageNumberFromCurrentScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        string numberText = sceneName.Replace("Stage", "");

        if (int.TryParse(numberText, out int stageNumber))
        {
            return stageNumber;
        }

        Debug.LogError($"현재 씬 이름에서 스테이지 번호를 가져올 수 없습니다: {sceneName}");
        return 1;
    }
}
