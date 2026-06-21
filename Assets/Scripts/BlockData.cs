//서현아 작성

using UnityEngine;

//이건 승객 type과 다름 (미니게임 여부 -> 나중에 다른 기믹 추가 가능)
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
    public Sprite blockSprite;
    public Sprite blockOutlineSprite;
    public Vector2 spriteOffset;
    public BlockType blockType = BlockType.Normal;

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
