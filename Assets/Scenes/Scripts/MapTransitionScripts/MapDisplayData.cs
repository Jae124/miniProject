// Put this class definition either inside StageSelectionManager.cs (above the main class)
// or in its own separate C# file (e.g., MapDisplayData.cs)
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic; // Needed for List
using System.Linq; // Optional: For easier searching with Linq


[System.Serializable] // Make it visible in the Inspector
public class MapDisplayData
{
    public string mapID;             // e.g., "ForestMap", "DesertMap"
    public Sprite backgroundSprite;  // The background image for this map
    public List<Vector2> stageButtonPositions; // The specific X,Y Anchored Positions for this map's buttons
}