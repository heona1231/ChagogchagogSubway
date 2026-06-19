// 강혜원 작성

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using static Unity.Collections.AllocatorManager;

public class Player : MonoBehaviour
{
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D dragCursor;
    private Vector2 hotSpot = Vector2.zero; // 커서의 클릭 판정 지점

    private BlockTest hoveredBlock = null;
    private BlockTest draggingBlock = null;
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

        BlockTest hitBlock = hit != null ? hit.GetComponent<BlockTest>() : null;

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
            if (draggingBlock != null || hoveredBlock != null)
            {
                Debug.Log("우클릭 - 회전 함수 호출");
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

    private void StartDrag(BlockTest targetBlock)
    {
        if (targetBlock.CurrentType == PassengerType.Villain)
        {
            // 미니게임 함수 호출
            targetBlock.spriteRenderer.color = Color.red;
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
                draggingBlock.ApplyToBoard(Board.Main, Board.Main.GetSnappedPosition(rawPos, draggingBlock.shapeOffset));
                CheckGameClear();
            }
            else
            {
                draggingBlock.ReturnToStart();
            }
        }
        else if (Board.Background != null && Board.Background.IsValidPlacement(rawPos, draggingBlock.shapeOffset, draggingBlock.shapeCells))
        {
            draggingBlock.ApplyToBoard(Board.Background, Board.Background.GetSnappedPosition(rawPos, draggingBlock.shapeOffset));
        }
        else
        {
            draggingBlock.ReturnToStart();
        }

        // 드래그 종료 처리
        BlockTest currentDroppingBlock = draggingBlock;
        draggingBlock = null;

        // 드래그를 놓은 후 마우스가 벗어났는지 확인
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Physics2D.OverlapPoint(mousePos) == null)
        {
            OnHoverExit(currentDroppingBlock);
            hoveredBlock = null;
        }
    }

    private void OnHoverEnter(BlockTest block)
    {
        // Debug.Log("호버 시작");
        block.spriteRenderer.color = Color.red;
    }

    private void OnHoverExit(BlockTest block)
    {
        // Debug.Log("호버 끝남");
        block.spriteRenderer.color = Color.white;
    }

    private void CheckGameClear()
    {
        BlockTest[] allBlocks = Object.FindObjectsByType<BlockTest>(FindObjectsSortMode.None);

        foreach (BlockTest block in allBlocks)
        {
            if (block.currentBoard != Board.Main)
            {
                return; // 클리어 아님
            }
        }
        Debug.Log("[클리어!]");
    }
}
