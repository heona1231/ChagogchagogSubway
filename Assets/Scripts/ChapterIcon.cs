// 박세은 작성
using UnityEngine;

public class ChapterIcon : MonoBehaviour
{
    [SerializeField] private bool isBackIcon = false;

    private void OnMouseDown()
    {
        if (isBackIcon)
        {
            GameManager.Instance.BackToTitle();
        }
    }
}
