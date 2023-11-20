using UnityEngine;
using UnityEngine.EventSystems;

namespace SliBoxEngine
{
    enum ButtonHandlerType
    {
        PointClick,
        PointDown,
        PointUp,
    }
    public class ButtonClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        [HideInInspector] public Vector2 stampScale;
        [SerializeField] ButtonHandlerType buttonHandlerType;




        public virtual void Awake()
        {
            stampScale = transform.localScale;
        }
        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            transform.localScale = stampScale;
            if (buttonHandlerType == ButtonHandlerType.PointClick) PointerHandler();
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            transform.localScale = stampScale * 0.97f;
            if (buttonHandlerType == ButtonHandlerType.PointDown)
            {
                PointerHandler();
            }
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            transform.localScale = stampScale;
            if (buttonHandlerType == ButtonHandlerType.PointUp) PointerHandler();
        }

        public virtual void PointerHandler()
        {

        }

    }
}

