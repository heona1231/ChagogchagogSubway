using UnityEngine;

public enum BlockType
{ 
    Normal,
    Minigame
}

[System.Serializable]
public struct BlockRow
{
    public bool[] cols;
}

[CreateAssetMenu(fileName = "BlockData", menuName = "Scriptable Objects/BlockData")]
public class BlockData : ScriptableObject
{
    //블럭 기본 옵션 설정
    [Header("BasicOption")]
    public string blockName;
    public BlockType blockType;
    public Sprite blockSprite;
    public Sprite blockOutlineSprite;

    //블럭 모양 설정
    [Header("BlockShape")]
    public BlockRow[] shapeRows = new BlockRow[3];

    public bool GetShapeAt(int x, int y)
    {
        if (shapeRows == null || shapeRows.Length <= y) return false;
        if (shapeRows[y].cols == null || shapeRows[y].cols.Length <= x) return false;

        return shapeRows[y].cols[x];
    }
}
