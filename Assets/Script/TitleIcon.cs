// 박세은 작성
using UnityEngine;

public class TitleIcon : MonoBehaviour
{
    [SerializeField] private bool isStartIcon = false;
    [SerializeField] private bool isQuitIcon = false;

    private void OnMouseDown()
    {
        if (isStartIcon)
        {
            GameManager.Instance.StartGame();
        }

        if (isQuitIcon)
        {
            GameManager.Instance.QuitGame();
        }
    }
}
