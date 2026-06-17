using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MinigameMashClick : MonoBehaviour
{
    [Header("UI")]
    [SerializeField]
    private GameObject minigameCanvas;
    private Slider slider;

    [Header("guage")]
    [SerializeField]
    private float maxGauge = 100f;
    private float autoChargeSpeed = 1f; //초당 게이지 차오르는 속도
    private float clickDecreaseAmount = 5f; //클릭 한 번당 깎이는 양

    [Header("sprite")]
    [SerializeField]
    //key 값인 int 까지는 sprite를 보여줌
    private Dictionary<int, Sprite> spriteTillGauge = new Dictionary<int, Sprite>();

    [Header("clearBlockData")]
    [SerializeField]
    private BlockData clearedBlockData;

    private float currentGauge;
    private bool isGameActive = false;
    private Block block;

    private void Awake()
    {
        block = GetComponent<Block>();
        if(minigameCanvas != null)
        {
            minigameCanvas.SetActive(false);
        }
    }

    //미니게임 시작 함수
    public void StartMinigame()
    {
        if (isGameActive) return;

        isGameActive = true;
        currentGauge = maxGauge;
        
        slider.value = currentGauge;

        minigameCanvas.SetActive(true);
        UpdateSprite();

        StartCoroutine(MinigameLoop());
    }

    private void UpdateSprite()
    {
        foreach(var num in spriteTillGauge.Keys)
        {
            if(currentGauge > num)
            {
                block.ChangeBlockSprite(spriteTillGauge[num]);
            }
        }
    }

    private IEnumerator MinigameLoop()
    {
        while (isGameActive)
        {
            if(Input.GetMouseButtonDown(0))
            {
                currentGauge -= clickDecreaseAmount;
                if(currentGauge < 0)
                {
                    currentGauge = 0;
                }

                UpdateSprite();
            }
            else
            {
                currentGauge += autoChargeSpeed * Time.deltaTime;
                if(currentGauge >maxGauge)
                {
                    currentGauge = maxGauge;
                }

                UpdateSprite();
            }

            slider.value = currentGauge;
            if (currentGauge <= 0)
            {
                Clear();
                yield break;
            }

            yield return null;
        }
    }

    //클리어시 블럭 변경
    private void Clear()
    {
        isGameActive = false;

        block.Initialize(clearedBlockData);
        minigameCanvas.SetActive(false);
        
        this.enabled = false;
    }
}
