using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine. InputSystem;

public class RecordNRewind : MonoBehaviour
{
    public InputActionReference toggleReference=null;
    // Start is called before the first frame update

    private void Awake()
    {
        toggleReference.action.started += Toggle;
    }

    private void OnDestroy()
    {
        toggleReference.action.started -= Toggle;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Toggle(InputAction.CallbackContext context)
    {
        bool isActive=!gameObject.activeSelf;
        gameObject.SetActive(isActive);
    }

    internal void Toggle()
    {
        throw new NotImplementedException();
    }
}
