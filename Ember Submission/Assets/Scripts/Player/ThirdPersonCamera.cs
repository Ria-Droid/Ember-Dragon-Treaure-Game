using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Camera Settings")]
    public float sensitivity = 3f;
    public float distance = 5f;
    public float height = 2f;

    [Header("Vertical Limits")]
    public float minPitch = -30f;
    public float maxPitch = 60f;

    private float yaw;
    private float pitch = 15f;

    void Start()
    {
        // Start the camera behind the player
        yaw = target.eulerAngles.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        // Get mouse movement
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Rotate camera
        yaw += mouseX * sensitivity;
        pitch -= mouseY * sensitivity;

        // Limit looking up/down
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // Rotate the PLAYER horizontally with the camera
        target.rotation = Quaternion.Euler(0f, yaw, 0f);

        // Camera rotation
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        // Camera position
        Vector3 targetPosition = target.position + Vector3.up * height;

        transform.position =
            targetPosition - rotation * Vector3.forward * distance;

        transform.rotation = rotation;
    }
}