using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UnityEngine.XR.Interaction.Toolkit
{

    public class TimeControllerVR : MonoBehaviour
    {
        private bool isRecording = false;
        private List<RecordedState> recordedStates;
        private Transform playerHead;
        private ActionBasedContinuousMoveProvider continuousMoveProvider;

        private struct RecordedState
        {
            public Vector3 position;
            public Quaternion rotation;
            public float timestamp;
        }

        public float rewindSpeed = 0.5f;

        void Start()
        {
            recordedStates = new List<RecordedState>();
            playerHead = Camera.main.transform;
             continuousMoveProvider = FindObjectOfType<ActionBasedContinuousMoveProvider>();
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

            if (Input.GetKeyDown(KeyCode.T))
            {
                Debug.Log("PlayRecording");
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
            state.position = playerHead.position;
            state.rotation = playerHead.rotation;
            state.timestamp = Time.time;
            recordedStates.Add(state);
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
                // Disable input or any other actions that can interfere during rewind
                continuousMoveProvider.enabled = false;

                for (int i = recordedStates.Count - 1; i >= 0; i--)
                {
                    if (i < recordedStates.Count)
                    {
                        playerHead.position = recordedStates[i].position;
                        playerHead.rotation = recordedStates[i].rotation;

                        // Calculate the time difference between the current and previous recorded state
                        float rewindDuration = Time.time - recordedStates[i].timestamp;
                        float rewindDelay = rewindDuration * (1f - rewindSpeed);

                        yield return new WaitForSeconds(rewindDelay);
                    }
                }

                // Enable input or any other actions after the rewind is complete
                 continuousMoveProvider.enabled = true;
            }
        }
    }
}
