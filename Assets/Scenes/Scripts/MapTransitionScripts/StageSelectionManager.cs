// StageSelectionManager.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic; // Needed for List
using System.Linq; // Optional: For easier searching with Linq

// [System.Serializable] // MapDisplayData class defined here or in its own file
// public class MapDisplayData { ... }

public class StageSelectionManager : MonoBehaviour
{
    public GameObject stageButtonPrefab;    // Drag Prefab here
    public Transform buttonContainer;      // Drag StageButtonContainer GameObject here
    public Text mapTitleText;             // Drag Title Text here
    public Image mapBackgroundComponent;   // << NEW: Drag the UI Image for the background here
    public int stagesPerMap = 10;

    // --- NEW: List to hold data for all maps ---
    // Configure this list in the Inspector!
    public List<MapDisplayData> allMapDisplayData;

    private string currentMapID;
    private MapDisplayData currentMapData; // To store the data for the selected map

    void Start()
    {
        // --- Get Map ID (same as before) ---
        if (SaveManager.Instance != null) {
            currentMapID = SaveManager.Instance.CurrentSelectedMapID;
            if (string.IsNullOrEmpty(currentMapID)) {
                Debug.LogError("CurrentSelectedMapID not set! Returning to Map Selection.");
                SceneManager.LoadScene("MapSelectionScene");
                return;
            }
        } else {
            Debug.LogError("SaveManager instance not found!"); return;
        }

        // --- Find the Display Data for the Current Map ---
        currentMapData = allMapDisplayData.FirstOrDefault(mapData => mapData.mapID == currentMapID);
        // Alternative without Linq:
        // foreach(MapDisplayData data in allMapDisplayData) {
        //     if (data.mapID == currentMapID) {
        //         currentMapData = data;
        //         break;
        //     }
        // }

        if (currentMapData == null)
        {
            Debug.LogError($"Display data for map ID '{currentMapID}' not found in allMapDisplayData list! Check Inspector setup.");
            // Maybe load a default map or return to map select
            SceneManager.LoadScene("MapSelectionScene");
            return;
        }

        // --- Check Button Position Count ---
        if (currentMapData.stageButtonPositions == null || currentMapData.stageButtonPositions.Count != stagesPerMap)
        {
            Debug.LogError($"Button Positions list for map '{currentMapID}' not set correctly! Expected {stagesPerMap}, found {currentMapData.stageButtonPositions?.Count ?? 0}.");
            return;
        }

        // --- Apply Map-Specific Background ---
        if (mapBackgroundComponent != null)
        {
            if (currentMapData.backgroundSprite != null) {
                mapBackgroundComponent.sprite = currentMapData.backgroundSprite;
            } else {
                Debug.LogWarning($"Background sprite for map '{currentMapID}' is not assigned in the Inspector.");
                // Optionally set a default background color or sprite
                // mapBackgroundComponent.sprite = null;
                // mapBackgroundComponent.color = Color.grey;
            }
        } else {
            Debug.LogError("Map Background Component is not assigned in the Inspector!");
        }


        // --- Update Title (same as before) ---
        if (mapTitleText != null) { mapTitleText.text = GetFormattedMapName(currentMapID); }

        PopulateStages();
    }

    void PopulateStages()
    {
        // --- Clear existing buttons (same as before) ---
        foreach (Transform child in buttonContainer) { Destroy(child.gameObject); }

        // --- Get Progress (same as before) ---
        int highestStageCleared = 0;
        if (SaveManager.Instance != null) { highestStageCleared = SaveManager.Instance.playerProgress.GetHighestStageForMap(currentMapID); }

        // --- Instantiate and POSITION buttons using CURRENT MAP'S layout ---
        if (currentMapData == null) // Safety check in case Start failed silently
        {
             Debug.LogError("Cannot populate stages because currentMapData is null.");
             return;
        }

        for (int i = 0; i < stagesPerMap; i++)
        {
            GameObject buttonGO = Instantiate(stageButtonPrefab, buttonContainer);
            StageButton stageButton = buttonGO.GetComponent<StageButton>();
            RectTransform buttonRect = buttonGO.GetComponent<RectTransform>();

            if (stageButton != null && buttonRect != null)
            {
                // --- Set Position using data for the CURRENT map ---
                // Check index bounds just in case list size check in Start failed somehow
                if (i < currentMapData.stageButtonPositions.Count) {
                     buttonRect.anchoredPosition = currentMapData.stageButtonPositions[i];
                } else {
                     Debug.LogError($"Index {i} out of bounds for stageButtonPositions on map {currentMapID}!");
                     // Position at center or hide?
                     buttonRect.anchoredPosition = Vector2.zero;
                }


                // --- Determine Lock State (same as before) ---
                bool isLocked = i > highestStageCleared;

                // --- Setup Button (same as before) ---
                stageButton.Setup(currentMapID, i, isLocked);
            }
            else
            {
                Debug.LogError($"Prefab missing StageButton script or RectTransform at index {i}!");
                Destroy(buttonGO);
            }
        }
    }

    // --- GetFormattedMapName (same as before) ---
    string GetFormattedMapName(string mapID)
    {
        switch (mapID)
        {
            case "ForestMap": return "Forest of Shadows";
            case "OceanMap": return "Scorched Dunes";
            case "VolcanoMap": return "Inferno Core";
            default: return "Unknown Map";
        }
    }

    // --- GoToMapSelection (same as before) ---
    public void GoToMapSelection()
    {
        SceneManager.LoadScene("MapSelectionScene");
    }}