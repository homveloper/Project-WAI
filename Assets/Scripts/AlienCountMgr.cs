using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AlienCountMgr : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update

    [SerializeField]
    private BGMSelect bgmMgr;
    public int chgCode;
    public int orginCode;
    int cnt = 1;
    // Start is called before the first frame update
    void Awake()
    {
        if (PhotonNetwork.IsConnected)
            if (!photonView.IsMine)
                return;
        Debug.Log("star in");
        bgmMgr = GameObject.Find("Main Camera").GetComponent<BGMSelect>();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PhotonView>() == null || other.GetComponent<PhotonView>().IsMine == false)
            return;

        if(cnt == 0)
            return ;

        if(other.gameObject.tag == "Player" && other.GetComponent<Player>().IsAlienObject())
        {
            Debug.Log("trigger in");
            orginCode = bgmMgr.stat;
            bgmMgr.stat = chgCode;
            cnt--;
        }
        
    }
    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PhotonView>() == null || other.GetComponent<PhotonView>().IsMine == false)
            return;
        
        if(cnt == 1)
            return;

        bgmMgr.stat = orginCode;
        cnt++;
            
    }
}
