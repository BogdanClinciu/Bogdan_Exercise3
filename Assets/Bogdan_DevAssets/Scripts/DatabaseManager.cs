using System.Collections.Generic;
using UnityEngine;
using SaveDataTypes;

public class DatabaseManager : MonoBehaviour
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

    ///<summary>
    ///Add a word to the dictionary database, if the dictionary already contains the word we update the definition of that <paramref name="wordKey"/>
    ///</summary>
    public static void AddWord(string wordKey, string definitionValue)
    {
        if(ActiveDatabase.ContainsKey(wordKey))
        {
            ActiveDatabase[wordKey] = definitionValue;
        }
        else
        {
            ActiveDatabase.Add(wordKey, definitionValue);
        }
        SaveDatabase();
    }


    ///<summary>
    ///Remove at key <paramref name="wordKey"/> from the dictionary database
    ///</summary>
    public static bool RemoveWord(string wordKey)
    {
        if(ActiveDatabase.ContainsKey(wordKey))
        {
            ActiveDatabase.Remove(wordKey);
            SaveDatabase();
            return true;
        }
        else
        {
            Debug.Log("Trying to remove a word that does not exist");
            return false;
        }
    }

    ///<summary>
    ///Returns true if <paramref name="wordKey"/> definition is not empty within the dictionary database, and if <paramref name="wordKey"/> exists.
    ///</summary>
    public static bool WordHasDefinition(string wordKey)
    {
        return ActiveDatabase.ContainsKey(wordKey) && !ActiveDatabase[wordKey].Equals(string.Empty);
    }

    ///<summary>
    ///Convets the curently loaded database to a SaveData object and writes it to the persistent data path.
    ///</summary>
    public static void SaveDatabase()
    {
        SaveData data = new SaveData(ActiveDatabase);
		string contents = JsonUtility.ToJson (data, true);
		System.IO.File.WriteAllText (path, contents);
	}

    ///<summary>
    ///Loads a SaveData type from the persistent data path and convets it to a Dictionary of string string, if loading fails it creates the file and initializes and empty dictionary.
    ///</summary>
    private void LoadDatabase()
    {
		try
        {
			if (System.IO.File.Exists(path))
            {
				SaveData data = JsonUtility.FromJson<SaveData>(System.IO.File.ReadAllText(path));
                ActiveDatabase = data.ToDictionary();
                //Debug.Log("Active database loaded");
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

    //        ActiveDatabase = JsonUtility.FromJson<SaveData>(Resources.Load<TextAsset>(path).text).ToDictionary();
}
