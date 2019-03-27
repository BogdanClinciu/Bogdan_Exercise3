using UnityEngine;
using TMPro;
using System.Collections;

public class UnknownWordManager : MonoBehaviour
{
    private TMP_TextInfo textInfo;
    private TMP_WordInfo wordInfo;

    private const string LINK_BEGIN = "<link=\""; //insert ID (int) here
    private const string LINK_CLOSE = "\">";
    private const string LINK_END = "</link>";

    private const string COLOR_BEGIN = "<#72a6f9>"; //insert hex code color here
    private const string COLOR_END = "</color>";

    private const string SPACE = " ";

    public void CreateDefinitionLinks(TMP_Text definitionsText)
    {
        StartCoroutine(Timer(definitionsText));
    }

    private IEnumerator Timer(TMP_Text definitionsText)
    {
        yield return null;
        string linkedText = string.Empty;

        if(definitionsText.text.Length > 0)
        {
            textInfo = definitionsText.textInfo;
            for (int i = 0; i < textInfo.wordCount; i++)
            {
                wordInfo = textInfo.wordInfo[i];

                if(wordInfo.GetWord().Length > 2 && !DatabaseManager.ActiveDatabase.ContainsKey(wordInfo.GetWord().ToLower()))
                {
                    linkedText += LINK_BEGIN + wordInfo.GetWord() + LINK_CLOSE + COLOR_BEGIN +
                        wordInfo.GetWord() + COLOR_END + LINK_END + SPACE;
                }
                else
                {
                    linkedText += wordInfo.GetWord() + SPACE;
                }
            }
            definitionsText.text = linkedText;
        }
    }
}
