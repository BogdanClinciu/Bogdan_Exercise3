using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public List<SaveElement> wordsData;

    public SaveData(Dictionary<string,string> dictionary)
    {
        wordsData = new List<SaveElement>();

        foreach (KeyValuePair<string,string> kvp in dictionary)
        {
            wordsData.Add(new SaveElement(kvp.Key, kvp.Value));
        }
    }

    public Dictionary<string,string> ToDictionary()
    {
        Dictionary<string,string> returnDict = new Dictionary<string, string>();

        foreach(SaveElement element in wordsData)
        {
            returnDict.Add(element.word, element.desc);
        }

        return returnDict;
    }
}

[System.Serializable]
public class SaveElement
{
    public string word;
    public string desc;

    public SaveElement(string _word, string _desc)
    {
        word = _word;
        desc = _desc;
    }
}
