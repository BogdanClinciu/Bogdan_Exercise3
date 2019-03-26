using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SaveDataTypes
{
    ///<summary>
    ///Class containing list of serializable SaveElements used as a proxy to save and load the string-string type dictionary used to hold the dictionary data.
    ///<seealso cref="SaveElement.cs"/>
    ///</summary>
    [System.Serializable]
    public class SaveData
    {
        public List<SaveElement> wordsData;

        ///<summary>
        ///Creates a new SaveData object with the SaveElements matching those if the input <paramref name="dictionary"/>.
        ///<seealso cref="SaveElement.cs"/>
        ///</summary>
        public SaveData(Dictionary<string,string> dictionary)
        {
            wordsData = new List<SaveElement>();

            foreach (KeyValuePair<string,string> kvp in dictionary)
            {
                wordsData.Add(new SaveElement(kvp.Key, kvp.Value));
            }
        }

        ///<summary>
        ///Converts a SaveData object to a dictionary.
        ///</summary>
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

    ///<summary>
    ///Class conaining an emulated and serializable key value pair proxy for use with the SaveData object
    ///<seealso cref="SaveData.cs"/>
    ///</summary>
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
}
