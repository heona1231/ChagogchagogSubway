// 강혜원 작성

using System.Collections.Generic;
using UnityEngine;

public enum BoardType
{
    Main,       // 게임 보드
    Background  // 배경 보드
}

// 특수 좌석 설정을 위한 구조체
[System.Serializable]
public struct SpecialSeat
{
    public Vector2Int gridIndex;       // 좌석의 (X, Y) 인덱스 위치
    public PassengerType requiredType; // 이 자리에 앉아야 하는 승객 타입
    public BlockDirection initialDirection; // 의자의 초기 방향 설정
}

[CreateAssetMenu(fileName = "BoardData", menuName = "Scriptable Objects/BoardData")]
public class BoardData : ScriptableObject
{
    public float gridSize = 1.32f;

    public BoardType type;
    public string[] boardShape; // 숫자로 이루어진 보드 모양 0: 빈 공간, 1: 일반 타일, 2: 일반석, 3: 노약자석, 4: 임산부석
    public Vector2 boardOffset = Vector2.zero;

    public List<SpecialSeat> specialSeats = new List<SpecialSeat>();

    public GameObject tilePrefab; // 게임 보드 일반 타일 프리팹
    public GameObject[] chairPrefabs; // 게임 보드 의자 프리팹
    public Vector2 tileOffset = Vector2.zero;
}
