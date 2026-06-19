//서현아 작성

using System.Collections.Generic;
using UnityEngine;

// 강혜원 작성
public enum PassengerType
{
    Normal,
    Villain,
    Elderly,    // 노약자
    Pregnant    // 임산부
}

public class Block : MonoBehaviour
{
    [SerializeField] private GameObject blockCell;
    [SerializeField] private float gridSize = 1f;
    [SerializeField] public BlockData blockData;

    [SerializeField] private GameObject blockSprite;
    [SerializeField] private GameObject blockOutlineSprite;

    // 강혜원 작성
    [SerializeField] private PassengerType currentType = PassengerType.Normal;
    public PassengerType CurrentType => currentType;
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
        blockSprite.GetComponent<SpriteRenderer>().sprite = blockData.blockSprite;
        blockOutlineSprite.GetComponent<SpriteRenderer>().sprite = blockData.blockOutlineSprite;
        blockOutlineSprite.gameObject.SetActive(false);

        Vector3 offsetPosition = new Vector3(blockData.spriteOffset.x, blockData.spriteOffset.y, 0);
        blockSprite.transform.localPosition = offsetPosition;
        blockOutlineSprite.transform.localPosition = offsetPosition;

        List<Vector2Int> cellsList = new List<Vector2Int>(); // 강혜원 작성, Board 전달용 shapeCells 생성

        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                if (blockData.GetShapeAt(y,x))  
                {
                    GameObject cell = Instantiate(blockCell, transform);

                    float localX = (x - 1) * gridSize;
                    float localY = (1 - y) * gridSize;

                    cell.transform.localPosition = new Vector3(localX, localY, 0);
                    
                    if(cell.TryGetComponent<SpriteRenderer>(out var sr))
                    {
                        sr.color = Color.white;
                    }

                    cellsList.Add(new Vector2Int(x - 1, 1 - y)); // 강혜원 작성, Board가 인식할 보드 인덱스용 상대 좌표 계산
                }
            }
        }

        shapeCells = cellsList.ToArray(); // 강혜원 작성
    }

    //blockType에 따라 컴포넌트 추가 부여
    private void GetGimmickComponenet()
    {
        if (blockData.blockType == BlockType.Minigame)
        {
            gameObject.AddComponent<MinigameMashClick>();
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
}
