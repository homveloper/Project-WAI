using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temple1_2_Switch : MonoBehaviour
{
    // Start is called before the first frame update
     public GameObject swichFlash;
     public GameObject mgr;

     int tmp = 1;
    void Start()
    {
        swichFlash.SetActive(false);
    }
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        swichFlash.SetActive(true);
        if(tmp == 1)
        {
            mgr.GetComponent<Temple1_2_Mgr>().cnt--;
            tmp--;
        }
    }

    // Collider 컴포넌트의 is Trigger가 true인 상태로 충돌중일 때

    private void OnTriggerStay(Collider other)
    {
       
    }
}
