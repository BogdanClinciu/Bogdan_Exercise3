﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text definitionText;
    [SerializeField]
    private Text definitionWordTitle;


    [SerializeField]
    private Text sortButtonText;

    [SerializeField]
    private Button removeWordButton;
    [SerializeField]
    private Button editDefWordButton;
    [SerializeField]
    private Button saveEditButton;
    [SerializeField]
    private GameObject saveWarningText;

    [SerializeField]
    private GameObject editPanel;
    [SerializeField]
    private InputField editWordInput;
    [SerializeField]
    private InputField editDefInput;

    private bool isEditingDef = false;
    private UnknownWordManager unknownWordManager;

    public static WordObject ActiveWordObject {get; private set;}

    private delegate void ChangeWordObject();
    private static ChangeWordObject OnChangeWordObject;

    private const string SORT_ASCENDING = "A ► Z";
    private const string SORT_DESCENDING = "Z ► A";
    private const string SORT_HEADER = "Sorting:\n";
    private const string NO_WORD = "No word selected.";
    private const string NO_DEFNITION = "No definition.";


    private void Start()
    {
        unknownWordManager = GetComponent<UnknownWordManager>();
        OnChangeWordObject += UpdateDefinitionUI;
        UpdateDefinitionUI();
    }

    private void Update()
    {
        if(WordSelector.CurentLink.Length > 0 && Input.GetMouseButtonDown(0))
        {
            OpenEditPanel(WordSelector.CurentLink);
        }
    }

    //set the active word object and update ui accordingly
    public static void SetActiveWord(WordObject wordObject)
    {
        ActiveWordObject = (wordObject == ActiveWordObject) ? null : wordObject;
        OnChangeWordObject.Invoke();
    }

    public void UpdateSortButtonText(bool isSortAscending)
    {
        sortButtonText.text = SORT_HEADER + ((isSortAscending) ? SORT_ASCENDING : SORT_DESCENDING);
    }

    public void OpenEditPanel(bool isEdit)
    {
        if(isEdit)
        {
            editWordInput.interactable = false;
            editWordInput.text = ActiveWordObject.word;
            editDefInput.text = DatabaseManager.ActiveDatabase[ActiveWordObject.word];
        }
        else
        {
            editWordInput.interactable = true;
            editWordInput.text = (WordHandler.CurentWordInput.Length > 0) ? WordHandler.CurentWordInput : string.Empty;
            editDefInput.text = string.Empty;
        }

        CheckIfCanSave();

        editPanel.SetActive(true);
    }

    public void OpenEditPanel(string clickedWord)
    {
        editWordInput.interactable = false;
        editWordInput.text = clickedWord;
        editDefInput.text = string.Empty;
        CheckIfCanSave();
        editPanel.SetActive(true);
    }

    public void CloseEditPanel()
    {
        editPanel.SetActive(false);
        SetActiveWord(null);
    }


    //Determines if the save button can be pressed and sets interactability (triggered from the editWordInputField's on value changed)
    public void CheckIfCanSave()
    {
        if(editWordInput.text.Length > 0)
        {
            if(DatabaseManager.ActiveDatabase.ContainsKey(editWordInput.text.ToLower()))
            {
                saveWarningText.SetActive(true);
            }
            else
            {
                saveWarningText.SetActive(false);
            }

            saveEditButton.interactable = true;
        }
        else
        {
            saveEditButton.interactable = false;
        }
    }


    //Updates the definiton ui panel (triggered every time the ActiveWord changes )
    private void UpdateDefinitionUI()
    {
        if(ActiveWordObject != null)
        {
            definitionText.text = DatabaseManager.ActiveDatabase[ActiveWordObject.word];
            definitionWordTitle.text = ActiveWordObject.word.ToUpper();
            unknownWordManager.CreateTextMarkup(definitionText);
        }
        else
        {
            definitionText.text = NO_DEFNITION;
            definitionWordTitle.text = NO_WORD;
        }
        ToggleModificationButtons(ActiveWordObject != null);
    }

    public void ToggleModificationButtons(bool state)
    {
        if(removeWordButton.interactable != state)
        {
            removeWordButton.interactable = state;
            editDefWordButton.interactable = state;
        }
    }
}
