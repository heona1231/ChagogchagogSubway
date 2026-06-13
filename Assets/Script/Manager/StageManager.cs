// 박세은 작성
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] private float limitTime = 60f;

    private void Start()
    {
        GameManager gameManager = FindAnyObjectByType<GameManager>();

        gameManager.StartStage(limitTime);
    }

    public void SaveStageStar(int stageNumber, int starCount)
    {
        string key = "Chapter1Stage" + stageNumber + "Star";

        int savedStar = PlayerPrefs.GetInt(key, 0);

        if (starCount > savedStar)
        {
            PlayerPrefs.SetInt(key, starCount);
            PlayerPrefs.Save();
        }
    }
}
