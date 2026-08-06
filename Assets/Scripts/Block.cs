//서현아 작성

using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] private GameObject blockCell;
    [SerializeField] private float gridSize = 1f;
    [SerializeField] public BlockData blockData;

    [SerializeField] private GameObject blockSprite;
    [SerializeField] private GameObject blockOutlineSprite;

    // 강혜원 작성
    //[SerializeField] private PassengerType currentType = PassengerType.Normal;
    //public PassengerType CurrentType => currentType;
    //서현아 수정, blockData에서 설정하도록 옮김
    [SerializeField] public Vector2 shapeOffset = Vector2.zero;
    [HideInInspector] public Vector2Int[] shapeCells;
    [HideInInspector] public Board currentBoard = null;
    [HideInInspector] public Vector2 startDragPosition;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public int originalOrder;

    private void Awake()
    {
        // 강혜원 작성, sprite order 초기값 저장
        if (blockSprite != null)
        {
            spriteRenderer = blockSprite.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                originalOrder = spriteRenderer.sortingOrder;
            }
        }
    }

    private void Start()
    {
        if (blockData != null)
        {
            Initialize(blockData);
        }

        // 강혜원 작성, 생성 시 어느 보드에 있는지 확인 후 해당 자리를 true로 변경
        if (Board.Main != null && Board.Main.IsValidPlacement(transform.position, shapeOffset, shapeCells))
        {
            ApplyToBoard(Board.Main, Board.Main.GetSnappedPosition(transform.position, shapeOffset));
        }
        else if (Board.Background != null && Board.Background.IsValidPlacement(transform.position, shapeOffset, shapeCells))
        {
            ApplyToBoard(Board.Background, Board.Background.GetSnappedPosition(transform.position, shapeOffset));
        }
    }

    public void Initialize(BlockData inputBlockData)
    {
        this.blockData = inputBlockData;

        BuildBlock();
        GetGimmickComponenet();
    }

    //blockData를 토대로 모양 구성
    private void BuildBlock()
    {
        foreach (Transform child in transform)
        {
            if (child == blockSprite.transform || child == blockOutlineSprite.transform)
                continue;
            Destroy(child.gameObject);
        }

        blockSprite.GetComponent<SpriteRenderer>().sprite = blockData.blockSprite;
        blockOutlineSprite.GetComponent<SpriteRenderer>().sprite = blockData.blockOutlineSprite;
        blockOutlineSprite.gameObject.SetActive(false);

        Vector3 offsetPosition = new Vector3(blockData.spriteOffset.x, blockData.spriteOffset.y, 0);
        blockSprite.transform.localPosition = offsetPosition;
        blockOutlineSprite.transform.localPosition = offsetPosition;

        List<Vector2Int> cellsList = new List<Vector2Int>(); // 강혜원 작성, Board 전달용 shapeCells 생성
        int rotationCount = Mathf.RoundToInt(transform.eulerAngles.z / 90f) % 4; // 강혜원 작성, 초기 회전 값 적용 위함

        // 강혜원 수정, 초기 회전 값 적용 위함
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                if (blockData.GetShapeAt(y,x))  
                {
                    // 기본 상대 좌표 (데이터 기반)
                    int rx = x - 1;
                    int ry = 1 - y;

                    // 현재 회전 상태(rotationCount)만큼 좌표를 미리 회전시킴
                    for (int i = 0; i < rotationCount; i++)
                    {
                        int temp = rx;
                        rx = -ry;
                        ry = temp;
                    }

                    GameObject cell = Instantiate(blockCell, transform);

                    // 회전된 좌표를 기반으로 배치
                    cell.transform.localPosition = new Vector3(rx * gridSize, ry * gridSize, 0);

                    if (cell.TryGetComponent<SpriteRenderer>(out var sr))
                    {
                        sr.color = Color.white;
                    }

                    // 회전이 반영된 최종 좌표를 Board 전달용 리스트에 추가
                    cellsList.Add(new Vector2Int(rx, ry));
                }
            }
        }

        shapeCells = cellsList.ToArray(); // 강혜원 작성
    }

    //blockType에 따라 컴포넌트 추가 부여
    private void GetGimmickComponenet()
    {
        if (blockData.blockType == BlockType.Minigame && blockData is BlockDataMinigame minigameData)
        {
            gameObject.AddComponent<MinigameMashClick>();
            this.GetComponent<MinigameMashClick>().SetMinigameBlock(minigameData);
        }
    }

    //이동 함수
    public void MoveTO(Vector2 newPosition)
    {
        transform.position = newPosition;
    }

    //회전 함수 (호출시 90도 돌아감)
    public void RotateBlock()
    {
        transform.Rotate(0, 0, 90f);

        // 강혜원 작성, 회전 시 보드가 검사해야 하는 좌표도 함께 처리
        for (int i = 0; i < shapeCells.Length; i++)
        {
            int nx = -shapeCells[i].y;
            int ny = shapeCells[i].x;
            shapeCells[i] = new Vector2Int(nx, ny);
        }
    }

    //테두리 보이기/끄기
    public void ShowOutline(bool isShown)
    {
        blockOutlineSprite.gameObject.SetActive(isShown);
    }

    //블럭 sprite 변경
    public void ChangeBlockSprite(Sprite sprite)
    {
        blockSprite.GetComponent<SpriteRenderer>().sprite = sprite;
    }

    //앉았을때 블럭 sprite 변경
    public void ChangeBlockSpriteSitdown(int sitState)
    {
        switch (sitState)
        {
            case 0:
                ChangeBlockSprite(blockData.blockSprite);
                break;
            case 1:
                ChangeBlockSprite(blockData.blockSpriteSit[0]);
                break;
            case 2:
                ChangeBlockSprite(blockData.blockSpriteSit[1]);
                break;
            case 3:
                ChangeBlockSprite(blockData.blockSpriteSit[2]);
                break;
        }
    }

    // 강혜원 작성, 보드에 등록하고 위치 설정
    public void ApplyToBoard(Board targetBoard, Vector2 targetPosition)
    {
        transform.position = targetPosition;
        targetBoard.PlaceBlock(this, transform.position, shapeOffset, shapeCells);
        currentBoard = targetBoard;
    }

    // 강혜원 작성, 실패 시 원래 자리로 되돌아가는 함수
    public void ReturnToStart()
    {
        transform.position = startDragPosition;
        if (currentBoard != null)
        {
            currentBoard.PlaceBlock(this, transform.position, shapeOffset, shapeCells);
        }
    }

    // 강혜원 작성, 아웃라인 활성화 상태를 반환(빌런 블럭 미니게임 후 사용)
    public bool isOutlineActive()
    {
        return blockOutlineSprite.activeSelf;
    }

    // 강혜원 작성, 현재 블럭의 회전 각도를 기반으로 방향을 반환하는 함수
    public BlockDirection GetCurrentDirection()
    {
        int angle = Mathf.RoundToInt(transform.eulerAngles.z) % 360;
        if (angle < 0) angle += 360;

        switch (angle)
        {
            case 90: return BlockDirection.Left;
            case 180: return BlockDirection.Up;
            case 270: return BlockDirection.Right;
            default: return BlockDirection.Down;    // 기본 아래
        }
    }

    public PassengerType GetPassengerType()
    {
        return blockData.passengerType;
    }
}
