using UnityEngine;

public class CollectionUI : MonoBehaviour
{
    public BlockData[] allBlocks;

    public void OpenCollection()
    {
        foreach (BlockData block in allBlocks)
        {
            bool isUnlocked = GameManager.Instance.IsStageCleared(block.stageNumber.x, block.stageNumber.y);

            UpdateBlockUI(block, isUnlocked);
        }
    }

    private void UpdateBlockUI(BlockData block, bool isUnlocked)
    {
        // 해금 여부에 따라 UI 갱신
    }
}