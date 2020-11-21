using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temple2_Mgr : MonoBehaviour
{
    public GameObject jewelry;
    public int cnt = 3; //3개 패널에서 스테이지 클리어 시 유적 클리어.
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(cnt == 0)
        {
            jewelry.SetActive(true);
        }
    }
}
