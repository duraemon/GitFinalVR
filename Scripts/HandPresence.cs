using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;


public class HandPresence : MonoBehaviour
{
    //private InputDevice targetDevice;
    //private Animator handAnimator;
    //private GameObject spwanHandModel;
    //public GameObject LefthandModel; 
    // Start is called before the first frame update
    void Start()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevices(devices);
        //InputDeviceCharacteristics rightControllerCharacterstics = InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
        //InputDevices.GetDevicesWithCharacteristics(rightControllerCharacterstics, devices);

    


       foreach (var item in devices)
        {
            Debug.Log(item.name + item.characteristics);
        }

        //if (devices.Count > 0)
        //{
        //    targetDevice = devices[0];
        //}

        //spwanHandModel = Instantiate(LefthandModel, transform);
        //handAnimator=spwanHandModel.GetComponent<Animator>();
    }
    // void UpdateHandAnimator()
    //{
    //    if(targetDevice.TryGetFeatureValue(CommonUsages.grip,out float gripValue))
    //    {
    //        handAnimator.SetFloat("Grip", gripValue);
    //    }
    //    else
    //    {
    //        handAnimator.SetFloat("Grip", 0);
    //    }
    //}
    // Update is called once per frame
    void Update()
    {
      //UpdateHandAnimator();
        //if ( targetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButtonValue) && primaryButtonValue)
      //  Debug.Log("Pressing Primary Button");


      //  if(targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue)&& triggerValue > 0.1f);
      //  Debug.Log("Pressing Primary Button");



    }

}
