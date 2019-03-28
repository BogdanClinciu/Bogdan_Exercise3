using UnityEngine;
using TMPro;
using System.Collections;

///<summary>
///Handles creating the text links every time we select a word from the word panel
///</summary>
public class UnknownWordManager : MonoBehaviour
{
    private TMP_TextInfo textInfoCache;
    private TMP_WordInfo wordInfoCache;

    #region Constant Strings
        private const string LINK_BEGIN = "<link=\""; //insert ID (int) here
        private const string LINK_CLOSE = "\">";
        private const string LINK_END = "</link>";
        private const string COLOR_BEGIN = "<#72a6f9>"; //insert hex code color here
        private const string COLOR_END = "</color>";
        private const string SPACE = " ";
    #endregion


    ///<summary>
    ///Creates text link markup for <paramref name="definitionsText"/> and then applies changes to the text object's text property.
    ///</summary>
    public void CreateTextMarkup(TMP_Text definitionsText)
    {
        //The coroutine here is used to delay the formating (presumably because either unity or text mesh pro processes occur at late update, so we skip the curent frame)
        StartCoroutine(CreateMarkup(definitionsText));
    }

    private IEnumerator CreateMarkup(TMP_Text definitionsText)
    {
        yield return null;
        string linkedText = string.Empty;

        if(definitionsText.text.Length > 0)
        {
            textInfoCache = definitionsText.textInfo;
            for (int i = 0; i < textInfoCache.wordCount; i++)
            {
                wordInfoCache = textInfoCache.wordInfo[i];

                if(wordInfoCache.GetWord().Length > 2 && !DatabaseManager.ActiveDatabase.ContainsKey(wordInfoCache.GetWord().ToLower()))
                {
                    linkedText += LINK_BEGIN + wordInfoCache.GetWord() + LINK_CLOSE + COLOR_BEGIN +
                        wordInfoCache.GetWord() + COLOR_END + LINK_END + SPACE;
                }
                else
                {
                    linkedText += wordInfoCache.GetWord() + SPACE;
                }
            }
            definitionsText.text = linkedText;
        }
    }
}
