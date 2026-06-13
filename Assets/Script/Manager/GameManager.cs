// 박세은 작성
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int CurrentStageNumber { get; private set; }

    [Header("Stage Time")]
    [SerializeField] private float limitTime = 60f;

    private float currentTime;
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
        if (!isPlaying)
        {
            return;
        }

        currentTime += Time.deltaTime;

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

    public void StartStage(float stageLimitTime)
    {
        limitTime = stageLimitTime;
        currentTime = 0f;
        isPlaying = true;

        Debug.Log($"스테이지 시작 / 제한 시간: {limitTime}초");
    }

    public void EndGame()
    {
        isPlaying = false;
        Debug.Log("스테이지 종료");
    }

    public void RestartGame()   // 나중에 StageScene에서 재시작 버튼 만들고 연결해주면 됨
    {
        Debug.Log($"{CurrentStageNumber}번 스테이지 재시작");
        SceneManager.LoadScene("StageScene");
    }
}
