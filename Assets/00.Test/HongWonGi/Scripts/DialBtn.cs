using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class DialBtn : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    public RectTransform handle; // 핸들 오브젝트
    public RectTransform centerPoint; // 중심점 오브젝트(따로 깡통 오브젝트 만들기)
    public Canvas canvas; // 현재 사용 중인 캔버스
    // public Text valueText; // 값을 표시할 텍스트
    public Image fillImage; // 모드가 Radial 360 fill인 이미지

    public int maxChips = 360; // 최대 값 (100%에 해당)
    private float radius; // 중심점과 핸들 사이의 고정된 거리 (반지름)
    private float maxAngle = 360f; // 최대 각도 (260도)
    private float currentAngle = 0f; // 현재 핸들의 각도

    void Start()
    {
        // 중심점과 핸들 사이의 초기 거리(반지름)를 계산
        radius = Vector2.Distance(handle.anchoredPosition, centerPoint.anchoredPosition);
        UpdateHandlePosition(maxAngle); // 초기 위치를 0%로 설정
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        UpdateHandlePosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        UpdateHandlePosition(eventData);
    }

    private void UpdateHandlePosition(PointerEventData eventData)
    {
        // 마우스의 화면 좌표를 캔버스의 로컬 좌표로 변환
        Vector2 localMousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(centerPoint, eventData.position, canvas.worldCamera,
            out localMousePosition);

        // 중심점에서 마우스 위치 사이의 벡터를 계산
        Vector2 direction = (localMousePosition - (Vector2)centerPoint.anchoredPosition).normalized;

        // 마우스 위치로부터 각도 계산 (0~360도)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360; // 각도를 0~360도로 조정

        // 시계방향으로 돌 때 각도 제한
        if (angle > maxAngle)
        {
            currentAngle = maxAngle; // 시계방향으로 최대값을 넘어가면 멈춤
        }
        else
        {
            currentAngle = Mathf.Clamp(angle, 0, maxAngle);
            // 핸들의 새로운 위치 계산: 중심점에서 방향 벡터에 반지름을 곱한 거리만큼 이동
            Vector2 newPosition = (Vector2)centerPoint.anchoredPosition +
                                  new Vector2(Mathf.Cos(currentAngle * Mathf.Deg2Rad),
                                      Mathf.Sin(currentAngle * Mathf.Deg2Rad)) * radius;

            // 핸들의 위치를 업데이트
            handle.anchoredPosition = newPosition;

            // 현재 값 및 이미지 업데이트
            UpdateValueText();
            UpdateFillImage();
        }
    }

    private void UpdateHandlePosition(float angle)
    {
        // 초기 위치 설정 함수
        currentAngle = angle;
        Vector2 newPosition = (Vector2)centerPoint.anchoredPosition +
                              new Vector2(Mathf.Cos(currentAngle * Mathf.Deg2Rad),
                                  Mathf.Sin(currentAngle * Mathf.Deg2Rad)) * radius;
        handle.anchoredPosition = newPosition;
        UpdateValueText();
        UpdateFillImage();
    }

    private void UpdateValueText()
    {
        // 각도를 최대 칩 수로 변환하여 텍스트 업데이트
        // currentAngle이 최대일 때 0이 표시되도록 반대로 계산
        int currentValue = Mathf.RoundToInt(1-((currentAngle / maxAngle)) * maxChips);

        // valueText.text = currentValue.ToString();
    }

    private void UpdateFillImage()
    {
        // 각도를 0에서 1 사이의 값으로 변환하여 Radial Fill에 적용
        // FillAmount는 핸들이 움직인 비율로 설정해야 합니다.
        // fillImage.fillAmount = 1-(currentAngle / 360);
        float normalizedAngle = currentAngle / 360f; // 0~1 사이 값으로 정규화
        float roundedValue = Mathf.Round(normalizedAngle * 10) / 10f; // 0.1 단위로 반올림
    
        fillImage.fillAmount = 1 - roundedValue;
    }
}