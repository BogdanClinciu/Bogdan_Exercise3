# Bogdan_Exercise3
Exercise requirement:
Using dictionaries, build an English dictionary application with the following capabilities:
- Allow the user to search for a word and show the definition after the word is selected
- Allow the user to add a new word with a definition
- Allow the user to remove a word and definition
- Allow the user to edit a words definition
- Show all words A-Z
- Show all words Z-A
- Data and changes should be persistent
 
Feel free to design the UI as you see fit.

- Extra: valid words appear as the user types (auto complete feature)
- Extra 2: FAST ACCESS : When you click/select a word that’s in the dictionary, a small popup should appear with its definition. If the word is not present in the dictionary, the popup should allow you to add a definition for it.


Implementation:
The chosen key-value-pair was string-string, with the key being the word itself and the value the definition for that word.

For saving and loading the data DatabaseManager.cs is used along with the JsonUtility, however, Unity/JsonUtility cannot serialize Dictionaries so a proxy serializable class was needed in order to save the data. The SaveDataTypes namespace does exactly this, containing a serializable class SaveData.cs with a constructor to create one using the appropriate dictionary data and a .ToDictionary(SaveData sd) method returning the desired populated dictionary.

The Database manager is also used to find whether or not a word or description exists within our currently loaded database, and Add/Remove/Modify functionality for single words.

In order to display words a prefab with WordObject.cs has been created. This allows for assigning functionality at instantiate. In order to keep a reference to active word objects they are stored in a string-WordObject dictionary, this allows us to easily enable/disable them and prevent instantiate if a GameObject with that key(which in this case is the word) already exists. 

Using that data WordHandler.cs is used to create the data for, filter, sort and select words.
Word object instantiate happens(as described above) here at Start() and when a new word is added.

Filtering is done based on the string value of the search/word input field by comparing the input value against all key entries in our current word objects, is they match they remain enables and otherwise are disabled.  This feature also includes the auto-complete requirement displaying only words whose character order also matches the search word.

Sorting is done by creating a sorted list(sorted by the associated key of each existing word object) and reordering the word object transforms within the hierarchy of their parent based on the newly obtained sorted list.

Modification of the word database is handled by WordHandler.cs, which is used to either add a completely new word with its definition, remove a word, or modify the definition of an existing word.

The UI_Manager handles most ui functionality for the main application screen and for the modification/creation panel.

Bonus Tasks:
- Autocomplete has been integrated into the search functionality as mentioned above.
- The fast access bonus task has been completed using the TextMeshPro unity package.
UnknownWordManager.cs highlights words missing from the database whenever a word is selected. WordSelector.cs handles word selection highlighting and updating the highlighted word for use with the PopupHandler that displays a quick view of the word and its definition, if any.


