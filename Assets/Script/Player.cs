// 강혜원 작성

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

// Block 클래스에 따라 변경 예정
public enum PassengerType
{
    Normal,
    Villain,
}

public class Player : MonoBehaviour
{
    // Block 클래스에 따라 변경 예정
    [SerializeField] private PassengerType currentType = PassengerType.Normal;
    [SerializeField] private Vector2 shapeOffset = Vector2.zero;

    [SerializeField] private bool isDragging = false;
    private bool isHovering = false;

    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D dragCursor;

    private Vector2 hotSpot = Vector2.zero;

    private SpriteRenderer spriteRenderer;
    private int originalOrder;
    private int draggingOrder = 100;

    private void Start()
    {
        Cursor.SetCursor(defaultCursor, hotSpot, CursorMode.Auto);

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalOrder = spriteRenderer.sortingOrder;
        }
    }

    private void Update()
    {
        if (isDragging)
        {
            transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if ((isHovering || isDragging) && Input.GetMouseButtonDown(1))
        {
            Debug.Log("우클릭");
        }
    }

    private void OnMouseDown()
    {
        switch (currentType)
        {
            case PassengerType.Villain:
                // 미니게임 함수 호출
                GetComponent<SpriteRenderer>().color = Color.red;
                // 미니게임 성공 시 PassengerType을 Normal로 바꿀 것
                break;

            case PassengerType.Normal:
                isDragging = true;
                Cursor.SetCursor(dragCursor, hotSpot, CursorMode.Auto);

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

        if (Board.Instance != null)
        {
            Vector2 snappedPos = Board.Instance.GetSnappedPosition(transform.position, shapeOffset);
            transform.position = snappedPos;
        }
    }

    private void OnMouseEnter()
    {
        // Debug.Log("호버 시작");
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
}
