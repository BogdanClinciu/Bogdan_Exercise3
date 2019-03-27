using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class WordHandler : MonoBehaviour
{
    [SerializeField]
    private RectTransform referenceRect;
    [SerializeField]
    private RectTransform wordsRect;
    [SerializeField]
    private GameObject wordPrefab;

    [SerializeField]
    private InputField wordInput;


    [SerializeField]
    private InputField editWordInput;
    [SerializeField]
    private InputField editDefInput;

    public static string CurentWordInput {get; private set;} = string.Empty;

    private UI_Manager uI_Manager;
    private Dictionary<string, WordObject> activeWordObjects;
    private List<string> results;
    private float singleWordHeight;
    private bool isSortAscending = false;

    private const int WORDS_PER_HEIGHT = 12;


    private void Start()
    {
        uI_Manager = GetComponent<UI_Manager>();
        activeWordObjects = new Dictionary<string, WordObject>();

        //we poulate the word list at start with the entire database
        UpdateWordPanel();
    }

    #region Button Actions

        public void Sort()
        {
            isSortAscending = !isSortAscending;
            uI_Manager.UpdateSortButtonText(isSortAscending);
            SortWords();
        }

        //saves the edit done in the edit panel
        public void SaveEdit()
        {
            DatabaseManager.AddWord(editWordInput.text.ToLower(), editDefInput.text);
            UpdateWordPanel();
        }

        public void RemoveWord()
        {
            string wordToRemove = UI_Manager.ActiveWordObject.word;
            DatabaseManager.RemoveWord(wordToRemove);
            Destroy(activeWordObjects[wordToRemove].gameObject);
            activeWordObjects.Remove(wordToRemove);
            results.Remove(wordToRemove);
            SizeWordsRect(results.Count);
            UI_Manager.SetActiveWord(null);
        }

    #endregion


    //triggered onValueChanged of word input
    public void FilterSearch()
    {
        CurentWordInput = wordInput.text;
        if(UI_Manager.ActiveWordObject != null)
        {
            UI_Manager.SetActiveWord(null);
        }

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
    }

    //Updates the word list to match the saved word database
    private void UpdateWordPanel()
    {
        foreach (string key in DatabaseManager.ActiveDatabase.Keys)
        {
            if(!activeWordObjects.ContainsKey(key))
            {
                //We create and initialize word objects
                WordObject wordObjectCache = Instantiate(wordPrefab, wordsRect).GetComponent<WordObject>();
                wordObjectCache.SetWord(key);
                activeWordObjects.Add(key, wordObjectCache);
            }
        }

        results = DatabaseManager.ActiveDatabase.Keys.ToList();
        SizeWordsRect(results.Count);
    }

    //Sorts the wordObjects and then updates their child index, the vertical layout manager then displays the gameobjects in the corect order
    private void SortWords()
    {
        List<WordObject> sortedChildren = (isSortAscending) ?
            activeWordObjects.Values.OrderBy(a => a.word).ToList() :
            activeWordObjects.Values.OrderByDescending(a => a.word).ToList();

        for (int i = 0; i < sortedChildren.Count; i++)
        {
            sortedChildren[i].transform.SetSiblingIndex(i);
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
