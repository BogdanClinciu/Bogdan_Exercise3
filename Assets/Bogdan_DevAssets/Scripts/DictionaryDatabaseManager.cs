using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DictionaryDatabaseManager : MonoBehaviour
{
    public static WordDatabase ActiveDatabase {get; private set;}

    private static string path;
	private const string filename = "/wordsDatabase.json";

    private void Awake()
    {
		path = Application.persistentDataPath + filename;
		LoadDatabase();
	}

    public static void AddWord(string word, string definition)
    {
        if(ActiveDatabase.Data.ContainsKey(word))
        {
            ActiveDatabase.Data[word] = definition;
        }
        else
        {
            ActiveDatabase.Data.Add(word, definition);
        }
    }

    public static void RemoveWord(string word)
    {
        if(ActiveDatabase.Data.ContainsKey(word))
        {
            ActiveDatabase.Data.Remove(word);
        }
        else
        {
            Debug.Log("Trying to remove a word that does not exist");
        }
    }

    public static bool WordExistsInDatabase(string word)
    {
        return ActiveDatabase.Data.ContainsKey(word);
    }

    public static bool WordHasDefinition(string word)
    {
        return WordExistsInDatabase(word) && !ActiveDatabase.Data[word].Equals(string.Empty);
    }

    public static void SaveDatabase()
    {
		string contents = JsonUtility.ToJson (ActiveDatabase, true);
		System.IO.File.WriteAllText (path, contents);
		Debug.Log ("SavedData");
	}

    private void LoadDatabase()
    {
		try
        {
			if (System.IO.File.Exists(path))
            {
				string content = System.IO.File.ReadAllText(path);
				ActiveDatabase = JsonUtility.FromJson<WordDatabase>(content);
			}
            else
            {
				Debug.Log("Unable to read input file, creating blank database");
				ActiveDatabase = new WordDatabase();
                SaveDatabase();
			}
		}

		catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}
}
