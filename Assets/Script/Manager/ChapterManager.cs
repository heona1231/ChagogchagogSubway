// 박세은 작성
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChapterManager : MonoBehaviour
{
    [SerializeField] private TMP_Text totalStarText;

    [Header("Stage")]
    [SerializeField] private Transform stageGrid;
    [SerializeField] private Transform starGrid;

    [Header("StageSprites")]
    [SerializeField] private Sprite stageOpenSprite;
    [SerializeField] private Sprite stageLockSprite;

    [Header("Star Sprites")]
    [SerializeField] private Sprite star1Sprite;
    [SerializeField] private Sprite star2Sprite;
    [SerializeField] private Sprite star3Sprite;

    private const int TOTAL_STAGE_COUNT = 15;
    private const int MAX_STAR_PER_STAGE = 3;

    private void Start()
    {
        UpdateTotalStarText();
        UpdateStageStarIcons();
        UpdateStageLockIcons();
    }

    private void UpdateTotalStarText()
    {
        int totalStar = 0;
        int chapterNumber = GameManager.Instance.CurrentChapterNumber;

        for (int i =1; i <= TOTAL_STAGE_COUNT; i++)
        {
            totalStar += PlayerPrefs.GetInt($"Chapter{chapterNumber}Stage{i}Star", 0);
        }

        int maxStar = TOTAL_STAGE_COUNT * MAX_STAR_PER_STAGE;

        totalStarText.text = totalStar + "/" + maxStar;
    }

    private void UpdateStageStarIcons()
    {
        for (int i = 1; i <= TOTAL_STAGE_COUNT; i++)
        {
            int chapterNumber = GameManager.Instance.CurrentChapterNumber;

            int starCount = PlayerPrefs.GetInt($"Chapter{chapterNumber}Stage{i}Star", 0);

            Image starImage = starGrid.GetChild(i - 1).GetComponent<Image>();

            if (starCount <= 0)
            {
                starImage.gameObject.SetActive(false);
            }
            else
            {
                starImage.gameObject.SetActive(true);

                if (starCount == 1)
                {
                    starImage.sprite = star1Sprite;
                }
                else if (starCount == 2)
                {
                    starImage.sprite = star2Sprite;
                }
                else if (starCount == 3)
                {
                    starImage.sprite = star3Sprite;
                }
            }
        }
    }

    private void UpdateStageLockIcons()
    {
        int chapterNumber = GameManager.Instance.CurrentChapterNumber;

        for (int i = 1; i <= TOTAL_STAGE_COUNT; i++)
        {
            bool isUnlocked = i == 1 || PlayerPrefs.GetInt($"Chapter{chapterNumber}Stage{i - 1}Star", 0) > 0;

            Image stageImage = stageGrid.GetChild(i - 1).GetComponent<Image>();

           if (isUnlocked)
            {
                stageImage.sprite = stageOpenSprite;
                stageImage.raycastTarget = true;
            }
            else
            {
                stageImage.sprite = stageLockSprite;
                stageImage.raycastTarget = true;
            }
        }
    }
}
