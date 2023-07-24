using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class PlayerMovement : MonoBehaviour
{
    //public TextMeshProUGUI uiText;
    public bool CanMove { get; private set; } = true;
    private bool IsSprinitng => canSprint && Input.GetKey(sprintKey);
    private bool ShouldJump => Input.GetKeyDown(jumpKey) && characterController.isGrounded;
    private bool ShouldCrouch => Input.GetKeyDown(crouchKey) && !duringCrouchAnimation && characterController.isGrounded;

    [Header("Functional Options")]
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canCrouch = true;
    [SerializeField] private bool canInteract = true;
    [SerializeField] private bool useFootsteps = true;
    [Header("Controls")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintSpeed = 6.0f;
    [Header("Jumping Parameters")]
    [SerializeField] private float jumpForce = 8.0f;
    [SerializeField] private float gravity = 30.0f;
    [Header("Interaction")]
    [SerializeField] private Vector3 interactionRayPont = default;
    [SerializeField] private float interactionDistance = default;
    [SerializeField] private LayerMask interactionLayer = default;

    [Header("FootSteps Parameters")]
    [SerializeField] private float baseStepSpeed = 0.3f;
    [SerializeField] private float crouchStepMultipler = 1.5f;
    [SerializeField] private float sprintStepMultipler = 0.6f;
    [SerializeField] private AudioSource footstepAudioSource = default;
    [SerializeField] private AudioClip[] footStepClips = default;

   


    private float footstepTimer = 0;
    private float GetCurrentOffset => isCrouching ? baseStepSpeed * crouchStepMultipler : IsSprinitng ? baseStepSpeed * sprintStepMultipler : baseStepSpeed;


    [Header("Look Parameters")]
    [SerializeField, Range(1, 10)] private float lookSpeedX = 2.0f;
    [SerializeField, Range(1, 10)] private float lookSpeedY = 2.0f;
    [SerializeField, Range(1, 180)] private float upperLookLimit = 80.0f;
    [SerializeField, Range(1, 180)] private float lowerLookLimit = 80.0f;
    [Header("Crouch Parameters")]
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float standinghHeight = 2f;
    [SerializeField] private float timeToCrouch = 0.25f;
    [SerializeField] private Vector3 croucingCenter = new Vector3(0, 0.5f, 0);
    [SerializeField] private Vector3 standingCenter = new Vector3(0, 0, 0);
    private bool isCrouching;
    private bool duringCrouchAnimation;
    [Header("Items")]
    public KeyCode deactivationKey;
    public GameObject object1ToDeactivate;
    public GameObject object2ToDeactivate;
    private bool isInsideObject1Trigger = false;
    private bool isInsideObject2Trigger = false;


    private Camera playerCamera;
    private CharacterController characterController;

    private Vector3 moveDirection;
    private Vector2 currentInput;

    private float rotationX = 0;
    public static PlayerMovement instance;




    void Awake()
    {
        instance = this;
        playerCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }
    private void Start()
    {
        
       // uiText.gameObject.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Debug.Log("Shoot");

        }


        if (CanMove)
        {
            HandleMovementInput();
            HandleMouseLook();
            if (useFootsteps)
                Handle_Footsteps();

            if (canJump)
                HandleJump();
            if (canCrouch)
                HandleCrouch();




            ApplyFinalMovements();

        }
        if (Input.GetKeyDown(deactivationKey))
        {
            // Deactivate any game objects that the player is inside
            if (isInsideObject1Trigger)
            {
                object1ToDeactivate.SetActive(false);
            }
            else if (isInsideObject2Trigger)
            {
                object2ToDeactivate.SetActive(false);
            }
        }

    }

    //Deactivate the game objects 

    private void OnTriggerEnter(Collider other)
    {
       
        if (other.gameObject == object1ToDeactivate)
        {

            //uiText.gameObject.SetActive(true);
            isInsideObject1Trigger = true;
        }
        else if (other.gameObject == object2ToDeactivate)
        {
           isInsideObject2Trigger = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
       // uiText.gameObject.SetActive(false);
        if (other.gameObject == object1ToDeactivate)
        {
            
            isInsideObject1Trigger = false;
        }
        
        else if (other.gameObject == object2ToDeactivate)
        {
           
            isInsideObject2Trigger = false;
          
        }
    }




    private void HandleMovementInput()

    {
        currentInput = new Vector2((IsSprinitng ? sprintSpeed : walkSpeed) * Input.GetAxis("Vertical"), (IsSprinitng ? sprintSpeed : walkSpeed) * Input.GetAxis("Horizontal"));
        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
        moveDirection.y = moveDirectionY;
    }
    private void HandleMouseLook()
    {
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeedX, 0);

    }





    private void HandleJump()
    {
        if (ShouldJump)

            moveDirection.y = jumpForce;

    }


    private void ApplyFinalMovements()
    {
        if (!characterController.isGrounded)
            moveDirection.y -= gravity * Time.deltaTime;
        characterController.Move(moveDirection * Time.deltaTime);


    }
    private void HandleCrouch()
    {
        if (ShouldCrouch)
            StartCoroutine(CrouchStand());
    }
    private IEnumerator CrouchStand()
    {
        if (isCrouching && Physics.Raycast(playerCamera.transform.position, Vector3.up, 1f))
            yield break;

        duringCrouchAnimation = true;
        float timeElapsed = 0;
        float targetHeight = isCrouching ? standinghHeight : crouchHeight;
        float currentHeight = characterController.height;
        Vector3 targetCenter = isCrouching ? standingCenter : croucingCenter;
        Vector3 currentCenter = characterController.center;
        while (timeElapsed < timeToCrouch)
        {
            characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / timeToCrouch);
            characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;

        }
        characterController.height = targetHeight;
        characterController.center = targetCenter;
        isCrouching = !isCrouching;

        duringCrouchAnimation = false;
    }
    private void Handle_Footsteps()
    {
        if (!characterController.isGrounded) return;
        if (currentInput == Vector2.zero) return;

        footstepTimer -= Time.deltaTime;
        if (footstepTimer <= 0)
        {
            footstepAudioSource.PlayOneShot(footStepClips[Random.Range(0, footStepClips.Length-1)]);
            if (Physics.Raycast(playerCamera.transform.position, Vector3.down, out RaycastHit hit, 3))
            {
                switch (hit.collider.tag)
                {
                    case "Footsteps/Grass":
                        footstepAudioSource.PlayOneShot(footStepClips[Random.Range(0, footStepClips.Length - 2)]);
                        break;
                    default:
                        footstepAudioSource.PlayOneShot(footStepClips[Random.Range(0, footStepClips.Length - 2)]);
                        break;

                }

            }
            footstepTimer = GetCurrentOffset;
        }
    }
}
