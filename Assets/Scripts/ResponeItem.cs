using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;
using System;

public class ResponeItem : MonoBehaviourPunCallbacks
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
            int tmp =  UnityEngine.Random.Range(1 ,itemSize );
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
