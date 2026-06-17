using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] private SpriteRenderer blockSprite;
    [SerializeField] private SpriteRenderer blockOutlineSprite;
    [SerializeField] private GameObject blockCell;
    [SerializeField] private float gridSize = 1f;

    private BlockData blockData;

    public void Initialize(BlockData inputBlockData)
    {
        this.blockData = inputBlockData;

        BuildBlock();
        GetGimmickComponenet();
    }

    //blockData를 토대로 모양 구성
    private void BuildBlock()
    {
        blockSprite.sprite = blockData.blockSprite;
        blockOutlineSprite.sprite = blockData.blockOutlineSprite;
        blockOutlineSprite.gameObject.SetActive(false);

        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                if (blockData.GetShapeAt(x, y))
                {
                    GameObject cell = Instantiate(blockCell, transform);

                    float localX = (x - 1) * gridSize;
                    float localY = (y - 1) * gridSize;

                    cell.transform.localPosition = new Vector3(localX, localY, 0);
                    
                    if(cell.TryGetComponent<SpriteRenderer>(out var sr))
                    {
                        sr.color = Color.white;
                    }
                }
            }
        }
    }

    //blockType에 따라 컴포넌트 추가 부여
    private void GetGimmickComponenet()
    {
        if (blockData.blockType == BlockType.Minigame)
        {
            gameObject.AddComponent<MinigameMashClick>();
        }
    }

    //이동 함수
    public void MoveTO(Vector2 newPosition)
    {
        transform.position = newPosition;
    }

    //회전 함수 (호출시 90도 돌아감)
    public void RotateBlock()
    {
        transform.Rotate(0, 0, 90f);
    }

    //테두리 보이기/끄기
    public void ShowOutline(bool isShown)
    {
        blockOutlineSprite.gameObject.SetActive(isShown);
    }

    //블럭 sprite 변경
    public void ChangeBlockSprite(Sprite sprite)
    {
        blockSprite.sprite = sprite;
    }
}
