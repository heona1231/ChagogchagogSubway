// 강혜원 작성

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
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
        if(isDragging)
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
        isDragging = true;
        Cursor.SetCursor(dragCursor, hotSpot, CursorMode.Auto);

        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = draggingOrder;
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
            GetComponent<SpriteRenderer>().color = Color.white;
            // 호버시 블록에 아웃라인 꺼지도록 호출처리
        }
    }

    private void OnMouseEnter()
    {
        // Debug.Log("호버 시작");
        isHovering = true;
        GetComponent<SpriteRenderer>().color = Color.red;
        // 호버시 블록에 아웃라인 뜨도록 호출처리
    }

    private void OnMouseExit()
    {
        // Debug.Log("호버 끝남");
        isHovering = false;

        if (!isDragging)
        {
            GetComponent<SpriteRenderer>().color = Color.white;
            // 호버시 블록에 아웃라인 꺼지도록 호출처리
        }
    }
}
