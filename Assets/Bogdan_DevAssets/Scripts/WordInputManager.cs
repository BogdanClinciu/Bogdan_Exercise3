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

    [Header("Refrence rect transforms")]
    [SerializeField]
    private RectTransform defPanelOpenRect;
    [SerializeField]
    private RectTransform defPanelClosedRect;
    [SerializeField]
    private RectTransform inputPopupOpenRect;
    [SerializeField]
    private RectTransform inputPopupClosedRect;

    private bool isOpen = false;
    private bool editing = false;

    private const float SLIDE_TIME = 0.25f;
    private const int MIN_DEF_CHARACTERS = 6;
    private const string NO_WORD = "Word not found.\nAdd a description to define the word.";
    private const string DEF_CHANGE = "Definition has been changed.\nTo save changes click Update Definition";
    private const string DEFINE_WORD = "Define new word";
    private const string UPDATE_DEF = "Update word Definition";


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

    public void DismissInputPopup(bool commitChanges)
    {
        if(isOpen)
        {
            StopAllCoroutines();
            StartCoroutine(OpenPopup());
            if(commitChanges)
            {
                DictionaryDatabaseManager.AddWord((editing) ? WordsPanelManager.ActiveWord : wordInput.text, defInput.text);
            }
            editing = false;
        }
    }

    //Triggered on change of definition input
    public void CheckDefinition()
    {
        if(!isOpen)
        {
            if(WordsPanelManager.ActiveWord != null && defInput.text != DictionaryDatabaseManager.ActiveDatabase[WordsPanelManager.ActiveWord])
            {
                TriggerInputPopup(true);
            }
        }

        if(isOpen)
        {
            defineWordButton.interactable = defInput.text.Length >= MIN_DEF_CHARACTERS;

            if(WordsPanelManager.ActiveWord != null && defInput.text == DictionaryDatabaseManager.ActiveDatabase[WordsPanelManager.ActiveWord])
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
