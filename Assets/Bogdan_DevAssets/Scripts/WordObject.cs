using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WordObject : MonoBehaviour , IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public string word {get; private set;} = string.Empty;
    [SerializeField]
    private Text wordText;

    private bool isDown = false;

    //we call SetWord when instantiating thw word object.
    public void SetWord(string _word)
    {
        word = _word;
        wordText.text = word;
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
                UI_Manager.SetActiveWord(this);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        WordSelector.TogglePopupCurentLink(word);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        WordSelector.TogglePopupCurentLink(string.Empty);
    }
}
