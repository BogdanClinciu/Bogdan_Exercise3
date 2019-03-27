using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class WordObject : MonoBehaviour , IPointerDownHandler, IPointerUpHandler
{
    public string word {get; private set;} = string.Empty;
    [SerializeField]
    private Text wordText;

    private bool isDown = false;

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
}
