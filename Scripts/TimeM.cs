using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class TimeM : MonoBehaviour
{
    private bool isRecording = false;
    private List<RecordedState> recordedStates;
    private CharacterController characterController;
    public ParticleSystem particleSystem;
    private struct RecordedState
    {
        public Vector3 position;
        public Quaternion rotation;
        public float timestamp;
    }

    public float rewindSpeed = 0.5f;
    public float maxRecordingTime = 30f;

    void Start()
    {
        recordedStates = new List<RecordedState>();
        characterController = GetComponent<CharacterController>();
        //particleSystem = GetComponent<ParticleSystem>();
        particleSystem.Stop();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!isRecording)
            {
                StartRecording();
                
                Debug.Log("Recording");
            }
            else
                StopRecording();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Rewind");
            particleSystem.Play(); 
            StartCoroutine(Rewind());
        }
    }

    void FixedUpdate()
    {
        if (isRecording)
            RecordState();
    }

    void RecordState()
    {
        RecordedState state;
        state.position = transform.position;
        state.rotation = transform.rotation;
        state.timestamp = Time.time;
        recordedStates.Add(state);

        // Remove recorded states older than maxRecordingTime
        float minTimestamp = Time.time - maxRecordingTime;
        while (recordedStates.Count > 0 && recordedStates[0].timestamp < minTimestamp)
        {
            recordedStates.RemoveAt(0);
        }
    }

    void StartRecording()
    {
        isRecording = true;
        recordedStates.Clear();
    }

    void StopRecording()
    {
        isRecording = false;
    }

    IEnumerator Rewind()
    {
        if (recordedStates.Count > 0)
        {
            characterController.enabled = false;

            for (int i = recordedStates.Count - 1; i >= 0; i--)
            {
                transform.position = recordedStates[i].position;
                transform.rotation = recordedStates[i].rotation;

                float rewindDuration = Time.time - recordedStates[i].timestamp;
                float rewindDelay = rewindDuration * (1f - rewindSpeed);

                yield return new WaitForSeconds(rewindDelay);

                recordedStates.RemoveAt(i);
            }

            characterController.enabled = true;
        }
    }
}

