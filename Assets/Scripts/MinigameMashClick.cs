//서현아 작성

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MinigameMashClick : MonoBehaviour
{
    [Header("guage")]
    [SerializeField] private float maxGauge = 100f;
    [SerializeField] private float autoChargeSpeed = 1f; //초당 게이지 차오르는 속도
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
        currentGauge = maxGauge;
        
        MinigameManager.Instance.StartMashClickMinigame(this.transform.position, maxGauge);
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
            if(Input.GetMouseButtonDown(0))
            {
                currentGauge -= clickDecreaseAmount;
                if(currentGauge < 0)
                {
                    currentGauge = 0;
                }
            }
            else
            {
                currentGauge += autoChargeSpeed * Time.deltaTime;
                if(currentGauge >maxGauge)
                {
                    currentGauge = maxGauge;
                }
            }

            MinigameManager.Instance.ReceiveCurrentGauge(currentGauge);
            UpdateSprite();

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
        MinigameManager.Instance.EndMinigame();

        this.enabled = false;
    }
}
