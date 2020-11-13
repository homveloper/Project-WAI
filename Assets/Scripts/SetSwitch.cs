using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSwitch : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject swichFlash;
    public GameObject moveWell;
    public GameObject hiddenGate;
    public AudioSource wallSound;
    
    int cnt = 1;
    void Start()
    {
        swichFlash.SetActive(false);
    }
    // Start is called before the first frame update
    void OnTriggerEnter(Collider other)
    {

    }
    void OnTriggerStay(Collider other)
    {
        swichFlash.SetActive(true);

        if(moveWell == null)
            return;
        else
        {
            moveWell.SetActive(false);
            wallSound.Play();
        }
            
        if(hiddenGate != null)
            hiddenGate.SetActive(true);

    }
    void OnTriggerExit(Collider other)
    {
        
    }
}
