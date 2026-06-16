// 박세은 작성
using UnityEngine;

[System.Serializable]
public class BlockSpawnData
{
    public GameObject blockPrefab;
    public Vector3 spawnPosition;
    public Vector3 spawnRotation;
}

[CreateAssetMenu(fileName = "StageData", menuName = "Stage/StageData")]
public class StageData : ScriptableObject
{
    public int chapterNumber = 1;
    public int stageNumber = 1;

    [Header("Score Time Setting")]
    public float limitTime = 60f;
    public float targetTime = 30f;

    [Header("Stage Layout")]
    public GameObject boardPrefab;
    public BlockSpawnData[] blockSpawnDatas;

    //public BlockAnswerData[] blockAnswers;
}
