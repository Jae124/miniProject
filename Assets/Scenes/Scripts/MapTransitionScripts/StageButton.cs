// StageButton.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageButton : MonoBehaviour
{
    public Text stageNumberText; // Assign in Prefab Inspector
    public GameObject lockIcon;   // Assign Lock Image GameObject in Prefab Inspector (optional)
    // Add other UI elements like star images if needed

    private Button button;
    private string mapID;
    private int stageIndex; // The index within this map (0-9)

    void Awake()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnStageSelected);
        }
    }

    // This method will be called by the StageSelectionManager to set up each button
    public void Setup(string currentMapID, int index, bool isLocked, int starsEarned = 0)
    {
        mapID = currentMapID;
        stageIndex = index;

        if (stageNumberText != null)
        {
            stageNumberText.text = (stageIndex + 1).ToString(); // Display 1-10
        }

        // Handle lock state
        bool interactable = !isLocked;
        if (button != null) button.interactable = interactable;
        if (lockIcon != null) lockIcon.SetActive(isLocked);

        // Handle stars display (example)
        // ShowStars(starsEarned);
    }

    void OnStageSelected()
    {
        Debug.Log($"Stage Selected: Map '{mapID}', Stage Index {stageIndex} (Level {(stageIndex + 1)})");

        // Store selected stage info (optional, could also pass via SceneManager if needed)
        // If your GameplayScene needs this info, store it in SaveManager or another persistent manager
        if (SaveManager.Instance != null) {
            // Optionally store stageIndex if GameplayScene needs it directly
            // SaveManager.Instance.CurrentSelectedStageIndex = stageIndex;
        }


        // Load the actual gameplay scene
        // You might need to pass mapID and stageIndex to the GameplayScene
        // For now, just load the scene:
        SceneManager.LoadScene("SampleScene"); // << USE YOUR GAMEPLAY SCENE NAME
    }

     void OnDestroy() // Clean up listener
    {
         if (button != null) { button.onClick.RemoveListener(OnStageSelected); }
    }
}