using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VR_Jump : MonoBehaviour
{
    [SerializeField] private InputActionReference jumpButton;
    [SerializeField] private InputActionReference rewindButton;
    [SerializeField] private InputActionReference recordButton;
    [SerializeField] private float jumpHeight = 2.0f;
    [SerializeField] private float gravityValue = -9.8f;
    [SerializeField] private float maxRecordTime = 30.0f; // Maximum record time in seconds
    [SerializeField] private float maxRewindTime = 30.0f; // Maximum rewind time in seconds
    [SerializeField] private float rewindSpeed = 1.0f; // Rewind speed factor

    private bool isRecording = false;
    private List<RecordedState> recordedStates;
    private CharacterController characterController;
    private Vector3 playerVelocity;

    private Transform cameraTransform;
    private bool isCameraMovementEnabled = true; // Flag variable to control camera movement

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;
    }

    private void OnEnable()
    {
        recordButton.action.performed += StartRecording;
        rewindButton.action.performed += StartRewind;
        jumpButton.action.performed += Jump;
    }

    private void OnDisable()
    {
        recordButton.action.performed -= StartRecording;
        rewindButton.action.performed -= StartRewind;
        jumpButton.action.performed -= Jump;
    }

    private void Start()
    {
        recordedStates = new List<RecordedState>();
    }

    private void Update()
    {
        if (characterController.isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0;
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        characterController.Move(playerVelocity * Time.deltaTime);

        if (isRecording)
        {
            RecordStateCoroutine();
        }

        if (isCameraMovementEnabled)
        {
            // Update camera movement if it is enabled
            // Example: cameraTransform.position = ...
            //          cameraTransform.rotation = ...
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (!characterController.isGrounded)
            return;

        playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
    }

    private void StartRecording(InputAction.CallbackContext context)
    {
        if (!isRecording)
        {
            isRecording = true;
            recordedStates.Clear();
            StartCoroutine(RecordStateCoroutine());
            Debug.Log("Recording started.");
        }
    }

    private IEnumerator RecordStateCoroutine()
    {
        float startTime = Time.time;

        while (isRecording && Time.time - startTime <= maxRecordTime)
        {
            RecordedState recordedState = new RecordedState(transform.position, transform.rotation, cameraTransform.position, cameraTransform.rotation, Time.time);
            recordedStates.Add(recordedState);
            yield return null;
        }

        StopRecording();
    }

    private void StopRecording()
    {
        isRecording = false;
        Debug.Log("Recording stopped.");
    }

    private void StartRewind(InputAction.CallbackContext context)
    {
        StartCoroutine(Rewind());
    }

    private IEnumerator Rewind()
    {
        characterController.enabled = false;
        isCameraMovementEnabled = false; // Disable camera movement during rewind

        float rewindStartTime = Time.time;
        float rewindEndTime = rewindStartTime - maxRewindTime;
        float currentTime = Time.time;

        for (int i = recordedStates.Count - 1; i >= 0; i--)
        {
            RecordedState recordedState = recordedStates[i];
            float deltaTime = currentTime - recordedState.RecordedTime;

            if (deltaTime <= maxRewindTime)
            {
                float rewindProgress = (currentTime - rewindStartTime) / (rewindEndTime - rewindStartTime);

                // Rewind player position and rotation
                transform.position = Vector3.Lerp(recordedState.PlayerPosition, transform.position, rewindProgress);
                transform.rotation = Quaternion.Lerp(recordedState.PlayerRotation, transform.rotation, rewindProgress);

                // Rewind camera position and rotation
                cameraTransform.position = Vector3.Lerp(recordedState.CameraPosition, cameraTransform.position, rewindProgress);
                cameraTransform.rotation = Quaternion.Lerp(recordedState.CameraRotation, cameraTransform.rotation, rewindProgress);

                currentTime -= Time.deltaTime * rewindSpeed;
            }
            else
            {
                break;
            }

            yield return null;
        }

        characterController.enabled = true;
        isCameraMovementEnabled = true; // Enable camera movement after rewind
    }

    private struct RecordedState
    {
        public Vector3 PlayerPosition;
        public Quaternion PlayerRotation;
        public Vector3 CameraPosition;
        public Quaternion CameraRotation;
        public float RecordedTime;

        public RecordedState(Vector3 playerPosition, Quaternion playerRotation, Vector3 cameraPosition, Quaternion cameraRotation, float recordedTime)
        {
            PlayerPosition = playerPosition;
            PlayerRotation = playerRotation;
            CameraPosition = cameraPosition;
            CameraRotation = cameraRotation;
            RecordedTime = recordedTime;
        }
    }
}
