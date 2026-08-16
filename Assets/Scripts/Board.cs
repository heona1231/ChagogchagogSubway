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
    private GameObject[,] chairObjects; // 의자 타일 오브젝트들을 기억하는 배열

    // 블럭이 놓여질 위치를 보여주는 오브젝트 선언
    private GameObject previewObject;
    private SpriteRenderer previewRenderer;

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
        chairObjects = new GameObject[columns, rows];

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
                            BlockDirection spawnDir = BlockDirection.Down; // 기본값
                            if (boardData.specialSeats != null)
                            {
                                foreach (var seat in boardData.specialSeats)
                                {
                                    if (seat.gridIndex.x == x && seat.gridIndex.y == y)
                                    {
                                        spawnDir = seat.initialDirection;
                                        break;
                                    }
                                }
                            }

                            // 방향에 따른 초기 각도 계산
                            float initAngle = 0f;
                            switch (spawnDir)
                            {
                                case BlockDirection.Down: initAngle = 0f; break;
                                case BlockDirection.Right: initAngle = 90f; break;
                                case BlockDirection.Up: initAngle = 180f; break;
                                case BlockDirection.Left: initAngle = 270f; break;
                            }

                            // 계산된 회전값을 적용하여 생성
                            GameObject spawnedTile = Instantiate(prefabToInstantiate, pos, Quaternion.Euler(0, 0, initAngle), transform);

                            if (rowStr[x] == '2')
                            {
                                chairObjects[x, y] = spawnedTile;
                            }
                        }
                    }
                }
                else
                {
                    isPlayableCell[x, y] = false; // 0이면 뚫려있는 빈 공간
                }
            }
        }

        // 블럭이 놓여질 위치를 보여주는 오브젝트 동적 생성
        if (previewObject == null)
        {
            previewObject = new GameObject("BlockPreview");
            previewObject.transform.SetParent(this.transform);

            previewRenderer = previewObject.AddComponent<SpriteRenderer>();
            previewRenderer.sortingOrder = 100; // 다른 블록들보다 항상 위에 보이도록 설정

            previewObject.SetActive(false); // 처음에는 숨겨둠
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
    public Vector2 GetSnappedPosition(Vector2 dropPosition, Vector2 blockOffset, Vector2Int[] shapeCells)
    {
        Vector2 origin = GetBottomLeftOrigin();
        Vector2 basePos = dropPosition - blockOffset;

        int gridX = Mathf.RoundToInt((basePos.x - origin.x) / boardData.gridSize);
        int gridY = Mathf.RoundToInt((basePos.y - origin.y) / boardData.gridSize);

        if (shapeCells != null && shapeCells.Length > 0)
        {
            // 블록을 구성하는 칸들의 최소/최대 인덱스(범위) 구하기
            int minX = int.MaxValue, maxX = int.MinValue;
            int minY = int.MaxValue, maxY = int.MinValue;

            foreach (Vector2Int cell in shapeCells)
            {
                if (cell.x < minX) minX = cell.x;
                if (cell.x > maxX) maxX = cell.x;
                if (cell.y < minY) minY = cell.y;
                if (cell.y > maxY) maxY = cell.y;
            }

            // 블록의 어느 한 칸도 보드를 벗어나지 않도록 기준점의 위치를 강제 제한
            gridX = Mathf.Clamp(gridX, -minX, columns - 1 - maxX);
            gridY = Mathf.Clamp(gridY, -minY, rows - 1 - maxY);
        }
        else
        {
            gridX = Mathf.Clamp(gridX, 0, columns - 1);
            gridY = Mathf.Clamp(gridY, 0, rows - 1);
        }

        return new Vector2(origin.x + gridX * boardData.gridSize, origin.y + gridY * boardData.gridSize) + blockOffset;
    }

    // 위치를 보드 안쪽으로 강제 제한하여 반환 (드래그 시 밖으로 못 나가게 막는 용도)
    public Vector2 GetClampedRawPosition(Vector2 rawPosition, Vector2 blockOffset, Vector2Int[] shapeCells)
    {
        Vector2 origin = GetBottomLeftOrigin();
        Vector2 basePos = rawPosition - blockOffset;

        int minX = 0, maxX = columns - 1;
        int minY = 0, maxY = rows - 1;

        if (shapeCells != null && shapeCells.Length > 0)
        {
            minX = int.MaxValue; maxX = int.MinValue;
            minY = int.MaxValue; maxY = int.MinValue;

            foreach (Vector2Int cell in shapeCells)
            {
                if (cell.x < minX) minX = cell.x;
                if (cell.x > maxX) maxX = cell.x;
                if (cell.y < minY) minY = cell.y;
                if (cell.y > maxY) maxY = cell.y;
            }
        }

        // 보드의 실제 월드 좌표 경계 계산 (블록의 크기도 고려됨)
        float minWorldX = origin.x + (-minX) * boardData.gridSize;
        float maxWorldX = origin.x + (columns - 1 - maxX) * boardData.gridSize;
        float minWorldY = origin.y + (-minY) * boardData.gridSize;
        float maxWorldY = origin.y + (rows - 1 - maxY) * boardData.gridSize;

        // 경계선을 벗어나려 하면 끝부분으로 위치를 강제(Clamp)
        float clampedX = Mathf.Clamp(basePos.x, minWorldX, maxWorldX);
        float clampedY = Mathf.Clamp(basePos.y, minWorldY, maxWorldY);

        return new Vector2(clampedX, clampedY) + blockOffset;
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

        BlockDirection currentDir = block.GetCurrentDirection();
        float chairAngle = 0f;

        // 블록 방향에 따른 회전 각도 계산
        switch (currentDir)
        {
            case BlockDirection.Down: chairAngle = 0f; break;
            case BlockDirection.Left: chairAngle = 90f; break;
            case BlockDirection.Up: chairAngle = 180f; break;
            case BlockDirection.Right: chairAngle = 270f; break;
        }

        foreach (Vector2Int cellOffset in shapeCells)
        {
            int checkX = baseGridX + cellOffset.x;
            int checkY = baseGridY + cellOffset.y;

            if (checkX >= 0 && checkX < columns && checkY >= 0 && checkY < rows)
            {
                occupiedCells[checkX, checkY] = block;

                // 해당 자리가 의자('2')라면 생성되어 있는 의자 타일의 회전값을 바로 변경
                string rowStr = boardData.boardShape[rows - 1 - checkY];
                if (checkX < rowStr.Length && rowStr[checkX] == '2')
                {
                    if (chairObjects[checkX, checkY] != null)
                    {
                        chairObjects[checkX, checkY].transform.rotation = Quaternion.Euler(0, 0, chairAngle);
                    }
                }
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

    // 해당 그리드 위치가 의자 타일(2)인지 판별
    public bool IsChairCell(Vector2 position, Vector2 blockOffset, Vector2Int[] shapeCells)
    {
        if (shapeCells == null) return false;

        Vector2 origin = GetBottomLeftOrigin();
        Vector2 basePos = position - blockOffset;

        int baseGridX = Mathf.RoundToInt((basePos.x - origin.x) / boardData.gridSize);
        int baseGridY = Mathf.RoundToInt((basePos.y - origin.y) / boardData.gridSize);
 
        // 블록이 닿는 칸 중 '2'가 하나라도 포함되어 있는지 검사
        foreach (Vector2Int cellOffset in shapeCells)
        {
            int checkX = baseGridX + cellOffset.x;
            int checkY = baseGridY + cellOffset.y;

            if (checkX >= 0 && checkX < columns && checkY >= 0 && checkY < rows)
            {
                string rowStr = boardData.boardShape[rows - 1 - checkY];
                if (checkX < rowStr.Length && rowStr[checkX] == '2')
                {
                    return true;
                }
            }
        }

        return false;
    }

    // 드래그 중일 때 마우스 위치의 의자 방향을 블록에 맞춰 실시간으로 변경
    public void UpdateChairsDirectionForBlock(Block block, Vector2 position, Vector2 blockOffset, Vector2Int[] shapeCells)
    {
        if (shapeCells == null) return;

        Vector2 origin = GetBottomLeftOrigin();
        Vector2 basePos = position - blockOffset;

        int baseGridX = Mathf.RoundToInt((basePos.x - origin.x) / boardData.gridSize);
        int baseGridY = Mathf.RoundToInt((basePos.y - origin.y) / boardData.gridSize);

        BlockDirection currentDir = block.GetCurrentDirection();
        float chairAngle = 0f;

        switch (currentDir)
        {
            case BlockDirection.Down: chairAngle = 0f; break;
            case BlockDirection.Left: chairAngle = 90f; break;
            case BlockDirection.Up: chairAngle = 180f; break;
            case BlockDirection.Right: chairAngle = 270f; break;
        }

        foreach (Vector2Int cellOffset in shapeCells)
        {
            int checkX = baseGridX + cellOffset.x;
            int checkY = baseGridY + cellOffset.y;

            if (checkX >= 0 && checkX < columns && checkY >= 0 && checkY < rows)
            {
                string rowStr = boardData.boardShape[rows - 1 - checkY];
                if (checkX < rowStr.Length && rowStr[checkX] == '2')
                {
                    if (chairObjects[checkX, checkY] != null)
                    {
                        chairObjects[checkX, checkY].transform.rotation = Quaternion.Euler(0, 0, chairAngle);
                    }
                }
            }
        }
    }

    // 보드에 놓여질 위치 보기 활성화
    public void ShowPreview(Block block, Vector2 position, Vector2 blockOffset, Vector2Int[] shapeCells)
    {
        if (block == null || block.blockData.blockOutlineSprite == null) return;

        // 마우스 위치를 기반으로 보드판에 스냅된 월드 좌표 계산
        Vector2 snappedPos = GetSnappedPosition(position, blockOffset, shapeCells);

        // 프리뷰 오브젝트 활성화 및 이미지 적용
        previewObject.SetActive(true);
        previewRenderer.sprite = block.blockData.blockOutlineSprite;

        // 블록의 실제 회전값을 그대로 프리뷰에 복사
        previewObject.transform.rotation = block.transform.rotation;

        // 블록이 회전한 각도만큼 오프셋(offset)도 같이 회전시켜서 더해줌
        Vector3 rotatedOffset = block.transform.rotation * (Vector3)block.blockData.spriteOffset;
        previewObject.transform.position = (Vector3)snappedPos + rotatedOffset;

        // 배치 가능 여부에 따라 아웃라인 색상 변경
        bool isValid = IsValidPlacement(snappedPos, blockOffset, shapeCells);
        if (isValid)
        {
            // 빈 자리면 반투명 하얀색
            previewRenderer.color = new Color(1f, 1f, 1f, 0.5f);
        }
        else
        {
            // 겹치거나 X칸이면 반투명 빨간색
            previewRenderer.color = new Color(1f, 0f, 0f, 0.5f);
        }
    }

    // 보드에 놓여질 위치 보기 비활성화
    public void HidePreview()
    {
        if (previewObject != null)
        {
            previewObject.SetActive(false);
        }
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