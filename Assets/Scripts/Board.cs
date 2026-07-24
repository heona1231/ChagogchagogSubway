// 강혜원 작성

using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board Main { get; private set; }
    public static Board Background { get; private set; }

    [SerializeField] public BoardData boardData;

    private int columns; // 보드의 가로 칸 수
    private int rows;    // 보드의 세로 칸 수

    private bool[,] isPlayableCell; // 구멍(X)인지 정상 칸(O)인지 판별하는 배열
    private Block[,] occupiedCells; // 보드의 어느 칸이 채워져 있는지 기억하는 2차원 배열

    private void Awake()
    {
        Initialize(boardData); // stageData 사용 안 할 때만 사용
    }

    // 외부에서 호출하여 boardData를 설정하고 초기화하는 함수
    public void Initialize(BoardData data)
    {
        this.boardData = data;

        if (boardData.type == BoardType.Main) Main = this;
        else if (boardData.type == BoardType.Background) Background = this;

        InitializeBoard();
    }

    private void InitializeBoard()
    {
        if (boardData == null) return;

        // 문자열 배열에 맞춰 가로/세로 크기 계산
        rows = boardData.boardShape.Length;
        columns = rows > 0 ? boardData.boardShape[0].Length : 0;

        occupiedCells = new Block[columns, rows];
        isPlayableCell = new bool[columns, rows];

        Vector2 origin = GetBottomLeftOrigin();

        // 인스펙터에 적힌 O, X를 분석하여 배열에 세팅
        for (int y = 0; y < rows; y++)
        {
            // 인스펙터의 첫 번째 줄(index 0)이 시각적으로 맨 위쪽(가장 큰 y)이 되도록 역순 매핑
            string rowStr = boardData.boardShape[rows - 1 - y];
            for (int x = 0; x < columns; x++)
            {
                if (x < rowStr.Length && (rowStr[x] == '1' || rowStr[x] == '2'))
                {
                    isPlayableCell[x, y] = true;  // 1 또는 2이면 배치 가능한 자리

                    // Main 보드일 경우 타일 생성
                    if (boardData.type == BoardType.Main && boardData.tilePrefab != null)
                    {
                        Vector2 pos = origin + new Vector2(x * boardData.gridSize, y * boardData.gridSize) + boardData.tileOffset;

                        // 1과 2에 따라 생성할 프리팹 결정
                        GameObject prefabToInstantiate = null;
                        if (rowStr[x] == '1')
                        {
                            prefabToInstantiate = boardData.tilePrefab; // 일반 타일 프리팹
                        }
                        else if (rowStr[x] == '2')
                        {
                            prefabToInstantiate = boardData.chairPrefab; // 의자 프리팹
                        }

                        // 프리팹이 할당되어 있다면 생성
                        if (prefabToInstantiate != null)
                        {
                            Instantiate(prefabToInstantiate, pos, Quaternion.identity, transform);
                        }
                    }
                }
                else
                {
                    isPlayableCell[x, y] = false; // 0이면 뚫려있는 빈 공간
                }
            }
        }
    }

    // 보드의 왼쪽 아래 칸의 월드 좌표를 계산
    private Vector2 GetBottomLeftOrigin()
    {
        Vector2 center = (Vector2)transform.position + boardData.boardOffset;

        float startX = center.x - (columns - 1) * boardData.gridSize / 2f;
        float startY = center.y - (rows - 1) * boardData.gridSize / 2f;

        return new Vector2(startX, startY);
    }

    // 위치를 보드 안쪽으로 강제 제한하여 반환
    public Vector2 GetSnappedPosition(Vector2 dropPosition, Vector2 blockOffset)
    {
        Vector2 origin = GetBottomLeftOrigin();

        // 1. 블록의 0,0 기준 위치 계산
        Vector2 basePos = dropPosition - blockOffset;

        // 2. 가장 가까운 그리드 인덱스 찾기
        int gridX = Mathf.RoundToInt((basePos.x - origin.x) / boardData.gridSize);
        int gridY = Mathf.RoundToInt((basePos.y - origin.y) / boardData.gridSize);

        // 3. 보드 범위 내로 제한
        gridX = Mathf.Clamp(gridX, 0, columns - 1);
        gridY = Mathf.Clamp(gridY, 0, rows - 1);

        // 4. 스냅된 월드 좌표 반환
        return new Vector2(origin.x + gridX * boardData.gridSize, origin.y + gridY * boardData.gridSize) + blockOffset;
    }

    // 해당 블럭이 보드 안쪽인지 체크
    public bool IsValidPlacement(Vector2 position, Vector2 blockOffset, Vector2Int[] shapeCells)
    {
        if (shapeCells == null) return false;

        Vector2 origin = GetBottomLeftOrigin();
        Vector2 basePos = position - blockOffset;

        // 1. 그리드 인덱스 계산 (Floor 대신 Round를 사용하여 그리드 정중앙 정렬 유도)
        int baseGridX = Mathf.RoundToInt((basePos.x - origin.x) / boardData.gridSize);
        int baseGridY = Mathf.RoundToInt((basePos.y - origin.y) / boardData.gridSize);

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

        int baseGridX = Mathf.RoundToInt((basePos.x - origin.x) / boardData.gridSize);
        int baseGridY = Mathf.RoundToInt((basePos.y - origin.y) / boardData.gridSize);

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

        int baseGridX = Mathf.RoundToInt((basePos.x - origin.x) / boardData.gridSize);
        int baseGridY = Mathf.RoundToInt((basePos.y - origin.y) / boardData.gridSize);

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

        int baseGridX = Mathf.RoundToInt((basePos.x - origin.x) / boardData.gridSize);
        int baseGridY = Mathf.RoundToInt((basePos.y - origin.y) / boardData.gridSize);

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
        if (boardData.specialSeats == null || boardData.specialSeats.Count == 0) return true;

        foreach (SpecialSeat seat in boardData.specialSeats)
        {
            int x = seat.gridIndex.x;
            int y = seat.gridIndex.y;

            Block occupant = occupiedCells[x, y];
            if (occupant == null || occupant.CurrentType != seat.requiredType) return false;
        }

        Debug.Log("특수 좌석 배치 완료");
        return true;
    }

    /** 디버깅용이므로 주석처리
    private void OnDrawGizmos()
    {
        if (boardData.boardShape == null || boardData.boardShape.Length == 0) return;

        int r = boardData.boardShape.Length;
        int c = boardData.boardShape[0].Length;
        Vector2 center = (Vector2)transform.position + boardData.boardOffset;
        Vector2 origin = new Vector2(center.x - (c - 1) * boardData.gridSize / 2f, center.y - (r - 1) * boardData.gridSize / 2f);

        if (boardData.specialSeats != null)
        {
            foreach (var seat in boardData.specialSeats)
            {
                Vector2 seatPos = origin + new Vector2(seat.gridIndex.x * boardData.gridSize, seat.gridIndex.y * boardData.gridSize);
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(seatPos, boardData.gridSize * 0.4f);
            }
        }
    }
    **/
}