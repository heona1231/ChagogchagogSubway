// 강혜원 작성

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using static Unity.Collections.AllocatorManager;

public class Player : MonoBehaviour
{
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D dragCursor;
    private Vector2 hotSpot = Vector2.zero; // Ŀ���� Ŭ�� ���� ����

    private BlockTest hoveredBlock = null;
    private BlockTest draggingBlock = null;
    private int draggingOrder = 100;

    private void Start()
    {Cursor.SetCursor(defaultCursor, hotSpot, CursorMode.Auto);
    }

    private void Update()
    {
        HandleHover();
        HandleInput();
    }

    // ���콺 ��ġ�� ������ �ִ��� ����
    private void HandleHover()
    {
        // �巡�� ���� ���� �ٸ� ���� ȣ�� ����
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

    // Ŭ�� �� �巡�� ó��
    private void HandleInput()
    {
        // ��Ŭ�� (�巡�� ����)
        if (Input.GetMouseButtonDown(0) && hoveredBlock != null)
        {
            StartDrag(hoveredBlock);
        }

        // ��Ŭ�� (ȸ��)
        if (Input.GetMouseButtonDown(1))
        {
            if (draggingBlock != null || hoveredBlock != null)
            {
                Debug.Log("��Ŭ�� - ȸ�� �Լ� ȣ��");
            }
        }

        // �巡�� �� ��ġ �̵�
        if (Input.GetMouseButton(0) && draggingBlock != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;
            draggingBlock.transform.position = mousePos;
        }

        // ��Ŭ�� �� (�巡�� ���� �� ��ġ)
        if (Input.GetMouseButtonUp(0) && draggingBlock != null)
        {
            EndDrag();
        }
    }

    private void StartDrag(BlockTest targetBlock)
    {
        if (targetBlock.CurrentType == PassengerType.Villain)
        {
            // �̴ϰ��� �Լ� ȣ��
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

        // FindFirstObjectByType<StageManager>().CheckClear(); // 박세은, 클리어 판단 코드

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

        // �巡�� ���� ó��
        BlockTest currentDroppingBlock = draggingBlock;
        draggingBlock = null;

        // �巡�׸� ���� �� ���콺�� ������� Ȯ��
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Physics2D.OverlapPoint(mousePos) == null)
        {
            OnHoverExit(currentDroppingBlock);
            hoveredBlock = null;
        }
    }

    private void OnHoverEnter(BlockTest block)
    {
        // Debug.Log("ȣ�� ����");
        block.spriteRenderer.color = Color.red;
    }

    private void OnHoverExit(BlockTest block)
    {
        // Debug.Log("ȣ�� ����");
        block.spriteRenderer.color = Color.white;
    }

    private void CheckGameClear()
    {
        BlockTest[] allBlocks = Object.FindObjectsByType<BlockTest>(FindObjectsSortMode.None);

        foreach (BlockTest block in allBlocks)
        {
            if (block.currentBoard != Board.Main)
            {
                return; // Ŭ���� �ƴ�
            }
        }
        Debug.Log("[Ŭ����!]");
    }
}
