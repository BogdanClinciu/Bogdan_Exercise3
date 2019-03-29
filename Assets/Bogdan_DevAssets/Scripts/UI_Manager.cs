using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(UnknownWordManager))]
public class UI_Manager : MonoBehaviour
{
    [SerializeField]
    private Text sortButtonText;

    [Header("Definition pannel refrences")]
    [SerializeField]
    private TMP_Text definitionText;
    [SerializeField]
    private Text definitionWordTitle;

    [Header("Modification buttons")]
    [SerializeField]
    private Button removeWordButton;
    [SerializeField]
    private Button editDefWordButton;

    [Header("Edit pannel refrences")]
    [SerializeField]
    private Button saveEditButton;
    [SerializeField]
    private GameObject saveWarningTextParent;
    [SerializeField]
    private Text saveWarningText;
    [SerializeField]
    private GameObject editPanel;
    [SerializeField]
    private InputField editWordInput;
    [SerializeField]
    private InputField editDefInput;

    [Header("Word Object Colors")]
    [SerializeField]
    private Color wordObjectNormalColor;
    [SerializeField]
    private Color wordObjectHighlightColor;

    private bool isEditingDef = false;
    private UnknownWordManager unknownWordManager;

    public static WordObject ActiveWordObject {get; private set;}
    public static Color WordObjectNormalColor {get; private set;}
    public static Color WordObjectHighlightColor {get; private set;}

    private delegate void ChangeWordObject();
    private static ChangeWordObject OnChangeWordObject;

    #region Constant strings
        private const string SORT_ASCENDING = "A ► Z";
        private const string SORT_DESCENDING = "Z ► A";
        private const string SORT_HEADER = "Sorting:\n";
        private const string NO_WORD = "No word selected.";
        private const string NO_DEFNITION = "No definition.";
        private const string INCOMPLETE_EDIT_WARNING = "Both the word and the definition field need to be filled in order to add or edit an entry";
        private const string WORD_EXISTS_WARNING = "The database already contains this word, saving will overwrite\nthe curent definition entry of this word.";
    #endregion


    private void Start()
    {
        WordObjectNormalColor = wordObjectNormalColor;
        WordObjectHighlightColor = wordObjectHighlightColor;
        unknownWordManager = GetComponent<UnknownWordManager>();
        OnChangeWordObject += UpdateDefinitionUI;
        UpdateDefinitionUI();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(WordSelector.CurentLink.Length > 0)
            {
                OpenEditPanel(WordSelector.CurentLink);
            }
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
            editWordInput.text = ActiveWordObject.word.ToUpper();
            editDefInput.text = DatabaseManager.ActiveDatabase[ActiveWordObject.word];
        }
        else
        {
            editWordInput.interactable = true;
            editWordInput.text = (WordHandler.CurentWordInput.Length > 0) ? WordHandler.CurentWordInput.ToUpper() : string.Empty;
            editDefInput.text = string.Empty;
        }

        CheckIfCanSave();

        editPanel.SetActive(true);
    }

    public void OpenEditPanel(string clickedWord)
    {
        editWordInput.interactable = false;
        editWordInput.text = clickedWord.ToUpper();
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
            if (editDefInput.text.Length < 1)
            {
                saveWarningTextParent.SetActive(true);
                saveWarningText.text = INCOMPLETE_EDIT_WARNING;
                saveEditButton.interactable = false;
            }
            else if (DatabaseManager.ActiveDatabase.ContainsKey(editWordInput.text.ToLower()))
            {
                saveWarningTextParent.SetActive(true);
                saveWarningText.text = WORD_EXISTS_WARNING;
                saveEditButton.interactable = (editDefInput.text.Length > 0) ? true : false;
            }
            else
            {
                saveWarningTextParent.SetActive(false);
                saveEditButton.interactable = true;
            }
    }

    public void ToggleModificationButtons(bool state)
    {
        if(removeWordButton.interactable != state)
        {
            removeWordButton.interactable = state;
            editDefWordButton.interactable = state;
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
}
