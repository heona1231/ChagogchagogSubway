using UnityEngine;

public class Block : MonoBehaviour
{
    private GameObject[] childBlocks;
    public Sprite blockSprite;

    public void MoveTO(Vector2 newPosition)
    {
        transform.position = newPosition;
    }

    public void RotateBlock()
    {
        transform.Rotate(0, 0, 90f);
    }

    public void ShowOutline()
    {
        foreach (var cb in childBlocks)
        {
            if(cb != null)
            {
                cb.SetActive(true);
            }
        }
    }
}
