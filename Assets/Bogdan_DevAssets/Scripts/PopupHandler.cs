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
        //Calculate the popup panel limits and set the popup panel alpha to zero
        SetPopupPositionLimits();
        popupPanelCanvasGroup.alpha = 0;
    }

    private void Update()
    {
        // if the popup panel is visible we set its position to the clamped pointer position
        if(isShowing)
        {
            panelTransform.position = LimitedPosition();
        }
    }

    ///<summary>
    ///Toggles the popup panel to state and updateds the popup panel displayed information.
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

    ///<summary>
    ///Returns the pointer position clamped between the precalculated min limit and max limit vectors;
    ///<seealso cref="SetPopupPositionLimits.cs"/>
    ///</summary>
    private Vector2 LimitedPosition()
    {
        Vector2 targetPosition;
        targetPosition.x = Mathf.Clamp(Input.mousePosition.x, xLimitsMin.x, xLimitsMax.x);
        targetPosition.y = Mathf.Clamp(Input.mousePosition.y, xLimitsMin.y, xLimitsMax.y);

        return targetPosition;
    }

    ///<summary>
    ///Smothly lerps the popup panel's canvas group alpha from whatever the value is when the coroutine starts (startOpacity) to the required alpha valye (endOpacity, 0 or 1).
    ///</summary>
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

    ///<summary>
    ///Sets the limits needed in order to clamp the popup panel position in such a way to keep the popup panel entirely on screen
    ///</summary>
    private void SetPopupPositionLimits()
    {
        xLimitsMin.x = fullScreenRect.TransformPoint(fullScreenRect.rect.min).x + (panelTransform.position.x - panelTransform.TransformPoint(panelTransform.rect.min).x);
        xLimitsMax.x = fullScreenRect.TransformPoint(fullScreenRect.rect.max).x - (panelTransform.TransformPoint(panelTransform.rect.max).x - panelTransform.position.x);
        xLimitsMin.y = fullScreenRect.TransformPoint(fullScreenRect.rect.min).y + (panelTransform.position.y - panelTransform.TransformPoint(panelTransform.rect.min).y);
        xLimitsMax.y = fullScreenRect.TransformPoint(fullScreenRect.rect.max).y - (panelTransform.TransformPoint(panelTransform.rect.max).y - panelTransform.position.y);
    }
}
