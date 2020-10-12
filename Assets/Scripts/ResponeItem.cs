using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine;

public class ResponeItem : MonoBehaviourPunCallbacks
{
    public GameObject box;
    public int countOfBox = 10;
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
        int[] a = new int[countOfBox];
        bool[] isSelected = new bool[itemSize];
        Transform[] points = GameObject.Find("RandomBoxRespone").GetComponentsInChildren<Transform>();
        
        for(int i=0; i<countOfBox; i++){
            int position = Random.Range(0,itemSize);

            if(!isSelected[position]){
                a[i] = position;
                isSelected[position] = true;
            }else{
                i--;
            }
        }

        // ToToMo : 상자가 중복되어 리스폰 되는 문제가 있어 수정함


        // while (true)
        // {
        //     int tmp = UnityEngine.Random.Range(1, itemSize);
        //     a[cnt] = tmp;
        //     cnt++;
        //     if (cnt == 10)
        //         break;
        // }

        for (int i = 0; i < countOfBox; i++)
        {
            Instantiate(box, points[a[i]].position, Quaternion.identity);
        }
    }
}
