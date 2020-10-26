using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temple1_2_Mgr : MonoBehaviour
{
    public int cnt = 6;
    public GameObject Goal;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(cnt == 0)
        {
            Goal.SetActive(false);
            cnt = 6;
        }
    }
}
