// 박세은 작성
using UnityEngine;

public class ChapterIcon : MonoBehaviour
{
    //[SerializeField] private bool isBackIcon = false;

    public void BackToTitle()
    {
        GameManager.Instance.BackToTitle();
    }
}
