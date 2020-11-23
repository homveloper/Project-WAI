using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Temple3_Switch : MonoBehaviourPun
{
    // Start is called before the first frame update
    public GameObject swichFlash;

    public GameObject mgr;

    public int tmp = 1;

    void Start()
    {
        swichFlash.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PhotonView>() == null || other.GetComponent<PhotonView>().IsMine == false)
            return;

        if(other.gameObject.tag != "HitBox")
            return ;
            
        photonView.RPC("OnLightUp", RpcTarget.AllBuffered);

        if(tmp == 1)
        {
            photonView.RPC("OnCntDown", RpcTarget.AllBuffered);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<PhotonView>() == null || other.GetComponent<PhotonView>().IsMine == false)
            return;

        if(other.gameObject.tag != "HitBox")
            return ;

        other.GetComponentInParent<Player>().statO2++;
    }
    // Update is called once per frame

    [PunRPC]
    public void OnLightUp()
    {
        swichFlash.SetActive(true);
    }

    [PunRPC]
    public void OnCntDown()
    {
        mgr.GetComponent<Temple3_Mgr>().cnt--;
        tmp--;
    }
}
