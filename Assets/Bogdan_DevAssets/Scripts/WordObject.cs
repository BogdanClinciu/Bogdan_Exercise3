using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class WordObject : MonoBehaviour , IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private Text wordText;


    private UnityAction leftClickAction;
    private UnityAction rightClickAction;
    private bool isDown = false;

    public void Initialize(UnityAction onLeftClick, UnityAction onRightClick)
    {
        leftClickAction = onLeftClick;
        rightClickAction = onRightClick;
        wordText.text = name;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDown = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(isDown && !eventData.dragging)
        {
            isDown = false;
            if(eventData.pointerId.Equals(-1))
            {
                leftClickAction.Invoke();
            }
            else if (eventData.pointerId.Equals(-2))
            {
                rightClickAction.Invoke();
            }
        }
    }
}
