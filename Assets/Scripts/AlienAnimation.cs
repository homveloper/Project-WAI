using Photon.Pun;
using UnityEngine;

public class AlienAnimation : MonoBehaviourPunCallbacks
{
    [SerializeField]
    public Animator animator;

    void Update()
    {
        if (PhotonNetwork.IsConnected)
            if (!photonView.IsMine)
                return;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        bool hasHorizontalInput = !Mathf.Approximately(horizontal,0f);
        bool hasVeritcalInput = !Mathf.Approximately(vertical,0f);

        bool isRun = hasHorizontalInput || hasVeritcalInput;

        animator.SetBool("isRun",isRun);
    }
}
