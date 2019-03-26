using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class WordsPanelManager : MonoBehaviour
{
    public static string ActiveWord {get; private set;}

    [SerializeField]
    private WordInputManager wordInputManager;

    [SerializeField]
    private RectTransform referenceRect;
    [SerializeField]
    private RectTransform wordsRect;
    [SerializeField]
    private GameObject wordPrefab;

    [SerializeField]
    private Text sortText;
    [SerializeField]
    private InputField wordInput;
    [SerializeField]
    private InputField definitionInput;

    [SerializeField]
    private RectTransform selectionMarker;

    private Dictionary<string, WordObject> activeWordObjects;
    private List<string> results;
    private Transform selectionMarkerHolder;
    private float singleWordHeight;

    private bool isSortAscending = true;

    private const string SORT_ASCENDING = "A ► Z";
    private const string SORT_DESCENDING = "Z ► A";
    private const string SORT_HEADER = "Sorting:\n";
    private const int WORDS_PER_HEIGHT = 12;

    private void Start()
    {
        results = new List<string>();
        activeWordObjects = new Dictionary<string, WordObject>();
        UpdateWords(false);

        selectionMarkerHolder = selectionMarker.parent;
        selectionMarker.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, wordsRect.rect.width);
        selectionMarker.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, singleWordHeight);
    }

    public void SortButtonAction()
    {
        isSortAscending = !isSortAscending;
        sortText.text = SORT_HEADER + ((isSortAscending) ? SORT_ASCENDING : SORT_DESCENDING);

        SortWords();
    }

    //Updates the word list to match the saved word database
    public void UpdateWords(bool newWordInput)
    {
        SizeWordsRect((newWordInput) ? 1 : DatabaseManager.ActiveDatabase.Count);

        foreach (string key in DatabaseManager.ActiveDatabase.Keys)
        {
            if(!activeWordObjects.ContainsKey(key))
            {
                //We create and initialize word objects with onLefClick action to display the definition and onRightClick to remove that wordObject along with its asociated database key
                WordObject wordObjectCache = Instantiate(wordPrefab, wordsRect).GetComponent<WordObject>();
                wordObjectCache.name = key;
                wordObjectCache.Initialize(() => ToggleDescription(key, wordObjectCache.transform), () => RemoveWord(key));
                activeWordObjects.Add(key, wordObjectCache);
            }
        }
    }

    //triggered onValueChanged of word input
    public void FilterSearch()
    {
        if(wordInput.text.Length > 0)
        {
            results = new List<string>();

            //Disable all word objects
            foreach (WordObject wordObj in activeWordObjects.Values)
            {
                wordObj.gameObject.SetActive(false);
            }

            //Find matching words
            foreach (string key in DatabaseManager.ActiveDatabase.Keys)
            {
                if(StringCompare(key, wordInput.text))
                {
                    results.Add(key);
                }
            }

            //Reactivate matching words
            foreach (string word in results)
            {
                activeWordObjects[word].gameObject.SetActive(true);
            }

            //Resize the container rect
            SizeWordsRect(results.Count);
        }
        else if (results.Count != DatabaseManager.ActiveDatabase.Count)
        {
            results = DatabaseManager.ActiveDatabase.Keys.ToList();

            foreach (string word in results)
            {
                activeWordObjects[word].gameObject.SetActive(true);
            }

            SizeWordsRect(results.Count);

        }

        //if no word are found matching the search string we trigger the word input popup pannel and move the selection marker off canvas
        if(results.Count.Equals(0))
        {
            wordInputManager.TriggerInputPopup(false);
            selectionMarker.SetParent(selectionMarkerHolder);
            definitionInput.text = string.Empty;
        }
        else
        {
            wordInputManager.DismissInputPopup(false);
        }
    }

    //Sorts the wordObjects and then updates their child index, the vertical layout manager then displays the gameobjects in the corect order
    private void SortWords()
    {
        List<WordObject> sortedChildren = (isSortAscending) ?
            activeWordObjects.Values.OrderBy(a => a.name).ToList() :
            activeWordObjects.Values.OrderByDescending(a => a.name).ToList();

        for (int i = 0; i < sortedChildren.Count; i++)
        {
            sortedChildren[i].transform.SetSiblingIndex(i);
        }
    }

    //Toggles the description in the description input field
    private void ToggleDescription(string word, Transform parent)
    {
        if (ActiveWord != word)
        {
            ActiveWord = word;
            definitionInput.text = DatabaseManager.ActiveDatabase[word];
            selectionMarker.SetParent(parent);
            selectionMarker.localPosition = Vector3.zero;
            selectionMarker.SetAsFirstSibling();
        }
        else
        {
            definitionInput.text = string.Empty;
            selectionMarker.SetParent(selectionMarkerHolder);
        }
    }

    private void RemoveWord(string key)
    {
        if(DatabaseManager.RemoveWord(key))
        {
            selectionMarker.SetParent(selectionMarkerHolder);
            Destroy(activeWordObjects[key].gameObject);
            activeWordObjects.Remove(key);
            FilterSearch();
        }
    }

    //Resizes the word rect transform to corect size based on the itemCount parameter
    private void SizeWordsRect(int itemCount)
    {
        singleWordHeight = referenceRect.rect.height / WORDS_PER_HEIGHT;
        wordsRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, itemCount * singleWordHeight);
    }

    //Makes both strings uppercase in order to ignore case sensitivity, returns true if string s1 contains string s2 (Edit: now checks character order as well)
    private bool StringCompare(string s1, string s2)
    {
        if(s1.ToUpper().Contains(s2.ToUpper()))
        {
            for (int i = 0; i < s2.Length; i++)
            {
                if(s1.ToUpper()[i] != s2.ToUpper()[i])
                {
                    return false;
                }
            }

            return true;
        }

        return false;
    }
}
