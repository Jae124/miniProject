using UnityEngine;
using UnityEngine.UI; // For buttons if needed later
using UnityEngine.SceneManagement; // For stage loading
using System.Collections;

public class MapZoomManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera uiCamera; // Assign your UICamera
    [SerializeField] private GameObject mapQuadrantButtonsContainer; // Assign the parent of the 4 quadrant buttons
    [SerializeField] private GameObject quadrantStagesViewContainer; // Assign the container for the zoomed-in stage buttons/panel
    [SerializeField] private TMPro.TextMeshProUGUI quadrantTitleText; // Optional: Assign the text element for the quadrant title

    [Header("Zoom Settings")]
    [SerializeField] private float zoomedOutSize = 5f; // Orthographic size when fully zoomed out (MATCH INITIAL CAMERA SIZE)
    [SerializeField] private float zoomedInSize = 0.5f; // Orthographic size when zoomed into a quadrant
    [SerializeField] private float zoomDuration = 0.5f; // How long the zoom animation takes

    [SerializeField] private Vector3[] quadrantCenterPositions = new Vector3[4] {
        new Vector3(-2.5f, 2.5f, -10f), // 0: Top-Left (Negative X, Positive Y)
        new Vector3(2.5f, 2.5f, -10f),  // 1: Top-Right (Positive X, Positive Y)
        new Vector3(-2.5f, -2.5f, -10f), // 2: Bottom-Left (Negative X, Negative Y)
        new Vector3(2.5f, -2.5f, -10f)   // 3: Bottom-Right (Positive X, Negative Y)
    };

    private Vector3 initialCameraPosition;
    private Coroutine zoomCoroutine;
    private int currentQuadrantIndex = -1; // -1 means zoomed out

    void Start()
    {
        if (uiCamera == null) uiCamera = Camera.main; // Fallback, but assignment is better
        if (uiCamera != null)
        {
            initialCameraPosition = uiCamera.transform.position;
            uiCamera.orthographicSize = zoomedOutSize; // Ensure starting size is correct
        }
        else
        {
            Debug.LogError("UI Camera not assigned or found!");
        }

        // Start zoomed out
        ShowMapView();
    }

    // Assign quadrantIndex 0 to TL button, 1 to TR, 2 to BL, 3 to BR in Inspector
    public void OnQuadrantClicked(int quadrantIndex)
    {
        if (quadrantIndex < 0 || quadrantIndex >= quadrantCenterPositions.Length)
        {
            Debug.LogError("Invalid Quadrant Index: " + quadrantIndex);
            return;
        }

        Debug.Log($"OnQuadrantClicked: Index {quadrantIndex}. Attempting to start ZoomRoutine.");

        if (zoomCoroutine != null) StopCoroutine(zoomCoroutine); // Stop existing zoom if any
        currentQuadrantIndex = quadrantIndex;
        zoomCoroutine = StartCoroutine(ZoomRoutine(quadrantCenterPositions[quadrantIndex], zoomedInSize, true));
    }

    // --- Function Called by Back Button in Quadrant View ---
    public void ZoomOut()
    {
        if (zoomCoroutine != null) StopCoroutine(zoomCoroutine);
        currentQuadrantIndex = -1;
        zoomCoroutine = StartCoroutine(ZoomRoutine(initialCameraPosition, zoomedOutSize, false));
    }

    // --- Coroutine for Smooth Zooming ---
    private IEnumerator ZoomRoutine(Vector3 targetPosition, float targetSize, bool isZoomingIn)
    {
        // Immediately hide the buttons we're moving away from
        if (isZoomingIn) mapQuadrantButtonsContainer?.SetActive(false);
        else quadrantStagesViewContainer?.SetActive(false);


        Debug.Log("ZoomRoutine: COROUTINE STARTED. TargetPos=" + targetPosition + ", TargetSize=" + targetSize + ", IsZoomingIn=" + isZoomingIn);
        float elapsedTime = 0f;
        Vector3 startPosition = uiCamera.transform.position;
        float startSize = uiCamera.orthographicSize;

        Debug.Log($"ZoomRoutine: StartPos={startPosition}, TargetPos={targetPosition}, StartSize={startSize}, TargetSize={targetSize}");
        // Check for no change
        if (startPosition == targetPosition && startSize == targetSize) {
            Debug.LogError("ZoomRoutine: Start and Target values are identical! Zoom will not occur.");
            // Optionally force completion logic here if needed, though it indicates a config error
            // if (isZoomingIn) ShowQuadrantStagesView(); else ShowMapView();
            yield break; // Exit coroutine if no change needed
        }

        Debug.Log("Current TimeScale: " + Time.timeScale);

        while (elapsedTime < zoomDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / zoomDuration);
            // Optional: Add smoothstep or other easing functions for t
            // t = t * t * (3f - 2f * t); // Smoothstep example

            uiCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            uiCamera.orthographicSize = Mathf.Lerp(startSize, targetSize, t);

            yield return null; // Wait for the next frame
        }

        // Ensure final values are exact
        uiCamera.transform.position = targetPosition;
        uiCamera.orthographicSize = targetSize;

        // Show the appropriate UI elements after zooming
        if (isZoomingIn) ShowQuadrantStagesView();
        else ShowMapView();

        zoomCoroutine = null;
    }

    private void ShowMapView()
    {
        mapQuadrantButtonsContainer?.SetActive(true);
        quadrantStagesViewContainer?.SetActive(false);
    }

    private void ShowQuadrantStagesView()
    {
        mapQuadrantButtonsContainer?.SetActive(false);
        quadrantStagesViewContainer?.SetActive(true);

        // --- Optional: Update quadrant view based on currentQuadrantIndex ---
        if (quadrantTitleText != null)
        {
            quadrantTitleText.text = $"Region {currentQuadrantIndex + 1} Stages"; // Example title
        }
        // TODO: Add logic here to enable/disable specific stage buttons
        // or load different stage lists based on 'currentQuadrantIndex'
        Debug.Log($"Showing stages for Quadrant {currentQuadrantIndex}");
    }

    public void SelectStageInQuadrant(int stageIndexWithinQuadrant)
    {
        // Determine the actual difficulty/scene based on BOTH
        // currentQuadrantIndex and stageIndexWithinQuadrant
        int overallDifficulty = 1; // Default
        string sceneToLoad = "GameScene_Default"; // Default

        // Example logic - Replace with your actual stage mapping
        if (currentQuadrantIndex == 0) { // Top Left
            if (stageIndexWithinQuadrant == 0) { overallDifficulty = 1; sceneToLoad = "GameScene_Level1"; }
            if (stageIndexWithinQuadrant == 1) { overallDifficulty = 2; sceneToLoad = "GameScene_Level2"; }
        } else if (currentQuadrantIndex == 1) { // Top Right
             if (stageIndexWithinQuadrant == 0) { overallDifficulty = 3; sceneToLoad = "GameScene_Level3"; }
             if (stageIndexWithinQuadrant == 1) { overallDifficulty = 4; sceneToLoad = "GameScene_Level4"; }
        }

        SelectedStageInfo.SelectedDifficulty = overallDifficulty;
        SelectedStageInfo.SceneToLoad = sceneToLoad;

        Debug.Log($"Loading Stage - Scene: {SelectedStageInfo.SceneToLoad}, Difficulty: {SelectedStageInfo.SelectedDifficulty}");
        SceneManager.LoadScene(SelectedStageInfo.SceneToLoad);
    }

}
