using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Temple1_2_Mgr : MonoBehaviourPun
{
    public int cnt = 6;
    public GameObject Goal;
    public AudioSource wallSound;
    // Start is called before the first frame update
    void Start()
    {
        wallSound.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if(cnt == 0)
        {
            photonView.RPC("OnMoveWall", RpcTarget.AllBuffered);
        }
    }
    [PunRPC]
    public void OnMoveWall()
    {
        Goal.SetActive(false);
        wallSound.Play();
        cnt = 6;
    }
}
