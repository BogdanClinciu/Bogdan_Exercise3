using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    [SerializeField]
    private Text sortButtonText;
    [SerializeField]
    private UnknownWordManager unknownWordManager;

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
        //set the static color refferences to the private ones set in the unity inspector
        WordObjectNormalColor = wordObjectNormalColor;
        WordObjectHighlightColor = wordObjectHighlightColor;

        //Subscribe the update definitions method to the on change word static delegate
        OnChangeWordObject += UpdateDefinitionUI;
        UpdateDefinitionUI();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(!string.IsNullOrEmpty(WordSelector.CurrentWord))
            {
                //open the edit panel if the curent word is not empty
                OpenEditPanel(WordSelector.CurrentWord);
            }
        }
    }

    ///<summary>
    /// Set the active word object and invoke onChangeWordObject which updates the definitions panel ui.
    ///</summary>
    public static void SetActiveWord(WordObject wordObject)
    {
        ActiveWordObject = (wordObject == ActiveWordObject) ? null : wordObject;
        OnChangeWordObject.Invoke();
    }

    ///<summary>
    /// Updates the sort button text component to display the curent sorting method.
    ///</summary>
    public void UpdateSortButtonText(bool isSortAscending)
    {
        sortButtonText.text = SORT_HEADER + ((isSortAscending) ? SORT_ASCENDING : SORT_DESCENDING);
    }

    ///<summary>
    /// Opens the edit/add word panel and sets the ui interactability and text values acording to <paramref name="isEdit"/> parameter.
    ///</summary>
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
            editWordInput.text = (!string.IsNullOrEmpty(WordHandler.CurentWordInput)) ? WordHandler.CurentWordInput.ToUpper() : string.Empty;
            editDefInput.text = string.Empty;
        }

        CheckIfCanSave();

        editPanel.SetActive(true);
    }

    ///<summary>
    /// Alternete method to open edit panel with a specific <paramref name="clickedWord"/> string, triggerd when clicking a word from the definition panel.
    ///</summary>
    public void OpenEditPanel(string clickedWord)
    {
        editWordInput.interactable = false;
        editWordInput.text = clickedWord.ToUpper();
        editDefInput.text = (DatabaseManager.ActiveDatabase.ContainsKey(clickedWord.ToLower())) ? DatabaseManager.ActiveDatabase[clickedWord.ToLower()] : string.Empty;
        CheckIfCanSave();
        editPanel.SetActive(true);
    }

    ///<summary>
    /// Closes the edit pannel and clears the active word, clearing the definition panel as well.
    ///</summary>
    public void CloseEditPanel()
    {
        editPanel.SetActive(false);
        //We null the active word which updates the definition panel as well
        SetActiveWord(null);
    }

    ///<summary>
    /// Determines and sets save button interactability (triggered from the editWordInputField's on value changed), and shows/updates the warning text if necessary by
    /// checking the word and definition input fields of the edit panel.
    ///</summary>
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

    ///<summary>
    /// Toggles the edit and the remove buttons above the definition panel to <paramref name="state"/> value.
    ///</summary>
    public void ToggleModificationButtons(bool state)
    {
        if(removeWordButton.interactable != state)
        {
            removeWordButton.interactable = state;
            editDefWordButton.interactable = state;
        }
    }

    ///<summary>
    /// Updates the definition text whenever the active word object changes or is deselected, this also triggeres word higlighting if the active word object is not null.
    ///</summary>
    private void UpdateDefinitionUI()
    {
        if(ActiveWordObject != null)
        {
            definitionText.text = DatabaseManager.ActiveDatabase[ActiveWordObject.word];
            definitionWordTitle.text = ActiveWordObject.word.ToUpper();
            unknownWordManager.HilightUnknownWords(definitionText);
        }
        else
        {
            definitionText.text = NO_DEFNITION;
            definitionWordTitle.text = NO_WORD;
        }

        ToggleModificationButtons(ActiveWordObject != null);
    }
}
