// 박세은 작성

using UnityEngine;
using UnityEngine.UI;

public class StageSlotUI : MonoBehaviour
{
    [Header("Stage")]
    [SerializeField] private StageSelectButton stageSelectButton;
    [SerializeField] private Button stageButton;
    [SerializeField] private Image stageImage;

    [Header("Star")]
    [SerializeField] private Image starImage;

    private void Awake()
    {
        // 스테이지 클릭 이벤트 연결
        stageButton.onClick.AddListener(
            stageSelectButton.SelectStage
        );

        // 별 이미지가 버튼 클릭을 방해하지 않도록 설정
        starImage.raycastTarget = false;
    }

    public void Setup(
        int chapterNumber,
        int stageNumber,
        int starCount,
        bool isUnlocked,
        Sprite openSprite,
        Sprite lockSprite,
        Sprite star0Sprite,
        Sprite star1Sprite,
        Sprite star2Sprite,
        Sprite star3Sprite)
    {
        // 챕터와 스테이지 번호 설정
        stageSelectButton.Initialize(chapterNumber, stageNumber);

        // 스테이지 잠금 상태 설정
        stageImage.sprite = isUnlocked
            ? openSprite
            : lockSprite;

        stageButton.interactable = isUnlocked;

        // 별 개수에 따른 이미지 설정
        starCount = Mathf.Clamp(starCount, 0, 3);


        if (!isUnlocked)
        {
            // 잠긴 스테이지는 별 이미지 숨기기
            starImage.gameObject.SetActive(false);
        }
        else
        {
            // 열린 스테이지는 별 이미지 표시
            starImage.gameObject.SetActive(true);

            if (starCount == 0)
                starImage.sprite = star0Sprite;
            else if (starCount == 1)
                starImage.sprite = star1Sprite;
            else if (starCount == 2)
                starImage.sprite = star2Sprite;
            else if (starCount == 3)
                starImage.sprite = star3Sprite;
        }

    }
}
