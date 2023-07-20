using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChairAnim : MonoBehaviour
{
    public Animator chairAnim;
    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            chairAnim.Play("ChairAnim");
        }
    }
}
