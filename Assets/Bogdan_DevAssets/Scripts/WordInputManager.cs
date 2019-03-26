using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordInputManager : MonoBehaviour
{
    [SerializeField]
    private InputField wordInput;
    [SerializeField]
    private InputField defInput;

    [SerializeField]
    private RectTransform defPanelRect;
    [SerializeField]
    private RectTransform inputPopupRect;

    [SerializeField]
    private Button defineWordButton;
    [SerializeField]
    private Text defineWordButonLabel;
    [SerializeField]
    private Text defineWordLabel;

    [SerializeField]
    private WordsPanelManager wordsPanelManager;

    private bool isOpen = false;
    private bool editing = false;

    #region Refrence rectTransforms
        [Header("Refrence rect transforms")]
        [SerializeField]
        private RectTransform defPanelOpenRect;
        [SerializeField]
        private RectTransform defPanelClosedRect;
        [SerializeField]
        private RectTransform inputPopupOpenRect;
        [SerializeField]
        private RectTransform inputPopupClosedRect;
    #endregion

    #region Constants
        private const float SLIDE_TIME = 0.15f;
        private const int MIN_DEF_CHARACTERS = 6;
        private const string NO_WORD = "Word not found.\nAdd a description to define the word.";
        private const string DEF_CHANGE = "Definition has been changed.\nTo save changes click Update Definition";
        private const string DEFINE_WORD = "Define new word";
        private const string UPDATE_DEF = "Update word Definition";
    #endregion


    //Opens the input popup panel and sets the apropriate ui ellements
    public void TriggerInputPopup(bool isEditDefinition)
    {
        if(!isOpen)
        {
            editing = isEditDefinition;
            defineWordLabel.text = (isEditDefinition) ? DEF_CHANGE : NO_WORD;
            defineWordButonLabel.text = (isEditDefinition) ? UPDATE_DEF : DEFINE_WORD;
            defineWordButton.interactable = isEditDefinition;
            StopAllCoroutines();
            StartCoroutine(OpenPopup());
        }
    }

    //Closes the input popup panel and applies changes if the commitChanges parameter is true
    public void DismissInputPopup(bool commitChanges)
    {
        if(isOpen)
        {
            StopAllCoroutines();
            StartCoroutine(OpenPopup());
            if(commitChanges)
            {
                DatabaseManager.AddWord((editing) ? WordsPanelManager.ActiveWord : wordInput.text, defInput.text);
                wordsPanelManager.UpdateWords(true);
            }
            editing = false;
        }
    }

    //Triggered on change of definition input string
    public void CheckDefinition()
    {
        if(!isOpen)
        {
            if(WordsPanelManager.ActiveWord != null && defInput.text != DatabaseManager.ActiveDatabase[WordsPanelManager.ActiveWord])
            {
                TriggerInputPopup(true);
            }
        }

        else if(isOpen)
        {
            defineWordButton.interactable = defInput.text.Length >= MIN_DEF_CHARACTERS;

            if(WordsPanelManager.ActiveWord != null && defInput.text == DatabaseManager.ActiveDatabase[WordsPanelManager.ActiveWord])
            {
                DismissInputPopup(false);
            }
        }
    }

    private IEnumerator OpenPopup() {
        Vector3 startPos = inputPopupRect.position;
        Vector3 endPos = isOpen ? inputPopupClosedRect.position : inputPopupOpenRect.position;
        float startSize = defPanelRect.rect.height;
        float endSize = isOpen ? defPanelOpenRect.rect.height : defPanelClosedRect.rect.height;
        isOpen = !isOpen;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime/SLIDE_TIME)
        {
            inputPopupRect.position = Vector3.Lerp(startPos, endPos, t);
            defPanelRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Lerp(startSize, endSize, t));
            yield return null;
        }
        defPanelRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, endSize);
        inputPopupRect.position = endPos;
    }
}
