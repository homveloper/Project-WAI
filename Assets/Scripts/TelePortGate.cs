using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelePortGate : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform temple;
    GameObject mPlayer;
    bool cnaInput = false;
    
 
    void OnTriggerStay(Collider other)
    {
        if(Input.GetKeyDown(KeyCode.E))
            other.transform.position = temple.transform.position;
    }

}
