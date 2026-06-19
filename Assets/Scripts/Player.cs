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

    private void Start()
    {
        Cursor.SetCursor(defaultCursor, hotSpot, CursorMode.Auto);
    }

    private void Update()
    {
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
            if (hoveredBlock != null) OnHoverExit(hoveredBlock);
            hoveredBlock = hitBlock;
            if (hoveredBlock != null) OnHoverEnter(hoveredBlock);
        }
    }

    // 클릭 및 드래그 처리
    private void HandleInput()
    {
        // 좌클릭 (드래그 시작)
        if (Input.GetMouseButtonDown(0) && hoveredBlock != null)
        {
            StartDrag(hoveredBlock);
        }

        // 우클릭 (회전)
        if (Input.GetMouseButtonDown(1))
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

                bool canPlace = true;
                if (originalBoard != null)
                {
                    // 보드 위에 있던 블록이라면 회전 후 배치가 가능한지 확인
                    canPlace = originalBoard.IsValidPlacement(originalPos, targetToRotate.shapeOffset, targetToRotate.shapeCells);
                }

                if (canPlace)
                {
                    Debug.Log("[회전 성공]");
                    if (originalBoard != null)
                    {
                        targetToRotate.ApplyToBoard(originalBoard, originalPos);
                    }
                }
                else
                {
                    Debug.Log("[회전 실패] 배치가 불가능하여 회전 취소");

                    // 회전 취소 (3번 더 돌리면 원래대로 돌아옴)
                    targetToRotate.RotateBlock();
                    targetToRotate.RotateBlock();
                    targetToRotate.RotateBlock();

                    // 원래 자리에 다시 배치
                    if (originalBoard != null)
                    {
                        targetToRotate.ApplyToBoard(originalBoard, originalPos);
                    }
                }
            }
        }

        // 드래그 중 위치 이동
        if (Input.GetMouseButton(0) && draggingBlock != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;
            draggingBlock.transform.position = mousePos;
        }

        // 좌클릭 업 (드래그 종료 및 배치)
        if (Input.GetMouseButtonUp(0) && draggingBlock != null)
        {
            EndDrag();
        }
    }

    private void StartDrag(Block targetBlock)
    {
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

        draggingBlock.ShowOutline(false);

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
            block.ShowOutline(true);
        }
    }

    private void OnHoverExit(Block block)
    {
        // Debug.Log("호버 끝남");
        if (block != null)
        {
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

        Board.Main.CheckAllSpecialSeatsSatisfied();
        Debug.Log("[클리어!]");
    }
}