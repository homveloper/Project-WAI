﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMChanger : MonoBehaviour
{
    public GameObject bgmMgr;
    public int chgCode;
    public int orginCode;
    int cnt = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        if(cnt == 0)
            return;
        Debug.Log("enter   "+orginCode);
        orginCode = bgmMgr.GetComponent<BGMSelect>().stat;
        bgmMgr.GetComponent<BGMSelect>().stat = chgCode;
        cnt--;
    }
    void OnTriggerExit(Collider other)
    {
        if(cnt == 1)
            return;
        bgmMgr.GetComponent<BGMSelect>().stat = orginCode;
        cnt++;
    }
}
