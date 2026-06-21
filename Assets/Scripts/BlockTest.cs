// 강혜원 작성

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class BlockTest : MonoBehaviour
{
    [SerializeField] private PassengerType currentType = PassengerType.Normal;
    public PassengerType CurrentType => currentType;

    [SerializeField] public Vector2 shapeOffset = Vector2.zero;
    [SerializeField] public Vector2Int[] shapeCells = { new Vector2Int(0, 0) };

    [HideInInspector] public Board currentBoard = null;
    [HideInInspector] public Vector2 startDragPosition;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public int originalOrder;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalOrder = spriteRenderer.sortingOrder;
        }
    }

    private void Start()
    {
        // 생성 시 어느 보드에 있는지 확인 후 해당 자리를 true로 변경
        if (Board.Main != null && Board.Main.IsValidPlacement(transform.position, shapeOffset, shapeCells))
        {
            ApplyToBoard(Board.Main, Board.Main.GetSnappedPosition(transform.position, shapeOffset));
        }
        else if (Board.Background != null && Board.Background.IsValidPlacement(transform.position, shapeOffset, shapeCells))
        {
            ApplyToBoard(Board.Background, Board.Background.GetSnappedPosition(transform.position, shapeOffset));
        }
    }

    // 보드에 등록하고 위치 설정
    public void ApplyToBoard(Board targetBoard, Vector2 targetPosition)
    {
        transform.position = targetPosition;
        // targetBoard.PlaceBlock(this, transform.position, shapeOffset, shapeCells);
        currentBoard = targetBoard;
    }

    // 실패 시 원래 자리로 되돌아가는 함수
    public void ReturnToStart()
    {
        transform.position = startDragPosition;
        if (currentBoard != null)
        {
            // currentBoard.PlaceBlock(this, transform.position, shapeOffset, shapeCells);
        }
    }
}
