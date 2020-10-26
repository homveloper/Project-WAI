using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CraftController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<PhotonView>().IsMine == false)
            return;
    }
    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PhotonView>().IsMine == false)
            return;
    }
}
