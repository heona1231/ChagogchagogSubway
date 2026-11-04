// 강혜원 작성
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonExplanation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject targetImage; // 호버 시 보일 이미지

    private void Start()
    {
        // 시작할 때 이미지가 확실히 꺼져있도록 초기화
        if (targetImage != null)
        {
            targetImage.SetActive(false);
        }
    }

    // 마우스 커서가 오브젝트 영역 안으로 들어왔을 때 실행
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (targetImage != null)
        {
            targetImage.SetActive(true); // 이미지 켜기
        }
    }

    // 마우스 커서가 오브젝트 영역 밖으로 빠져나갔을 때 실행
    public void OnPointerExit(PointerEventData eventData)
    {
        if (targetImage != null)
        {
            targetImage.SetActive(false); // 이미지 끄기
        }
    }
}