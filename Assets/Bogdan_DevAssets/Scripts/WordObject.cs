using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WordObject : MonoBehaviour , IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Text wordText;
    [SerializeField]
    private Image backgroundImage;

    public string word {get; private set;} = string.Empty;

    private bool isDown = false;


    private void Start()
    {
        word = name;
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
        backgroundImage.color = UI_Manager.WordObjectHighlightColor;
        WordSelector.TogglePopupCurentLink(word);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        backgroundImage.color = UI_Manager.WordObjectNormalColor;
        WordSelector.TogglePopupCurentLink(string.Empty);
    }

}
