// EnergyDisplayUI.cs
using UnityEngine;
using UnityEngine.UI;
using System; // Needed for TimeSpan formatting
using System.Collections; // Required for Coroutines


public class EnergyDisplayUI : MonoBehaviour
{
    public Text energyAmountText; // Assign the "X / Y" text element
    public Text nextEnergyTimerText; // Assign the "Next in..." text element (optional)
    public Slider energySlider; // Assign a Slider (optional)
    public Image energyFillImage; // Assign an Image with Fill method (optional)

    private const float UI_UPDATE_INTERVAL = 1.0f; // Update UI every second

    void Start()
    {
        if (energyAmountText == null && GetComponentInChildren<Text>()) {
             Debug.LogWarning("EnergyAmountText not assigned, trying to find child Text.");
             // Attempt to find common text elements if not assigned (optional)
        }
         // etc for other UI elements

        // Start the repeating update process
        StartCoroutine(UpdateDisplayRoutine());
    }

     // Stop the coroutine if the object is disabled or destroyed
     void OnDisable() {
         StopAllCoroutines();
     }
      void OnDestroy() {
         StopAllCoroutines();
     }

    IEnumerator UpdateDisplayRoutine()
    {
        // Infinite loop that yields control back to Unity each iteration
        while (true)
        {
            UpdateDisplay(); // Call the actual update logic
            yield return new WaitForSeconds(UI_UPDATE_INTERVAL); // Wait for specified interval
        }
    }

    void UpdateDisplay() {
         if (SaveManager.Instance != null && SaveManager.Instance.playerProgress != null)
        {
            int current = SaveManager.Instance.GetCurrentEnergy();
            int max = SaveManager.Instance.GetMaxEnergy();

            // Update Amount Text
            if (energyAmountText != null) { energyAmountText.text = $"Energy: {current} / {max}"; }

            // Update Visual Bar
            float fillAmount = (max > 0) ? (float)current / max : 0f;
            if (energySlider != null) { energySlider.value = fillAmount; }
            if (energyFillImage != null) { energyFillImage.fillAmount = fillAmount; }

            // Update Timer Text
            if (nextEnergyTimerText != null)
            {
                if (current < max) {
                    TimeSpan timeToNext = SaveManager.Instance.GetTimeUntilNextEnergy();
                    nextEnergyTimerText.text = $"Next in: {timeToNext:mm\\:ss}";
                } else {
                    nextEnergyTimerText.text = "Full";
                }
            }
        }
        else {
            // Handle cases where SaveManager isn't ready yet (optional)
            if (energyAmountText != null) energyAmountText.text = "Energy: - / -";
            if (nextEnergyTimerText != null) nextEnergyTimerText.text = "";
            if (energySlider != null) energySlider.value = 0;
            if (energyFillImage != null) energyFillImage.fillAmount = 0;
        }
    }
}