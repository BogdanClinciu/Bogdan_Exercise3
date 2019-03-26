using System.Collections.Generic;
using UnityEngine;

public class DictionaryDatabaseManager : MonoBehaviour
{
    public static Dictionary<string,string> ActiveDatabase {get; private set;}

    private static string path;
	private const string filename = "/wordsDatabase.json";
    private const string NO_VALUE_ERROR = "Description not found";

    private void Awake()
    {
		path = Application.persistentDataPath + filename;
		LoadDatabase();
	}

    public static void AddWord(string word, string definition)
    {
        if(ActiveDatabase.ContainsKey(word))
        {
            ActiveDatabase[word] = definition;
        }
        else
        {
            ActiveDatabase.Add(word, definition);
        }
        SaveDatabase();
    }

    public static bool RemoveWord(string word)
    {
        if(ActiveDatabase.ContainsKey(word))
        {
            ActiveDatabase.Remove(word);
            SaveDatabase();
            return true;
        }
        else
        {
            Debug.Log("Trying to remove a word that does not exist");
            return false;
        }
    }

    public static bool WordExistsInDatabase(string word)
    {
        return ActiveDatabase.ContainsKey(word);
    }

    public static bool WordHasDefinition(string word)
    {
        return WordExistsInDatabase(word) && !ActiveDatabase[word].Equals(string.Empty);
    }

    public static string GetDefinition(string wordKey)
    {
        if(DictionaryDatabaseManager.ActiveDatabase.TryGetValue(wordKey, out string result))
        {
            return result;
        }

        return NO_VALUE_ERROR;
    }

    public static void SaveDatabase()
    {
        SaveData data = new SaveData(ActiveDatabase);
		string contents = JsonUtility.ToJson (data, true);
		System.IO.File.WriteAllText (path, contents);
		Debug.Log ("SavedData ");
	}

    private void LoadDatabase()
    {
		try
        {
			if (System.IO.File.Exists(path))
            {
				SaveData data = JsonUtility.FromJson<SaveData>(System.IO.File.ReadAllText(path));
                ActiveDatabase = data.ToDictionary();
                Debug.Log("Active database loaded");
			}
            else
            {
				Debug.Log("Unable to read input file, creating blank database");
				ActiveDatabase = new Dictionary<string, string>();
                SaveDatabase();
			}
		}

		catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}
}
