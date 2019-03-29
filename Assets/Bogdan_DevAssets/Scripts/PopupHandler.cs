using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PopupHandler : MonoBehaviour
{
    [SerializeField]
    private RectTransform panelTransform;
    [SerializeField]
    private Text wordText;
    [SerializeField]
    private Text definitionText;
    [SerializeField]
    private CanvasGroup popupPanelCanvasGroup;
    [SerializeField]
    private RectTransform fullScreenRect;

    [Header("Popup text colors")]
    [SerializeField]
    private Color wordExistsColor;
    [SerializeField]
    private Color wordNotExistColor;

    private bool isShowing;
    private Vector2 xLimitsMin;
    private Vector2 xLimitsMax;

    private const string NO_WORD = "No dictionary entry for word\nLeft click the word to add it.";
    private const float POP_TIME = 0.1f;

    private void Start()
    {
        SetMouseInputLimits();
        popupPanelCanvasGroup.alpha = 0;
    }

    private void Update()
    {
        if(isShowing)
        {
            panelTransform.position = LimitedPosition();
        }
    }

    ///<summary>
    ///Toggles the popup panel to state and sets popup panel to display corect information;
    ///</summary>
    public void TogglePopupPanel(string word)
    {
        StopAllCoroutines();
        if(!string.IsNullOrEmpty(word))
        {
            isShowing = true;
            panelTransform.position = Input.mousePosition;
            wordText.text = word.ToUpper();

            if(DatabaseManager.ActiveDatabase.ContainsKey(word.ToLower()))
            {
                wordText.color = wordExistsColor;
                definitionText.color = Color.white;
                definitionText.text = DatabaseManager.ActiveDatabase[word.ToLower()];
            }
            else
            {
                wordText.color = wordNotExistColor;
                definitionText.color = wordNotExistColor;
                definitionText.text = NO_WORD;
            }
        }
        else
        {
            isShowing = false;
        }
        StartCoroutine(FadePopup());
    }

    //Prevents the popup pannel from going off screen
    private Vector2 LimitedPosition()
    {
        Vector2 targetPosition;
        targetPosition.x = Mathf.Clamp(Input.mousePosition.x, xLimitsMin.x, xLimitsMax.x);
        targetPosition.y = Mathf.Clamp(Input.mousePosition.y, xLimitsMin.y, xLimitsMax.y);

        return targetPosition;
    }

    //smoothly fades in the popup panel
    private IEnumerator FadePopup()
    {
        float startOpacity = popupPanelCanvasGroup.alpha;
        float endOpacity = (isShowing) ? 1 : 0;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime/POP_TIME)
        {
            popupPanelCanvasGroup.alpha = Mathf.Lerp(startOpacity, endOpacity, t);
            yield return null;
        }
        popupPanelCanvasGroup.alpha = endOpacity;
    }

    //sets the limits needed in order to clamp the mouse position in such a way to keep the popup panel entirely on screen
    private void SetMouseInputLimits()
    {
        xLimitsMin.x = fullScreenRect.TransformPoint(fullScreenRect.rect.min).x + (panelTransform.position.x - panelTransform.TransformPoint(panelTransform.rect.min).x);
        xLimitsMax.x = fullScreenRect.TransformPoint(fullScreenRect.rect.max).x - (panelTransform.TransformPoint(panelTransform.rect.max).x - panelTransform.position.x);
        xLimitsMin.y = fullScreenRect.TransformPoint(fullScreenRect.rect.min).y + (panelTransform.position.y - panelTransform.TransformPoint(panelTransform.rect.min).y);
        xLimitsMax.y = fullScreenRect.TransformPoint(fullScreenRect.rect.max).y - (panelTransform.TransformPoint(panelTransform.rect.max).y - panelTransform.position.y);
    }
}
