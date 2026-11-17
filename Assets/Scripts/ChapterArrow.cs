// 박세은 작성: 챕터 간 이동을 위한 ..

using UnityEngine;

public class ChapterArrow : MonoBehaviour
{
    [SerializeField] private ChapterManager chapterManager;
    [SerializeField] private bool isNextChapter;

    private void OnMouseDown()
    {
        if (chapterManager == null)
        {
            Debug.LogError("ChapterManager가 연결되지 않았습니다.");
            return;
        }

        if (isNextChapter)
        {
            chapterManager.NextChapter();
        }
        else
        {
            chapterManager.PreviousChapter();
        }
    }
}
