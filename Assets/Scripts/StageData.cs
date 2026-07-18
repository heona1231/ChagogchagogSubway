// 박세은 작성
using UnityEngine;

[System.Serializable]
public class BlockSpawnData
{
    public BlockData blockDataPrefab;
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
    // 강혜원 작성, 프리팹 설정이 아닌 board 데이터 설정으로 수정
    public BoardData bgBoardData; 
    public BoardData gameBoardData;

    public BlockSpawnData[] blockSpawnDatas;

    //public BlockAnswerData[] blockAnswers;
}
