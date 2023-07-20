using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using Unity.XR.CoreUtils;

public class TimeControlVR2 : MonoBehaviour
{
    // public HandComplete HandC;
    //public GameObject XROrigin;
    private bool isRecording = false;
    private List<Vector3> recordedPositions;
    private ActionBasedContinuousMoveProvider continuousMoveProvider;
    private int currentRecordedIndex = 0;
    private bool isRewinding = false;
    public float rewindSpeed = 1f;
    public CharacterController characterController;
    //public CharacterController characterController;
   

    void Start()
    {
        recordedPositions = new List<Vector3>();
        continuousMoveProvider = GetComponent<ActionBasedContinuousMoveProvider>();
       // characterController=XROrigin.GetComponent<CharacterController>();
       //HandC=GetComponent<HandComplete>();
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
            StartCoroutine(Rewind());
        }

    }
    //public void StartC()
    
    //    {
    //    Debug.Log("PlayRecording");
    //    StartCoroutine(Rewind());
    //}
    

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
        
       //continuousMoveProvider.enabled = false;
         
        characterController.enabled = false;

        for (int i = recordedPositions.Count - 1; i >= 0; i--)
        {
            transform.position = recordedPositions[i];
            currentRecordedIndex = i;

            float rewindDelay = Time.fixedDeltaTime * (1f - rewindSpeed);
            yield return new WaitForSeconds(rewindDelay);
        }

      // continuousMoveProvider.enabled = true;
     
        characterController.enabled = true;
        
    }
}
