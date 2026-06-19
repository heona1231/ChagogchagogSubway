//서현아 작성

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct GaugeSpritePair
{
    public int gaugeValue;
    public Sprite sprite;
}

[CreateAssetMenu(fileName = "BlockDataMinigame", menuName = "Scriptable Objects/BlockDataMinigame")]
public class BlockDataMinigame : BlockData
{
    private void OnEnable()
    {
        blockType = BlockType.Minigame;
    }

    //클리어 시 변신할 일반 BlockData
    [SerializeField] public BlockData clearedBlockData;
    [SerializeField] public List<GaugeSpritePair> gaugeSprites;
}
