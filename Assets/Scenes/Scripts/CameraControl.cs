using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of camera movement

    void Update()
    {
        // Get horizontal input (e.g., arrow keys or A/D keys)
        float horizontalInput = Input.GetAxis("Horizontal");

        // Move the camera horizontally based on input
        transform.Translate(Vector3.right * horizontalInput * moveSpeed * Time.deltaTime);
    }
}
