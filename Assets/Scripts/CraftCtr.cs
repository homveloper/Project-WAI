using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftCtr : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject window;
    public GameObject info;
    Vector3 tmp1;
    Vector3 tmp2;
    void Start()
    {
        tmp1 = info.transform.localScale;
        tmp2 = window.transform.localScale;
        info.transform.localScale = new Vector3(0,0,0);
        window.transform.localScale = new Vector3(0,0,0);
    }

    private void OnTriggerStay(Collider other)
    {
        if(window.transform.localScale.x != tmp2.x)
            info.transform.localScale = tmp1;

        if(Input.GetKeyDown(KeyCode.E))
        {
            window.transform.localScale = tmp2;
            info.transform.localScale = new Vector3(0,0,0);

        }
    }
    void OnTriggerExit(Collider other)
    {
        window.transform.localScale = new Vector3(0,0,0);
        info.transform.localScale = new Vector3(0,0,0);

    }
}
