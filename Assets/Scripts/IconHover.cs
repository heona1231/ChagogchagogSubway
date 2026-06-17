using UnityEngine;

public class IconHover : MonoBehaviour
{
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite hoverSprite;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnMouseEnter()
    {
        spriteRenderer.sprite = hoverSprite;
    }
    private void OnMouseExit()
    {
        spriteRenderer.sprite = normalSprite;
    }
}
