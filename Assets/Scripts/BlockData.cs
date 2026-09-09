//서현아 작성

using UnityEngine;

//이건 승객 type과 다름 (미니게임 여부 -> 나중에 다른 기믹 추가 가능)
public enum BlockType
{ 
    Normal,
    Minigame
}

public enum PassengerType
{
    Normal,
    Villain,
    Elderly,    // 노약자
    Pregnant    // 임산부
}

// 강혜원 작성, 블럭 방향
public enum BlockDirection
{
    Down = 0,   // 아래(기본)
    Left = 90,  // 왼쪽
    Up = 180,   // 위
    Right = 270 // 오른쪽
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
    public Sprite[] blockSpriteSit = new Sprite[4];
    public Sprite blockOutlineSprite;
    public Sprite blockOutlineSpriteR;
    public Vector2 spriteOffset;
    public BlockType blockType = BlockType.Normal;
    public BlockDirection defaultDirection = BlockDirection.Down; // 강헤원 작성, 기본 방향 (아래)
    public PassengerType passengerType;

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
