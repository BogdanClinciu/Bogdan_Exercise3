using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


///<summary>
///Modified text mesh pro example:
///<seealso cref="TMP_TextSelector_B"/>
///</summary>
[RequireComponent(typeof(PopupHandler))]
public class WordSelector : MonoBehaviour
{
    public static string CurentLink {get; private set;} = string.Empty;

    [SerializeField]
    private Canvas mainCanvas;
    [SerializeField]
    private TextMeshProUGUI textMeshPro;

    [Header("TextColors")]
    [SerializeField]
    private Color32 existingUnselected;
    [SerializeField]
    private Color32 existingSelected;
    [SerializeField]
    private Color32 newUnselected;
    [SerializeField]
    private Color32 newSelected;

    private bool isHoveringObject;
    private int selectedWord = -1;
    private int lastIndex = -1;

    private Matrix4x4 matrix;
    private TMP_MeshInfo[] cachedMeshInfoVertexData;

    private static PopupHandler popupHandler;


    private void Awake()
    {
        popupHandler = GetComponent<PopupHandler>();
    }

    private void LateUpdate()
    {
        if (isHoveringObject)
        {
            //Check if Mouse intersects any words and if so assign a random color to that word.
            int wordIndex = TMP_TextUtilities.FindIntersectingWord(textMeshPro, Input.mousePosition, null);

            if (selectedWord != -1 && (wordIndex == -1 || wordIndex != selectedWord))
            {
                ClearTextSelection();
            }

            if (wordIndex != -1 && wordIndex != selectedWord)
            {
                HandleTextSelection(wordIndex);
            }
        }
        else
        {
            // Restore any character that may have been modified
            if (lastIndex != -1)
            {
                RestoreCachedVertexAttributes(lastIndex);
                lastIndex = -1;
            }

            //Clear link
            if (CurentLink != string.Empty)
            {
                CurentLink = string.Empty;
            }
        }
    }

    public void PointerEnter()
    {
        isHoveringObject = true;
    }

    public void PointerExit()
    {
        isHoveringObject = false;
        if (CurentLink != string.Empty)
        {
            CurentLink = string.Empty;
            ClearTextSelection();
        }
    }

    //Change the curent word link and toggle the popup group if not empty
    public static void TogglePopupCurentLink(string word)
    {
        CurentLink = word;
        popupHandler.TogglePopupPanel(CurentLink != string.Empty);
    }

    // Clear previous word selection.
    private void ClearTextSelection()
    {
        CurentLink = string.Empty;
        popupHandler.TogglePopupPanel(false);

        TMP_WordInfo wInfo = textMeshPro.textInfo.wordInfo[selectedWord];
        Color32 c;
        if(DatabaseManager.ActiveDatabase.ContainsKey(wInfo.GetWord().ToLower()) || wInfo.GetWord().Length <= 2)
        {
            c = existingUnselected;
        }
        else
        {
            c = newUnselected;
        }

        // Iterate through each of the characters of the word.
        for (int i = 0; i < wInfo.characterCount; i++)
        {
            int characterIndex = wInfo.firstCharacterIndex + i;

            // Get the index of the material / sub text object used by this character.
            int meshIndex = textMeshPro.textInfo.characterInfo[characterIndex].materialReferenceIndex;

            // Get the index of the first vertex of this character.
            int vertexIndex = textMeshPro.textInfo.characterInfo[characterIndex].vertexIndex;

            // Get a reference to the vertex color
            Color32[] vertexColors = textMeshPro.textInfo.meshInfo[meshIndex].colors32;

            //Color32 c = vertexColors[vertexIndex + 0].Tint(1.33333f);
            vertexColors[vertexIndex + 0] = c;
            vertexColors[vertexIndex + 1] = c;
            vertexColors[vertexIndex + 2] = c;
            vertexColors[vertexIndex + 3] = c;
        }
        // Update Geometry
        textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.All);

        selectedWord = -1;
    }

    private void HandleTextSelection(int wordIndex)
    {
        selectedWord = wordIndex;

        TMP_WordInfo wInfo = textMeshPro.textInfo.wordInfo[wordIndex];
        CurentLink = (wInfo.GetWord().Length > 2) ? wInfo.GetWord() : string.Empty;
        popupHandler.TogglePopupPanel(CurentLink != string.Empty);

        Color32 c = DatabaseManager.ActiveDatabase.ContainsKey(wInfo.GetWord().ToLower()) ? existingSelected : newSelected;

        if(DatabaseManager.ActiveDatabase.ContainsKey(wInfo.GetWord().ToLower()) || wInfo.GetWord().Length <= 2)
        {
            c = existingSelected;
        }
        else
        {
            c = newSelected;
        }

        // Iterate through each of the characters of the word.
        for (int i = 0; i < wInfo.characterCount; i++)
        {
            int characterIndex = wInfo.firstCharacterIndex + i;

            // Get the index of the material / sub text object used by this character.
            int meshIndex = textMeshPro.textInfo.characterInfo[characterIndex].materialReferenceIndex;

            int vertexIndex = textMeshPro.textInfo.characterInfo[characterIndex].vertexIndex;

            // Get a reference to the vertex color
            Color32[] vertexColors = textMeshPro.textInfo.meshInfo[meshIndex].colors32;

            //Color32 c = vertexColors[vertexIndex + 0].Tint(0.75f);
            vertexColors[vertexIndex + 0] = c;
            vertexColors[vertexIndex + 1] = c;
            vertexColors[vertexIndex + 2] = c;
            vertexColors[vertexIndex + 3] = c;
        }

        // Update Geometry
        textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
    }

    private void RestoreCachedVertexAttributes(int index)
    {
        if (index == -1 || index > textMeshPro.textInfo.characterCount - 1) return;

        // Get the index of the material / sub text object used by this character.
        int materialIndex = textMeshPro.textInfo.characterInfo[index].materialReferenceIndex;

        // Get the index of the first vertex of the selected character.
        int vertexIndex = textMeshPro.textInfo.characterInfo[index].vertexIndex;

        // Restore Vertices
        // Get a reference to the cached / original vertices.
        Vector3[] src_vertices = cachedMeshInfoVertexData[materialIndex].vertices;

        // Get a reference to the vertices that we need to replace.
        Vector3[] dst_vertices = textMeshPro.textInfo.meshInfo[materialIndex].vertices;

        // Restore / Copy vertices from source to destination
        dst_vertices[vertexIndex + 0] = src_vertices[vertexIndex + 0];
        dst_vertices[vertexIndex + 1] = src_vertices[vertexIndex + 1];
        dst_vertices[vertexIndex + 2] = src_vertices[vertexIndex + 2];
        dst_vertices[vertexIndex + 3] = src_vertices[vertexIndex + 3];

        // Restore Vertex Colors
        // Get a reference to the vertex colors we need to replace.
        Color32[] dst_colors = textMeshPro.textInfo.meshInfo[materialIndex].colors32;

        // Get a reference to the cached / original vertex colors.
        Color32[] src_colors = cachedMeshInfoVertexData[materialIndex].colors32;

        // Copy the vertex colors from source to destination.
        dst_colors[vertexIndex + 0] = src_colors[vertexIndex + 0];
        dst_colors[vertexIndex + 1] = src_colors[vertexIndex + 1];
        dst_colors[vertexIndex + 2] = src_colors[vertexIndex + 2];
        dst_colors[vertexIndex + 3] = src_colors[vertexIndex + 3];

        // Restore UV0S
        // UVS0
        Vector2[] src_uv0s = cachedMeshInfoVertexData[materialIndex].uvs0;
        Vector2[] dst_uv0s = textMeshPro.textInfo.meshInfo[materialIndex].uvs0;
        dst_uv0s[vertexIndex + 0] = src_uv0s[vertexIndex + 0];
        dst_uv0s[vertexIndex + 1] = src_uv0s[vertexIndex + 1];
        dst_uv0s[vertexIndex + 2] = src_uv0s[vertexIndex + 2];
        dst_uv0s[vertexIndex + 3] = src_uv0s[vertexIndex + 3];

        // UVS2
        Vector2[] src_uv2s = cachedMeshInfoVertexData[materialIndex].uvs2;
        Vector2[] dst_uv2s = textMeshPro.textInfo.meshInfo[materialIndex].uvs2;
        dst_uv2s[vertexIndex + 0] = src_uv2s[vertexIndex + 0];
        dst_uv2s[vertexIndex + 1] = src_uv2s[vertexIndex + 1];
        dst_uv2s[vertexIndex + 2] = src_uv2s[vertexIndex + 2];
        dst_uv2s[vertexIndex + 3] = src_uv2s[vertexIndex + 3];


        // Restore last vertex attribute as we swapped it as well
        int lastIndex = (src_vertices.Length / 4 - 1) * 4;

        // Vertices
        dst_vertices[lastIndex + 0] = src_vertices[lastIndex + 0];
        dst_vertices[lastIndex + 1] = src_vertices[lastIndex + 1];
        dst_vertices[lastIndex + 2] = src_vertices[lastIndex + 2];
        dst_vertices[lastIndex + 3] = src_vertices[lastIndex + 3];

        // Vertex Colors
        src_colors = cachedMeshInfoVertexData[materialIndex].colors32;
        dst_colors = textMeshPro.textInfo.meshInfo[materialIndex].colors32;
        dst_colors[lastIndex + 0] = src_colors[lastIndex + 0];
        dst_colors[lastIndex + 1] = src_colors[lastIndex + 1];
        dst_colors[lastIndex + 2] = src_colors[lastIndex + 2];
        dst_colors[lastIndex + 3] = src_colors[lastIndex + 3];

        // UVS0
        src_uv0s = cachedMeshInfoVertexData[materialIndex].uvs0;
        dst_uv0s = textMeshPro.textInfo.meshInfo[materialIndex].uvs0;
        dst_uv0s[lastIndex + 0] = src_uv0s[lastIndex + 0];
        dst_uv0s[lastIndex + 1] = src_uv0s[lastIndex + 1];
        dst_uv0s[lastIndex + 2] = src_uv0s[lastIndex + 2];
        dst_uv0s[lastIndex + 3] = src_uv0s[lastIndex + 3];

        // UVS2
        src_uv2s = cachedMeshInfoVertexData[materialIndex].uvs2;
        dst_uv2s = textMeshPro.textInfo.meshInfo[materialIndex].uvs2;
        dst_uv2s[lastIndex + 0] = src_uv2s[lastIndex + 0];
        dst_uv2s[lastIndex + 1] = src_uv2s[lastIndex + 1];
        dst_uv2s[lastIndex + 2] = src_uv2s[lastIndex + 2];
        dst_uv2s[lastIndex + 3] = src_uv2s[lastIndex + 3];

        // Need to update the appropriate
        textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
    }
}
