using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class TimeControlV3 : MonoBehaviour
{
    public InputActionReference toggleReference = null;
    //private RecordNRewind recordNRewindScript;
    private bool isRecording = false;
    private List<Vector3> recordedPositions;
    private ActionBasedContinuousMoveProvider continuousMoveProvider;
    private int currentRecordedIndex = 0;
    private bool isRewinding = false;
    public float rewindSpeed = 1f;
    public CharacterController characterController;

    void Start()
    {
        recordedPositions = new List<Vector3>();
        continuousMoveProvider = GetComponent<ActionBasedContinuousMoveProvider>();
        characterController = GetComponent<CharacterController>();

       // recordNRewindScript = GetComponent<RecordNRewind>(); // Get the RecordNRewind script attached to the same game object
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("StartRecording");
            if (!isRecording)
                StartRecording();
            else
                StopRecording();
        }

        if (Input.GetKeyDown(KeyCode.T) && !isRewinding) // Check if not already rewinding
        {
            Debug.Log("PlayRecording");
            //recordNRewindScript.Toggle(toggleReference.action.ReadValue<InputAction.CallbackContext>());
            //Rewind();
        }
    }

    public void Toggle(InputAction.CallbackContext context)
    {
        //bool isActive = !gameObject.activeSelf;
        //gameObject.SetActive(isActive);
       // Rewind();
    }

    void FixedUpdate()
    {
        if (isRecording)
            RecordPosition();
    }

    void RecordPosition()
    {
        recordedPositions.Add(transform.position);
    }

    void StartRecording()
    {
        isRecording = true;
        recordedPositions.Clear();
    }

    void StopRecording()
    {
        isRecording = false;
    }

    IEnumerator Rewind()
    {
        characterController.enabled = false;

        for (int i = recordedPositions.Count - 1; i >= 0; i--)
        {
            if (characterController.enabled)
            {
                transform.position = recordedPositions[i];
                currentRecordedIndex = i;
            }
            float rewindDelay = Time.fixedDeltaTime * (1f - rewindSpeed);
            yield return new WaitForSeconds(rewindDelay);
        }

        characterController.enabled = true;
    }
}
