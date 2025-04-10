using UnityEngine;
using UnityEngine.UI;

public class LogicScript : MonoBehaviour
{
    public int playerHealth;
    public Text healthText;

    [ContextMenu("Decrease Health")]
    public void updateHealth() {
        playerHealth += 1;
        healthText.text = playerHealth.ToString();
    }
}
