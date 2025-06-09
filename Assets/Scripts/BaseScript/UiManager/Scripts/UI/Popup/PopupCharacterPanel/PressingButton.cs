using UnityEngine;
using UnityEngine.EventSystems;

public class PressingButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public bool IsHeld { get; private set; }
    public void OnPointerDown(PointerEventData eventData)
    {
        IsHeld = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsHeld = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsHeld = false;
    }
}
