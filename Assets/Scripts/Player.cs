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

    private Block keyboardHeldBlock = null;

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
        HandleKeyboardHeldBlock();
    }

    // 마우스 위치에 블록이 있는지 감지
    private void HandleHover()
    {
        // 드래그 중일 때는 다른 블록 호버 무시
        if (draggingBlock != null || keyboardHeldBlock != null) return; // 박세은, 수정

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(mousePos);

        Block hitBlock = hit != null ? hit.GetComponentInParent<Block>() : null;

        if (hitBlock != hoveredBlock)
        {
            if (hoveredBlock != null) OnHoverExit(hoveredBlock);
            hoveredBlock = hitBlock;
            if (hoveredBlock != null) OnHoverEnter(hoveredBlock);
        }
        else if (hitBlock != null && !hitBlock.isOutlineActive()) // 블럭 아웃라인이 활성화되어 있지 않다면
        {
            OnHoverEnter(hitBlock);
        }
    }

    // 클릭 및 드래그 처리
    private void HandleInput()
    {
        // 박세은 추가, E키로 집기&놓기(마우스 드래그와 함께)
        if (keyboardHeldBlock != null && Input.GetMouseButtonDown(0))
        {
            EndKeyboardPickUp();
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            HandleKeyboardPickUp();
            return;
        }

        // 좌클릭 (드래그 시작)
        if (Input.GetMouseButtonDown(0) && hoveredBlock != null && keyboardHeldBlock == null)   // 박세은, 마우스 드래그와 E키의 동시 사용 막음
        {
            if (draggingBlock != null)
            {
                // 이미 들고 있다면 놓기 (클릭 모드에서만 가능)
                if (isClickToAttach) EndDrag();
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
            Block targetToRotate = draggingBlock != null ? draggingBlock : hoveredBlock;

            if (targetToRotate != null)
            {
                // 회전 전 현재 상태 저장
                Vector2 originalPos = targetToRotate.transform.position;
                Board originalBoard = targetToRotate.currentBoard;

                // 기존 보드 점유 지우기
                if (originalBoard != null)
                {
                    originalBoard.RemoveBlock(originalPos, targetToRotate.shapeOffset, targetToRotate.shapeCells);
                }

                // 회전 함수
                targetToRotate.RotateBlock();

                if (originalBoard != null)
                {
                    // 원래 위치(originalPos)가 회전 후에도 유효한지 체크
                    if (originalBoard.IsValidPlacement(originalPos, targetToRotate.shapeOffset, targetToRotate.shapeCells))
                    {
                        targetToRotate.ApplyToBoard(originalBoard, originalPos);
                    }
                    else
                    {
                        // 만약 제자리가 안된다면, 해당 보드 내에서 가장 가까운 가능한 위치로 다시 스냅 시도
                        Vector2 snappedPos = originalBoard.GetSnappedPosition(originalPos, targetToRotate.shapeOffset);

                        if (originalBoard.IsValidPlacement(snappedPos, targetToRotate.shapeOffset, targetToRotate.shapeCells))
                        {
                            targetToRotate.ApplyToBoard(originalBoard, snappedPos);
                        }
                        else
                        {
                            // 아예 놓을 데가 없으면 회전 취소 후 원위치
                            targetToRotate.RotateBlock();
                            targetToRotate.RotateBlock();
                            targetToRotate.RotateBlock();
                            targetToRotate.ApplyToBoard(originalBoard, originalPos);
                            Debug.Log("[회전 실패] 배치 공간 부족");
                        }
                    }
                }
            }
        }

        // 블록 이동 처리
        if (draggingBlock != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;
            draggingBlock.transform.position = mousePos;

            // 드래그 모드 전용 - 마우스를 떼면 배치
            if (!isClickToAttach && Input.GetMouseButtonUp(0))
            {
                EndDrag();
            }
        }

        // 박세은 추가, 키 바인딩(E키)
        if (Input.GetKeyDown(KeyCode.E))
        {
            HandleKeyboardPickUp();
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
            //targetBlock.spriteRenderer.color = Color.red;
            Debug.Log("미니게임 호출");
            targetBlock.GetComponent<MinigameMashClick>().StartMinigame();
            return;
        }

        draggingBlock = targetBlock;
        Cursor.SetCursor(dragCursor, hotSpot, CursorMode.Auto);

        draggingBlock.startDragPosition = draggingBlock.transform.position;

        if (draggingBlock.currentBoard != null)
        {
            draggingBlock.currentBoard.RemoveBlock(draggingBlock.startDragPosition, draggingBlock.shapeOffset, draggingBlock.shapeCells);
        }

        if (draggingBlock.spriteRenderer != null)
        {
            draggingBlock.spriteRenderer.sortingOrder = draggingOrder;
        }
    }

    private void EndDrag()
    {
        Cursor.SetCursor(defaultCursor, hotSpot, CursorMode.Auto);

        if (draggingBlock.spriteRenderer != null)
        {
            draggingBlock.spriteRenderer.sortingOrder = draggingBlock.originalOrder;
        }

        Vector2 rawPos = draggingBlock.transform.position;
        bool isOverlappingMain = Board.Main != null && Board.Main.IsOverlappingBoard(rawPos, draggingBlock.shapeOffset, draggingBlock.shapeCells);

        if (isOverlappingMain)
        {
            if (Board.Main.IsValidPlacement(rawPos, draggingBlock.shapeOffset, draggingBlock.shapeCells))
            {
                Vector2 snappedPos = Board.Main.GetSnappedPosition(rawPos, draggingBlock.shapeOffset);
                draggingBlock.ApplyToBoard(Board.Main, snappedPos);

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
            Vector2 snappedPos = Board.Background.GetSnappedPosition(rawPos, draggingBlock.shapeOffset);
            draggingBlock.ApplyToBoard(Board.Background, snappedPos);

            Debug.Log($"<color=yellow>[배치 성공]</color> '{draggingBlock.name}' 블록이 <b>Background 보드</b>로 이동했습니다. 위치: {snappedPos}");
        }
        else
        {
            draggingBlock.ReturnToStart();
        }

        // 드래그 종료 처리
        Block currentDroppingBlock = draggingBlock;
        draggingBlock = null;

        // 드래그를 놓은 후 마우스가 벗어났는지 확인
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Physics2D.OverlapPoint(mousePos) == null)
        {
            OnHoverExit(currentDroppingBlock);
            hoveredBlock = null;
        }
    }

    private void OnHoverEnter(Block block)
    {
        // Debug.Log("호버 시작");
        if (block != null)
        {
            if (block.blockData.blockType == BlockType.Minigame) return;

            block.ShowOutline(true);
        }
    }

    private void OnHoverExit(Block block)
    {
        // Debug.Log("호버 끝남");
        if (block != null)
        {
            if (block.blockData.blockType == BlockType.Minigame) return;

            block.ShowOutline(false);
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

    // E키 바인딩
    private void HandleKeyboardPickUp()
    {
        // 이미 E키로 블럭을 들고 있다면 놓기
        if (keyboardHeldBlock != null)
        {
            EndKeyboardPickUp();
            return;
        }

        // 잡을 블럭이 없다면 실행 X
        if (hoveredBlock == null)
        {
            return;
        }

        if (hoveredBlock.blockData.blockType == BlockType.Minigame)
        {
            hoveredBlock.GetComponent<MinigameMashClick>()?.StartMinigame();
            return;
        }

        StartKeyBoardPickUp(hoveredBlock);
    }

    private void StartKeyBoardPickUp(Block targetBlock)
    {
        keyboardHeldBlock = targetBlock;

        Cursor.SetCursor(dragCursor, hotSpot, CursorMode.Auto);

        keyboardHeldBlock.startDragPosition = keyboardHeldBlock.transform.position;

        if (keyboardHeldBlock.currentBoard != null)
        {
            keyboardHeldBlock.currentBoard.RemoveBlock(
                keyboardHeldBlock.startDragPosition,
                keyboardHeldBlock.shapeOffset,
                keyboardHeldBlock.shapeCells
                );

            keyboardHeldBlock.currentBoard = null;
        }

        if (keyboardHeldBlock.spriteRenderer != null)
        {
            keyboardHeldBlock.spriteRenderer.sortingOrder = draggingOrder;
        }

        Debug.Log($"[E키 집기] {keyboardHeldBlock.name}");
    }

    private void HandleKeyboardHeldBlock()
    {
        if (keyboardHeldBlock == null)
        {
            return;
        }

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        mousePos.z = 0f;
        keyboardHeldBlock.transform.position = mousePos;
    }

    private void EndKeyboardPickUp()
    {
        if (keyboardHeldBlock == null)
        {
            return;
        }

        Block blockToPlace = keyboardHeldBlock;
        Vector2 rawPos = blockToPlace.transform.position;

        bool isPlaced = false;

        bool isOverlappingMain = Board.Main != null &&
            Board.Main.IsOverlappingBoard(rawPos, blockToPlace.shapeOffset, blockToPlace.shapeCells);

        if (isOverlappingMain && Board.Main.IsValidPlacement(rawPos, blockToPlace.shapeOffset, blockToPlace.shapeCells))
        {
            Vector2 snappedPos = Board.Main.GetSnappedPosition(rawPos, blockToPlace.shapeOffset);

            blockToPlace.ApplyToBoard(Board.Main, snappedPos);
            isPlaced = true;

            Debug.Log($"[E키 배치 성공] {blockToPlace.name} 블럭이 Main 보드에 배치되었습니다.");
        }
        else if (Board.Background != null &&
            Board.Background.IsValidPlacement(rawPos, blockToPlace.shapeOffset, blockToPlace.shapeCells))
        {
            Vector2 snappedPos = Board.Background.GetSnappedPosition(rawPos, blockToPlace.shapeOffset);

            blockToPlace.ApplyToBoard(Board.Background, snappedPos);

            isPlaced = true;

            Debug.Log($"[E키 배치 성공] {blockToPlace.name} 블럭이 Background 보드에 배치되었습니다.");
        }

        if (!isPlaced)
        {
            blockToPlace.ReturnToStart();
            Debug.Log("[E키 배치 실패] 원래 위치로 돌아갑니다.");
        }

        if (blockToPlace.spriteRenderer != null)
        {
            blockToPlace.spriteRenderer.sortingOrder = blockToPlace.originalOrder;
        }

        keyboardHeldBlock = null;
        Cursor.SetCursor(defaultCursor, hotSpot, CursorMode.Auto);

        if (isPlaced)
        {
            CheckGameClear();
        }
    }
}