using UnityEngine;
using TMPro;
using System.Collections;

public class UnknownWordManager : MonoBehaviour
{
    [SerializeField]
    private bool disableRun;
    [SerializeField]
    private TMP_Text definitionsText;
    [SerializeField]
    private TMP_InputField definitionsInput;

    private TMP_TextInfo textInfo;
    private TMP_WordInfo wordInfo;

    private const string LINK_BEGIN = "<link=\"ID_"; //insert ID (int) here
    private const string LINK_CLOSE = "\">";
    private const string LINK_END = "</link>";

    private const string COLOR_BEGIN = "<#72a6f9>"; //insert hex code color here
    private const string COLOR_END = "</color>";

    private const string SPACE = " ";



    public void CreateDefinitionLinks()
    {
        if(definitionsInput.text.Contains(LINK_END) || disableRun)
        {
            return;
        }
        StopAllCoroutines();
        StartCoroutine(LoopText());
    }

    private IEnumerator LoopText()
    {
        yield return new WaitForSeconds(1);
        string linkedText = string.Empty;
        if(definitionsText.text.Length > 0)
        {
            textInfo = definitionsText.textInfo;
            for (int i = 0; i < textInfo.wordCount; i++)
            {
                wordInfo = textInfo.wordInfo[i];

                if(wordInfo.GetWord().Length > 2 && !DatabaseManager.ActiveDatabase.ContainsKey(wordInfo.GetWord().ToLower()))
                {
                    //Debug.Log(wordInfo.GetWord());

                    linkedText += LINK_BEGIN + i.ToString("#00") + LINK_CLOSE + COLOR_BEGIN +
                        wordInfo.GetWord() + COLOR_END + LINK_END + SPACE;
                }
                else
                {
                    linkedText += wordInfo.GetWord() + SPACE;
                }
            }

            definitionsInput.text = linkedText;
        }
    }

    //See the <link="ID_01"><u><#40A0FF>online documentation</color></u></link>
    //for more information <u><link="ID_02">about this product</link></u> \U0001F60A
}
