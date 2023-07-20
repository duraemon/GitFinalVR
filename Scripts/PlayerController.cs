using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
//using Unity.XR.CoreUtils;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionReference jumpActionReference;
    [SerializeField] private float jumpForce = 8.0f; // Adjust the jump force as needed.

    private XROrigin xrOrigin;
    private CharacterController characterController;

    private bool IsGrounded => characterController.isGrounded;

    private Vector2 inputMovement;

    void Start()
    {
        xrOrigin = GetComponent<XROrigin>();
        characterController = GetComponent<CharacterController>();
        jumpActionReference.action.performed += OnJump;
    }

    void Update()
    {
        MoveCharacter();
        AlignColliderWithCamera();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (!IsGrounded) return;
        Debug.Log("Jump");
        characterController.Move(Vector3.up * jumpForce);
    }

    private void MoveCharacter()
    {
        // Get the input movement from the Input System.
        inputMovement = jumpActionReference.action.ReadValue<Vector2>();

        // Convert input movement to world space based on the camera orientation.
        Vector3 moveDirection = xrOrigin.Camera.transform.TransformDirection(new Vector3(inputMovement.x, 0.0f, inputMovement.y));

        // Apply movement to the character controller.
        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void AlignColliderWithCamera()
    {
        var center = xrOrigin.CameraInOriginSpacePos;
        characterController.center = new Vector3(center.x, characterController.center.y, center.z);
        characterController.height = xrOrigin.CameraInOriginSpaceHeight;
    }
}
