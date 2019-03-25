using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WordObject : MonoBehaviour , IPointerDownHandler
{
    [SerializeField]
    private Text wordText;

    public void Initialize()
    {
        //Add button listener to update desctription panel
        //wordButton.onClick.AddListener()
        wordText.text = name;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Clicked: " + name);
    }
}
