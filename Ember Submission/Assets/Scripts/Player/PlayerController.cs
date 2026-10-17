using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public bool canMove = true;
    public float moveSpeed = 6f;
    public float turnSpeed = 10f;
    public float gravity = -9.81f;
    

    [Header("Camera")]
    public Transform cameraTransform;

    private CharacterController controller;
    private float verticalVelocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (!canMove) return;
        HandleMovement();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 inputDir = new Vector3(horizontal, 0f, vertical);
        inputDir = Vector3.ClampMagnitude(inputDir, 1f);

        // Get camera directions
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        // Keep movement horizontal
        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        // Move relative to camera
        Vector3 moveDir =
            cameraForward * vertical +
            cameraRight * horizontal;

        moveDir = Vector3.ClampMagnitude(moveDir, 1f);

        moveDir *= moveSpeed;

        Vector3 flat = new Vector3(moveDir.x, 0f, moveDir.z);
        if (flat.sqrMagnitude > 0.01f)
        {
            Quaternion target = Quaternion.LookRotation(flat);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, turnSpeed * Time.deltaTime);
        }

        // Gravity
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        moveDir.y = verticalVelocity;

        controller.Move(moveDir * Time.deltaTime);
    }
}