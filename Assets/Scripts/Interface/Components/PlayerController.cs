using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Movement speed of the player
    public float speed = 5f;

    void Update()
    {
        // Get the horizontal and vertical input axes
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate the movement vector based on the input axes and speed
        Vector3 vertical = Vector3.up * verticalInput * speed * Time.deltaTime;
        Vector3 horizontal = Vector3.right * horizontalInput * speed * Time.deltaTime;

        transform.position = transform.position + vertical + horizontal;
    }
}