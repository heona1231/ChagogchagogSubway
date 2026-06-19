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
}

public class Board : MonoBehaviour
{
    public static Board Main { get; private set; }
    public static Board Background { get; private set; }

    [SerializeField] private BoardType type;

    [SerializeField] private float gridSize = 1.32f;
    [SerializeField] private Vector2 boardOffset = Vector2.zero;

    [SerializeField] private string[] boardShape = new string[] { };
    private int columns; // 보드의 가로 칸 수
    private int rows;    // 보드의 세로 칸 수

    private bool[,] isPlayableCell; // 구멍(X)인지 정상 칸(O)인지 판별하는 배열
    private Block[,] occupiedCells; // 보드의 어느 칸이 채워져 있는지 기억하는 2차원 배열
    [SerializeField] private List<SpecialSeat> specialSeats = new List<SpecialSeat>(); // 특수좌석 위치

    private void Awake()
    {
        if (type == BoardType.Main) Main = this;
        else if (type == BoardType.Background) Background = this;

        // 문자열 배열에 맞춰 가로/세로 크기 계산
        rows = boardShape.Length;
        columns = rows > 0 ? boardShape[0].Length : 0;

        occupiedCells = new Block[columns, rows];
        isPlayableCell = new bool[columns, rows];

        // 인스펙터에 적힌 O, X를 분석하여 배열에 세팅
        for (int y = 0; y < rows; y++)
        {
            // 인스펙터의 첫 번째 줄(index 0)이 시각적으로 맨 위쪽(가장 큰 y)이 되도록 역순 매핑
            string rowStr = boardShape[rows - 1 - y];
            for (int x = 0; x < columns; x++)
            {
                if (x < rowStr.Length && (rowStr[x] == 'O' || rowStr[x] == 'o'))
                {
                    isPlayableCell[x, y] = true;  // O면 배치 가능한 자리
                }
                else
                {
                    isPlayableCell[x, y] = false; // X면 뚫려있는 빈 공간
                }
            }
        }
    }

    // 보드의 왼쪽 아래 칸의 월드 좌표를 계산
    private Vector2 GetBottomLeftOrigin()
    {
        Vector2 center = (Vector2)transform.position + boardOffset;

        float startX = center.x - (columns - 1) * gridSize / 2f;
        float startY = center.y - (rows - 1) * gridSize / 2f;

        return new Vector2(startX, startY);
    }

    // 위치를 보드 안쪽으로 강제 제한하여 반환
    public Vector2 GetSnappedPosition(Vector2 dropPosition, Vector2 blockOffset)
    {
        Vector2 origin = GetBottomLeftOrigin();

        // 1. 블록의 0,0 기준 위치 계산
        Vector2 basePos = dropPosition - blockOffset;

        // 2. 가장 가까운 그리드 인덱스 찾기
        int gridX = Mathf.RoundToInt((basePos.x - origin.x) / gridSize);
        int gridY = Mathf.RoundToInt((basePos.y - origin.y) / gridSize);

        // 3. 보드 범위 내로 제한
        gridX = Mathf.Clamp(gridX, 0, columns - 1);
        gridY = Mathf.Clamp(gridY, 0, rows - 1);

        // 4. 스냅된 월드 좌표 반환
        return new Vector2(origin.x + gridX * gridSize, origin.y + gridY * gridSize) + blockOffset;
    }

    // 해당 블럭이 보드 안쪽인지 체크
    public bool IsValidPlacement(Vector2 position, Vector2 blockOffset, Vector2Int[] shapeCells)
    {
        if (shapeCells == null) return false;

        Vector2 origin = GetBottomLeftOrigin();
        Vector2 basePos = position - blockOffset;

        // 1. 그리드 인덱스 계산 (Floor 대신 Round를 사용하여 그리드 정중앙 정렬 유도)
        int baseGridX = Mathf.RoundToInt((basePos.x - origin.x) / gridSize);
        int baseGridY = Mathf.RoundToInt((basePos.y - origin.y) / gridSize);

        // 2. 블록을 구성하는 모든 칸의 위치를 검사
        foreach (Vector2Int cellOffset in shapeCells)
        {
            int checkX = baseGridX + cellOffset.x;
            int checkY = baseGridY + cellOffset.y;

            // 단 한 칸이라도 보드 범위를 벗어나면 설치 불가
            if (checkX < 0 || checkX >= columns || checkY < 0 || checkY >= rows)
            {
                Debug.Log("[배치 실패] 범위를 벗어남");
                return false;
            }

            // 해당 칸이 뚫려있는(X) 칸이면 설치 불가
            if (!isPlayableCell[checkX, checkY])
            {
                Debug.Log("[배치 실패] 비활성 칸(X)");
                return false;
            }

            // 다른 블록이 이미 채워져 있으면 설치 불가
            if (occupiedCells[checkX, checkY] != null)
            {
                Debug.Log("[배치 실패] 이미 블록 있음");
                return false;
            }
        }

        Debug.Log("[배치 성공]");
        return true;
    }

    // 블록을 구성하는 칸 중 하나라도 보드 안쪽 인덱스에 들어온다면 true 반환
    public bool IsOverlappingBoard(Vector2 position, Vector2 blockOffset, Vector2Int[] shapeCells)
    {
        if (shapeCells == null) return false;

        Vector2 origin = GetBottomLeftOrigin();
        Vector2 basePos = position - blockOffset;

        int baseGridX = Mathf.RoundToInt((basePos.x - origin.x) / gridSize);
        int baseGridY = Mathf.RoundToInt((basePos.y - origin.y) / gridSize);

        foreach (Vector2Int cellOffset in shapeCells)
        {
            int checkX = baseGridX + cellOffset.x;
            int checkY = baseGridY + cellOffset.y;

            if (checkX >= 0 && checkX < columns && checkY >= 0 && checkY < rows)
            {
                if (isPlayableCell[checkX, checkY]) return true;
            }
        }
        return false;
    }

    // 블록이 놓일 때 해당 칸들을 true으로 변경
    public void PlaceBlock(Block block, Vector2 position, Vector2 blockOffset, Vector2Int[] shapeCells)
    {
        if (shapeCells == null) return;

        Vector2 origin = GetBottomLeftOrigin();
        Vector2 basePos = position - blockOffset;

        int baseGridX = Mathf.RoundToInt((basePos.x - origin.x) / gridSize);
        int baseGridY = Mathf.RoundToInt((basePos.y - origin.y) / gridSize);

        foreach (Vector2Int cellOffset in shapeCells)
        {
            int checkX = baseGridX + cellOffset.x;
            int checkY = baseGridY + cellOffset.y;

            if (checkX >= 0 && checkX < columns && checkY >= 0 && checkY < rows)
            {
                occupiedCells[checkX, checkY] = block;
            }
        }
    }

    // 블록을 들어올릴 때 해당 칸들을 false으로 변경
    public void RemoveBlock(Vector2 position, Vector2 blockOffset, Vector2Int[] shapeCells)
    {
        if (shapeCells == null) return;

        Vector2 origin = GetBottomLeftOrigin();
        Vector2 basePos = position - blockOffset;

        int baseGridX = Mathf.RoundToInt((basePos.x - origin.x) / gridSize);
        int baseGridY = Mathf.RoundToInt((basePos.y - origin.y) / gridSize);

        foreach (Vector2Int cellOffset in shapeCells)
        {
            int checkX = baseGridX + cellOffset.x;
            int checkY = baseGridY + cellOffset.y;

            if (checkX >= 0 && checkX < columns && checkY >= 0 && checkY < rows)
            {
                occupiedCells[checkX, checkY] = null;
            }
        }
    }

    // 모든 특수 좌석 조건이 만족되었는지 체크하여 bool을 반환하는 함수
    public bool CheckAllSpecialSeatsSatisfied()
    {
        if (specialSeats == null || specialSeats.Count == 0) return true;

        foreach (SpecialSeat seat in specialSeats)
        {
            int x = seat.gridIndex.x;
            int y = seat.gridIndex.y;

            Block occupant = occupiedCells[x, y];
            if (occupant == null || occupant.CurrentType != seat.requiredType) return false;
        }

        Debug.Log("특수 좌석 배치 완료");
        return true;
    }

    private void OnDrawGizmos()
    {
        if (boardShape == null || boardShape.Length == 0) return;

        int r = boardShape.Length;
        int c = boardShape[0].Length;
        Vector2 center = (Vector2)transform.position + boardOffset;
        Vector2 origin = new Vector2(center.x - (c - 1) * gridSize / 2f, center.y - (r - 1) * gridSize / 2f);

        if (specialSeats != null)
        {
            foreach (var seat in specialSeats)
            {
                Vector2 seatPos = origin + new Vector2(seat.gridIndex.x * gridSize, seat.gridIndex.y * gridSize);
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(seatPos, gridSize * 0.4f);
            }
        }
    }
}