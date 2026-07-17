//서현아 작성

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MinigameMashClick : MonoBehaviour
{
    [Header("guage")]
    [SerializeField] private float maxGauge = 100f;
    [SerializeField] private float autoChargeSpeed = 6f; //초당 게이지 차오르는 속도
    [SerializeField] private float clickDecreaseAmount = 5f; //클릭 한 번당 깎이는 양

    [Header("sprite")]
    [SerializeField]
    //key 값인 int 까지는 sprite를 보여줌
    private List<GaugeSpritePair> myGaugeSprites = new List<GaugeSpritePair>();

    [Header("clearBlockData")]
    [SerializeField]
    private BlockData clearedBlockData;

    private float currentGauge;
    private bool isGameActive = false;
    private Block block;
    private float fullGaugeTimer;

    private void Awake()
    {
        block = GetComponent<Block>();
    }

    //미니게임 세팅
    public void SetMinigameBlock(BlockDataMinigame inputBlockData)
    {
        clearedBlockData = inputBlockData.clearedBlockData;
        myGaugeSprites = inputBlockData.gaugeSprites;
    }

    //미니게임 시작 함수
    public void StartMinigame()
    {
        if (isGameActive) return;

        isGameActive = true;
        currentGauge = 80;
        
        MinigameManager.Instance.StartMashClickMinigame(this, this.transform.position, maxGauge);
        UpdateSprite();
        StartCoroutine(MinigameLoop());
    }

    private void UpdateSprite()
    {
        foreach (var pair in myGaugeSprites)
        {
            if (currentGauge > pair.gaugeValue)
            {
                block.ChangeBlockSprite(pair.sprite);
                break;
            }
        }
    }

    private IEnumerator MinigameLoop()
    {
        while (isGameActive)
        {
            currentGauge += autoChargeSpeed * Time.deltaTime;
            if (currentGauge > maxGauge)
            {
                currentGauge = maxGauge;
            }

            if (Input.GetMouseButtonDown(0))
            {
                currentGauge -= clickDecreaseAmount;
                if(currentGauge < 0)
                {
                    currentGauge = 0;
                }
            }

            MinigameManager.Instance.ReceiveCurrentGauge(currentGauge);
            UpdateSprite();

            if (currentGauge >= maxGauge)
            {
                fullGaugeTimer += Time.deltaTime;

                if (fullGaugeTimer >= 1f)
                {
                    ResetMinigame();
                    MinigameManager.Instance.EndMinigame();
                    yield break;
                }
            }
            else
            {
                fullGaugeTimer = 0f;
            }

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

        if (block.currentBoard != null)
        {
            block.currentBoard.RemoveBlock(
                block.transform.position,
                block.shapeOffset,
                block.shapeCells
            );
        }

        block.Initialize(clearedBlockData);

        if (block.currentBoard != null)
        {
            block.currentBoard.PlaceBlock(
                block,
                block.transform.position,
                block.shapeOffset,
                block.shapeCells
            );
        }

        MinigameManager.Instance.EndMinigame();

        this.enabled = false;
    }

    //미니게임 리셋
    public void ResetMinigame()
    {
        StopAllCoroutines();
        isGameActive = false;

        currentGauge = maxGauge;

        UpdateSprite();
    }
}
