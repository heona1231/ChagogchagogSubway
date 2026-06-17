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

        if (!isPlaying) return;

        currentTime = Time.realtimeSinceStartup - stageStartTime;

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

    public void BackToTitle()
    {
        Debug.Log("타이틀 화면으로 돌아가기");
        SceneManager.LoadScene("TitleScene");
    }

    public void SelectStage(int stageNumber)
    {
        CurrentStageNumber = stageNumber;
        Debug.Log($"{stageNumber}번 스테이지로 이동하기");
        SceneManager.LoadScene("StageScene");
    }

    public void StartStage(float stageLimitTime, float stageTargetTime)
    {
        limitTime = stageLimitTime;
        targetTime = stageTargetTime;

        stageStartTime = Time.realtimeSinceStartup;
        currentTime = 0f;
        isPlaying = true;

        Debug.Log($"스테이지 시작 | 제한 시간: {limitTime}초 / 목표 시간: {targetTime}초");
    }

    public void ClearStage(bool isSpecialSeatSuccess)
    {
        isPlaying = false;

        int starCount = CalculateStarCount(isSpecialSeatSuccess);

        Debug.Log($"현재 스테이지 번호: {CurrentStageNumber}");
        Debug.Log($"특수 좌석 성공 여부: {isSpecialSeatSuccess}");
        Debug.Log($"계산된 별 개수: {starCount}");

        SaveStageStar(CurrentStageNumber, starCount);

        SceneManager.LoadScene("ChapterScene");
    }

    public void EndGame()
    {
        isPlaying = false;
        Debug.Log("스테이지 종료");
    }

    public void RestartGame()   // 나중에 StageScene에서 재시작 버튼 만들고 연결해주면 됨
    {
        Debug.Log($"[GameManager] {CurrentStageNumber}번 스테이지 재시작");
        SceneManager.LoadScene("StageScene");
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
}
