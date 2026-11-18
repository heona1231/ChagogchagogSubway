// 박세은 작성
// 박세은 수정: StageSlot Prefab으로 변경하면서, 코드 싹 갈아엎었습니당


using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChapterManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text totalStarText;

    [Header("Chapter Navigation")]
    [SerializeField] private TMP_Text chapterNumberText;

    [Header("Stage Grid")]
    [SerializeField] private Transform stageGrid;
    [SerializeField] private StageSlotUI stageSlotPrefab;

    [Header("Stage Count")]
    [SerializeField] private int totalStageCount = 30;
    [SerializeField] private int stagesPerPage = 15;

    [Header("Stage Sprites")]
    [SerializeField] private Sprite stageOpenSprite;
    [SerializeField] private Sprite stageLockSprite;

    [Header("Star Sprites")]
    [SerializeField] private Sprite star0Sprite;
    [SerializeField] private Sprite star1Sprite;
    [SerializeField] private Sprite star2Sprite;
    [SerializeField] private Sprite star3Sprite;

    private const int MAX_STAR_PER_STAGE = 3;

    // 현재 페이지 (0 = 1~15, 1 = 16~30)
    private int currentPage = 0;

    private int displayedChapter = -1;

    private readonly List<StageSlotUI> spawnedSlots =
        new List<StageSlotUI>();

    private void Start()
    {
        RefreshCurrentChapter();
    }

    private void OnEnable()
    {
        if (displayedChapter >= 0)
        {
            RefreshCurrentChapter();
        }
    }

    private void Update()
    {
        if (GameManager.Instance == null)
            return;

        int currentChapter =
            GameManager.Instance.CurrentChapterNumber;

        // 실제 챕터가 바뀌었을 때 첫 페이지로 이동
        if (displayedChapter != currentChapter)
        {
            currentPage = 0;
            RefreshCurrentChapter();
        }
    }

    // 오른쪽 화살표
    public void NextPage()
    {
        ChangePage(1);
    }

    // 왼쪽 화살표
    public void PreviousPage()
    {
        ChangePage(-1);
    }

    // 기존 ChapterArrow.cs와의 호환성 유지
    public void NextChapter()
    {
        NextPage();
    }

    public void PreviousChapter()
    {
        PreviousPage();
    }

    private void ChangePage(int amount)
    {
        int pageCount = Mathf.CeilToInt(
            (float)totalStageCount / stagesPerPage
        );

        int newPage = Mathf.Clamp(
            currentPage + amount,
            0,
            pageCount - 1
        );

        if (newPage == currentPage)
            return;

        currentPage = newPage;

        Debug.Log($"페이지 {currentPage + 1}로 이동");

        RefreshCurrentChapter();
    }

    public void RefreshCurrentChapter()
    {
        if (GameManager.Instance == null)
            return;

        int chapterNumber =
            GameManager.Instance.CurrentChapterNumber;

        ShowChapter(chapterNumber);
    }

    private void ShowChapter(int chapterNumber)
    {
        if (stageGrid == null ||
            stageSlotPrefab == null ||
            totalStarText == null)
        {
            Debug.LogError("ChapterManager UI 연결 확인!");
            return;
        }

        // 기존 슬롯 제거
        foreach (StageSlotUI slot in spawnedSlots)
        {
            if (slot != null)
            {
                slot.gameObject.SetActive(false);
                Destroy(slot.gameObject);
            }
        }

        spawnedSlots.Clear();

        // 현재 페이지에 표시할 스테이지 범위
        int startStage =
            currentPage * stagesPerPage + 1;

        int endStage = Mathf.Min(
            startStage + stagesPerPage - 1,
            totalStageCount
        );

        // 현재 페이지의 스테이지 15개 생성
        for (int i = startStage; i <= endStage; i++)
        {
            int starCount = PlayerPrefs.GetInt(
                $"Chapter{chapterNumber}Stage{i}Star", 0
            );

            starCount = Mathf.Clamp(
                starCount, 0, MAX_STAR_PER_STAGE
            );

            bool isUnlocked =
                i == 1 ||
                PlayerPrefs.GetInt(
                    $"Chapter{chapterNumber}Stage{i - 1}Star", 0
                ) > 0;

            StageSlotUI slot = Instantiate(
                stageSlotPrefab,
                stageGrid
            );

            slot.gameObject.name = $"StageSlot_{i}";

            slot.Setup(
                chapterNumber,
                i,
                starCount,
                isUnlocked,
                stageOpenSprite,
                stageLockSprite,
                star0Sprite,
                star1Sprite,
                star2Sprite,
                star3Sprite
            );

            spawnedSlots.Add(slot);
        }


        // 현재 페이지의 별 개수만 계산
        int totalStar = 0;

        for (int i = startStage; i <= endStage; i++)
        {
            totalStar += Mathf.Clamp(
                PlayerPrefs.GetInt(
                    $"Chapter{chapterNumber}Stage{i}Star", 0
                ),
                0,
                MAX_STAR_PER_STAGE
            );
        }

        // 현재 페이지의 최대 별 개수
        int stageCountOnPage = endStage - startStage + 1;
        int maxStar = stageCountOnPage * MAX_STAR_PER_STAGE;

        totalStarText.text = totalStar + "/" + maxStar;


        // 챕터 번호 표시
        if (chapterNumberText != null)
        {
            chapterNumberText.text =
                chapterNumber.ToString();
        }

        displayedChapter = chapterNumber;

        Debug.Log(
            $"챕터 {chapterNumber} | " +
            $"페이지 {currentPage + 1} | " +
            $"스테이지 {startStage}~{endStage}"
        );
    }
}
