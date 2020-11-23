using Photon.Pun;
using UnityEngine;

public class ResearcherAnimation : AnimationController
{
    public void Dead()
    {
        if (PhotonNetwork.IsConnected)
            if (!photonView.IsMine)
                return;

        animator.SetTrigger("dead");
    }
    
    public void OnCasting()
    {
        if (PhotonNetwork.IsConnected)
            if (!photonView.IsMine)
                return;

        animator.SetTrigger("casting");
    }

    public void EndAnimation()
    {
        if (PhotonNetwork.IsConnected)
            if (!photonView.IsMine)
                return;

        animator.SetTrigger("end");
    }
}
