using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace TzarGPT
{
    public class HoldableButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public UnityEvent onPress;
        public UnityEvent onRelease;

        public bool IsPressed { get; private set; }

        public void OnPointerDown(PointerEventData eventData)
        {
            IsPressed = true;
            onPress?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            IsPressed = false;
            onRelease?.Invoke();
        }
    }

}