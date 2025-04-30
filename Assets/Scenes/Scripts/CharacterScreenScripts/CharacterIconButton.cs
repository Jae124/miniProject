// CharacterIconButton.cs
using UnityEngine;
using UnityEngine.UI;
using System; // Needed for Action

public class CharacterIconButton : MonoBehaviour
{
    // --- Assign in Prefab Inspector ---
    public Image lockedIconImage;
    public Image characterDisplayImage;
    public Button button;
    // ---------------------------------

    private string characterID;
    private bool isUnlocked;
    private Action<string> onCharacterSelectedCallback; // Action to call when clicked

    void Awake()
    {
        if (button == null) button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
    }

    public void Setup(string charID, Sprite displaySprite, bool unlocked, Action<string> selectionCallback)
    {
        characterID = charID;
        isUnlocked = unlocked;
        onCharacterSelectedCallback = selectionCallback;

        // Show locked or unlocked state
        if (lockedIconImage != null) lockedIconImage.gameObject.SetActive(!isUnlocked);
        if (characterDisplayImage != null)
        {
            characterDisplayImage.gameObject.SetActive(isUnlocked);
            if (isUnlocked && displaySprite != null)
            {
                characterDisplayImage.sprite = displaySprite;
            }
            else if (isUnlocked)
            {
                // Optional: Set a default "unlocked but no sprite" image or color
                characterDisplayImage.sprite = null; // Or a silhouette?
                characterDisplayImage.color = Color.grey;
            }
        }

        // Button should only be interactable if unlocked
        button.interactable = isUnlocked;
    }

    void OnButtonClicked()
    {
        if (isUnlocked && onCharacterSelectedCallback != null)
        {
            Debug.Log($"Icon Button Clicked: {characterID}");
            onCharacterSelectedCallback(characterID); // Notify the manager
        }
    }

     void OnDestroy()
    {
        if(button != null) button.onClick.RemoveListener(OnButtonClicked);
    }
}