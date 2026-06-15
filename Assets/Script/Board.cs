using UnityEngine;

public enum BoardType
{
    Main,       // 게임 보드
    Background  // 배경 보드
}

public class Board : MonoBehaviour
{
    public static Board Main { get; private set; }
    public static Board Background { get; private set; }

    [SerializeField] private BoardType type;

    [SerializeField] private float gridSize = 1.32f;
    [SerializeField] private Vector2 boardOffset = Vector2.zero;

    [SerializeField] private int columns = 5; // 보드의 가로 칸 수
    [SerializeField] private int rows = 5;    // 보드의 세로 칸 수

    private bool[,] occupiedCells; // 보드의 어느 칸이 채워져 있는지 기억하는 2차원 배열

    private void Awake()
    {
        if (type == BoardType.Main) Main = this;
        else if (type == BoardType.Background) Background = this;

        occupiedCells = new bool[columns, rows];
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
        Vector2 basePos = dropPosition - blockOffset;
        Vector2 origin = GetBottomLeftOrigin();

        int gridX = Mathf.RoundToInt((basePos.x - origin.x) / gridSize);
        int gridY = Mathf.RoundToInt((basePos.y - origin.y) / gridSize);

        gridX = Mathf.Clamp(gridX, 0, columns - 1);
        gridY = Mathf.Clamp(gridY, 0, rows - 1);

        float snappedX = origin.x + gridX * gridSize;
        float snappedY = origin.y + gridY * gridSize;

        return new Vector2(snappedX, snappedY) + blockOffset;
    }

    // 해당 블럭이 보드 안쪽인지 체크
    public bool IsValidPlacement(Vector2 position, Vector2 blockOffset, Vector2Int[] shapeCells)
    {
        Vector2 basePos = position - blockOffset;
        Vector2 origin = GetBottomLeftOrigin();

        // 블록의 기준점이 위치할 보드의 X, Y 인덱스를 구함
        int baseGridX = Mathf.RoundToInt((basePos.x - origin.x) / gridSize);
        int baseGridY = Mathf.RoundToInt((basePos.y - origin.y) / gridSize);

        // 블록을 구성하는 모든 칸의 위치를 검사
        foreach (Vector2Int cellOffset in shapeCells)
        {
            // 기준점 인덱스 + 현재 칸의 상대적 인덱스
            int checkX = baseGridX + cellOffset.x;
            int checkY = baseGridY + cellOffset.y;

            // 단 한 칸이라도 보드 범위를 벗어나면 false (설치 불가)
            if (checkX < 0 || checkX >= columns || checkY < 0 || checkY >= rows)
            {
                return false;
            }

            if (occupiedCells[checkX, checkY])
            {
                return false;
            }
        }

        return true;
    }

    // 블록을 구성하는 칸 중 하나라도 보드 안쪽 인덱스에 들어온다면 true 반환
    public bool IsOverlappingBoard(Vector2 position, Vector2 blockOffset, Vector2Int[] shapeCells)
    {
        Vector2 basePos = position - blockOffset;
        Vector2 origin = GetBottomLeftOrigin();

        int baseGridX = Mathf.RoundToInt((basePos.x - origin.x) / gridSize);
        int baseGridY = Mathf.RoundToInt((basePos.y - origin.y) / gridSize);

        foreach (Vector2Int cellOffset in shapeCells)
        {
            int checkX = baseGridX + cellOffset.x;
            int checkY = baseGridY + cellOffset.y;

            if (checkX >= 0 && checkX < columns && checkY >= 0 && checkY < rows)
            {
                return true;
            }
        }
        return false;
    }

    // 블록이 놓일 때 해당 칸들을 true으로 변경
    public void PlaceBlock(Vector2 position, Vector2 blockOffset, Vector2Int[] shapeCells)
    {
        Vector2 basePos = position - blockOffset;
        Vector2 origin = GetBottomLeftOrigin();

        int baseGridX = Mathf.RoundToInt((basePos.x - origin.x) / gridSize);
        int baseGridY = Mathf.RoundToInt((basePos.y - origin.y) / gridSize);

        foreach (Vector2Int cellOffset in shapeCells)
        {
            int checkX = baseGridX + cellOffset.x;
            int checkY = baseGridY + cellOffset.y;

            if (checkX >= 0 && checkX < columns && checkY >= 0 && checkY < rows)
            {
                occupiedCells[checkX, checkY] = true;
            }
        }
    }

    // 블록을 들어올릴 때 해당 칸들을 false으로 변경
    public void RemoveBlock(Vector2 position, Vector2 blockOffset, Vector2Int[] shapeCells)
    {
        Vector2 basePos = position - blockOffset;
        Vector2 origin = GetBottomLeftOrigin();

        int baseGridX = Mathf.RoundToInt((basePos.x - origin.x) / gridSize);
        int baseGridY = Mathf.RoundToInt((basePos.y - origin.y) / gridSize);

        foreach (Vector2Int cellOffset in shapeCells)
        {
            int checkX = baseGridX + cellOffset.x;
            int checkY = baseGridY + cellOffset.y;

            if (checkX >= 0 && checkX < columns && checkY >= 0 && checkY < rows)
            {
                occupiedCells[checkX, checkY] = false;
            }
        }
    }
}
