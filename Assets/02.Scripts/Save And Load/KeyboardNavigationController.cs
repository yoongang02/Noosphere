using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class KeyboardNavigationController : MonoBehaviour
{
    public UIKeyboardNavigator curHoveredNavigator;
    public List<UIKeyboardNavigator> navigators = new List<UIKeyboardNavigator>();

    int _currentIndex = -1;

    // 더블클릭 체크 관련
    [SerializeField] private float doubleClickThreshold = 0.3f; // 두 번째 입력이 이 시간 안에 들어오면 더블클릭
    private float _lastClickTime = -1f;

    void Start()
    {
        // 시작 시 현재 포커스가 없거나 비활성이라면 첫 활성 항목으로
        if (curHoveredNavigator == null || !IsAvailable(curHoveredNavigator))
            HoverFirstAvailable();
        else
            _currentIndex = navigators.IndexOf(curHoveredNavigator);
    }
    void Update()
    {
        if (navigators == null || navigators.Count == 0) return;

        if (curHoveredNavigator == null || !IsAvailable(curHoveredNavigator))
            HoverFirstAvailable();

        if (Input.GetKeyDown(KeyCode.W)) Move(-1);
        if (Input.GetKeyDown(KeyCode.S)) Move(+1);

        if (Input.GetKeyDown(KeyCode.Space)) HandleSpace();
    }

    public void Move(int dir) // dir: -1(위), +1(아래)
    {
        if (_currentIndex < 0) HoverFirstAvailable();

        int n = navigators.Count;
        if (n == 0) return;

        // 최대 n번까지 탐색(모두 비활성인 경우 대비)
        for (int step = 1; step <= n; step++)
        {
            int idx = Mod(_currentIndex + dir * step, n);
            var cand = navigators[idx];
            if (IsAvailable(cand))
            {
                SetHover(idx);
                return;
            }
        }
        // 모두 비활성인 경우: 아무 것도 하지 않음
    }

    public void HandleSpace()
    {
        float time = Time.unscaledTime;

        if (time - _lastClickTime <= doubleClickThreshold)
        {
            OnDoubleClick();
            _lastClickTime = -1f; // 초기화 (다시 새로 시작)
        }
        else
        {
            OnClick();
            _lastClickTime = time;
        }
    }

    private void OnClick()
    {
        if (curHoveredNavigator != null)
        {
            curHoveredNavigator.OnClick();
        }
    }

    private void OnDoubleClick()
    {
        if (curHoveredNavigator != null)
        {
            curHoveredNavigator.OnDoubleClick();
        }
    }

    void HoverFirstAvailable()
    {
        int n = navigators.Count;
        for (int i = 0; i < n; i++)
        {
            if (IsAvailable(navigators[i]))
            {
                SetHover(i);
                return;
            }
        }
        // 활성 항목이 하나도 없다면 포커스 해제
        ClearHover();
    }

    void SetHover(int index)
    {
        if (index < 0 || index >= navigators.Count) return;

        var next = navigators[index];
        if (!IsAvailable(next)) return;

        curHoveredNavigator = next;
        _currentIndex = index;

        // 실제 UI 포커스 반영(이벤트 시스템)
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(next.gameObject);

        // 슬롯과 버튼 구분
        curHoveredNavigator.OnHoverEnter();
    }

    void ClearHover()
    {
        curHoveredNavigator = null;
        _currentIndex = -1;
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

    bool IsAvailable(UIKeyboardNavigator nav)
    {
        return nav != null
               && nav.IsActive()
               && nav.gameObject.activeInHierarchy; // 게임오브젝트도 살아 있는지 체크(선택사항)
    }

    int Mod(int a, int m) => (a % m + m) % m; // 음수 래핑 안전
}
