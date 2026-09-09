// 강혜원 작성

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using static Unity.Collections.AllocatorManager;

public class Player : MonoBehaviour
{
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D dragCursor;
    private Vector2 hotSpot = new Vector2 (0.5f, 0.5f); // 커서의 클릭 판정 지점

    private Block hoveredBlock = null;
    private Block draggingBlock = null;
    private int draggingOrder = 100;

    [SerializeField] private bool isClickToAttach = false; // true: 클릭, false: 드래그
    // 박세은 추가: 키 바인딩
    [Header("Keyboard Interaction")]
    [SerializeField] private StageIcon menuIcon;

    private void Start()
    {
        Cursor.SetCursor(defaultCursor, hotSpot, CursorMode.Auto);
    }

    private void Update()
    {
        // 박세은 추가: 키 바인딩
        HandleMenuInput();

        // 메뉴 창이 열려있다면 게임 조작을 막음
        if (GameManager.Instance != null && GameManager.Instance.IsPaused)
        {
            return;
        }

        HandleHover();
        HandleInput();
    }
    
    // 마우스 위치에 블록이 있는지 감지
    private void HandleHover()
    {
        // 드래그 중일 때는 다른 블록 호버 무시
        if (draggingBlock != null) return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(mousePos);

        Block hitBlock = hit != null ? hit.GetComponentInParent<Block>() : null;

        if (hitBlock != hoveredBlock)
        {
            hoveredBlock = hitBlock;
        }
    }

    // 클릭 및 드래그 처리
    private void HandleInput()
    {
        // 좌클릭 (드래그 시작)
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E))
        {
            if (draggingBlock != null)
            {
                // 이미 들고 있다면 놓기 (클릭 모드에서만 가능)
                if (isClickToAttach || Input.GetKeyDown(KeyCode.E))
                {
                    EndDrag();
                }
            }
            else if (hoveredBlock != null)
            {
                // 들고 있지 않다면 잡기
                StartDrag(hoveredBlock);
            }
        }

        // 우클릭 (회전)
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.R)) // 박세은, R키 바인딩
        {
            Block targetBlock = draggingBlock != null ? draggingBlock : hoveredBlock;

            if (targetBlock != null)
            {
                TryRotateBlock(targetBlock);
            }
        }

        // 블록 이동 처리
        if (draggingBlock != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;

            // 배경 보드의 경계를 벗어나는 경우 블록의 위치를 강제
            if (Board.Background != null)
            {
                mousePos = Board.Background.GetClampedRawPosition(mousePos, draggingBlock.shapeOffset, draggingBlock.shapeCells);
            }

            draggingBlock.transform.position = mousePos;

            // 드래그하면서 의자 근처를 지나갈 때 실시간으로 의자 방향 회전
            /**
            if (Board.Main != null)
            {
                Vector2 rawPos = draggingBlock.transform.position;
                if (Board.Main.IsOverlappingBoard(rawPos, draggingBlock.shapeOffset, draggingBlock.shapeCells))
                {
                    Vector2 snappedPos = Board.Main.GetSnappedPosition(rawPos, draggingBlock.shapeOffset, draggingBlock.shapeCells);
                    // 마우스가 위치한 곳의 의자들에 블록의 현재 방향을 실시간 반영
                    Board.Main.UpdateChairsDirectionForBlock(draggingBlock, snappedPos, draggingBlock.shapeOffset, draggingBlock.shapeCells);
                }
            }
            **/

            // 보드에 놓여질 위치 보기 활성화
            UpdatePreview(draggingBlock, draggingBlock.transform.position);

            // 드래그 모드 전용 - 마우스를 떼면 배치
            if (!isClickToAttach && Input.GetMouseButtonUp(0))
            {
                EndDrag();
            }
        }
    }

    private void StartDrag(Block targetBlock)
    {
        //서현아 수정 : 다른 블럭 클릭 시 미니게임 초기화
        var activeMinigame = MinigameManager.Instance.CurrentlyActiveMinigame;
        if (activeMinigame != null)
        {
            if (targetBlock.gameObject != activeMinigame.gameObject)
            {
                activeMinigame.ResetMinigame();
                MinigameManager.Instance.EndMinigame();
            }
        }

        if (targetBlock.blockData.blockType == BlockType.Minigame)
        {
            // 미니게임 함수 호출
            Debug.Log("미니게임 호출");
            targetBlock.GetComponent<MinigameMashClick>().StartMinigame();
            return;
        }

        draggingBlock = targetBlock;
        Cursor.SetCursor(dragCursor, hotSpot, CursorMode.Auto);

        draggingBlock.startDragPosition = draggingBlock.transform.position;
        draggingBlock.SaveOriginalState();

        if (draggingBlock.currentBoard != null)
        {
            draggingBlock.currentBoard.RemoveBlock(draggingBlock.startDragPosition, draggingBlock.shapeOffset, draggingBlock.shapeCells);
            draggingBlock.currentBoard = null;
        }

        if (draggingBlock.spriteRenderer != null)
        {
            draggingBlock.spriteRenderer.sortingOrder = draggingOrder;
        }
    }

    private void EndDrag()
    {
        // 보드에 놓여질 위치 보기 비활성화
        if (Board.Main != null) Board.Main.HidePreview();
        if (Board.Background != null) Board.Background.HidePreview();

        Cursor.SetCursor(defaultCursor, hotSpot, CursorMode.Auto);

        if (draggingBlock.spriteRenderer != null)
        {
            draggingBlock.spriteRenderer.sortingOrder = draggingBlock.originalOrder;
        }

        Vector2 rawPos = draggingBlock.transform.position;
        bool isOverlappingMain = Board.Main != null && Board.Main.IsOverlappingBoard(rawPos, draggingBlock.shapeOffset, draggingBlock.shapeCells);

        if (isOverlappingMain)
        {
            Vector2 snappedPos = Board.Main.GetSnappedPosition(rawPos, draggingBlock.shapeOffset, draggingBlock.shapeCells);

            if (Board.Main.IsValidPlacement(snappedPos, draggingBlock.shapeOffset, draggingBlock.shapeCells))
            {
                draggingBlock.ApplyToBoard(Board.Main, snappedPos);


                // Main 보드에 둘 때 의자 타일인지 확인 후 모양 변경
                bool isChair = Board.Main.IsChairCell(snappedPos, draggingBlock.shapeOffset, draggingBlock.shapeCells);
                draggingBlock.ChangeBlockSpriteSitdown(1);

                Debug.Log($"<color=cyan>[배치 성공]</color> '{draggingBlock.name}' 블록이 <b>Main 보드</b>에 배치되었습니다. 위치: {snappedPos}");

                CheckGameClear();
            }
            else
            {
                draggingBlock.ReturnToStart();
            }
        }
        else if (Board.Background != null && Board.Background.IsValidPlacement(rawPos, draggingBlock.shapeOffset, draggingBlock.shapeCells))
        {
            Vector2 snappedPos = Board.Background.GetSnappedPosition(rawPos, draggingBlock.shapeOffset, draggingBlock.shapeCells);
            draggingBlock.ApplyToBoard(Board.Background, snappedPos);

            // Background 보드로 갈 때는 일반 모양(서 있는 모양)으로 설정
            draggingBlock.ChangeBlockSpriteSitdown(0);

            Debug.Log($"<color=yellow>[배치 성공]</color> '{draggingBlock.name}' 블록이 <b>Background 보드</b>로 이동했습니다. 위치: {snappedPos}");
        }
        else
        {
            draggingBlock.ReturnToStart();

            // 보드 바깥으로 돌아갈 때도 서 있는 모양으로 초기화
            draggingBlock.ChangeBlockSpriteSitdown(0);
        }

        // 드래그 종료 처리
        Block currentDroppingBlock = draggingBlock;
        draggingBlock = null;

        // 드래그를 놓은 후 마우스가 벗어났는지 확인
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Physics2D.OverlapPoint(mousePos) == null)
        {
            hoveredBlock = null;
        }
    }

    private void TryRotateBlock(Block targetBlock)
    {
        // 다음 회전 시의 가상 좌표를 계산
        Vector2Int[] currentCells = targetBlock.shapeCells;
        Vector2Int[] nextCells = new Vector2Int[currentCells.Length];
        for (int i = 0; i < currentCells.Length; i++)
        {
            nextCells[i] = new Vector2Int(-currentCells[i].y, currentCells[i].x);
        }

        Vector2 currentPos = targetBlock.transform.position;
        bool isDragging = (targetBlock == draggingBlock);

        // 바닥에 놓여있는 상태일 때의 예외 처리
        if (!isDragging)
        {
            Board board = targetBlock.currentBoard;
            if (board != null)
            {
                // 현재 자리에서 블록을 임시로 빼고 회전 시 배치 가능한지 검사
                board.RemoveBlock(currentPos, targetBlock.shapeOffset, currentCells);
                bool isValid = board.IsValidPlacement(currentPos, targetBlock.shapeOffset, nextCells);

                // 배경 보드에 있는 블록이 회전하면서 메인 보드 영역을 침범하는지 검사
                if (isValid && board == Board.Background && Board.Main != null)
                {
                    if (Board.Main.IsOverlappingBoard(currentPos, targetBlock.shapeOffset, nextCells))
                    {
                        isValid = false; // 메인 보드와 겹치면 회전 불가
                    }
                }

                if (!isValid)
                {
                    // 불가능하면 원상복구하고 회전 취소
                    board.PlaceBlock(targetBlock, currentPos, targetBlock.shapeOffset, currentCells);
                    // 회전 실패 시 피드백 추가
                    return;
                }
            }
        }

        // 모든 예외 처리를 통과했으므로 블록 실제 회전
        targetBlock.RotateBlock();

        // 바닥에 놓여있던 블록이면 갱신된 형태로 보드에 다시 등록
        if (!isDragging && targetBlock.currentBoard != null)
        {
            // RotateBlock() 내부에서 targetBlock.shapeCells가 갱신되었으므로 이를 사용해 다시 배치
            targetBlock.currentBoard.PlaceBlock(targetBlock, currentPos, targetBlock.shapeOffset, targetBlock.shapeCells);
        }
    }

    private void CheckGameClear()
    {
        Block[] allBlocks = Object.FindObjectsByType<Block>(FindObjectsSortMode.None);

        foreach (Block block in allBlocks)
        {
            if (block.currentBoard != Board.Main)
            {
                return; // 클리어 아님
            }
        }

        Debug.Log("[클리어!]");
        FindFirstObjectByType<StageManager>().CheckClear(); // 박세은, 클리어 판단 코드
    }

    // 박세은 추가: 키 바인딩(ESC키)
    private void HandleMenuInput()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
        {
            return;
        }

        if (menuIcon == null)
        {
            Debug.LogError("Player의 Menu Icon이 연결되지 않았습니다.");
            return;
        }

        menuIcon.ToggleMenu();
    }

    // Main과 Background 중 어느 보드에 미리보기를 띄울지 결정하는 함수
    private void UpdatePreview(Block block, Vector2 rawPos)
    {
        if (block == null) return;

        // Main 보드에 조금이라도 걸쳐있는지 확인
        bool isOverlappingMain = Board.Main != null && Board.Main.IsOverlappingBoard(rawPos, block.shapeOffset, block.shapeCells);

        if (isOverlappingMain)
        {
            // Main에 걸쳤다면 Main에 보여주고 Background 프리뷰는 끔
            if (Board.Main != null) Board.Main.ShowPreview(block, rawPos, block.shapeOffset, block.shapeCells);
            if (Board.Background != null) Board.Background.HidePreview();
        }
        else
        {
            // Main에 안 걸쳤다면 Background에 띄우기 시도
            if (Board.Main != null) Board.Main.HidePreview();
            if (Board.Background != null) Board.Background.ShowPreview(block, rawPos, block.shapeOffset, block.shapeCells);
        }
    }
}