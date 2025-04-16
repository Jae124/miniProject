using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuManager : MonoBehaviour
{
    [Tooltip("The exact name of the game scene file to load")]
    public string gameSceneName = "StageScene";

    public void StartGame()
    {
        // Check if the scene name is provided
        if (string.IsNullOrEmpty(gameSceneName))
        {
            Debug.LogError("Game Scene Name is not set in the MainMenuManager Inspector!");
            return;
        }

        Debug.Log($"Loading scene: {gameSceneName}...");

        // Load the specified scene
        SceneManager.LoadScene(gameSceneName);

        // --- Optional: For more advanced loading ---
        // SceneManager.LoadSceneAsync(gameSceneName); // Use this for loading screens
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game Requested!");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

}
