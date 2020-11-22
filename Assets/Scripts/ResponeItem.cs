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
    public int countOfBox = 15;
    int itemSize = 24;
    int waitTime = 90;
    int term = 15;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(this.CreateBox());
    }
    
    // Update is called once per frame
    public void CreateItem()
    {
        if (PhotonNetwork.IsMasterClient == false)
            return;

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
            PhotonNetwork.Instantiate("Item/" + box.name, points[a[i]].position, Quaternion.Euler(0,0,0));
        }
    }
    IEnumerator CreateBox()
    {
        while(true)
        {
            yield return new WaitForSeconds(waitTime);

            DestoryBox();

            yield return new WaitForSeconds(term);

            CreateItem();
        }
    }
    void DestoryBox()
    {
        if (PhotonNetwork.IsMasterClient == false)
            return;

        photonView.RPC("DestroyItem", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void DestroyItem()
    {
        GameObject[] boxs = GameObject.FindGameObjectsWithTag("Box");
        foreach(GameObject box in boxs)
            GameObject.Destroy(box);
    }
}
