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
    private bool canGoNextStage;

    public bool IsPaused => isPaused;

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
        // 박세은 수정: 키 바인딩을 Player.cs에서 사용해야 하므로 주석 처리(영향X)
        //if (isPlaying && Input.GetKeyDown(KeyCode.R))
        //{
        //    RestartGame();
        //}

        if (canGoNextStage && Input.GetKeyDown(KeyCode.C))
        {
            canGoNextStage = false;
            NextStage();
            return;
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

    public void SelectStage(int chapterNumber, int stageNumber)
    {
        CurrentChapterNumber = chapterNumber;
        CurrentStageNumber = stageNumber;

        string sceneName = $"Chapter{CurrentChapterNumber}Stage{CurrentStageNumber}";

        Debug.Log($"챕터 {CurrentChapterNumber} " + $"스테이지 {CurrentStageNumber}로 이동하기");

        SceneManager.LoadScene(sceneName);
    }

    public void StartStage(float stageLimitTime, float stageTargetTime)
    {
        //if (CurrentStageNumber <= 0)
        //{
        //    CurrentStageNumber = GetStageNumberFromCurrentScene();
        //}
        // ㄴ 챕터 번호까지 읽어야 하므로 아래와 같이 변경
        SetChapterAndStageFromCurrentSceneIfNeeded();

        limitTime = stageLimitTime;
        targetTime = stageTargetTime;

        totalPausedTime = 0f;
        isPaused = false;
        canGoNextStage = false;

        stageStartTime = Time.realtimeSinceStartup;
        currentTime = 0f;
        isPlaying = true;
        
        Debug.Log(
            $"챕터 {CurrentChapterNumber} " +
            $"스테이지 {CurrentStageNumber} 시작 | " +
            $"제한 시간: {limitTime}초 / 목표 시간: {targetTime}초"
        );
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
        canGoNextStage = true;

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

        SelectStage(CurrentChapterNumber, nextStageNumber);
    }

    public void EndGame()
    {
        isPlaying = false;
        canGoNextStage = false;

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
        string currentSceneName = SceneManager.GetActiveScene().name;

        Debug.Log($"[GameManager] 챕터 {CurrentStageNumber} " + $"스테이지 {CurrentStageNumber} 재시작");
        SceneManager.LoadScene(currentSceneName);
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
        if (isSpecialSeatSuccess && currentTime <= targetTime)
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

    private void SetChapterAndStageFromCurrentSceneIfNeeded()
    {
        if (CurrentChapterNumber > 0 && CurrentStageNumber > 0)
        {
            return;
        }

        string sceneName = SceneManager.GetActiveScene().name;

        // 저장 예시: Chapter1Stage2
        int chapterIndex = sceneName.IndexOf("Chapter");
        int stageIndex = sceneName.IndexOf("Stage");

        if (chapterIndex < 0 || stageIndex < 0)
        {
            Debug.LogError($"씬 이름에서 챕터/스테이지 번호를 찾을 수 없습니다: {sceneName}");
            return;
        }

        string chapterText = sceneName.Substring(
            chapterIndex + "Chapter".Length,
            stageIndex - (chapterIndex + "Chapter".Length)
            );
        string stageText = sceneName.Substring(
            stageIndex + "Stage".Length
            );

        bool chapterParsed = int.TryParse(chapterText, out int chapterNumber);
        bool stageParsed = int.TryParse(stageText, out int stageNumber);

        if (!chapterParsed || !stageParsed)
        {
            Debug.LogError($"씬 이름에서 숫자 변환에 실패했습니다: {sceneName}");
            return;
        }

        CurrentChapterNumber = chapterNumber;
        CurrentStageNumber = stageNumber;

        Debug.Log($"씬 이름에서 번호 설정 완료: "
            + $"챕터 {CurrentChapterNumber}, "
            + $"스테이지 {CurrentStageNumber}"
            );
    }
}
