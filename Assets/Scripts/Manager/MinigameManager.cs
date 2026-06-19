//서현아 작성

using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager Instance { get; private set; }

    [Header("Minigame UI Prefab")]
    [SerializeField] private GameObject mashClickUiPrefab;

    private GameObject currentUiInstance;
    private MinigameUi currentUiScript;

    private void Awake()
    {
        Instance = this;
    }

    //미니게임 시작 시, 미니게임스크립트에서 호출할 함수
    public void StartMashClickMinigame(Vector3 position, float maxGauge)
    {
        // 이미 미니게임 UI가 켜져 있다면 중복 실행 방지
        if (currentUiInstance != null) return;

        currentUiInstance = Instantiate(mashClickUiPrefab, position, Quaternion.identity);
        if (currentUiInstance.TryGetComponent<MinigameUi>(out currentUiScript))
        {
            currentUiScript.SetupSlider(maxGauge);
        }
    }

    //미니게임 진행 시, 미니게임스크립트에서 호출할 함수
    public void ReceiveCurrentGauge(float currentValue)
    {
        if (currentUiScript != null)
        {
            currentUiScript.UpdateSlider(currentValue);
        }
    }

    //미니게임 끝날 시, 미니게임스크립트에서 호출할 함수
    public void EndMinigame()
    {
        if (currentUiInstance != null)
        {
            Destroy(currentUiInstance);

            currentUiInstance = null;
            currentUiScript = null;
        }
    }
}