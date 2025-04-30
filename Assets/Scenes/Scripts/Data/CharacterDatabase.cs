// CharacterDatabase.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Needed for Linq searches like FirstOrDefault

public static class CharacterDatabase // Static class - no instance needed
{
    private static Dictionary<string, CharacterData> _database; // Store by characterID
    private static bool _isInitialized = false;

    // Folder within any "Resources" folder where the CharacterData assets are stored
    private const string CHARACTER_DATA_PATH = "CharacterData";

    // Call this once early in your game (e.g., from a loading scene or initial manager)
    public static void Initialize()
    {
        if (_isInitialized) return; // Already initialized

        _database = new Dictionary<string, CharacterData>();

        // Load all CharacterData assets from the Resources/CharacterData folder
        CharacterData[] allData = Resources.LoadAll<CharacterData>(CHARACTER_DATA_PATH);

        if (allData.Length == 0) {
             Debug.LogError($"No CharacterData assets found in Resources/{CHARACTER_DATA_PATH}! Make sure the folder exists and assets are inside.");
        }

        foreach (CharacterData data in allData)
        {
            if (data != null && !string.IsNullOrEmpty(data.characterID))
            {
                if (!_database.ContainsKey(data.characterID))
                {
                    _database.Add(data.characterID, data);
                }
                else
                {
                    Debug.LogWarning($"Duplicate CharacterID found in CharacterData assets: {data.characterID}. Ignoring duplicate named {data.name}.");
                }
            } else if (data != null) {
                 Debug.LogWarning($"CharacterData asset named '{data.name}' is missing a CharacterID.");
            }
        }

        _isInitialized = true;
        Debug.Log($"Character Database Initialized. Loaded {_database.Count} character entries.");
    }

    // Get data for a specific character
    public static CharacterData GetCharacterData(string characterID)
    {
        // Ensure initialized (lazy init - alternatively, call Initialize() explicitly at game start)
        if (!_isInitialized) Initialize();

        if (_database.TryGetValue(characterID, out CharacterData data))
        {
            return data;
        }
        Debug.LogError($"Character Data not found for ID: {characterID}");
        return null; // Return null if not found
    }

    // Get a list of all defined character IDs
    public static List<string> GetAllCharacterIDs()
    {
        if (!_isInitialized) Initialize();
        return new List<string>(_database.Keys);
    }

     // Get all loaded CharacterData objects
    public static List<CharacterData> GetAllCharacterData()
    {
        if (!_isInitialized) Initialize();
        return new List<CharacterData>(_database.Values);
    }
}