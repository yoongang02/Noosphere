using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ComputerBtn : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Sprite _emailSpr;
    [SerializeField] private Image _targetImage;
    [SerializeField] private string _btnId; // 버튼 식별자
    [SerializeField] private ButtonEventChannel _eventChannel; // SO 참조
    private Button _emailBtn;
    private Image _btnImg;
    //테스트용 나중에 이미지 갈아끼우는걸로 바꾸려나??
    [Header("Colors")]
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _selectedColor = Color.yellow;

    private void Awake()
    {
         _emailBtn = GetComponent<Button>();
         _btnImg = GetComponent<Image>();

        _eventChannel.OnButtonClicked += HandleBtnClicked;
        _emailBtn.onClick.AddListener(OnBtnClick);
    }

    private void OnBtnClick()
    {
        _eventChannel.RaiseEvent(_btnId);
        _targetImage.sprite = _emailSpr;
    }

    private void HandleBtnClicked(string clickedBtnId)
    {
        _btnImg.color = (clickedBtnId == _btnId) ? _selectedColor : _normalColor;

        // 선택된 버튼인 경우 이미지 업데이트
        if (clickedBtnId == _btnId)
        {
            _targetImage.sprite = _emailSpr;
        }
    }

    private void OnDestroy()
    {
        if (_eventChannel != null)
            _eventChannel.OnButtonClicked -= HandleBtnClicked;
        if (_emailBtn != null)
            _emailBtn.onClick.RemoveListener(OnBtnClick);
    }
}