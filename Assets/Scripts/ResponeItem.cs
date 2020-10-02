﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponeItem : MonoBehaviour
{
    public GameObject box;
    int itemSize = 24;
    // Start is called before the first frame update
    void Start()
    {
        CreateItem();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void CreateItem()
    {
        int cnt= 0;
        int[] a = new int[10];
        Transform[] points = GameObject.Find("RandomBoxRespone").GetComponentsInChildren<Transform>();
        while(true)
        {
            int tmp =  Random.Range(1 ,itemSize );
            a[cnt] = tmp;
            cnt++;
            if(cnt == 10)
                break;    
        }

        for(int i = 0; i<cnt; i++)
        {
        Instantiate(box, points[a[i]].position , Quaternion.identity);
        }
    }
}
