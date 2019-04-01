using UnityEngine;
using TMPro;
using System.Collections;

///<summary>
///Handles creating the text links every time we select a word from the word panel
///</summary>
public class UnknownWordManager : MonoBehaviour
{
    [SerializeField]
    private Color32 highlightColor;

    private TMP_TextInfo textInfoCache;
    private TMP_WordInfo wordInfoCache;


    ///<summary>
    ///Applies word color highlights to <paramref name="definitionsText"/>'s words that are not contained by the word database.
    ///</summary>
    public void HilightUnknownWords(TMP_Text definitionsText)
    {
        StartCoroutine(HighlighWords(definitionsText));
    }

    private IEnumerator HighlighWords(TMP_Text definitionsText) {
        yield return null;
        textInfoCache = definitionsText.textInfo;

        if(definitionsText.text.Length > 0)
        {
            textInfoCache = definitionsText.textInfo;


            for (int i = 0; i < textInfoCache.wordCount; i++)
            {
                wordInfoCache = textInfoCache.wordInfo[i];

                if(wordInfoCache.GetWord().Length > 2 && !DatabaseManager.ActiveDatabase.ContainsKey(wordInfoCache.GetWord().ToLower()))
                {
                    yield return new WaitForSeconds(0.05f);
                    // Iterate through each of the characters of the word.
                    for (int j = 0; j < wordInfoCache.characterCount; ++j)
                    {
                        int charIndex = wordInfoCache.firstCharacterIndex + j;
                        int meshIndex = definitionsText.textInfo.characterInfo[charIndex].materialReferenceIndex;
                        int vertexIndex = definitionsText.textInfo.characterInfo[charIndex].vertexIndex;

                        Color32[] vertexColors = definitionsText.textInfo.meshInfo[meshIndex].colors32;
                        vertexColors[vertexIndex + 0] = highlightColor;
                        vertexColors[vertexIndex + 1] = highlightColor;
                        vertexColors[vertexIndex + 2] = highlightColor;
                        vertexColors[vertexIndex + 3] = highlightColor;
                    }
                }
                definitionsText.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
            }
        }
    }
}
