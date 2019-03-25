using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordsPanelManager : MonoBehaviour
{
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

    private Dictionary<string, WordObject> activeWordObjects;
    private float singleWordHeight;

    private const int WORDS_PER_HEIGHT = 12;

    private void Start()
    {
        activeWordObjects = new Dictionary<string, WordObject>();
        SizeWordsRect();
        UpdateWords();
    }

    public void SortButtonAction()
    {
        /*  Case based (check on word frield change)
                if word or part of word Exitsts || input field is empty > toggle sort type and sort

                else "+" (add word)
                    while description input field is null maker red; check on decription field change
         */

         DictionaryDatabaseManager.AddWord(wordInput.text, definitionInput.text);
    }

    private void UpdateWords()
    {
        Debug.Log("Updating words");
        Debug.Log(DictionaryDatabaseManager.ActiveDatabase.Count);
        foreach (string key in DictionaryDatabaseManager.ActiveDatabase.Keys)
        {
            Debug.Log(key);
            if(!activeWordObjects.ContainsKey(key))
            {
                WordObject wordObjectCache = Instantiate(wordPrefab, wordsRect).GetComponent<WordObject>();
                wordObjectCache.name = key;
                wordObjectCache.Initialize();
                activeWordObjects.Add(key, wordObjectCache);
            }
        }
    }

    private void SizeWordsRect()
    {
        singleWordHeight = referenceRect.rect.height / WORDS_PER_HEIGHT;
        wordsRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, DictionaryDatabaseManager.ActiveDatabase.Count * singleWordHeight);
    }
}
