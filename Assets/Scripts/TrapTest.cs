using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class TrapTest : MonoBehaviourPun
{
    // Start is called before the first frame update
    public GameObject center;
    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PhotonView>() == null || other.GetComponent<PhotonView>().IsMine == false)
            return;

        Debug.Log("TRAP");
       other.transform.position = center.transform.position;
    }
}
