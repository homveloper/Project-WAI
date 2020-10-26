using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftCtr : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject window;
    public GameObject info;

    void Start()
    {
        window.SetActive(false);
        info.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if(!window.active)
            info.SetActive(true);

        if(Input.GetKeyDown(KeyCode.E))
        {
             window.SetActive(true);
             info.SetActive(false);
        }
    }
    void OnTriggerExit(Collider other)
    {
        window.SetActive(false);
        info.SetActive(false);
    }
}
