// 강혜원 작성

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

// Block 클래스에 따라 변경 예정
public enum PassengerType
{
    Normal,
    Villain,
    Elderly,    // 노약자
    Pregnant    // 임산부
}

public class Player : MonoBehaviour
{
    // Block 클래스에 따라 변경 예정
    [SerializeField] private PassengerType currentType = PassengerType.Normal;
    public PassengerType CurrentType => currentType;
    [SerializeField] private Vector2 shapeOffset = Vector2.zero;
    // 블록이 차지하는 그리드 칸들의 로컬 인덱스 // ex. 가로 2칸: (0,0), (1,0) / 세로 2칸: (0,0), (0,1)
    [SerializeField] private Vector2Int[] shapeCells = { new Vector2Int(0, 0) };

    [SerializeField] private bool isDragging = false;
    private bool isHovering = false;
    public static bool isAnyDragging = false; // 어떠한 블럭을 드래그 중인지 판단

    private Board currentBoard = null;

    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D dragCursor;

    private Vector2 hotSpot = Vector2.zero;

    private SpriteRenderer spriteRenderer;
    private int originalOrder;
    private int draggingOrder = 100;

    private Vector2 startDragPosition; // 드래그를 시작한 원래 위치를 저장

    private void Start()
    {
        Cursor.SetCursor(defaultCursor, hotSpot, CursorMode.Auto);

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalOrder = spriteRenderer.sortingOrder;
        }

        // 어느 보드에 있는지 확인 후 해당 자리를 true로 변경
        if (Board.Main != null && Board.Main.IsValidPlacement(transform.position, shapeOffset, shapeCells))
        {
            ApplyToBoard(Board.Main, Board.Main.GetSnappedPosition(transform.position, shapeOffset));
        }
        else if (Board.Background != null && Board.Background.IsValidPlacement(transform.position, shapeOffset, shapeCells))
        {
            ApplyToBoard(Board.Background, Board.Background.GetSnappedPosition(transform.position, shapeOffset));
        }
    }

    private void Update()
    {
        if (isDragging)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10f;

            transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if ((isHovering || isDragging) && Input.GetMouseButtonDown(1))
        {
            // Debug.Log("우클릭");
            // 회전 함수 호출
        }
    }

    private void OnMouseDown()
    {
        switch (currentType)
        {
            case PassengerType.Villain:
                // 미니게임 함수 호출
                // GetComponent<SpriteRenderer>().color = Color.red;
                // 미니게임 성공 시 PassengerType을 Normal로 바꿀 것
                break;

            default: // Villain이 아니면 드래그 허용
                isDragging = true;
                isAnyDragging = true;
                Cursor.SetCursor(dragCursor, hotSpot, CursorMode.Auto);

                startDragPosition = transform.position;

                // 해당 블럭의 자리를 false로 변경
                if (currentBoard != null)
                {
                    currentBoard.RemoveBlock(startDragPosition, shapeOffset, shapeCells);
                }

                if (spriteRenderer != null)
                {
                    spriteRenderer.sortingOrder = draggingOrder;
                }
                break;
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;
        isAnyDragging = false;

        Cursor.SetCursor(defaultCursor, hotSpot, CursorMode.Auto);

        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = originalOrder;
        }

        if (!isHovering)
        {
            // GetComponent<SpriteRenderer>().color = Color.white;
            // 호버시 블록에 아웃라인 꺼지도록 호출처리
        }

        FindFirstObjectByType<StageManager>().CheckClear(); // 박세은, 클리어 판단 코드
        Vector2 rawPos = transform.position;

        // 게임 보드에 포함되는지 확인
        bool isOverlappingMain = Board.Main != null && Board.Main.IsOverlappingBoard(rawPos, shapeOffset, shapeCells);

        if (isOverlappingMain)
        {
            // 게임 보드에 포함되었다면, 게임 보드에서만 판단
            if (Board.Main.IsValidPlacement(rawPos, shapeOffset, shapeCells))
            {
                ApplyToBoard(Board.Main, Board.Main.GetSnappedPosition(rawPos, shapeOffset));
                CheckGameClear();
                // Debug.Log("게임 보드에 배치 완료");
            }
            else
            {
                // 게임 보드에 걸쳤으나 배치 불가(겹침, 밖으로 튀어나감 등)라면 무조건 제자리 복귀
                ReturnToStart();
                // Debug.Log("게임 보드 배치 실패 -> 제자리 복귀");
            }
        }
        // 게임 보드와 전혀 닿지 않았고, 배경 보드에 배치 가능할 때
        else if (Board.Background != null && Board.Background.IsValidPlacement(rawPos, shapeOffset, shapeCells))
        {
            ApplyToBoard(Board.Background, Board.Background.GetSnappedPosition(rawPos, shapeOffset));
            // Debug.Log("배경 보드에 배치 완료");
        }
        // 어느 보드에도 속하지 않을 때
        else
        {
            ReturnToStart();
            // Debug.Log("어느 보드에도 맞지 않음 -> 제자리 복귀");
        }
    }

    // 보드에 등록하고 위치 설정
    private void ApplyToBoard(Board targetBoard, Vector2 targetPosition)
    {
        transform.position = targetPosition;
        targetBoard.PlaceBlock(this, transform.position, shapeOffset, shapeCells);
        currentBoard = targetBoard;
    }

    // 실패 시 원래 자리로 되돌아가는 함수
    private void ReturnToStart()
    {
        transform.position = startDragPosition;
        if (currentBoard != null)
        {
            currentBoard.PlaceBlock(this, transform.position, shapeOffset, shapeCells);
        }
    }

    private void CheckGameClear()
    {
        // 씬에 존재하는 모든 Player(블록) 스크립트를 찾아옵니다.
        Player[] allBlocks = Object.FindObjectsByType<Player>(FindObjectsSortMode.None);

        foreach (Player block in allBlocks)
        {
            // 단 하나라도 게임 보드가 아닌 곳(대기열이나 공중)에 있다면 클리어가 아님
            if (block.currentBoard != Board.Main)
            {
                return; // 검사 즉시 중단 (아직 클리어 아님)
            }
        }

        // 위의 검사를 모두 통과했다면 모든 블록이 메인 보드에 있는 것
        // Debug.Log("[클리어!] 모든 블록을 게임 보드에 성공적으로 배치했습니다!");

        // bool isSpecial = Board.Main.CheckAllSpecialSeatsSatisfied();
        // Debug.Log($"[특수좌석 성공 여부] {isSpecial}");

        // 게임이 클리어 되고 난 이후의 처리 호출
    }

    private void OnMouseEnter()
    {
        // Debug.Log("호버 시작");
        if (isAnyDragging) return;

        isHovering = true;
        // GetComponent<SpriteRenderer>().color = Color.red;
        // 호버시 블록에 아웃라인 뜨도록 호출처리
    }

    private void OnMouseExit()
    {
        // Debug.Log("호버 끝남");
        isHovering = false;

        if (!isDragging)
        {
            // GetComponent<SpriteRenderer>().color = Color.white;
            // 호버시 블록에 아웃라인 꺼지도록 호출처리
        }
    }

    // 드래그 중이던 블록이 갑자기 파괴/비활성화 될 경우를 대비
    private void OnDisable()
    {
        if (isDragging)
        {
            isAnyDragging = false;
        }
    }
}
