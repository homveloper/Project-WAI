using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temple3_Switch : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject swichFlash;

    public GameObject mgr;

    public GameObject stone;

    public int tmp = 1;

    void Start()
    {
        swichFlash.SetActive(false);
        stone.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag != "Player")
            return ;
            
        swichFlash.SetActive(true);
        if(tmp == 1)
        {
            mgr.GetComponent<Temple3_Mgr>().cnt--;
            tmp--;
        }
    }
    private void OnTriggerStay(Collider other)
    {
       if(mgr.GetComponent<Temple3_Mgr>().cnt == 0)
            stone.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
