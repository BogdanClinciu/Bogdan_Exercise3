using System.Collections.Generic;
using UnityEngine;
using SaveDataTypes;

public class DatabaseManager : MonoBehaviour
{
    [SerializeField]
    private TextAsset defaultDatabase;

    public static Dictionary<string,string> ActiveDatabase {get; private set;}

    private static string path;
	private const string filename = "/wordsDatabase.json";
    private const string NO_VALUE_ERROR = "Description not found";


    private void Awake()
    {
        //set the path to the persistent application path.
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
    ///Convets the curently loaded database to a SaveData object and writes it to the persistent data path.
    ///</summary>
    public static void SaveDatabase()
    {
        SaveData data = new SaveData(ActiveDatabase);
		string contents = JsonUtility.ToJson (data, true);
		System.IO.File.WriteAllText (path, contents);
	}

    ///<summary>
    ///Loads a SaveData type from the persistent data path and convets it to a Dictionary of string string, if loading fails we load the default database and save that.
    ///</summary>
    private void LoadDatabase()
    {
        if (System.IO.File.Exists(path))
        {
            try
            {
				SaveData data = JsonUtility.FromJson<SaveData>(System.IO.File.ReadAllText(path));
                ActiveDatabase = data.ToDictionary();
			}
            catch (System.Exception ex)
            {
                // TODO: Add a "Error loading database"  UI message, with the posibility to load defaults or exit the application (also anounce the location of the database json)
                Debug.Log (ex.Message);
            }
        }
        else
        {
            try
            {
                // TODO: Add a "Unable to find word database file"  UI message, with the posibility to load defaults or exit the application (also anounce the location of the database json)
                Debug.Log("Unable to find word database file, writing default database");
                ActiveDatabase = JsonUtility.FromJson<SaveData>(defaultDatabase.ToString()).ToDictionary();
                SaveDatabase();
            }
            catch (System.Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
	}
}
		// try
        // {
		// 	if (System.IO.File.Exists(path))
        //     {
		// 		SaveData data = JsonUtility.FromJson<SaveData>(System.IO.File.ReadAllText(path));
        //         ActiveDatabase = data.ToDictionary();
		// 	}
        //     else
        //     {
        //         // TODO: Add a "Unable to find word database file"  UI message, with the posibility to load defaults or exit the application (also anounce the location of the database json)
		// 		Debug.Log("Unable to find word database file, writing default database");
        //         ActiveDatabase = JsonUtility.FromJson<SaveData>(defaultDatabase.ToString()).ToDictionary();
        //         SaveDatabase();
		// 	}
		// }

		// catch (System.Exception ex) {
        //     // TODO: Add a "Error loading database"  UI message, with the posibility to load defaults or exit the application (also anounce the location of the database json)
		// 	Debug.Log (ex.Message);
		// }
