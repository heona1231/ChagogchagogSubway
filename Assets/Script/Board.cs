using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board Instance { get; private set; }

    [SerializeField] private float gridSize = 1.32f;
    [SerializeField] private Vector2 boardOffset = Vector2.zero;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public Vector2 GetSnappedPosition(Vector2 dropPosition, Vector2 blockOffset)
    {
        Vector2 basePos = dropPosition - blockOffset;

        float snappedX = Mathf.Round((basePos.x - boardOffset.x) / gridSize) * gridSize + boardOffset.x;
        float snappedY = Mathf.Round((basePos.y - boardOffset.y) / gridSize) * gridSize + boardOffset.y;

        return new Vector2(snappedX, snappedY) + blockOffset;
    }

    // (확장 가능) 나중에 이곳에 '해당 위치가 보드 안쪽인지', '비어있는지' 체크하는 함수를 추가할 수 있습니다.
    /*
    public bool IsValidPlacement(Vector2 position) { ... }
    */
}
