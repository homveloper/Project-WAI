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
        Invoke("DestoryBox", 5f);
    }
    void CreateItem()
    {
        int[] a = new int[countOfBox];
        bool[] isSelected = new bool[itemSize];
        Transform[] points = GameObject.Find("RandomBoxRespone").GetComponentsInChildren<Transform>();
        
        for(int i=0; i<countOfBox; i++)
        {
            int position = Random.Range(0,itemSize);

            if(!isSelected[position])
            {
                a[i] = position;
                isSelected[position] = true;
            }
            else
            {
                i--;
            }
        }

        for (int i = 0; i < countOfBox; i++)
        {

            PhotonNetwork.Instantiate("Item/" + box.name, points[a[i]].position, Quaternion.Euler(0,180,0));
        }
    }
    void DestoryBox()
    {
        photonView.RPC("DestroyItem", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void DestroyItem() => Destroy(GameObject.FindWithTag("Box"));
}
