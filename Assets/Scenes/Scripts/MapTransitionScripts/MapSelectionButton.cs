// MapSelectionButton.cs
using UnityEngine;
using UnityEngine.SceneManagement; // Needed for scene loading
using UnityEngine.UI; // Needed for Button component

public class MapSelectionButton : MonoBehaviour
{
    // --- SET THIS IN THE INSPECTOR FOR EACH BUTTON ---
    public string mapID; // e.g., "ForestMap", "DesertMap"

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnMapSelected); // Add listener for clicks
        }

        // Optional: Disable button if the map isn't unlocked yet based on game rules
        // bool isUnlocked = CheckIfMapIsUnlocked(mapID);
        // button.interactable = isUnlocked;
    }

    void OnMapSelected()
    {
        Debug.Log($"Map Selected: {mapID}");

        // Store the selected map ID in the SaveManager (or a dedicated Level Manager)
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.CurrentSelectedMapID = mapID;

            // Load the Stage Selection Scene
            SceneManager.LoadScene("StageSelectionScene"); // << USE THE EXACT NAME OF YOUR STAGE SCENE
        }
        else
        {
            Debug.LogError("SaveManager instance not found! Cannot proceed.");
        }
    }

    // Optional: Add logic here if maps need to be unlocked sequentially
    // bool CheckIfMapIsUnlocked(string checkMapID) { ... return true/false ... }

    void OnDestroy() // Clean up listener
    {
         if (button != null) { button.onClick.RemoveListener(OnMapSelected); }
    }
}