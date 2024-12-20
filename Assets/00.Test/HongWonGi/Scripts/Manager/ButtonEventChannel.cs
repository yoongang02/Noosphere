using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ButtonEventChannel", menuName = "SO/Button Event Channel")]
public class ButtonEventChannel : ScriptableObject
{
    public UnityAction<int> OnButtonClicked;

    public void RaiseEvent(int buttonId)
    {
        OnButtonClicked?.Invoke(buttonId);
    }
}
